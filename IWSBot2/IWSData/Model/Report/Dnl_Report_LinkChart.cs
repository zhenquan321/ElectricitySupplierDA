using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 简报链接监测概况
    /// </summary>
    public class Dnl_Report_LinkChart
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public ObjectId CategoryId { get; set; }
        /// <summary>
        /// 使用的关键词组Id，多个用分号相连
        /// </summary>
        public string KeyCateIds { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 开始坐标点时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束坐标点时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 每个坐标点间时间间隔，以天为单位
        /// </summary>
        public int TimeInterval { get; set; }
        /// <summary>
        /// 传播事件统计图数据
        /// </summary>
        public string LineChart { get; set; }
        /// <summary>
        /// 传播内容占比图数据
        /// </summary>
        public string PieChart { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
        public bool IsDel { get; set; }
        /// <summary>
        /// 图表类型，1为折线图，2为饼图，3为气泡图
        /// </summary>
        public int CharyType { get; set; }
    }

    /// <summary>
    /// 链接图表设置因素
    /// </summary>
    public class LinkChartFactor
    {
        public string id { get; set; }
        public string reportId { get; set; }
        /// <summary>
        /// 图表归属分组Id
        /// </summary>
        public string lChartCateId { get; set; }
        /// <summary>
        /// 使用关键词组Id
        /// </summary>
        public string keyCateIds { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }
        /// <summary>
        /// 顶点数
        /// </summary>
        public int topNum { get; set; }
        /// <summary>
        /// 摘要数
        /// </summary>
        public int sumNum { get; set; }
        /// <summary>
        /// 坐标点时间间隔
        /// </summary>
        public int timeInterval { get; set; }
        /// <summary>
        /// 图表类型，1表示折线图，2表示饼图。
        /// </summary>
        public int chartType { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool isHide { get; set; }
        public int percent { get; set; }
    }

}
