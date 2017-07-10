using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 文字树类
    /// </summary>
    public class Dnl_WordTree
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Keyword { get; set; }
        public bool IsDel { get; set; }
    }

    /// <summary>
    /// 文字树类
    /// </summary>
    public class Dnl_WordTreeDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UsrId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Keyword { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
