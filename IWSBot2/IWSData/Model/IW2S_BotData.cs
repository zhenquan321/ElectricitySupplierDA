using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_BotData
    {
        public ObjectId _id { get; set; }


        /// <summary>
        /// 总用户数
        /// </summary>
        public int users { set; get; }

        /// <summary>
        /// 添加关键词的用户
        /// </summary>
        public int active_users { set; get; }

        /// <summary>
        /// 活动的项目
        /// </summary>
        public int active_projects { set; get; }

        /// <summary>
        /// 总项目数
        /// </summary>
        public int projects { set; get; }

        /// <summary>
        /// 总关键词数
        /// </summary>
        public int keywords { set; get; }

        /// <summary>
        /// 没有搜的关键词数
        /// </summary>
        public int kw_wait { set; get; }

        /// <summary>
        /// 正在搜的关键词数
        /// </summary>
        public int kw_sch { set; get; }

        /// <summary>
        /// 搜索完成的关键词数
        /// </summary>
        public int kw_complete { set; get; }
        /// <summary>
        /// 链接总数
        /// </summary>
        public int links { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime ins_time { set; get; }

        public int img_links { set; get; }
        public int sg_links { set; get; }
        public int wx_links { set; get; }
        public int wb_links { set; get; }

        public int wb_keywords { get; set; }
        public int wb_kw_sch { get; set; }
        public int wb_kw_complete { get; set; }
        public int wb_kw_wait { get; set; }
        public int wx_keywords { get; set; }
        public int wx_kw_sch { get; set; }
        public int wx_kw_complete { get; set; }
        public int wx_kw_wait { get; set; }
        public int sg_keywords { get; set; }
        public int sg_kw_sch { get; set; }
        public int sg_kw_complete { get; set; }
        public int sg_kw_wait { get; set; }
        public int img_keywords { get; set; }
        public int img_kw_sch { get; set; }
        public int img_kw_complete { get; set; }
        public int img_kw_wait { get; set; }
    }

    public class IW2S_BotDataDto
    {
        public string _id { get; set; }


        /// <summary>
        /// 总用户数
        /// </summary>
        public int users { set; get; }

        /// <summary>
        /// 添加关键词的用户
        /// </summary>
        public int active_users { set; get; }

        /// <summary>
        /// 活动的项目
        /// </summary>
        public int active_projects { set; get; }

        /// <summary>
        /// 总项目数
        /// </summary>
        public int projects { set; get; }

        /// <summary>
        /// 总关键词数
        /// </summary>
        public int keywords { set; get; }

        /// <summary>
        /// 没有搜的关键词数
        /// </summary>
        public int kw_wait { set; get; }

        /// <summary>
        /// 正在搜的关键词数
        /// </summary>
        public int kw_sch { set; get; }

        /// <summary>
        /// 搜索完成的关键词数
        /// </summary>
        public int kw_complete { set; get; }
        /// <summary>
        /// 链接总数
        /// </summary>
        public int links { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime ins_time { set; get; }
        public int img_links { set; get; }
        public int sg_links { set; get; }
        public int wx_links { set; get; }
        public int wb_links { set; get; }

        public int wb_keywords { get; set; }
        public int wb_kw_sch { get; set; }
        public int wb_kw_complete { get; set; }
        public int wb_kw_wait { get; set; }
        public int wx_keywords { get; set; }
        public int wx_kw_sch { get; set; }
        public int wx_kw_complete { get; set; }
        public int wx_kw_wait { get; set; }
        public int sg_keywords { get; set; }
        public int sg_kw_sch { get; set; }
        public int sg_kw_complete { get; set; }
        public int sg_kw_wait { get; set; }
        public int img_keywords { get; set; }
        public int img_kw_sch { get; set; }
        public int img_kw_complete { get; set; }
        public int img_kw_wait { get; set; }
    }
}
