using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using IW2S.Models.Emarknow;

namespace IW2S.Controllers
{
    public class DashboardController : ApiController
    {
        //信源
        [HttpGet]
        public List<FreeWebSite> GetWebSite(string user_id, string kid, string projectId)
        {
            try
            {
                MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();//多余语句
                // int user_id = WebSecurity.GetUserId(User.Identity.Name); 
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "projectId", new ObjectId(projectId) } };
                List<FreeBotItem> list = new List<FreeBotItem>();
                FieldsDocument fd = new FieldsDocument();//设置要提取的列，1表示同意提取
                fd.Add("SiteId", 1);
                fd.Add("SiteName", 1);
                MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");//旧式MongoDB使用方法
                var TaskList = col.Find(queryTask).SetFields(fd);
                List<FreeWebSite> am = new List<FreeWebSite>();
                var ary = from t in TaskList
                          group t by new { t.SiteId, t.SiteName } into g
                          select new FreeWebSite
                          {
                              _id = g.Key.SiteId.ToString(),
                              SiteName = g.Key.SiteName
                          };//以siteId,SiteName列进行分组，key为当前所处组
                return ary.ToList();
                //var builder = Builders<FreeBotItem>.Filter;
                //var filter = builder.Eq(x => x.UId, new ObjectId(user_id));
                //var TaskList = MongoDBHelper.Instance.Get_FreeBotItem().Find(filter).ToList();
                //List<FreeWebSite> am = new List<FreeWebSite>();
                //var ary = from t in TaskList
                //          group t by new { t.SiteId, t.SiteName } into g
                //          select new FreeWebSite
                //          {
                //              _id = g.Key.SiteId.ToString(),
                //              SiteName = g.Key.SiteName
                //          };
                //return ary.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        //监测到的总链接数
        [HttpGet]
        public long? GetTotalItems(string user_id, string kid, string projectId)
        {
            try
            {
                var col = MongoDBHelper.Instance.Get_FreeBotItem();
                var query = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                var agreresult = col.Find(query).Project(x => x._id);//projiect为只提取Id列
                return agreresult.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //监测到的总店铺数
        [HttpGet]
        public long? GetTotalShops(string user_id, string kid, string projectId)
        {
            try
            {
                var col = MongoDBHelper.Instance.Get_FreeBotShop();
                var query = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                var agreresult = col.Find(query).Project(x => x._id);
                return agreresult.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //30天销售量by关键词
        [HttpGet]
        public long? Get30SaleNum(string user_id, string kid, string projectId)
        {
            try
            {
                MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                //var TaskList = helper.FindData("level1link", queryTask);
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                List<FreeBotItem> list = new List<FreeBotItem>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("Recent30DaysSoldNum", 1);
                MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                var TaskList = col.Find(queryTask).SetFields(fd);
                var matchSum = TaskList.Sum(x => x.Recent30DaysSoldNum);//计算该关键词30天内总销售量
                return matchSum;
                //var col = MongoDBHelper.Instance.Get_FreeBotItem();
                //var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) } };
                //List<FreeBotItem> list = new List<FreeBotItem>();
                //FieldsDocument fd = new FieldsDocument();
                //fd.Add("Recent30DaysSoldNum", 1);
                //var agreresult = col.Find(queryTask).ToList();
                //var matchSum = agreresult.Sum(x => x.Recent30DaysSoldNum);
                //return matchSum;
            }
            catch (Exception wc)
            {
                return 0;
            }
        }

        //30天销售额by关键词
        [HttpGet]
        public double? Get30SaleVol(string user_id, string kid, string projectId)
        {
            try
            {
                var col = MongoDBHelper.Instance.Get_FreeBotItem();
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                List<FreeBotItem> list = new List<FreeBotItem>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("Price", 1);
                fd.Add("Recent30DaysSoldNum", 1);
                var agreresult = col.Find(queryTask).ToList();
                var matchSum = agreresult.Sum(x => x.Recent30DaysSoldNum * x.Price);
                return matchSum;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //30天销售产品均价
        [HttpGet]
        public double? Get30VgePrice(string user_id, string kid, string projectId)
        {
            try
            {
                //var col = MongoDBHelper.Instance.Get_FreeBotItem();
                //var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) } };
                //List<FreeBotItem> list = new List<FreeBotItem>();
                //FieldsDocument fd = new FieldsDocument();
                //fd.Add("Price", 1);
                //var TaskList = col.Find(queryTask).ToList();
                //double? matchSum = TaskList.Sum(x => x.Price);
                //double? vgePrice = matchSum / TaskList.Count();
                //return vgePrice;
                MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                //var TaskList = helper.FindData("level1link", queryTask);
                List<FreeBotItem> list = new List<FreeBotItem>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("Price", 1);
                MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                var TaskList = col.Find(queryTask).SetFields(fd);
                double? matchSum = TaskList.Sum(x => x.Price);
                double? vgePrice = matchSum / TaskList.Count();
                return vgePrice;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //图片显示，根据所选关键词找出一个销量最高宝贝对应的图片
        [HttpGet]
        public List<string> Getimg(string user_id, string kid, string projectId)
        {
            MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
            var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
            FieldsDocument fd = new FieldsDocument();
            fd.Add("pic_url", 1);
            List<FreeBotItem> list = new List<FreeBotItem>();
            MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
            SortByDocument s = new SortByDocument();
            s.Add("Recent30DaysSoldNum", -1);//获取数据时进行排序，-1为降序
            var TaskList = col.Find(queryTask).SetSortOrder(s).SetLimit(10).SetFields(fd);
            List<string> str = new List<string>();
            foreach (var item in TaskList)
            {
                string v;
                v = "http:" + item.pic_url;
                str.Add(v);
            }
            return str;
        }

        //获取有销量链接的占比
        [HttpGet]
        public double GetRatio(string user_id, string kid, string projectId)
        {
            var totoalItem = GetTotalItems(user_id, kid, projectId);
            var soveItem = GetTotalItemByNum(user_id, kid, projectId);
            if (totoalItem != 0)
            {
                double totalItem1 = Convert.ToDouble(totoalItem);
                double soveItem1 = Convert.ToDouble(soveItem);
                double unm = double.Parse(string.Format("{0:F}", (soveItem1 / totalItem1) * 100));
                return unm;
            }
            else
            {
                return 0;
            }

        }

        public long? GetTotalItemByNum(string user_id, string kid, string projectId)//获取有销量的链接数
        {
            try
            {
                var col = MongoDBHelper.Instance.Get_FreeBotItem();
                var builder = Builders<FreeBotItem>.Filter;
                var filter = builder.Eq(x => x.UId, new ObjectId(user_id));
                filter &= builder.Eq(x => x.taskId, new ObjectId(kid));
                filter &= builder.Gt(x => x.Recent30DaysSoldNum, 0);//Recent30DaysSoldNum列大于0的数据才会被获取
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
                List<FreeBotItem> list = new List<FreeBotItem>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("Price", 1);
                var agreresult = col.Find(filter).Project(x => x._id);
                return agreresult.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //30天销售额前十的地区
        [HttpGet]
        public List<FreeBotItemDto> GetItemList(string user_id, string kid, int? pagesize, string projectId)
        {
            try
            {
                MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                List<FreeBotItemDto> list = new List<FreeBotItemDto>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("Location", 1);
                fd.Add("Price", 1);
                fd.Add("Recent30DaysSoldNum", 1);
                MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                var TaskList = col.Find(queryTask).SetFields(fd);
                foreach (var item in TaskList)
                {

                    if (!string.IsNullOrEmpty(item.Location))
                    {
                        FreeBotItemDto v = new FreeBotItemDto();
                        var str = item.Location.Split(' ');
                        if (str.Length > 1)
                        {
                            v.Location = str[1];
                        }
                        else
                        {
                            v.Location = str[0];
                        }
                        v.salesAmount = item.Price * item.Recent30DaysSoldNum;
                        list.Add(v);
                    }

                    
                }

                List<FreeBotItemDto> am = new List<FreeBotItemDto>();
                var ary = from t in list
                          group t by new { t.Location } into g
                          select new FreeBotItemDto
                          {
                              _id = Guid.NewGuid().ToString(),
                              Location = g.Key.Location,
                              salesAmount = g.Sum(p => p.salesAmount),
                              SalesVolume = Math.Round(Convert.ToDouble(g.Sum(p => p.salesAmount)), 2).ToString("C2", new CultureInfo("zh-CN"))//C2为保留两位小数，new CultureInfo("zh-CN")为添加货币符号（中国）
                          };
                if (pagesize.HasValue)
                {
                    return ary.OrderByDescending(x => x.salesAmount).Take(Convert.ToInt16(pagesize)).ToList();
                }
                else
                {
                    return ary.OrderByDescending(x => x.salesAmount).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //30天销售额前十的链接
        [HttpGet]
        public List<FreeBotItemDto> GetShopList(string user_id, string kid, int? pagesize, string projectId)
        {
            try
            {
                MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                List<FreeBotItemDto> list = new List<FreeBotItemDto>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("_id", 1);
                fd.Add("ShopName", 1);

                fd.Add("Location", 1);
                fd.Add("Recent30DaysSoldNum", 1);
                fd.Add("ItemName", 1);
                fd.Add("Price", 1);
                fd.Add("ShopID", 1);
                fd.Add("DetailUrl", 1);
                MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                var TaskList = col.Find(queryTask).SetFields(fd);
                var ary = from t in TaskList
                          group t by new { t.ShopName, t.Location, t.Recent30DaysSoldNum, t.ItemName, t.Price, t.ShopID, t.DetailUrl } into g
                          select new FreeBotItemDto
                          {
                              _id = Guid.NewGuid().ToString(),
                              ShopName = g.Key.ShopName,
                              Location = g.Key.Location,
                              Recent30DaysSoldNum = g.Key.Recent30DaysSoldNum,
                              ItemName = g.Key.ItemName,
                              Price = g.Key.Price,
                              ShopID = g.Key.ShopID,
                              DetailUrl = g.Key.DetailUrl
                          };
                foreach (var item in ary)
                {
                    FreeBotItemDto v = new FreeBotItemDto();
                    v._id = item._id;
                    v.ShopID = item.ShopID;
                    v.ShopName = item.ShopName;
                    v.Location = item.Location;
                    v.ItemName = item.ItemName;
                    v.Price = item.Price;
                    v.salesAmount = item.Price * item.Recent30DaysSoldNum;
                    //Math.Round(Convert.ToDouble(io.Value), 2).ToString("C2", new CultureInfo("zh-CN"));
                    v.SalesVolume = Math.Round(Convert.ToDouble(item.Price * item.Recent30DaysSoldNum), 2).ToString("C2", new CultureInfo("zh-CN")); //double.Parse(String.Format("{0:N2}", item.Price * item.Recent30DaysSoldNum));
                    v.Price2 = Math.Round(Convert.ToDouble(item.Price), 2).ToString("C2", new CultureInfo("zh-CN"));
                    v.DetailUrl = item.DetailUrl;
                    list.Add(v);
                }
                if (pagesize.HasValue)
                {
                    list = list.OrderByDescending(x => x.salesAmount).Take(Convert.ToInt16(pagesize)).ToList();
                    return list;
                }
                else
                {
                    return list.OrderByDescending(x => x.salesAmount).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //获取气泡图，Y轴淘宝综合排名，X轴评论数，气泡:30天销售量
        [HttpGet]
        public List<FreeBotItemVo> GetBubbleList(string user_id, string kid, int? pagesize, string projectId)
        {
            var queryl = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) } };
            var colFS = MongoDBHelper.Instance.Get_FreeShopTimeline();
            var aryl = colFS.Find(queryl).SortByDescending(x => x.Recent30DaysSoldNum).ToList();
            List<FreeBotItemDto> fst = new List<FreeBotItemDto>();

            if (aryl.Count > 0)
            {
                foreach (var item in aryl)
                {
                    FreeBotItemDto fstDto = new FreeBotItemDto()
                    {
                        _id = item._id.ToString(),
                        CreatedAt = Convert.ToDateTime(item.CreatedAt),// item.CreatedAt,
                        CreatedAt2 = item.CreatedAt2,
                        Position = item.Position,
                        Recent30DaysSoldNum = item.Recent30DaysSoldNum,
                        ShopName = item.ShopName,
                        ItemId = item.SId,
                        taskId = item.taskId.ToString(),
                        TotalComments = item.TotalComments,
                        UId = item.UId.ToString()
                    };

                    fst.Add(fstDto);
                }
                List<FreeBotItemVo> am = new List<FreeBotItemVo>();
                foreach (var item in fst.GroupBy(x => x.CreatedAt))
                {
                    if (pagesize.HasValue)
                    {
                        //TaskList = col.Find(queryTask).SetFields(fd).SetSortOrder(s).SetLimit(Convert.ToInt16(pagesize)).ToList();
                        FreeBotItemVo amm = new FreeBotItemVo();
                        amm.TimeKey = item.Key.ToString();
                        amm.FreeBotItemList = item.OrderByDescending(x => x.Recent30DaysSoldNum).Take(Convert.ToInt16(pagesize)).ToList();
                        am.Add(amm);
                    }
                    else
                    {
                        //TaskList = col.Find(queryTask).SetFields(fd).SetSortOrder(s).ToList();
                        FreeBotItemVo amm = new FreeBotItemVo();
                        amm.TimeKey = item.Key.ToString();
                        amm.FreeBotItemList = item.OrderByDescending(x => x.Recent30DaysSoldNum).ToList();
                        am.Add(amm);
                    }
                }
                return am.OrderBy(x => x.TimeKey).ToList();
            }
            else
            {
                try
                {
                    MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                    var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                    List<FreeBotItemDto> list = new List<FreeBotItemDto>();
                    FieldsDocument fd = new FieldsDocument();
                    fd.Add("_id", 1);
                    fd.Add("Position", 1);
                    fd.Add("Location", 1);
                    fd.Add("Recent30DaysSoldNum", 1);
                    fd.Add("ItemName", 1);
                    fd.Add("ShopName", 1);
                    fd.Add("TotalComments", 1);
                    fd.Add("DetailUrl", 1);
                    fd.Add("CreatedAt", 1);
                    MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                    var TaskList = new List<FreeBotItem>();
                    SortByDocument s = new SortByDocument();
                    s.Add("Recent30DaysSoldNum", -1);
                    TaskList = col.Find(queryTask).SetFields(fd).SetSortOrder(s).ToList();
                    foreach (var item in TaskList)
                    {
                        FreeBotItemDto v = new FreeBotItemDto();
                        v._id = item._id.ToString();
                        v.Position = item.Position;
                        v.Location = item.Location;
                        v.Recent30DaysSoldNum = item.Recent30DaysSoldNum;
                        v.ItemName = item.ItemName;
                        v.ShopName = item.ShopName;
                        v.TotalComments = item.TotalComments;
                        v.DetailUrl = item.DetailUrl;
                        v.CreatedAt2 = Convert.ToDateTime(item.CreatedAt).ToString("yyyy/MM/dd");
                        list.Add(v);
                    }
                    var ary = from t in list
                              group t by new { t.ShopName, t.CreatedAt2 } into g
                              select new FreeBotItemDto
                              {
                                  _id = Guid.NewGuid().ToString(),
                                  ShopName = g.Key.ShopName,
                                  Position = Convert.ToInt32(g.Average(p => p.Position)),
                                  TotalComments = g.Sum(p => p.TotalComments),
                                  Recent30DaysSoldNum = g.Sum(p => p.Recent30DaysSoldNum),
                                  CreatedAt2 = g.Key.CreatedAt2
                              };
                    List<FreeBotItemVo> am = new List<FreeBotItemVo>();
                    foreach (var item in ary.GroupBy(x => x.CreatedAt2))
                    {
                        if (pagesize.HasValue)
                        {
                            FreeBotItemVo amm = new FreeBotItemVo();
                            amm.TimeKey = item.Key;//key即为CreateAt2
                            amm.FreeBotItemList = item.Take(Convert.ToInt16(pagesize)).ToList();
                            am.Add(amm);
                        }
                        else
                        {
                            FreeBotItemVo amm = new FreeBotItemVo();
                            amm.TimeKey = item.Key;
                            amm.FreeBotItemList = item.ToList();
                            am.Add(amm);
                        }
                    }
                    return am.OrderBy(x => x.TimeKey).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        //搜索返回数据
        [HttpGet]
        public List<FreeBotItemDto> GetBotResultList(string user_id, string kid, string projectId)
        {
            try
            {
                MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) }, { "ProjectId", new ObjectId(projectId) } };
                List<FreeBotItemDto> list = new List<FreeBotItemDto>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("_id", 1);
                fd.Add("ShopName", 1);
                fd.Add("Location", 1);
                fd.Add("Recent30DaysSoldNum", 1);
                fd.Add("ItemName", 1);
                fd.Add("Price", 1);
                fd.Add("CreatedAt", 1);
                MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                SortByDocument so = new SortByDocument();
                so.Add("CreatedAt", -1);
                var TaskList = col.Find(queryTask).SetSortOrder(so).SetFields(fd);
                foreach (var item in TaskList)
                {
                    FreeBotItemDto v = new FreeBotItemDto();
                    v._id = item._id.ToString();
                    v.ShopName = item.ShopName;
                    v.Location = item.Location;
                    v.ItemName = item.ItemName;
                    v.Price = item.Price;
                    v.Recent30DaysSoldNum = item.Recent30DaysSoldNum;
                    list.Add(v);
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }



        private List<ObjectId> GetIdListFromStr(string idStr)
        {
            List<ObjectId> commendIds = new List<ObjectId>();
            var idArray = idStr.Split(';');
            if (idArray.Length > 0)
            {
                for (int i = 0; i < idArray.Length; i++)
                {
                    commendIds.Add(new ObjectId(idArray[i]));
                }
            }
            return commendIds;
            //  return idArray.Where(x => !string.IsNullOrEmpty(x.ToString())).ToList();
        }

        //监测概览 获取气泡图
        [HttpGet]
        public List<FreeBotItemVo> GetShouyeBubbleList(string user_id, string kid, string projectId)
        {
            //  var queryl = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) } };

            var builder = Builders<FreeShopTimeline>.Filter;
            var filter = builder.Eq(x => x.UId, new ObjectId(user_id)) & builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var strList2 = kid.Split(';').Select(x => new ObjectId(x)).ToList();

            var groupBuilder = Builders<FreeKeywordGroup>.Filter;
            //获取所给节点ID下关键词ID
            var keywordFilter = groupBuilder.In(x => x.CommendCategoryId, strList2) & groupBuilder.Eq(x => x.IsDel, false);
            var keywordList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(keywordFilter).Project(x => x.BaiduCommendId).ToList();//.Select(x => x.ToString()).ToList();

            List<ObjectId> commendIds = new List<ObjectId>();
            if (!string.IsNullOrEmpty(kid))
            {
                var keyIds = GetIdListFromStr(kid);
                if (keyIds.Count > 0)
                {
                    commendIds.AddRange(keyIds);
                }
            }

            if (commendIds.Count > 0)
            {
                filter &= builder.In(x => x.taskId, keywordList);
            }

            //var col = MongoDBHelper.Instance.GetIW2S_SG_level1links();
            //var query = col.Find(filter)
            //   var queryl = new QueryDocument { { "UId", new ObjectId(user_id) }, { "taskId", new ObjectId(kid) } };
            var colFS = MongoDBHelper.Instance.Get_FreeShopTimeline();
            var aryl = colFS.Find(filter).SortByDescending(x => x.Recent30DaysSoldNum).ToList();
            List<FreeBotItemDto> fst = new List<FreeBotItemDto>();

            if (aryl.Count > 0)
            {
                foreach (var item in aryl)
                {
                    FreeBotItemDto fstDto = new FreeBotItemDto()
                    {
                        _id = item._id.ToString(),
                        CreatedAt = Convert.ToDateTime(item.CreatedAt),// item.CreatedAt,
                        CreatedAt2 = item.CreatedAt2,
                        Position = item.Position,
                        Recent30DaysSoldNum = item.Recent30DaysSoldNum,
                        ShopName = item.ShopName,
                        ItemId = item.SId,
                        taskId = item.taskId.ToString(),
                        TotalComments = item.TotalComments,
                        UId = item.UId.ToString()
                    };

                    fst.Add(fstDto);
                }
                List<FreeBotItemVo> am = new List<FreeBotItemVo>();
                foreach (var item in fst.GroupBy(x => x.CreatedAt))
                {

                    //TaskList = col.Find(queryTask).SetFields(fd).SetSortOrder(s).ToList();
                    FreeBotItemVo amm = new FreeBotItemVo();
                    amm.TimeKey = item.Key.ToString();
                    amm.FreeBotItemList = item.OrderByDescending(x => x.Recent30DaysSoldNum).ToList();
                    am.Add(amm);

                }
                return am.OrderBy(x => x.TimeKey).ToList();
            }
            else
            {
                try
                {


                    var strList = kid.Split(';').Select(x => new ObjectId(x)).ToList();

                    //MongoDBClass<FreeBotItem> helper = new MongoDBClass<FreeBotItem>();
                    //var queryTask = new QueryDocument { { "UId", new ObjectId(user_id) }, { "ProjectId", new ObjectId(projectId) } };
                    //queryTask.Add(MongoDB.Driver.Builders.Query.In("taskId", strList) as QueryDocument);// MongoDB.Driver.Builders.Query.In("_id", strList);
                    List<FreeBotItemDto> list = new List<FreeBotItemDto>();
                    //FieldsDocument fd = new FieldsDocument();
                    //fd.Add("_id", 1);
                    //fd.Add("Position", 1);
                    //fd.Add("Location", 1);
                    //fd.Add("Recent30DaysSoldNum", 1);
                    //fd.Add("ItemName", 1);
                    //fd.Add("ShopName", 1);
                    //fd.Add("TotalComments", 1);
                    //fd.Add("DetailUrl", 1);
                    //fd.Add("CreatedAt", 1);
                    //MongoCollection<FreeBotItem> col = new MongoDBClass<FreeBotItem>().GetMongoDB().GetCollection<FreeBotItem>("FreeBotItem");
                    //var TaskList = new List<FreeBotItem>();
                    //SortByDocument s = new SortByDocument();
                    //s.Add("Recent30DaysSoldNum", -1);
                    //TaskList = col.Find(queryTask).SetFields(fd).SetSortOrder(s).ToList();

                    var builder3 = Builders<FreeBotItem>.Filter;
                    var filter3 = builder3.Eq(x => x.UId, new ObjectId(user_id)) & builder3.Eq(x => x.ProjectId, new ObjectId(projectId));
                    List<ObjectId> commendIds3 = new List<ObjectId>();
                    if (!string.IsNullOrEmpty(kid))
                    {
                        var keyIds = GetIdListFromStr(kid);
                        if (keyIds.Count > 0)
                        {
                            commendIds3.AddRange(keyIds);
                        }
                    }
                    if (commendIds3.Count > 0)
                    {
                        filter3 &= builder3.In(x => x.taskId, keywordList);
                    }
                    var colFS3 = MongoDBHelper.Instance.Get_FreeBotItem();
                    var TaskList = colFS3.Find(filter3).SortByDescending(x => x.Recent30DaysSoldNum).ToList();

                    foreach (var item in TaskList)
                    {
                        FreeBotItemDto v = new FreeBotItemDto();
                        v._id = item._id.ToString();
                        v.Position = item.Position;
                        v.Location = item.Location;
                        v.Recent30DaysSoldNum = item.Recent30DaysSoldNum;
                        v.ItemName = item.ItemName;
                        v.ShopName = item.ShopName;
                        v.TotalComments = item.TotalComments;
                        v.DetailUrl = item.DetailUrl;
                        v.taskId = item.taskId.ToString();
                        v.taskName = item.taskName;
                        v.CreatedAt2 = Convert.ToDateTime(item.CreatedAt).ToString("yyyy/MM/dd");
                        list.Add(v);
                    }
                    var ary = from t in list
                              group t by new { t.ShopName, t.CreatedAt2, t.taskId, t.taskName } into g
                              select new FreeBotItemDto
                              {
                                  _id = Guid.NewGuid().ToString(),
                                  ShopName = g.Key.ShopName,
                                  Position = Convert.ToInt32(g.Average(p => p.Position)),
                                  TotalComments = g.Sum(p => p.TotalComments),
                                  Recent30DaysSoldNum = g.Sum(p => p.Recent30DaysSoldNum),
                                  CreatedAt2 = g.Key.CreatedAt2,
                                  taskId = g.Key.taskId,
                                  taskName = g.Key.taskName
                              };
                    List<FreeBotItemVo> am = new List<FreeBotItemVo>();
                    foreach (var item in ary.GroupBy(x => x.CreatedAt2))
                    {

                        FreeBotItemVo amm = new FreeBotItemVo();
                        amm.TimeKey = item.Key;
                        amm.FreeBotItemList = item.ToList();
                        am.Add(amm);

                    }
                    return am.OrderBy(x => x.TimeKey).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }





    }
}