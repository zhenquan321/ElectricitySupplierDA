using kuerjiudian.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace kuerjiudian.Controllers
{
    public class CategoryController : ApiController
    {
        string connCommonsStr = AISSystem.AppSettingHelper.GetAppSetting("MAppEntitiesPOCO");

        [HttpGet]

        public List<CategoryDto> GetCategory()
        {
            string sql = @"select ID,Name from huangguan_category";
            DataTable dt = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql);
            List<CategoryDto> list = new List<CategoryDto>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    CategoryDto dto = new CategoryDto();
                    dto.ID = Convert.ToInt32(item["ID"]);
                    dto.Name = item["Name"].ToString();
                    list.Add(dto);
                }
                return list;
            }
            else
            {
                return list;
            }
        }



        [HttpGet]
        public string DelCategory(int CategoryID)
        {
            string updatesql = string.Format(@"update huangguan_share set CategoryId=NULL where CategoryId='{0}'", CategoryID);
            MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
            string delsql = string.Format(@"delete from huangguan_category where ID={0}", CategoryID);
            int count = MySqlDbHelper.ExecuteSql(connCommonsStr, delsql);
            if (count > 0)
            {
                return "删除成功！";
            }
            else
            {
                return "删除失败，请重试！";
            }
        }


        [HttpGet]
        public string UpdateCategory(int CategoryID, string CategoryName)
        {
            string updatesql = string.Format(@"update huangguan_category set Name=N'{0}' where ID='{1}'",CategoryName, CategoryID);
            int count= MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
            if (count > 0)
            {
                return "修改成功！";
            }
            else
            {
                return "修改失败，请重试！";
            }
        }



    }
}