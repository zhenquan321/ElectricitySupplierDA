using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AISSystem
{
    public class DBHelper
    {
        public static int RunScript(string connectionString, string script, int timeoutSeconds = 180)
        {
            int rows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the command and set its properties.
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = script;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = timeoutSeconds;
                    connection.Open();
                    rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
            }
            return rows;
        }

        public static int RunScript2(string con, string script, params object[] parameters)
        {
            List<SqlParameter> ps = new List<SqlParameter>();
            for (int i = 0; i < parameters.Length; i += 2)
            {
                SqlParameter p = new SqlParameter { ParameterName = parameters[i] as string };
                if (parameters[i + 1] is byte[])
                {
                    p.SqlDbType = SqlDbType.VarBinary;
                    p.Size = int.MaxValue;
                }
                p.Value = parameters[i + 1];
                ps.Add(p);
            }
            return  RunScript(con, script, ps.ToArray());
        }


        public static int RunScript(string connectionString, string script, SqlParameter[] sqlParameters, int timeoutSeconds = 180)
        {
            int rows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the command and set its properties.
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = script;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = timeoutSeconds;
                    if(sqlParameters != null)
                        command.Parameters.AddRange(sqlParameters);
                    connection.Open();
                    rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
            }
            return rows;
        }

        public static List<T> Query<T>(string connectString, string script, Func<DataRow, T> selector)
        {
            string msg;
            DataTable dt = Query(connectString, script, out msg);
            if (dt == null && dt.Rows == null)
                return null;
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(selector(row));
            }
            return list;
        }

        public static DataTable Query(string connectionString, string script)
        {
            string errorMsg;
            return Query(connectionString, script, out errorMsg);
        }

        public static DataTable Query(string connectionString, string script, out string errorMsg)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = null;
            SqlDataAdapter da = null;
            errorMsg = null;
            try
            {
                connection = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand(script, connection);

                connection.Open();
                da = new SqlDataAdapter(cmd);
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

        public static DataTable Query(string connectionString, string script, SqlParameter[] sqlParameters, out string errorMsg)
        {
            var dt = new DataTable();
            SqlConnection connection = null;
            SqlDataAdapter da = null;
            errorMsg = null;
            try
            {
                connection = new SqlConnection(connectionString);

                var cmd = new SqlCommand(script, connection);
                if (sqlParameters != null)
                    cmd.Parameters.AddRange(sqlParameters);

                connection.Open();
                da = new SqlDataAdapter(cmd);
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

        public static Nullable<T> GetValue<T>(string conn, string script) where T : struct
        {
            string err;
            var dt = Query(conn, script, out err);
            if (dt == null || dt.Rows.Count == 0 || dt.Columns == null || dt.Columns.Count == 0)
                return null;
            object o = dt.Rows[0][0];
            if (o == null || o is System.DBNull)
                return null;
            return (T)o;
        }

        public static Nullable<T> GetValue<T>(string conn, SqlParameter[] slSqlParameters, string script) where T : struct
        {
            string err;
            var dt = Query(conn, script, slSqlParameters, out err);
            if (dt == null || dt.Rows.Count == 0 || dt.Columns == null || dt.Columns.Count == 0)
                return null;
            object o = dt.Rows[0][0];
            if (o == null || o is System.DBNull)
                return null;
            return (T)o;
        }

        public static T GetValueOrDefault<T>(string conn, string script)
        {
            string err;
            var dt = Query(conn, script, out err);
            if (dt == null || dt.Rows.Count == 0 || dt.Columns == null || dt.Columns.Count == 0)
                return default(T);
            object o = dt.Rows[0][0];
            if (o == null || o is System.DBNull)
                return default(T);
            return (T)o;
        }

        public static T GetValueOrDefault<T>(string conn, SqlParameter[] sqlParameters, string script)
        {
            string err;
            var dt = Query(conn, script, sqlParameters, out err);
            if (dt == null || dt.Rows.Count == 0 || dt.Columns == null || dt.Columns.Count == 0)
                return default(T);
            object o = dt.Rows[0][0];
            if (o == null || o is System.DBNull)
                return default(T);
            return (T)o;
        }

        public static void AddColumns(DataTable _dt, params object[] columnPairs)
        {
            for (int i = 0; i < columnPairs.Length; i += 2)
            {
                string columnName = columnPairs[i] as string;
                Type t = columnPairs[i + 1] as Type;
                DataColumn column = new DataColumn(columnName, t);
                column.AllowDBNull = true;
                _dt.Columns.Add(column);
            }
        }

        /// <summary>
        /// 使用Bulk方式拷贝数据
        /// </summary>
        /// <param name="conStr">连接字符串</param>
        /// <param name="dt">数据表</param>
        /// <param name="colMaps">列映射</param>
        /// <returns></returns>
        public static int BulkCopy(string conStr, DataTable dt, List<SqlBulkCopyColumnMapping> colMaps)
        {
            var count = 0;
            var con = new SqlConnection(conStr);
            var bulkCopy = new SqlBulkCopy(con) { BatchSize = dt.Rows.Count, DestinationTableName = dt.TableName };
            foreach (var mp in colMaps)
            {
                bulkCopy.ColumnMappings.Add(mp);
            }

            try
            {
                con.Open();
                if (dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                    count = bulkCopy.BatchSize;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when run DBHelper Bulk Copy {0}", ex);
                count = -1;
            }
            finally
            {
                con.Close();
                bulkCopy.Close();
            }
            return count;
        }

        public static int BulkCopy(string conStr, DataTable dt)
        {
            var count = 0;
            var con = new SqlConnection(conStr);
            var bulkCopy = new SqlBulkCopy(con) { BatchSize = dt.Rows.Count, DestinationTableName = dt.TableName };

            try
            {
                con.Open();
                if (dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                    count = bulkCopy.BatchSize;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when run DBHelper Bulk Copy {0}", ex);
                count = -1;
            }
            finally
            {
                con.Close();
                bulkCopy.Close();
            }
            return count;
        }

        public static bool RunTVP(string conStr, string usp, string pamteterName, DataTable table)
        {
            try
            {
                SqlConnection con;
                // modify connection string to connect to your database 
                con = new SqlConnection(conStr);
                con.Open();
                using (con)
                {
                    // Configure the SqlCommand and SqlParameter.
                    SqlCommand sqlCmd = new SqlCommand(usp, con);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandTimeout = 600;
                    SqlParameter tvpParam = sqlCmd.Parameters.AddWithValue(pamteterName, table); //Needed TVP
                    tvpParam.SqlDbType = SqlDbType.Structured; //tells ADO.NET we are passing TVP
                    sqlCmd.ExecuteNonQuery();
                }
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogerHelper.WriteLog("Error when run DBHelper.RunTVP", ex);
                return false;
            }
        }

        public static int RunTVPScript(string conStr, string script, string pamteterName, string pameterType, DataTable table)
        {
            int count = 0;
            try
            {
                SqlConnection con;
                // modify connection string to connect to your database 
                con = new SqlConnection(conStr);
                con.Open();
                using (con)
                {
                    // Configure the SqlCommand and SqlParameter.
                    SqlCommand sqlCmd = new SqlCommand(script, con);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandTimeout = 6000;
                    SqlParameter tvpParam = sqlCmd.Parameters.AddWithValue(pamteterName, table); //Needed TVP
                    tvpParam.SqlDbType = SqlDbType.Structured; //tells ADO.NET we are passing TVP
                    tvpParam.TypeName = pameterType;
                    count = sqlCmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                LogerHelper.WriteLog("Error when run DBHelper.RunTVP", ex);

            }

            return count;
        }

        /// <summary>
        /// 将ID字符串变成SQL参数形式
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="paraString"></param>
        /// <returns></returns>
        public static List<SqlParameter> Ids2SqlParameters(string ids, out string paraString)
        {
            var paraList = new List<SqlParameter>();
            var paraSb = new StringBuilder();
            var index = 0;
            var idLst = ids.Split(';').ToList();

            foreach (var id in idLst)
            {
                paraList.Add(new SqlParameter("@para" + index, id));
                paraSb.Append("@para" + index).Append(",");
                index++;
            }
            
            paraString = paraSb.ToString(0, paraSb.Length -1);
            return paraList;
        }

        /// <summary>
        /// 将ID字符串变成SQL参数形式
        /// </summary>
        /// <param name="idLst"></param>
        /// <param name="paraString"></param>
        /// <returns></returns>
        public static List<SqlParameter> Ids2SqlParameters(IEnumerable<Guid> idLst, out string paraString)
        {
            var paraList = new List<SqlParameter>();
            var paraSb = new StringBuilder();
            var index = 0;

            foreach (var id in idLst)
            {
                paraList.Add(new SqlParameter("@para" + index, id));
                paraSb.Append("@para" + index).Append(",");
                index++;
            }

            paraString = paraSb.ToString(0, paraSb.Length - 1);
            return paraList;
        }


        public static int RunSqlScript(string con, string sql, string token = "#Go")
        {
            var sqls = sql.SplitWith(token);
            int count = 0;
            foreach (var s in sqls)
            {
                var r = RunScript(con, s);
                if (r >= -1)
                    count++;
            }
            return count;
        }

        public static SqlConnection GetSqlConnection(string consStr,out SqlCommand cmd , out SqlTransaction tran)
        {
            var conn = new SqlConnection(consStr);
            conn.Open();
            tran = conn.BeginTransaction();
            cmd = new SqlCommand
            {
                Connection = conn,
                Transaction = tran
            };

            return conn;
        }
        
    }
}
