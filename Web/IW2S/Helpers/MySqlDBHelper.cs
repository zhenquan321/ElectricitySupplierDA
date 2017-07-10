using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using AISSystem;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace IW2S.Helpers
{
    
    public class MySqlDbHelper
    {
        internal static readonly string com = AISSystem.AppSettingHelper.GetAppSetting("commonsMySqlCon");
        public static int ExecuteSql(string conStr, string sql, IEnumerable<MySqlParameter> paras = null)
        {

            var conn = new MySqlConnection(conStr);
            var n = 0;
            try
            {
                conn.Open();
                var cmd = new MySqlCommand(sql, conn);
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras.ToArray());
                }

                n = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return n;
        }

        public static DataTable ExecuteQuery(string conStr, string sql, IEnumerable<MySqlParameter> paras = null)
        {
            var conn = new MySqlConnection(conStr);
            var dt = new DataTable();
            try
            {
                conn.Open();
                var cmd = new MySqlCommand(sql, conn);
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras.ToArray());
                }
                var da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }


        public static MySqlDataReader ExecuteQueryReader(string conStr, string sql, IEnumerable<MySqlParameter> paras = null)
        {
            var conn = new MySqlConnection(conStr);
            try
            {
                conn.Open();
                var cmd = new MySqlCommand(sql, conn);
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras.ToArray());
                }
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public static DataTable Query(string connectionString, string script, MySqlParameter[] sqlParameters, out string errorMsg)
        {
            var dt = new DataTable();
            MySqlConnection connection = null;
            MySqlDataAdapter da = null;
            errorMsg = null;
            connectionString = chk(connectionString);
            try
            {
                connection = new MySqlConnection(connectionString);

                var cmd = new MySqlCommand(script, connection);
                if (sqlParameters != null)
                    cmd.Parameters.AddRange(sqlParameters);

                connection.Open();
                da = new MySqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("Error:{0}, Exception at {1}", ex.Message, script);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
                if (da != null)
                    da.Dispose();
            }
            return dt;
        }

        private static string chk(string con)
        {
            if (con.IsContains("connection string=\""))
                con = con.SubAfter("connection string=\"").SubBefore("\"");
            return con;
        }

        public static int BatchInsert(string con, string table_name, MySqlTable table)
        {
            var paras = new List<MySqlParameter>();
            var sb = new StringBuilder();
            try
            {
                string sql = "insert " + table_name + "(" + string.Join(",", table.cols) + ") values ";

                sb.Append(sql);
                int rows = table.data.GetLength(0), cols = table.data.GetLength(1);
                for (int r = 0; r < rows; r++)
                {
                    sb.Append("(");
                    for (int c = 0; c < cols; c++)
                    {
                        var para = "@" + table.cols[c] + "para" + r;
                        sb.Append(para);
                        if (c < cols - 1)
                            sb.Append(",");
                        paras.Add(new MySqlParameter(para, table.data[r, c]));
                    }
                    sb.Append(")");
                    if (r < rows - 1)
                        sb.Append(",");
                }
                return MySqlDbHelper.ExecuteSql(con, sb.ToString(), paras);
            }
            catch (Exception ex)
            {
                //LogerHelper.WriteErrorLog("{0}\r\n{1}".FormatStr(ex.Message, ex.StackTrace));
                ExceptionHelper.LogExceptionErr(ex);
                try
                {
                    return MySqlDbHelper.ExecuteSql(con, sb.ToString(), paras);
                }
                catch (Exception ex1)
                {
                    //LogerHelper.WriteErrorLog("{0}\r\n{1}".FormatStr(ex.Message, ex.StackTrace));
                    ExceptionHelper.LogExceptionErr(ex1);
                    try
                    {
                        return MySqlDbHelper.ExecuteSql(con, sb.ToString(), paras);
                    }
                    catch (Exception ex2)
                    {
                        //LogerHelper.WriteErrorLog("{0}\r\n{1}".FormatStr(ex.Message, ex.StackTrace));
                        ExceptionHelper.LogExceptionErr(ex2);
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 将ID字符串变成SQL参数形式
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="paraString"></param>
        /// <returns></returns>
        public static List<MySqlParameter> Ids2SqlParameters(string ids, out string paraString)
        {
            var paraList = new List<MySqlParameter>();
            var paraSb = new StringBuilder();
            var index = 0;
            var idLst = ids.Split(';').ToList();

            foreach (var id in idLst)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    paraList.Add(new MySqlParameter("@para" + index, id));
                    paraSb.Append("@para" + index).Append(",");
                    index++;
                }
            }

            paraString = paraSb.ToString(0, paraSb.Length - 1);
            return paraList;
        }

        public static T GetValue<T>(string con, string sql)
        {
            try
            {
                var dt = ExecuteQuery(con, sql);
                T t = (T)dt.Rows[0][0];
                return t;
            }
            catch
            {
                return default(T);
            }
        }

        public static List<T> GetValues<T>(string con, string sql)
        {
            List<T> list = new List<T>();
            try
            {
                var dt = ExecuteQuery(con, sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    T t = (T)dt.Rows[i][0];
                    list.Add(t);
                }
            }
            catch
            {
            }
            return list;
        }

        public static List<T> GetEntities<T>(string conStr, string sql, IEnumerable<MySqlParameter> paras = null) where T : class
        {
            List<T> list = new List<T>();
            ExecuteQuery(conStr, sql, row =>
            {
                var t = GetEntity<T>(row);
                if (t != null)
                {
                    list.Add(t);
                }
            }, paras);
            return list;
        }

        public static List<T> GetEfEntities<T>(string conStr, string where, int? limit = null) where T : class
        {
            string table = typeof(T).Name;
            string sql = "select * from " + table + " where " + where;
            if (limit.HasValue)
                sql += " limit " + limit.Value;
            return GetEntities<T>(conStr, sql);
        }

        public static T GetEntity<T>(DataRow row) where T : class
        {
            var t = typeof(T);
            T result = Activator.CreateInstance(t) as T;
            var pis = t.GetProperties();
            foreach (var pi in pis)
            {
                try
                {
                    var obj = row[pi.Name];
                    pi.SetValue(result, obj);
                }
                catch
                {
                }
            }
            return result;
        }

        public static int Upsert<T>(T t, string con, string table, string id_col, bool is_auto_inc) where T : class
        {
            var type = t.GetType();
            var pis = type.GetProperties().Where(x => x.PropertyType.IsSealed && (x.Name != id_col || !is_auto_inc)).ToList();
            var id_pi = type.GetProperty(id_col);
            object id = id_pi.GetValue(t);
            string sql = "select * from {0} where {1}='{2}'".FormatStr(table, id_col, id);
            var o = GetEntities<T>(con, sql).FirstOrDefault();
            if (o == null)
            {
                int len = pis.Count;
                MySqlTable dt = new MySqlTable { cols = new string[len], data = new object[1, len] };
                for (int i = 0; i < pis.Count; i++)
                {
                    dt.cols[i] = pis[i].Name;
                    dt.data[0, i] = pis[i].GetValue(t);
                }
                return BatchInsert(con, table, dt);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("update " + table + " set ");
                List<MySqlParameter> paras = new List<MySqlParameter>();

                foreach (var pi in pis)
                {
                    string new_val = "{0}".FormatStr(pi.GetValue(t));
                    string org_val = "{0}".FormatStr(pi.GetValue(o));
                    if (string.IsNullOrEmpty(new_val) || new_val == org_val)
                        continue;
                    var para = "@" + pi.Name;
                    paras.Add(new MySqlParameter(para, pi.GetValue(t)));
                    sb.Append(" {0}.{1}={2},".FormatStr(table, pi.Name, para));
                }
                sb.Remove(sb.Length - 1, 1);
                string id_para = "@para_" + id_col;
                paras.Add(new MySqlParameter(id_para, id));
                sb.Append(" where {0}={1}".FormatStr(id_col, id_para));
                sql = sb.ToString();
                if (string.IsNullOrEmpty(sql.SubAfter("set").SubBefore(" where ").GetTrimed()))
                    return 0;
                return ExecuteSql(con, sql, paras);
            }
        }

        public static int BatchInsert<T>(List<T> list, string con, string table, string id_col, bool is_auto_inc) where T : class
        {
            var type = list[0].GetType();
            var pis = type.GetProperties().Where(x => x.PropertyType.IsSealed && (x.Name != id_col || !is_auto_inc)).ToList();

            int len = pis.Count;
            MySqlTable dt = new MySqlTable { cols = new string[len], data = new object[list.Count, len] };
            for (int i = 0; i < pis.Count; i++)
            {
                if (pis[i].Name == id_col && is_auto_inc)
                {
                    i--;
                    continue;
                }
                dt.cols[i] = pis[i].Name;
                for (int j = 0; j < list.Count; j++)
                {
                    dt.data[j, i] = pis[i].GetValue(list[j]);
                }
            }
            return BatchInsert(con, table, dt);
        }

        public static void ExecuteQuery(string conStr, string sql, Action<DataRow> retrieveData, IEnumerable<MySqlParameter> paras = null)
        {
            var dt = ExecuteQuery(conStr, sql, paras);
            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var dr = dt.Rows[i];
                retrieveData(dr);
            }
        }

        public static List<T> GetExsitsIds<T>(string con, string table_name, string id_column, params T[] ids)
        {
            var paras = new List<MySqlParameter>();
            string sql = "select " + id_column
                       + " from " + table_name
                       + " where " + table_name + "." + id_column + " in (\"" + string.Join("\",\"", ids) + "\")";
            var dt = ExecuteQuery(con, sql);
            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                return null;
            var r = dt.Rows.Cast<DataRow>().Select(x => x.Field<T>(id_column)).ToList();
            return r;
        }
    }

    public class MySqlTable
    {
        public string[] cols { get; set; }
        public object[,] data { get; set; }

        public void CopyData(List<object[]> _data)
        {
            data = new object[_data.Count, cols.Length];
            for (int row = 0; row < _data.Count; row++)
            {
                for (int col = 0; col < cols.Length; col++)
                {
                    data[row, col] = _data[row][col];
                }
            }
        }
    }

    public class MySqlQuery<T>
    {
        string expression;
        public string Expression { get { return expression; } }

        public MySqlQuery<T> Clear()
        {
            this.expression = "";
            return this;
        }

        public MySqlQuery<T> AndGT<TMember>(Expression<Func<T, TMember>> expr, TMember target)
        {
            string exp = GT(expr, target);
            this.And(exp);
            return this;
        }

        public MySqlQuery<T> OrGT<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string exp = GT(expr, target);
            this.Or(exp);
            return this;
        }

        public MySqlQuery<T> AndLT<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string exp = LT(expr, target);
            this.And(exp);
            return this;
        }

        public MySqlQuery<T> OrLT<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string exp = LT(expr, target);
            this.Or(exp);
            return this;
        }

        public MySqlQuery<T> AndEQ<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string exp = EQ(expr, target);
            this.And(exp);
            return this;
        }

        public MySqlQuery<T> OrEQ<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string exp = EQ(expr, target);
            this.Or(exp);
            return this;
        }

        public MySqlQuery<T> AndNIL<TMember>(Expression<Func<T, TMember>> expr)
        {
            string exp = isNil(expr);
            this.And(exp);
            return this;
        }

        public MySqlQuery<T> OrNil<TMember>(Expression<Func<T, TMember>> expr)
        {
            string exp = isNil(expr);
            this.Or(exp);
            return this;
        }

        public MySqlQuery<T> AndNotNIL<TMember>(Expression<Func<T, TMember>> expr)
        {
            string exp = isNotNil(expr);
            this.And(exp);
            return this;
        }

        public MySqlQuery<T> OrNotNil<TMember>(Expression<Func<T, TMember>> expr)
        {
            string exp = isNotNil(expr);
            this.Or(exp);
            return this;
        }



        public MySqlQuery<T> And(string exp)
        {
            if (string.IsNullOrEmpty(expression))
            {
                expression = exp;
                return this;
            }

            if (string.IsNullOrEmpty(exp))
                return this;

            expression = "({0}) and ({1})".FormatStr(exp, expression);
            return this;
        }

        public MySqlQuery<T> Or(string exp)
        {
            if (string.IsNullOrEmpty(expression))
            {
                expression = exp;
                return this;
            }
            if (string.IsNullOrEmpty(exp))
                return this;

            expression = "({0}) or ({1})".FormatStr(exp, expression);
            return this;
        }


        public static string GT<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string pn = get_property_name(expr);
            string exp = "{0} > {1} ".FormatStr(pn, target);
            return exp;
        }

        public static string LT<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string pn = get_property_name(expr);
            string exp = "{0} < {1} ".FormatStr(pn, target);
            return exp;
        }

        public static string EQ<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string pn = get_property_name(expr);
            string exp = "{0} = {1} ".FormatStr(pn, target);
            return exp;
        }

        public static string NE<TMember>(Expression<Func<T, TMember>> expr, object target)
        {
            string pn = get_property_name(expr);
            string exp = "{0} != {1} ".FormatStr(pn, target);
            return exp;
        }

        public static string isNil<TMember>(Expression<Func<T, TMember>> expr)
        {
            string pn = get_property_name(expr);
            string exp = "{0} is null  ".FormatStr(pn);
            return exp;
        }

        public static string isNotNil<TMember>(Expression<Func<T, TMember>> expr)
        {
            string pn = get_property_name(expr);
            string exp = "{0} is not null  ".FormatStr(pn);
            return exp;
        }

        public static string get_property_name<TMember>(Expression<Func<T, TMember>> expr)
        {
            var bodyExpr = expr.Body as System.Linq.Expressions.MemberExpression;
            if (bodyExpr == null)
            {
                throw new ArgumentException("Expression must be a MemberExpression!", "expr");
            }
            var propInfo = bodyExpr.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException("Expression must be a PropertyExpression!", "expr");
            }
            var propName = propInfo.Name;
            return propName;
        }

    }
}