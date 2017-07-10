using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 分词词库
    /// </summary>
    public class Dnl_StopWord
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        public DateTime CreatedAt { get; set; }
        //词库，多个间用分号相联
        public string Words { get; set; }
    }

    /// <summary>
    /// 分词词库
    /// </summary>
    public class Dnl_StopWordDto
    {
        public string _id { get; set; }
        public DateTime CreatedAt { get; set; }
        //词库，多个间用分号相联
        public List<string> Words { get; set; }
    }
}
