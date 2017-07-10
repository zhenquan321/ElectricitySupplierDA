using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace DBHelper.Models.MongoDB
{
    public class WolongWeiboTask
    {
        public ObjectId _id { set; get; }

        public int TaskId { set; get; }

        public string TaskName { get; set; }

        /// <summary>
        /// 英文逗号分隔
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 任务创建时间，注意：卧龙接口使用 UTC-8 2017-03-27 14:45:00
        /// </summary>
        public Nullable<DateTime> CreateAt { get; set; }

        /// <summary>
        /// 任务最近更新时间，注意：卧龙接口使用 UTC-8 2017-03-27 14:45:00
        /// </summary>
        public Nullable<DateTime> LastUpdateAt { get; set; }

        /// <summary>
        /// 任务状态，0：未提交，1：已提交，2：已完成
        /// </summary>
        public int Status { get; set; }        
    }
}
