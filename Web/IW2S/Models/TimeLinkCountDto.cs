using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IWSData.Model;

namespace IW2S.Models
{
    /// <summary>
    /// 折线图及饼图数据
    /// </summary>
    public class TimeLinkCountDto
    {
        /// <summary>
        /// 时间坐标
        /// </summary>
        public List<DateTime> Times { get; set; }
        /// <summary>
        /// 折线图数据，每个对象为一条线
        /// </summary>

        public List<LineData> LineDataList { get; set; }
        /// <summary>
        /// 精简版自动摘要，用于在折线图生成提示及生成饼图
        /// </summary>
        public List<SumData> Sum { get; set; }
        
    }

    /// <summary>
    /// 折线图单条线数据
    /// </summary>
    public class LineData
    {
        public string name { get; set; }
        public List<int> LinkCount { get; set; }
        public List<TopData> topData { get; set; }
        public string CategoryId { get; set; }
    }

    /// <summary>
    /// Top数据
    /// </summary>
    public class TopData
    {
        public DateTime X { get; set; }
        public int Y { get; set; }
        public string name { get; set; }
        public string CategoryId { get; set; }
    }

    /// <summary>
    /// 摘要数据
    /// </summary>
    public class SumData
    {
        public DateTime X { get; set; }
        public int Y { get; set; }
        public string Summary { get; set; }
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
    }

    /// <summary>
    /// 链接信息
    /// </summary>
    public class LinkStatus
    {
        public DateTime PublishTime { get; set; }
        public string CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
    }

    /// <summary>
    /// 链接分组信息
    /// </summary>
    public class CategoryList
    {
        public List<DateTime> PublishTime { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    #region 简报链接图表类，因部分使用了IW2S.Models命名空间内类，故放于此处
    /// <summary>
    /// 简报链接图表
    /// </summary>
    public class Dnl_Report_LinkChartDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string CategoryId { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 折线图坐标节点
        /// </summary>
        public List<DateTime> Times { get; set; }
        /// <summary>
        /// 折线图数据
        /// </summary>
        public List<LineData> LineChartData { get; set; }
        /// <summary>
        /// 饼图数据
        /// </summary>
        public List<SumData> PieChartData { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 图表类型，1为折线图，2为饼图，3为气泡图
        /// </summary>
        public int CharyType { get; set; }
    }

    /// <summary>
    /// 简报链接图表
    /// </summary>
    public class Dnl_Report_LinkOverviewDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string CategoryId { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 折线图坐标节点
        /// </summary>
        public List<DateTime> Times { get; set; }
        /// <summary>
        /// 折线图数据
        /// </summary>
        public List<LineData> LineChartData { get; set; }
        /// <summary>
        /// 饼图数据
        /// </summary>
        public List<SumData> PieChartData { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }


    /// <summary>
    /// 词组统计信息
    /// </summary>
    public class Report_LinkInfo
    {
        /// <summary>
        /// 分组名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 有数据的坐标点位置列表
        /// </summary>
        public List<int> IndexList { get; set; }
        /// <summary>
        /// 对应坐标点链接数
        /// </summary>
        public List<int> LinkCountList { get; set; }
        /// <summary>
        /// Top数据
        /// </summary>
        public List<TopData> topData { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        public string CategoryId { get; set; }
    }

    #endregion
    

}
