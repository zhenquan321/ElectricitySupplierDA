using AISSystem;
using IW2S.Helpers;
using IW2S.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace IW2S.Controllers
{
    public class IprCourtController : ApiController
    {
        [HttpGet]
        public List<CasecauseDto> GetCasecause()
        {
            List<CasecauseDto> result = new List<CasecauseDto>();

            string sql = "select (CASE WHEN casecause  ='' THEN '其他' ELSE casecause END) casecause,count(casecause) casecount from ipjdoc where Step='一审' and  CaseType=5 group by casecause";
            string con = MySqlDbHelper.com;
            var dt = MySqlDbHelper.ExecuteQuery(con, sql);

            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                return null;
            
            foreach (var dr in dt.Rows.Cast<DataRow>())
            {
                result.Add(new CasecauseDto
                {
                    Casecause = dr.Field<string>("casecause"),
                    CaseCount = dr.Field<Int64>("casecount")
                });
            }
            return result;
        }
    }
}