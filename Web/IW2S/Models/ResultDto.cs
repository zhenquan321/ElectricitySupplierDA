using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ResultDtoCategory
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string NewId { get;set; }
    }

    public class ResultKeywordDto
    {
        public bool IsSuccess { get; set; }
        public List<string> KeywordList { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Json文件返回结果类
    /// </summary>
    public class JsonResultDto
    {
        public bool IsSuccess { get; set; }
        public string Json { get; set; }
        public string Message { get; set; }
    }
}