using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IW2S.Helpers;
using AISSystem;

using System.Text;
using System.Net;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IW2S.Controllers
{
    public class ExportController : Controller
    {
        //文件下载  
        public FileResult DownLoadExcel(string path)
        {
            string fileName = Path.GetFileName(path);
            return File(path, "application/ms-excel", fileName);
        }

        [HttpPost]
        public string UpLoadFile()
        {
            string filename = "";
            if (Request.Files.Count == 0)
            {
                return filename;
            }
            if (Request.Files[0] != null && Request.Files[0].ContentLength != 0)
            {
                string name = Request.Files[0].FileName;
                 string path = Server.MapPath("../ExportFiles/ImportExcel/"); //当前路径 
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filename = Guid.NewGuid().ToString() + ".xls";
                path += filename;
                Request.Files[0].SaveAs(path);
            }
            return filename;
        }

        

	}
}