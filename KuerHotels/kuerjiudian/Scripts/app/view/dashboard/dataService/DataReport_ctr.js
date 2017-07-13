var DataReport_ctr = myApp.controller("DataReport_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {
    $scope.baogao_show = false;
    $scope.in_report = -1;
    $rootScope.tab = 1;
    $scope.tab2 = 11;
    $scope.tab_display = true;
    $scope.tab_display2 = true;
    $scope.tab3 = 1;
    $scope.tab_display3 = true;
    $scope.tab4 = 1;
    $scope.tab5 = 1;
    $scope.tab_display4 = true;
    $scope.tab_display5 = true;
    $scope.topN = 10;
    $scope.topN2 = 10;
    $scope.ID = "";
    $rootScope.ID = "";
    $scope.CompanyName = "";
    $scope.BrandName = "";
    $scope.Statement = "";
    $scope.Title1 = "";
    $scope.Title2 = "";
    $scope.Title3 = "";
    $scope.Title4 = "";
    $scope.Title1Content = "";
    $scope.Title2Content = "";
    $scope.Title3Content = "";
    $scope.Title4Content = "";
    $scope.Ending = "";
    $scope.HeadTitle = "";
    $scope.ShopSaleRank = "";
    $scope.Outline = "";
    $scope.DataOutline = "";
    $scope.DataChangeTrend = "";
    $scope.DataDisplay = "";
    $scope.DistrictSaleRank = "";
    $scope.ItemSaleRank = "";
    $scope.ProductSaleRank = "";
    $scope.Deduce = "";
    $scope.FakeShop = "";
    $scope.FakeItem = "";
    $rootScope.LangVersion = "";
    $scope.AuthorizedShopRatio = "";
    $scope.AuthorizedShopTrend = "";
    $scope.Conclusion = "";
    $scope.ConclusionContent = "";
    $scope.AuthorizedShopTrendDesc = "";
    $scope.ConclusionDesc = "";
    $scope.AuthorizedShopRatioDesc = "";
    $scope.OutlineDesc = "";
    $scope.ShopSoldDesc = "";
    $scope.DistrictSoldDesc = "";
    $scope.ItemSoldDesc = "";
    $scope.ProductSoldDesc = "";
    $scope.FakeShopDesc = "";
    $scope.FakeItemDesc = "";
    $scope.TrendDesc = "";
    $scope.PublishDate = "";
    $scope.select_map = "市";
    $scope.total_num = [];
    $scope.isAuthorized_h = 3;
    $scope.reportTime = "";
    $rootScope.page_name = "监测报告";
    $scope.feishouquan = 0;
    $scope.shouqua = 0;
    $scope.ing_report_nei = false;
    $scope.cart_height = {};
    $scope.fixed_left = {
        "height": '600px',
        "top": '185px',
        "width": '20px',
    }
    $scope.weith_s = false;
    $scope.month_list = [];
    $scope.publishState = "";
    $rootScope.LangVersion = 0;

    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    //选择报告
    $scope.select_b_fun = function (num) {
        if (num == 0) {
            $scope.baogao_show = false;
        } else {
            $scope.baogao_show = true;
        }

    }
    //选择报告——月份
    $scope.changeMonth = function (date, sitename) {
        $rootScope.selectMonth = date;
        $scope.reportTime = date;
        $scope.GetMDMReportDetailSetting();
        $cookieStore.put("selectMonth", $rootScope.selectMonth);
        $scope.ing_report_nei = true;
        $rootScope.SiteName_List1 = sitename;
        $timeout(function () {
            $scope.height_get();
        }, 100);
        $rootScope.tab = 1;
        $scope.tab2 = 11;
        $scope.tab3 = 1;
        $scope.tab4 = 1;
        $scope.tab5 = 1;
        $scope.backtop_fun();
    }
    //切换in-report
    $scope.in_report_fun = function (num) {

        $scope.in_report = num;
    }
    $scope.in_report_fun_l = function () {
        $scope.in_report = -1;
    }
    //获取屏幕可见区域的高度
    $scope.height_get = function () {
        var top_n = "185px";
        var height_n = window.screen.availHeight;
        var width_n = window.innerWidth;
        var width_a = document.getElementById("ppt5").style.width
        console.log(width_a);
        height_n = height_n - 350;
        height_n = height_n + "px";
        width_n = width_n - 1545;
        if (width_n > 10) {
            $scope.weith_s = true;
        } else {
            $scope.weith_s = false;
        }
        width_n = width_n + "px";
        $scope.fixed_left = {
            "height": height_n,
            "top": top_n,
            "width": width_n,
        }
    }

    $(window).resize(function () {
        $scope.$apply(function () {
            $scope.height_get();
        });
    });

    // 



    //3.获取项目
    $scope.gettimes = function () {
        $scope.date = new Date;
        $scope.date_year = $filter("date")($scope.date, "yyyy");
        $scope.date_month = $filter("date")($scope.date, "MM");

        $scope.month = ($scope.date_year - 2016) * 12 + $scope.date_month - 06;
        console.log($scope.month);
        $scope.a_y = 2016;
        $scope.a_m = 6;
        $scope.a = "";
        for (var i = 0; i < $scope.month; i++) {
            $scope.a_m = $scope.a_m + 1
            if ($scope.a_m > 12) {
                $scope.a_y = $scope.a_y + 1;
                $scope.a_m = 1;
            };
            if ($scope.a_m < 10) {
                $scope.a = $scope.a_y.toString() + "0" + $scope.a_m.toString();
            } else {
                $scope.a = $scope.a_y.toString() + $scope.a_m.toString();
            }
            $scope.b = parseInt($scope.a)
            $scope.month_list[i] = $scope.b;
        }

        $rootScope.month_list1 = $scope.month_list;
        $cookieStore.put("month_list1", $rootScope.month_list1);
        console.log($rootScope.month_list1);
    };

    //
    $scope.ing_report_nei_fun = function () {
        $scope.ing_report_nei = !$scope.ing_report_nei;
    }

    //4.1获取总体设置
    $scope.GetMDMReportSetting = function () {
        var url = "/api/MDMReport/GetMDMReportSetting?";
        var q = $http.d(url);
        q.success(function (response, status) {
            $scope.IsManager = response.IsManager;
            console.log(response);
            if (response != null) {
                if ($rootScope.LangVersion == 0) {
                    $scope.ReportSetting_list = response.Data[0];
                    console.log($scope.ReportSetting_list);
                } else if ($rootScope.LangVersion == 1) {
                    $scope.ReportSetting_list = response.Data[1];
                    console.log($scope.ReportSetting_list);
                }
                $rootScope.ID = $scope.ReportSetting_list.ID;
                $scope.CompanyName = $scope.ReportSetting_list.CompanyName;
                $scope.BrandName = $scope.ReportSetting_list.BrandName;
                $scope.Statement = $scope.ReportSetting_list.Statement;
                $scope.Title1 = $scope.ReportSetting_list.Title1;
                $scope.Title2 = $scope.ReportSetting_list.Title2;
                $scope.Title3 = $scope.ReportSetting_list.Title3;
                $scope.Title4 = $scope.ReportSetting_list.Title4;
                $scope.Title1Content = $scope.ReportSetting_list.Title1Content;
                $scope.Title2Content = $scope.ReportSetting_list.Title2Content;
                $scope.Title3Content = $scope.ReportSetting_list.Title3Content;
                $scope.Title4Content = $scope.ReportSetting_list.Title4Content;
                $scope.Ending = $scope.ReportSetting_list.Ending;
                $scope.HeadTitle = $scope.ReportSetting_list.HeadTitle;
                $scope.ShopSaleRank = $scope.ReportSetting_list.ShopSaleRank;
                $scope.Outline = $scope.ReportSetting_list.Outline;
                $scope.DataOutline = $scope.ReportSetting_list.DataOutline;
                $scope.DataChangeTrend = $scope.ReportSetting_list.DataChangeTrend;
                $scope.DataDisplay = $scope.ReportSetting_list.DataDisplay;
                $scope.DistrictSaleRank = $scope.ReportSetting_list.DistrictSaleRank;
                $scope.ItemSaleRank = $scope.ReportSetting_list.ItemSaleRank;
                $scope.ProductSaleRank = $scope.ReportSetting_list.ProductSaleRank;
                $scope.Deduce = $scope.ReportSetting_list.Deduce;
                $scope.FakeShop = $scope.ReportSetting_list.FakeShop;
                $scope.FakeItem = $scope.ReportSetting_list.FakeItem;
                $rootScope.LangVersion = $scope.ReportSetting_list.LangVersion;
            }

        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";
        });
    };

    //4.4获取项目列表

    $scope.GetMDMReportList = function () {
        var url = "/api/MDMReport/GetMDMReportList?publishState=" + $scope.publishState;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetMDMReportList = response.ReportTimes;
            console.log(response);
            $scope.changeList();
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";
        });
    };
    //4.5处理报告列表
    $scope.changeList = function () {

        $scope.chineseList = [];
        $scope.chineseTimeList = [];
        $scope.EnglishList = [];
        $scope.EnglishTimeList = [];
        var a = 0;
        var b = 0;
        for (var i = 0; i < $scope.GetMDMReportList.length; i++) {
            if ($scope.GetMDMReportList[i].LangVersion == 0) {
                $scope.chineseList.push({ LangVersion: 0, PublishDate: $scope.GetMDMReportList[i].PublishDate, ReportTime: $scope.GetMDMReportList[i].ReportTime, SiteName: $scope.GetMDMReportList[i].SiteName, Status: $scope.GetMDMReportList[i].Status });
                $scope.chineseTimeList[a] = $scope.GetMDMReportList[i].ReportTime;
                a++;
            } else if ($scope.GetMDMReportList[i].LangVersion == 1) {
                $scope.EnglishList.push({ LangVersion: 1, PublishDate: $scope.GetMDMReportList[i].PublishDate, ReportTime: $scope.GetMDMReportList[i].ReportTime, SiteName: $scope.GetMDMReportList[i].SiteName, Status: $scope.GetMDMReportList[i].Status });
                $scope.EnglishTimeList[b] = $scope.GetMDMReportList[i].ReportTime;
                b++;
            }
        };

        $scope.EnglishList_c = [];
        $scope.chineseTimeList_c = [];
        for (var i = 0; i < $scope.month_list.length; i++) {
            var flag1 = true;
            var flag2 = true;
            for (var j = 0; j < $scope.chineseTimeList.length; j++) {
                if ($scope.month_list[i] == $scope.chineseTimeList[j]) {
                    flag1 = false;
                };
            };
            if (flag1) {
                $scope.chineseTimeList_c.push($scope.month_list[i]);
            }

            for (var j = 0; j < $scope.EnglishTimeList.length; j++) {
                if ($scope.month_list[i] == $scope.EnglishTimeList[j]) {
                    flag2 = false;
                };
            };
            if (flag2) {
                $scope.EnglishList_c.push($scope.month_list[i]);
            }

        };
        for (var j = 0; j < $scope.EnglishList_c.length; j++) {
            $scope.EnglishList.push({ LangVersion: 1, PublishDate: null, ReportTime: $scope.EnglishList_c[j], SiteName: "淘宝/Taobao,lefeng.com,jumei.COM,jd.com", Status: 0 });
        };
        for (var bb = 0; bb < $scope.chineseTimeList_c.length; bb++) {
            $scope.chineseList.push({ LangVersion: 0, PublishDate: null, ReportTime: $scope.chineseTimeList_c[bb], SiteName: "淘宝/Taobao,lefeng.com,jumei.COM,jd.com", Status: 0 });
        };

    };


    //___________________________________________________
    //1.切换table1
    $scope.changeTab = function (id) {
        $rootScope.tab = id;


    };

    $scope.tab_display_fun = function () {
        if ($scope.tab_display == false) {
            $scope.tab_display = true;
        } else {
            $scope.tab_display = false;
        };
    };
    //1.2切换table2
    $scope.changeTab2 = function (id) {
        $scope.tab2 = id;

    };

    $scope.tab_display_fun2 = function () {
        if ($scope.tab_display2 == false) {
            $scope.tab_display2 = true;
        } else {
            $scope.tab_display2 = false;
        };
    };
    //1.3切换table3
    $scope.changeTab3 = function (id) {
        $scope.tab3 = id;

    };

    $scope.tab_display_fun3 = function () {
        if ($scope.tab_display3 == false) {
            $scope.tab_display3 = true;
        } else {
            $scope.tab_display3 = false;
        };
    };

    //1.3切换table4
    $scope.changeTab4 = function (id) {
        $scope.tab4 = id;

    };

    $scope.tab_display_fun4 = function () {
        if ($scope.tab_display4 == false) {
            $scope.tab_display4 = true;
        } else {
            $scope.tab_display4 = false;
        };
    };
    //1.3切换table5
    $scope.changeTab5 = function (id) {
        $scope.tab5 = id;
    };

    $scope.tab_display_fun5 = function () {
        if ($scope.tab_display5 == false) {
            $scope.tab_display5 = true;
        } else {
            $scope.tab_display5 = false;
        };
    };
    //1.点击页面_____________
    $scope.onclick_1 = function () {
        $scope.pieChart1();
        $scope.pieChart2();
        $scope.pieChart3();
    }

    //1.1饼图1

    $scope.pieChart1 = function () {
        var url = "api/MDMReport/GetDashboardTotalitem?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {

            console.log("112");
            console.log(response);
            if (response != null || response != []) {

                $scope.name = [];
                $scope.date = [];
                $scope.TotalItems0 = [];
                for (var i = 0; i < response.length; i++) {
                    $scope.TotalItems0[i] = response[i].TotalItems;
                }

                var Total = 0;
                for (var a = 0; a < $scope.TotalItems0.length; a++) {
                    Total += $scope.TotalItems0[a];
                }

                for (var ii = 0; ii < response.length; ii++) {
                    a1 = ($scope.TotalItems0[ii] / Total) * 100;
                    a = a1.toFixed(1) + "%";
                    $scope.name[ii] = response[ii].SiteName + "：" + $scope.TotalItems0[ii].toFixed(0) + "(" + a + ")";;
                    $scope.date.push({ value: response[ii].TotalItems, name: $scope.name[ii] })
                }


                if ($rootScope.chinese == true) {
                    var title_text = '在线宝贝数';
                    var name1 = '链接数';
                } else {
                    var title_text = 'Total Item';
                    var name1 = 'Links';
                }
                //基于准备好的dom，初始化echarts实例
                //指定图表的配置项和数据
                var myChart = echarts.init(document.getElementById('pieChart1'));
                option = {
                    title: {
                        text: title_text,
                        x: 'center',
                        y: '0%'
                    },
                    tooltip: {
                        trigger: 'item',
                        formatter: "{a} <br/>{b}"
                    },
                    legend: {
                        x: 'center',
                        y: '70%',
                        orient: 'vertical',
                        data: $scope.name
                    },
                    series: [
                          {
                              name: name1,
                              type: 'pie',
                              radius: ['40%', '60%'],
                              center: ['50%', 135],
                              avoidLabelOverlap: false,
                              label: {
                                  normal: {
                                      show: false,
                                      position: 'center'
                                  },
                                  emphasis: {
                                      show: false,
                                      textStyle: {
                                          fontSize: '25',
                                          fontWeight: 'bold'
                                      }
                                  }
                              },
                              lableLine: {
                                  normal: {
                                      show: false
                                  },
                                  emphasis: {
                                      show: true
                                  }
                              },
                              labelLine: {
                                  normal: {
                                      show: false
                                  }
                              },
                              data: $scope.date
                          }
                    ]
                };
            };
            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //1.2饼图2

    $scope.pieChart2 = function () {
        var url = "api/MDMReport/GetDashboardTotalStores?reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);

            $scope.name = [];
            $scope.date = [];
            $scope.TotalItems0 = [];
            for (var i = 0; i < response.length; i++) {
                $scope.TotalItems0[i] = response[i].TotalStores;
            }

            var Total = 0;
            for (var a = 0; a < $scope.TotalItems0.length; a++) {
                Total += $scope.TotalItems0[a];
            }

            for (var ii = 0; ii < response.length; ii++) {
                a1 = ($scope.TotalItems0[ii] / Total) * 100;
                a = a1.toFixed(1) + "%";
                $scope.name[ii] = response[ii].SiteName + "：" + $scope.TotalItems0[ii].toFixed(0) + "(" + a + ")";;
                $scope.date.push({ value: response[ii].TotalStores, name: $scope.name[ii] })
            }
            if ($rootScope.chinese == true) {
                var title_text = '在线店铺数';
                var name1 = '店铺数';
            } else {
                var title_text = 'Total stores';
                var name1 = 'Stores';
            }
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('pieChart2'));
            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b}"
                },
                legend: {
                    x: 'center',
                    y: '70%',
                    orient: 'vertical',
                    data: $scope.name
                },
                series: [
                    {
                        name: name1,
                        type: 'pie',
                        radius: ['40%', '60%'],
                        center: ['50%', 135],
                        avoidLabelOverlap: false,
                        label: {
                            normal: {
                                show: false,
                                position: 'center'
                            },
                            emphasis: {
                                show: false,
                                textStyle: {
                                    fontSize: '25',
                                    fontWeight: 'bold'
                                }
                            }
                        },
                        labelLine: {
                            normal: {
                                show: false
                            }
                        },
                        data: $scope.date
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };

    //1.1.3饼图1

    $scope.pieChart3 = function () {
        var url = "api/MDMReport/GetDashboardTotalSiteSolds?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);

            $scope.name = [];
            $scope.date = [];
            $scope.TotalItems0 = [];
            for (var i = 0; i < response.length; i++) {
                $scope.TotalItems0[i] = response[i].SoldVol;
            }

            var Total = 0;
            for (var a = 0; a < $scope.TotalItems0.length; a++) {
                Total += $scope.TotalItems0[a];
            }

            for (var ii = 0; ii < response.length; ii++) {
                a1 = ($scope.TotalItems0[ii] / Total) * 100;
                a = a1.toFixed(1) + "%";
                $scope.name[ii] = response[ii].SiteName + "：" + "￥" + $scope.TotalItems0[ii].toFixed(1) + "(" + a + ")";;
                $scope.date.push({ value: response[ii].SoldVol, name: $scope.name[ii] })
            }
            if ($rootScope.chinese == true) {
                var title_text = '当月销售额';
                var name1 = '销售额';
            } else {
                var title_text = 'Sale value';
                var name1 = 'Sale value';
            }
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('pieChart3'));
            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b}"
                },
                legend: {
                    x: 'center',
                    y: '70%',
                    orient: 'vertical',
                    data: $scope.name
                },
                series: [
                    {
                        name: name1,
                        type: 'pie',
                        radius: ['40%', '60%'],
                        center: ['50%', 135],
                        avoidLabelOverlap: false,
                        label: {
                            normal: {
                                show: false,
                                position: 'center'
                            },
                            emphasis: {
                                show: false,
                                textStyle: {
                                    fontSize: '25',
                                    fontWeight: 'bold'
                                }
                            }
                        },
                        labelLine: {
                            normal: {
                                show: false
                            }
                        },
                        data: $scope.date
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };

    //1.2点击页面1.2_____________
    $scope.onclick_12 = function () {
        $scope.shopgraph();
        $scope.Histogram();
        $scope.Linegraph();
    }

    //1.2.1柱状图
    $scope.Histogram = function () {
        var url = "api/MDMReport/GetDashboardAliSiteHistorySummaryBySoldVol?reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.taobao_count = [];
            $scope.jd_count = [];
            $scope.Tmall_count = [];
            $scope.lefeng_count = [];
            $scope.jumei_count = [];
            $scope.date = [];
            $scope.date1 = [];
            var a = 0;
            var b = 0;
            var c = 0;
            var d = 0;
            var z = 0;
            for (var i = 0; i < response.length; i++) {
                $scope.date1[i] = response[i].WeekBefore;

                if (response[i].SiteName == "淘宝/Taobao") {
                    $scope.taobao_count[a] = (response[i].SoldVol / 10000).toFixed(2)

                    a++
                } else if (response[i].SiteName == "lefeng.com") {
                    $scope.lefeng_count[b] = (response[i].SoldVol / 10000).toFixed(2);
                    b++
                } else if (response[i].SiteName == "jd.com") {
                    $scope.jd_count[c] = (response[i].SoldVol / 10000).toFixed(2);
                    c++
                } else if (response[i].SiteName == "jumei.COM") {
                    $scope.jumei_count[d] = (response[i].SoldVol / 10000).toFixed(2);
                    d++
                } else if (response[i].SiteName == "Tmall") {
                    $scope.Tmall_count[z] = (response[i].SoldVol / 10000).toFixed(2);
                    z++
                }
            };
            //获取时间
            var ddd = $scope.date1
            var uq = {};
            var rq = [];
            var prefix = '';
            for (var i = 0; i < $scope.date1.length; i++) {
                if (typeof ddd[i] == 'string') {
                    prefix = '_str';
                } else {
                    prefix = '';
                }
                if (!uq[ddd[i] + prefix]) {
                    uq[ddd[i] + prefix] = true;
                    rq.push(ddd[i]);
                }
            }
            $scope.date = rq;
            console.log($scope.date);

            $scope.date_All = [];
            $scope.name = [];
            $scope.bijiao = [];
            var e = 0;
            if ($scope.taobao_count[0]) {
                $scope.name[e] = "淘宝/Taobao";
                $scope.date_All[e] = {
                    name: '淘宝/Taobao',
                    type: 'line',
                    yAxisIndex: 1,
                    data: $scope.taobao_count
                },
                e = e + 1
            }
            if ($scope.jd_count[0]) {
                $scope.name[e] = "jd.com";
                $scope.date_All[e] = {
                    name: 'jd.com',
                    type: 'line',
                    data: $scope.jd_count
                },
                e = e + 1
            }
            if ($scope.jumei_count[0]) {
                $scope.name[e] = "jumei.COM";
                $scope.date_All[e] = {
                    name: 'jumei.COM',
                    type: 'line',
                    data: $scope.jumei_count
                },
                e = e + 1
            }
            if ($scope.lefeng_count[0]) {
                $scope.name[e] = "lefeng.com";
                $scope.date_All[e] = {
                    name: 'lefeng.com',
                    type: 'line',
                    data: $scope.lefeng_count
                },
                e = e + 1
            };
            if ($scope.Tmall_count[0]) {
                $scope.name[e] = "Tmall";
                $scope.date_All[e] = {
                    name: 'Tmall',
                    type: 'line',
                    yAxisIndex: 1,
                    data: $scope.Tmall_count,
                },
                e = e + 1
            };

            if ($rootScope.chinese == true) {
                var title_text = '月销售额变化趋势';
                var y_name1 = '其它平台(万元)';
                var y_name2 = '淘宝/Tmall(万元)';
            } else {
                var title_text = 'Monthly sales trends';
                var y_name1 = 'Others(ten thousand)';
                var y_name2 = 'Taobao/Tmall(ten thousand)';
            }


            $scope.zhutu = function () {
                //基于准备好的dom，初始化echarts实例
                //指定图表的配置项和数据
                var myChart = echarts.init(document.getElementById('Histogram'));
                option = {
                    title: {
                        text: title_text,
                        x: 'center',
                        y: '0%'
                    },
                    legend: {
                        x: 'center',
                        y: '88%',
                        data: $scope.name
                    },
                    grid: {
                        x: '14%',
                        x2: '14%',
                        y: '20%',
                        y2: '22%'
                    },
                    dataZoom: [
                    {
                        show: true,
                        realtime: true,
                        start: 80,
                        end: 100,
                        height: 10,
                        y: '85%'
                    },
                    ],
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                            type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
                        },
                        //formatter: "{a}:{b}万元"
                    },
                    xAxis: [
                        {
                            type: 'category',
                            data: $scope.date
                        }
                    ],
                    yAxis: [
                          {
                              type: 'value',
                              scale: true,
                              name: y_name1,
                              boundaryGap: [0.2, 0.2]
                          },
                          {
                              type: 'value',
                              scale: true,
                              name: y_name2,
                              boundaryGap: [0.2, 0.2]
                          }
                    ],
                    series: $scope.date_All
                };
                // 使用刚指定的配置项和数据显示图表。
                myChart.setOption(option);
            }
            $scope.zhutu();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //1.2.2折线图
    $scope.Linegraph = function () {
        var url = "api/MDMReport/GetDashboardAliSiteHistorySummary?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.taobao_count = [];
            $scope.jd_count = [];
            $scope.lefeng_count = [];
            $scope.jumei_count = [];
            $scope.Tmall_count = [];
            $scope.date = [];
            $scope.date1 = [];
            var a = 0;
            var b = 0;
            var c = 0;
            var d = 0;
            var z = 0;
            for (var i = 0; i < response.length; i++) {
                $scope.date1[i] = response[i].WeekBefore;
                if (response[i].SiteName == "淘宝/Taobao") {
                    $scope.taobao_count[a] = response[i].ItemCount;
                    a++

                } else if (response[i].SiteName == "lefeng.com") {
                    $scope.lefeng_count[b] = response[i].ItemCount;
                    b++

                } else if (response[i].SiteName == "jd.com") {
                    $scope.jd_count[c] = response[i].ItemCount;
                    c++

                } else if (response[i].SiteName == "jumei.COM") {
                    $scope.jumei_count[d] = response[i].ItemCount;
                    d++
                } else if (response[i].SiteName == "Tmall") {
                    $scope.Tmall_count[z] = response[i].ItemCount;
                    z++
                }
            }

            var ddd = $scope.date1
            var uq = {};
            var rq = [];
            var prefix = '';
            for (var i = 0; i < $scope.date1.length; i++) {
                if (typeof ddd[i] == 'string') {
                    prefix = '_str';
                } else {
                    prefix = '';
                }
                if (!uq[ddd[i] + prefix]) {
                    uq[ddd[i] + prefix] = true;
                    rq.push(ddd[i]);
                }
            }
            $scope.date = rq;
            console.log($scope.date);

            $scope.date_All = [];
            $scope.name = [];
            var e = 0;
            if ($scope.taobao_count[0]) {
                $scope.name[e] = "淘宝/Taobao";
                $scope.date_All[e] = {
                    name: '淘宝/Taobao',
                    type: 'line',
                    yAxisIndex: 1,
                    data: $scope.taobao_count
                },
                e = e + 1
            }
            if ($scope.jd_count[0]) {
                $scope.name[e] = "jd.com";
                $scope.date_All[e] = {
                    name: 'jd.com',
                    type: 'line',
                    data: $scope.jd_count
                },
                e = e + 1
            }
            if ($scope.jumei_count[0]) {
                $scope.name[e] = "jumei.COM";
                $scope.date_All[e] = {
                    name: 'jumei.COM',
                    type: 'line',
                    data: $scope.jumei_count
                },
                e = e + 1
            }
            if ($scope.lefeng_count[0]) {
                $scope.name[e] = "lefeng.com";
                $scope.date_All[e] = {
                    name: 'lefeng.com',
                    type: 'line',
                    data: $scope.lefeng_count
                },
                e = e + 1
            };
            if ($scope.Tmall_count[0]) {
                $scope.name[e] = "Tmall";
                $scope.date_All[e] = {
                    name: 'Tmall',
                    type: 'line',
                    data: $scope.Tmall_count,
                },
                e = e + 1
            };


            if ($rootScope.chinese == true) {
                var title_text = '新增宝贝数变化趋势';
                var y_name1 = '其它平台';
                var y_name2 = '淘宝/Tmall';
            } else {
                var title_text = 'Monthly new item trends';
                var y_name1 = 'Others';
                var y_name2 = 'Taobao/Tmall';
            }


            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('Linegraph'));

            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                legend: {
                    x: 'center',
                    y: '88%',
                    data: $scope.name
                },
                grid: {
                    x: '14%',
                    x2: '14%',
                    y: '20%',
                    y2: '22%'
                },
                dataZoom: [
                {
                    show: true,
                    realtime: true,
                    start: 80,
                    end: 100,
                    height: 10,
                    y: '85%'
                },
                ],
                yAxis: [
                      {
                          type: 'value',
                          scale: true,
                          name: y_name1,
                          boundaryGap: [0.2, 0.2]
                      },
                      {
                          type: 'value',
                          scale: true,
                          name: y_name2,
                          boundaryGap: [0.2, 0.2]
                      }
                ],

                tooltip: {
                    trigger: 'axis'
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: $scope.date,
                },
                series: $scope.date_All
            };


            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //1.2.3折线图
    //$scope.GetDashboardTotalStores_1 = function () {
    //    var url = "api/MDMReport/GetDashboardTotalStores?reportTime=" + $scope.reportTime;
    //    var q = $http.get(url);
    //    q.success(function (response, status) {
    //        console.log("总数");
    //        console.log(response);
    //        for (var i = 0; i < response.length;i++){
    //            $scope.total_num.push({ SiteName: '', ShopCount: '' });
    //            $scope.total_num[i].SiteName = response[i].SiteName;
    //            $scope.total_num[i].ShopCount = response[i].TotalStores;
    //        }

    //    });
    //    q.error(function (response) {
    //        $scope.error = "服务器连接出错";
    //        $scope.isActiveStart = false;
    //    });
    //};
    $scope.shopgraph = function () {
        var url = "api/MDMReport/GetDashboardShops?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.taobao_count = [];
            $scope.jd_count = [];
            $scope.lefeng_count = [];
            $scope.Tmall_count = [];
            $scope.jumei_count = [];
            $scope.date = [];
            var a = 0;
            var b = 0;
            var c = 0;
            var d = 0;
            var z = 0;
            $scope.date1 = [];
            for (var i = 0; i < response.length; i++) {
                $scope.date1[i] = response[i].WeekBefore;
                if (response[i].SiteName == "淘宝/Taobao") {
                    $scope.taobao_count[a] = response[i].ShopCount;
                    a++
                } else if (response[i].SiteName == "lefeng.com") {
                    $scope.lefeng_count[b] = response[i].ShopCount;
                    b++

                } else if (response[i].SiteName == "jd.com") {
                    $scope.jd_count[c] = response[i].ShopCount;
                    c++

                } else if (response[i].SiteName == "jumei.COM") {
                    $scope.jumei_count[d] = response[i].ShopCount;
                    d++
                } else if (response[i].SiteName == "Tmall") {
                    $scope.Tmall_count[z] = response[i].ShopCount;
                    z++

                }
            }
            var ddd = $scope.date1
            var uq = {};
            var rq = [];
            var prefix = '';
            for (var i = 0; i < $scope.date1.length; i++) {
                if (typeof ddd[i] == 'string') {
                    prefix = '_str';
                } else {
                    prefix = '';
                }
                if (!uq[ddd[i] + prefix]) {
                    uq[ddd[i] + prefix] = true;
                    rq.push(ddd[i]);
                }
            }
            $scope.date = rq;
            console.log($scope.date);


            $scope.date_All = [];
            $scope.name = [];
            var e = 0;
            if ($scope.taobao_count[0]) {
                $scope.name[e] = "淘宝/Taobao";
                $scope.date_All[e] = {
                    name: '淘宝/Taobao',
                    type: 'line',
                    yAxisIndex: 1,
                    data: $scope.taobao_count
                },
                e = e + 1
            }
            if ($scope.jd_count[0]) {
                $scope.name[e] = "jd.com";
                $scope.date_All[e] = {
                    name: 'jd.com',
                    type: 'line',
                    data: $scope.jd_count
                },
                e = e + 1
            }
            if ($scope.jumei_count[0]) {
                $scope.name[e] = "jumei.COM";
                $scope.date_All[e] = {
                    name: 'jumei.COM',
                    type: 'line',
                    data: $scope.jumei_count
                },
                e = e + 1
            }
            if ($scope.lefeng_count[0]) {
                $scope.name[e] = "lefeng.com";
                $scope.date_All[e] = {
                    name: 'lefeng.com',
                    type: 'line',
                    data: $scope.lefeng_count
                },
                e = e + 1
            };
            if ($scope.Tmall_count[0]) {
                $scope.name[e] = "Tmall";
                $scope.date_All[e] = {
                    name: 'Tmall',
                    type: 'line',
                    data: $scope.Tmall_count,
                },
                e = e + 1
            };
            console.log($scope.name);
            console.log($scope.date_All);

            if ($rootScope.chinese == true) {
                var title_text = '新增店铺数变化趋势';
                var y_name1 = '其它平台';
                var y_name2 = '淘宝/Tmall';
            } else {
                var title_text = 'Monthly new stores trends';
                var y_name1 = 'Others';
                var y_name2 = 'Taobao/Tmall';
            }

            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('shopgraph'));

            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                legend: {
                    x: 'center',
                    y: '88%',
                    data: $scope.name
                },
                grid: {
                    x: '14%',
                    x2: '14%',
                    y: '20%',
                    y2: '22%'
                },
                dataZoom: [
                {
                    show: true,
                    realtime: true,
                    start: 80,
                    end: 100,
                    height: 10,
                    y: '85%'
                },
                ],
                yAxis: [
                      {
                          type: 'value',
                          scale: true,
                          name: y_name1,
                          boundaryGap: [0.2, 0.2]
                      },
                      {
                          type: 'value',
                          scale: true,
                          name: y_name2,
                          boundaryGap: [0.2, 0.2]
                      }
                ],

                tooltip: {
                    trigger: 'axis'
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: $scope.date,
                },
                series: $scope.date_All
            };


            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };

    //1.3点击页面1_____________
    $scope.onclick_13 = function () {
        $scope.pieChart131();
        $scope.pieChart132();
        $scope.pieChart133();
    }

    //1.3.1饼图1

    $scope.pieChart131 = function () {
        var url = "api/MDMReport/GetDashboardAuthorizeItemTotal?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {

            console.log(response);
            for (var i = 0; i < response.length; i++) {
                if (response[i].IsAuthorized == true) {
                    $scope.shouquan = response[i].Total;
                    $scope.shouquan = Number($scope.shouquan);
                } else {
                    $scope.feishouquan = response[i].Total;
                    $scope.feishouquan = Number($scope.feishouquan);
                }
            }
            var aa = $scope.shouquan / ($scope.shouquan + $scope.feishouquan + 0.01) * 100;
            if (aa > 0) {
                var a = aa.toFixed(1) + "%";
            } else {
                var a = '0%';
            }
            var bb = $scope.feishouquan / ($scope.shouquan + $scope.feishouquan + 0.01) * 100;
            if (bb > 0) {
                var b = bb.toFixed(1) + "%";
            } else {
                var b = '0%';
            }
            if ($rootScope.chinese == true) {
                var title_text = '授权与非授权在线宝贝数占比';
                var data_name1 = ['授权在线宝贝', '非授权在线宝贝'];
                var name_S = '在线宝贝数';
                var name_1 = '授权：' + $scope.shouquan + '(' + a + ')';
                var name_2 = '非授：' + $scope.feishouquan + '(' + b + ')';
            } else {
                var title_text = 'Authorized Proportion';
                var data_name1 = ['Authorized Products ', 'Unauthorized Products '];
                var name_S = 'Products';
                var name_1 = 'Authorized: ' + $scope.shouquan + '(' + a + ')';
                var name_2 = 'Unauthorized: ' + $scope.feishouquan + '(' + b + ')';
            }

            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('pieChart131'));
            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b}"
                },
                legend: {
                    x: 'center',
                    y: '82%',
                    orient: 'vertical',
                    data: [name_1, name_2]
                },
                series: [
                    {
                        name: name_S,
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '50%'],
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: false
                            }
                        },
                        data: [
                            { value: $scope.shouquan, name: name_1 },
                            { value: $scope.feishouquan, name: name_2 },

                        ],
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //1.3.2饼图2

    $scope.pieChart132 = function () {
        var url = "api/MDMReport/GetDashboardAuthorizeShopTotal?reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);

            for (var i = 0; i < response.length; i++) {
                if (response[i].IsAuthorized == true) {
                    $scope.shouquan = response[i].Total;
                    $scope.shouquan = Number($scope.shouquan);
                } else {
                    $scope.feishouquan = response[i].Total;
                    $scope.feishouquan = Number($scope.feishouquan);
                }
            }
            var aa = $scope.shouquan / ($scope.shouquan + $scope.feishouquan + 0.01) * 100;
            if (aa > 0) {
                var a = aa.toFixed(1) + "%";
            } else {
                var a = '0%';
            }
            var bb = $scope.feishouquan / ($scope.shouquan + $scope.feishouquan + 0.01) * 100;
            if (bb > 0) {
                var b = bb.toFixed(1) + "%";
            } else {
                var b = '0%';
            }
            if ($rootScope.chinese == true) {
                var title_text = '授权与非授权在线店铺占比';
                var data_name1 = ['授权在线店铺', '非授权在线店铺'];
                var name_S = '在线店铺数';
                var name_1 = '授权：' + $scope.shouquan + '(' + a + ')';
                var name_2 = '非授：' + $scope.feishouquan + '(' + b + ')';
            } else {
                var title_text = 'Proportion of Authorized stores';
                var data_name1 = ['Authorized Stores ', 'Unauthorized Stores '];
                var name_S = 'Stores';
                var name_1 = 'Authorized: ' + $scope.shouquan + '(' + a + ')';
                var name_2 = 'Unauthorized: ' + $scope.feishouquan + '(' + b + ')';
            }
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('pieChart132'));
            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b}"
                },
                legend: {
                    x: 'center',
                    y: '82%',
                    orient: 'vertical',
                    data: [name_1, name_2]
                },
                series: [
                    {
                        name: name_S,
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '50%'],
                        data: [
                            { value: $scope.shouquan, name: name_1 },
                            { value: $scope.feishouquan, name: name_2 },
                        ],
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: false
                            }
                        },
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };

    //1.3.3饼图1
    $scope.pieChart133 = function () {
        var url = "api/MDMReport/GetDashboardAuthorizeSaleTotal?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            for (var i = 0; i < response.length; i++) {
                if (response[i].IsAuthorized == true) {
                    $scope.shouquan = response[i].Total.toFixed(2)
                    $scope.shouquan = Number($scope.shouquan);
                } else {
                    $scope.feishouquan = response[i].Total.toFixed(2);
                    $scope.feishouquan = Number($scope.feishouquan);
                }
            }
            var aa = $scope.shouquan / ($scope.shouquan + $scope.feishouquan + 0.01) * 100;
            if (aa > 0) {
                var a = aa.toFixed(1) + "%";
            } else {
                var a = '0%';
            }
            var bb = $scope.feishouquan / ($scope.shouquan + $scope.feishouquan + 0.01) * 100;
            if (bb > 0) {
                var b = bb.toFixed(1) + "%";
            } else {
                var b = '0%';
            }
            if ($rootScope.chinese == true) {
                var title_text = '授权与非授权店铺当月销售总额占比';
                var data_name1 = ['授权店铺当月销售总额', '非授权店铺当月销售总额'];
                var name_S = '月销售总额';
                var name_1 = '授权：' + '￥' + $scope.shouquan + '(' + a + ')';
                var name_2 = '非授：' + '￥' + $scope.feishouquan + '(' + b + ')';
            } else {
                var title_text = 'Proportion of Monthly sale';
                var data_name1 = ['Authorized stores Monthly sale', 'Unauthorized stores Monthly sale'];
                var name_S = 'Monthly sale value';
                var name_1 = 'Authorized: ' + '￥' + $scope.shouquan + '(' + a + ')';
                var name_2 = 'Unauthorized: ' + '￥' + $scope.feishouquan + '(' + b + ')';
            }
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据

            var myChart = echarts.init(document.getElementById('pieChart133'));
            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b}"
                },
                legend: {
                    x: 'center',
                    y: '82%',
                    orient: 'vertical',
                    data: [name_1, name_2]
                },
                series: [
                    {
                        name: name_S,
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '50%'],
                        data: [
                            { value: $scope.shouquan, name: name_1 },
                            { value: $scope.feishouquan, name: name_2 },

                        ],
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: false
                            }
                        },
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };



    //1.4点击页面1.4________
    $scope.onclick_14 = function () {
        $scope.Histogram141();
        $scope.Linegraph142();
        $scope.shopgraph143();
    }
    //1.4.1柱状图
    $scope.Histogram141 = function () {
        var url = "api/MDMReport/GetDashboardAuthorizeHistoryBySoldVol?reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.feishouquan = [];
            $scope.shouquan = [];

            $scope.date = [];
            var a = 0;
            var b = 0;
            for (var i = 0; i < response.length; i++) {
                if (response[i].IsAuthorized == false) {
                    $scope.feishouquan[a] = (response[i].SoldVol / 10000).toFixed(2);
                    $scope.date[a] = response[i].WeekBefore;
                    a++
                } else if (response[i].IsAuthorized == true) {
                    $scope.shouquan[b] = (response[i].SoldVol / 10000).toFixed(2);
                    b++
                }
            }
            $scope.date_All = [];
            $scope.name = [];
            var e = 0;
            if ($rootScope.chinese == true) {
                var title_text = '授权与非授权月销售额变化趋势';
                var y_name1 = '授权月销售额/(万元)';
                var y_name2 = '非授权月销售额/(万元)';
                var y_name3 = '月销售额/(万元)';
            } else {
                var title_text = 'Trend of monthly sale';
                var y_name1 = 'Authorized stores Monthly sale（￥10k）';
                var y_name2 = 'Unauthorized stores Monthly sale（￥10k）';
                var y_name3 = ' Monthly sale（￥10k）';
            }

            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('Histogram141'));

            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                legend: {
                    x: 'center',
                    y: '88%',
                    data: [y_name1, y_name2]
                },
                grid: {
                    x: '14%',
                    x2: '14%',
                    y: '20%',
                    y2: '22%'
                },
                dataZoom: [
                {
                    show: true,
                    realtime: true,
                    start: 80,
                    end: 100,
                    height: 10,
                    y: '85%'
                },
                ],
                tooltip: {
                    trigger: 'axis'
                },

                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: $scope.date
                },
                yAxis: {
                    type: 'value',
                    name: y_name3,
                    axisLabel: {
                        formatter: '{value}'
                    }
                },
                series: [
                    {
                        name: y_name1,
                        type: 'line',
                        data: $scope.shouquan,
                        markPoint: {
                            data: [
                                { type: 'max', name: '最大值' },
                                { type: 'min', name: '最小值' }
                            ]
                        },
                        markLine: {
                            data: [
                                { type: 'average', name: '平均值/(万元)' }
                            ]
                        }
                    },
                    {
                        name: y_name2,
                        type: 'line',
                        data: $scope.feishouquan,
                        markPoint: {
                            data: [
                                { name: '周最低', value: -2, xAxis: 1, yAxis: -1.5 }
                            ]
                        },
                        markLine: {
                            data: [
                                { type: 'average', name: '平均值' },
                                [{
                                    symbol: 'none',
                                    x: '90%',
                                    yAxis: 'max'
                                }, {
                                    symbol: 'circle',
                                    label: {
                                        normal: {
                                            position: 'start',
                                            formatter: '最大值'
                                        }
                                    },
                                    type: 'max',
                                    name: '最高点'
                                }]
                            ]
                        }
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);

        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //1.4.2折线图
    $scope.Linegraph142 = function () {
        var url = "api/MDMReport/GetDashboardAuthorizedItemHistory?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.feishouquan = [];
            $scope.shouquan = [];

            $scope.date = [];
            var a = 0;
            var b = 0;
            for (var i = 0; i < response.length; i++) {
                if (response[i].IsAuthorized == false) {
                    $scope.feishouquan[a] = response[i].ItemCount;
                    $scope.date[a] = response[i].WeekBefore;
                    a++
                } else if (response[i].IsAuthorized == true) {
                    $scope.shouquan[b] = response[i].ItemCount;
                    b++
                }
            }
            $scope.date_All = [];
            $scope.name = [];
            var e = 0;

            if ($rootScope.chinese == true) {
                var title_text = '授权与非授权新增宝贝变化趋势';
                var y_name1 = '授权新增宝贝';
                var y_name2 = '非授权新增宝贝';
                var y_name3 = '新增宝贝数';
            } else {
                var title_text = 'Trend of new item';
                var y_name1 = 'Authorized new item';
                var y_name2 = 'Unauthorized new item';
                var y_name3 = 'New item';
            }
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('Linegraph142'));

            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'axis'
                },
                dataZoom: [
                 {
                     show: true,
                     realtime: true,
                     start: 80,
                     end: 100,
                     height: 10,
                     y: '85%'
                 },
                ],
                legend: {
                    x: 'center',
                    y: '88%',
                    data: [y_name1, y_name2]
                },
                grid: {
                    x: '14%',
                    x2: '14%',
                    y: '20%',
                    y2: '22%'
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: $scope.date
                },
                yAxis: {
                    type: 'value',
                    name: y_name3,
                    axisLabel: {
                        formatter: '{value}'
                    }
                },
                series: [
                    {
                        name: y_name1,
                        type: 'line',
                        data: $scope.shouquan,
                        markPoint: {
                            data: [
                                { type: 'max', name: '最大值' },
                                { type: 'min', name: '最小值' }
                            ]
                        },
                        markLine: {
                            data: [
                                { type: 'average', name: '平均值' }
                            ]
                        }
                    },
                    {
                        name: y_name2,
                        type: 'line',
                        data: $scope.feishouquan,
                        markPoint: {
                            data: [
                                { name: '最低', value: -2, xAxis: 1, yAxis: -1.5 }
                            ]
                        },
                        markLine: {
                            data: [
                                { type: 'average', name: '平均值' },
                                [{
                                    symbol: 'none',
                                    x: '90%',
                                    yAxis: 'max'
                                }, {
                                    symbol: 'circle',
                                    label: {
                                        normal: {
                                            position: 'start',
                                            formatter: '最大值'
                                        }
                                    },
                                    type: 'max',
                                    name: '最高点'
                                }]
                            ]
                        }
                    }
                ]
            };
            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //1.4.3折线图
    $scope.shopgraph143 = function () {
        var url = "api/MDMReport/GetDashboardAuthorizedShopHistory?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.feishouquan = [];
            $scope.shouquan = [];

            $scope.date = [];
            var a = 0;
            var b = 0;
            for (var i = 0; i < response.length; i++) {
                if (response[i].IsAuthorized == false) {
                    $scope.feishouquan[a] = response[i].ShopCount;
                    $scope.date[a] = response[i].WeekBefore;
                    a++
                } else if (response[i].IsAuthorized == true) {
                    $scope.shouquan[b] = response[i].ShopCount;
                    b++
                }
            }
            $scope.date_All = [];
            $scope.name = [];
            var e = 0;

            if ($rootScope.chinese == true) {
                var title_text = '授权与非授权新增店铺变化趋势';
                var y_name1 = '授权新增店铺';
                var y_name2 = '非授权新增店铺';
                var y_name3 = '新增店铺数';
            } else {
                var title_text = 'Trend of new stores';
                var y_name1 = 'Authorized new stores';
                var y_name2 = 'Unauthorized new stores';
                var y_name3 = 'New stores';
            }
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('shopgraph143'));

            option = {
                title: {
                    text: title_text,
                    x: 'center',
                    y: '0%'
                },
                tooltip: {
                    trigger: 'axis'
                },
                dataZoom: [
                 {
                     show: true,
                     realtime: true,
                     start: 80,
                     end: 100,
                     height: 10,
                     y: '85%'
                 },
                ],
                grid: {
                    x: '14%',
                    x2: '14%',
                    y: '20%',
                    y2: '22%'
                },
                legend: {
                    x: 'center',
                    y: '88%',
                    data: [y_name1, y_name2]
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: $scope.date
                },
                yAxis: {
                    type: 'value',
                    name: y_name3,
                    axisLabel: {
                        formatter: '{value}'
                    }
                },
                series: [
                    {
                        name: y_name1,
                        type: 'line',
                        data: $scope.shouquan,
                        markPoint: {
                            data: [
                                { type: 'max', name: '最大值' },
                                { type: 'min', name: '最小值' }
                            ]
                        },
                        markLine: {
                            data: [
                                { type: 'average', name: '平均值' }
                            ]
                        }
                    },
                    {
                        name: y_name2,
                        type: 'line',
                        data: $scope.feishouquan,
                        markPoint: {
                            data: [
                                { name: '周最低', value: -2, xAxis: 1, yAxis: -1.5 }
                            ]
                        },
                        markLine: {
                            data: [
                                { type: 'average', name: '平均值' },
                                [{
                                    symbol: 'none',
                                    x: '90%',
                                    yAxis: 'max'
                                }, {
                                    symbol: 'circle',
                                    label: {
                                        normal: {
                                            position: 'start',
                                            formatter: '最大值'
                                        }
                                    },
                                    type: 'max',
                                    name: '最高点'
                                }]
                            ]
                        }
                    }
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };


    //2.1地图
    //2.2点击事件
    $scope.onclick_21 = function () {
        $scope.chainMap2();
        $scope.getlatlong();
    }
    //中国地图_市
    $scope.chainMap2 = function () {
        var url = "api/MDMReport/GetLocationList?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.date = [];
            $scope.max_num = 0;
            for (var i = 0; i < response.length; i++) {
                $scope.date.push({ name: "", value: "" });
                $scope.date[i].name = response[i].Location;
                $scope.date[i].value = response[i].Recent30SoldAmount.toFixed(2);
                if ($scope.max_num < response[i].Recent30SoldAmount) {
                    $scope.max_num = response[i].Recent30SoldAmount;
                }
            };
            var num_yuan = $scope.max_num;
            var num_lose = $scope.max_num / 250;
            if ($rootScope.chinese == true) {
                var title_text = '按每月销售额排序';
            } else {
                var title_text = 'Monthly sales ranking';
            }
            $scope.mapchart = function () {
                var myChart = echarts.init(document.getElementById('Mapchart'));
                var data = $scope.date;
                var geoCoordMap = $scope.MapData;                  
                var convertData = function (data) {
                    var res = [];
                    for (var i = 0; i < data.length; i++) {
                        var geoCoord = geoCoordMap[data[i].name];
                        if (geoCoord) {
                            res.push({
                                name: data[i].name,
                                value: geoCoord.concat(data[i].value / num_lose + 50)
                            });
                        }
                    }
                    return res;
                };
                option = {
                    backgroundColor: '#385868',
                    title: {
                        //subtext: title_text,
                        left: 'center',
                        textStyle: {
                            color: '#fff'
                        }
                    },
                    tooltip: {
                        formatter: function (obj) {
                            return 'Top' + (obj.dataIndex + 1) + '<br/>' + obj.name + ':' + '￥' + +data[obj.dataIndex].value;
                        }
                    },
                    geo: {
                        map: 'china',
                        label: {
                            emphasis: {
                                show: false
                            }
                        },
                        roam: true,
                        itemStyle: {
                            normal: {
                                areaColor: '#5F7986',
                                borderColor: '#ccc'
                            },
                            emphasis: {
                                areaColor: '#ccc'
                            }
                        }
                    },
                    series: [
                        {
                            name: 'pm2.5',
                            type: 'scatter',
                            coordinateSystem: 'geo',
                            data: convertData(data),
                            symbolSize: function (val) {
                                return val[2] / 10;
                            },
                            label: {
                                normal: {
                                    formatter: '{b}',
                                    position: 'right',
                                    show: false
                                },
                                emphasis: {
                                    show: true
                                }
                            },
                            itemStyle: {
                                normal: {
                                    color: '#fff'
                                }
                            }
                        },
                        {
                            name: 'Top 5',
                            type: 'effectScatter',
                            coordinateSystem: 'geo',
                            data: convertData(data.sort(function (a, b) {
                                return b.value - a.value;
                            }).slice(0, 6)),
                            symbolSize: function (val) {
                                return val[2] / 10;
                            },
                            showEffectOn: 'render',
                            rippleEffect: {
                                brushType: 'stroke'
                            },
                            hoverAnimation: true,
                            label: {
                                normal: {
                                    formatter: '{b}',
                                    position: 'right',
                                    show: true
                                }
                            },
                            itemStyle: {
                                normal: {
                                    color: '#fff',
                                    shadowBlur: 10,
                                    shadowColor: '#333'
                                }
                            },
                            zlevel: 1
                        }
                    ]
                };


                // 使用刚指定的配置项和数据显示图表。
                myChart.setOption(option);
            }
            $scope.mapchart();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;

        });
    };
    //中国地图_省
    $scope.chainMap3 = function () {
        var url = "api/MDMReport/GetLocationsByProvince?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.date_sheng = [];
            for (var i = 0; i < response.length; i++) {
                $scope.date_sheng.push({ name: "", value: "" });
                $scope.date_sheng[i].name = response[i].Location;
                $scope.date_sheng[i].value = response[i].Recent30SoldAmount.toFixed(2);
                if ($scope.max_num < response[i].Recent30SoldAmount) {
                    $scope.max_num = response[i].Recent30SoldAmount;
                }
            };
            var num_yuan = $scope.max_num;
            var num_lose = $scope.max_num / 250;
            if ($rootScope.chinese == true) {
                var title_text = '按每月销售额排序';
            } else {
                var title_text = 'Monthly sales ranking';
            }
            $scope.mapchart = function () {
                var myChart = echarts.init(document.getElementById('Mapchart2'));
                var data = $scope.date_sheng;
                var geoCoordMap = {
                    '西藏': [91.11, 29.97],
                    '上海': [121.48, 31.22],
                    '福建': [119.3, 26.08],
                    '山西': [112.53, 37.87],
                    '云南': [102.73, 25.04],
                    '辽宁': [123.38, 41.8],
                    '吉林': [125.35, 43.88],
                    '宁夏': [106.27, 38.47],
                    '青海': [101.74, 36.56],
                    '内蒙古': [111.65, 40.82],
                    '四川': [104.06, 30.67],
                    '陕西': [108.95, 34.27],
                    '重庆': [106.54, 29.59],
                    '江苏': [118.78, 32.04],
                    '贵州': [106.71, 26.57],
                    '北京': [116.46, 39.92],
                    '新疆': [87.68, 43.77],
                    '浙江': [120.19, 30.26],
                    '山东': [117, 36.65],
                    '甘肃': [103.73, 36.03],
                    '天津': [117.2, 39.13],
                    '河南': [113.65, 34.76],
                    '黑龙江': [126.63, 45.75],
                    '河北': [114.48, 38.03],
                    '安徽': [117.27, 31.86],
                    '湖北': [114.31, 30.52],
                    '海南': [109.511909, 18.252847],
                    '江西': [115.89, 28.68],
                    '湖南': [113, 28.21],
                    '广东': [113.23, 23.16],
                    '广西': [108.33, 22.84],
                    '台湾': [121.27, 23.86],
                    '香港': [114.2, 22.30],
                    '澳门': [113.60, 22.15],
                };
                var convertData = function (data) {
                    var res = [];
                    for (var i = 0; i < data.length; i++) {
                        var geoCoord = geoCoordMap[data[i].name];
                        if (geoCoord) {
                            res.push({
                                name: data[i].name,
                                value: geoCoord.concat(data[i].value / num_lose + 50)
                            });
                        }
                    }
                    return res;
                };
                option = {
                    backgroundColor: '#385868',
                    title: {

                        //subtext: title_text,
                        left: 'center',
                        textStyle: {
                            color: '#fff'
                        }
                    },
                    tooltip: {
                        formatter: function (obj) {
                            return 'Top' + (obj.dataIndex + 1) + '<br/>' + obj.name + ':' + '￥' + data[obj.dataIndex].value;
                        }
                    },
                    geo: {
                        map: 'china',
                        label: {
                            emphasis: {
                                show: false
                            }
                        },
                        roam: true,
                        itemStyle: {
                            normal: {
                                areaColor: '#5F7986',
                                borderColor: '#ccc'
                            },
                            emphasis: {
                                areaColor: '#ccc'
                            }
                        }
                    },
                    series: [
                        {
                            name: 'pm2.5',
                            type: 'scatter',
                            coordinateSystem: 'geo',
                            data: convertData(data),
                            symbolSize: function (val) {
                                return val[2] / 10;
                            },
                            label: {
                                normal: {
                                    formatter: '{b}',
                                    position: 'right',
                                    show: false
                                },
                                emphasis: {
                                    show: true
                                }
                            },
                            itemStyle: {
                                normal: {
                                    color: '#fff'
                                }
                            }
                        },
                        {
                            name: 'Top 5',
                            type: 'effectScatter',
                            coordinateSystem: 'geo',
                            data: convertData(data.sort(function (a, b) {
                                return b.value - a.value;
                            }).slice(0, 6)),
                            symbolSize: function (val) {
                                return val[2] / 10;
                            },
                            showEffectOn: 'render',
                            rippleEffect: {
                                brushType: 'stroke'
                            },
                            hoverAnimation: true,
                            label: {
                                normal: {
                                    formatter: '{b}',
                                    position: 'right',
                                    show: true
                                }
                            },
                            itemStyle: {
                                normal: {
                                    color: '#fff',
                                    shadowBlur: 10,
                                    shadowColor: '#333'
                                }
                            },
                            zlevel: 1
                        }
                    ]
                };


                // 使用刚指定的配置项和数据显示图表。
                myChart.setOption(option);
            }
            $scope.mapchart();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;

        });
    };
    ///获取地图坐标
    $scope.GetAllMap = function () {
        var url = "api/MDMReport/GetAllMap";
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.MapData = {};
            for (var i = 0; i < response.length; i++) {
                $scope.MapDatas = [];
                var MapDataa = '';
                var MapDataCitya = '';
                $scope.MapDatas.push(response[i].longitude);
                $scope.MapDatas.push(response[i].latitude);
                MapDataa = response[i].province + response[i].city;
                MapDataCitya = response[i].city;
                $scope.MapData[MapDataa] = $scope.MapDatas;
                $scope.MapData[MapDataCitya] = $scope.MapDatas;
            }
            console.log($scope.MapData)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    //世界地图
    //获取世界地图经纬度
    $scope.getlatlong = function () {
        var url = "api/MDMReport/GetCountries";
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.latlong = {};
            for (var i = 0; i < response.length; i++) {
                $scope.latlong[response[i].ProvinceName] = { 'latitude': response[i].Latitude, 'longitude': response[i].Longitude }
            }
            $scope.latlong['西藏'] = { 'latitude': 29.97, 'longitude': 91.11 };
            $scope.latlong['上海'] = { 'latitude': 31.22, 'longitude': 121.48 };
            $scope.latlong['福建'] = { 'latitude': 26.08, 'longitude': 119.03 };
            $scope.latlong['山西'] = { 'latitude': 37.87, 'longitude': 112.53 };
            $scope.latlong['云南'] = { 'latitude': 25.04, 'longitude': 102.73 };
            $scope.latlong['辽宁'] = { 'latitude': 41.8, 'longitude': 123.38 };
            $scope.latlong['吉林'] = { 'latitude': 43.88, 'longitude': 125.35 };
            $scope.latlong['宁夏'] = { 'latitude': 38.47, 'longitude': 106.27 };
            $scope.latlong['青海'] = { 'latitude': 36.56, 'longitude': 101.74 };
            $scope.latlong['内蒙古'] = { 'latitude': 40.82, 'longitude': 111.65 };
            $scope.latlong['四川'] = { 'latitude': 30.67, 'longitude': 104.06 };
            $scope.latlong['陕西'] = { 'latitude': 34.27, 'longitude': 108.95 };
            $scope.latlong['重庆'] = { 'latitude': 29.59, 'longitude': 106.54 };
            $scope.latlong['江苏'] = { 'latitude': 32.04, 'longitude': 118.78 };
            $scope.latlong['贵州'] = { 'latitude': 26.57, 'longitude': 106.71 };
            $scope.latlong['北京'] = { 'latitude': 39.92, 'longitude': 116.46 };
            $scope.latlong['新疆'] = { 'latitude': 43.77, 'longitude': 87.68 };
            $scope.latlong['浙江'] = { 'latitude': 30.26, 'longitude': 120.19 };
            $scope.latlong['山东'] = { 'latitude': 36.65, 'longitude': 117 };
            $scope.latlong['甘肃'] = { 'latitude': 36.03, 'longitude': 103.73 };
            $scope.latlong['天津'] = { 'latitude': 39.13, 'longitude': 117.2 };
            $scope.latlong['河南'] = { 'latitude': 34.76, 'longitude': 113.65 };
            $scope.latlong['黑龙江'] = { 'latitude': 45.75, 'longitude': 126.63 };
            $scope.latlong['河北'] = { 'latitude': 38.03, 'longitude': 114.48 };
            $scope.latlong['安徽'] = { 'latitude': 31.86, 'longitude': 117.27 };
            $scope.latlong['湖北'] = { 'latitude': 30.52, 'longitude': 114.31 };
            $scope.latlong['海南'] = { 'latitude': 18.252847, 'longitude': 109.511909 };
            $scope.latlong['江西'] = { 'latitude': 28.68, 'longitude': 115.89 };
            $scope.latlong['湖南'] = { 'latitude': 28.21, 'longitude': 113 };
            $scope.latlong['广东'] = { 'latitude': 23.16, 'longitude': 113.23 };
            $scope.latlong['广西'] = { 'latitude': 22.84, 'longitude': 108.33 };
            $scope.latlong['台湾'] = { 'latitude': 23.86, 'longitude': 121.27 };
            $scope.latlong['香港'] = { 'latitude': 22.30, 'longitude': 114.2 };
            $scope.latlong['澳门'] = { 'latitude': 22.15, 'longitude': 113.60 };
            console.log($scope.latlong);//国家经纬度和国内省的经纬度合并数组
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    $scope.chainMap4 = function () {
        var url = "api/MDMReport/GetLocationsByProvince?" + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            //表数据
            $scope.date_quanqiu = [];
            for (var i = 0; i < response.length; i++) {
                $scope.date_quanqiu.push({ name: response[i].Location, value: response[i].Recent30SoldAmount.toFixed(2) });
            };
            //图数据
            $scope.mapData = [];
            for (var i = 0; i < response.length; i++) {
                if ($scope.latlong[response[i].Location]) {
                    $scope.mapData.push({ 'code': response[i].Location, 'name': response[i].Location, 'value': response[i].Recent30SoldAmount, 'color': '#fff' });
                }
            }
            var myChart = echarts.init(document.getElementById('Mapchart3'));
            var latlong = $scope.latlong;
            var max = -Infinity;
            var min = Infinity;
            $scope.mapData.forEach(function (itemOpt) {
                if (itemOpt.value > max) {
                    max = itemOpt.value;
                }
                if (itemOpt.value < min) {
                    min = itemOpt.value;
                }
            });
            option = {
                backgroundColor: '#385868',
                tooltip: {
                    trigger: 'item',
                    formatter: function (params) {
                        return params.name + '<br/>' + '￥' + params.value[2];
                    }
                },
                visualMap: {
                    show: false,
                    min: 0,
                    max: max,
                    inRange: {
                        symbolSize: [6, 20]
                    }
                },
                geo: {
                    type: 'map',
                    map: 'world',
                    roam: true,
                    label: {
                        emphasis: {
                            show: false
                        }
                    },
                    itemStyle: {
                        normal: {
                            areaColor: '#5F7986',
                            borderColor: '#fff'
                        },
                        emphasis: {
                            areaColor: '#ccc'
                        }
                    }
                },
                series: [
                    {
                        type: 'scatter',
                        coordinateSystem: 'geo',
                        data: $scope.mapData.map(function (itemOpt) {
                            return {
                                name: itemOpt.name,
                                value: [
                                      latlong[itemOpt.code].longitude,
                                      latlong[itemOpt.code].latitude,
                                      itemOpt.value
                                ],
                                label: {
                                    emphasis: {
                                        position: 'right',
                                        show: true
                                    },
                                    normal: {
                                        formatter: '{b}',
                                        position: 'right',
                                        show: true
                                    }
                                },
                                itemStyle: {
                                    normal: {
                                        color: itemOpt.color
                                    }
                                }
                            };
                        })
                    }
                ]
            };
            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    };

    //选择地图
    $scope.select_Map = function (map_state) {
        $scope.select_map = map_state;
        if ($scope.select_map == "市") {
            $scope.chainMap2();
        } else if ($scope.select_map == "省") {
            $scope.chainMap3();
        } else if ($scope.select_map == "国际") {
            $scope.chainMap4();
        }
    };

    //2.2点击事件
    $scope.onclick_22 = function () {
        $scope.GetMDMDashboardShopSummary();


    }


    //2.2店铺月销量前十


    $scope.GetMDMDashboardShopSummary = function () {
        $scope.isAuthorized_h = 3;
        var url = "/api/MDMReport/GetMDMDashboardShopSummary?topN=" + $scope.topN + "&reportTime=" + $scope.reportTime + "&isAuthorized=";
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ShopSummary = response;
            console.log($scope.ShopSummary);
            $scope.name = [];
            $scope.data = [];
            if (response.length < 1) {
                return
            }
            for (var i = 0; i < response.length; i++) {
                if (response[i].ShopName.length > 8) {
                    name_s = (i + 1) + response[i].ShopName.substring(0, 10) + "...";
                } else {
                    name_s = (i + 1) + response[i].ShopName;
                }
                $scope.name[i] = name_s;
                $scope.data.push({ value: "", name: "" });
                $scope.data[i].value = response[i].SoldVol;
                $scope.data[i].name = name_s;
            }
            console.log($scope.data);

            var myChart = echarts.init(document.getElementById('ShopSalePie'));
            option = {
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    x: 'center',
                    y: 'bottom',
                    data: $scope.name
                },

                calculable: true,
                series: [
                    {
                        name: '前十店铺月销售量占比',
                        type: 'pie',
                        radius: [20, 110],
                        center: ['50%', '35%'],
                        roseType: 'radius',
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: true
                            }
                        },
                        data: $scope.data
                    },
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);



        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };
    //2.2.1切换授权非授权
    $scope.GetMDMDashboardShopSummary_is = function (num) {
        $scope.isAuthorized_h = num;
        var url = "/api/MDMReport/GetMDMDashboardShopSummary?topN=" + $scope.topN + "&reportTime=" + $scope.reportTime + "&isAuthorized=" + $scope.isAuthorized_h;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ShopSummary = response;
            console.log($scope.ShopSummary);
            $scope.name = [];
            $scope.data = [];
            if (response.length < 1) {
                return
            }
            for (var i = 0; i < response.length; i++) {
                if (response[i].ShopName.length > 8) {
                    name_s = (i + 1) + response[i].ShopName.substring(0, 10) + "...";
                } else {
                    name_s = (i + 1) + response[i].ShopName;
                }
                $scope.name[i] = name_s;
                $scope.data.push({ value: "", name: "" });
                $scope.data[i].value = response[i].SoldVol;
                $scope.data[i].name = name_s;
            }
            console.log($scope.data);

            var myChart = echarts.init(document.getElementById('ShopSalePie'));
            option = {
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    x: 'center',
                    y: 'bottom',
                    data: $scope.name
                },

                calculable: true,
                series: [
                    {
                        name: '前十店铺月销售量占比',
                        type: 'pie',
                        radius: [20, 110],
                        center: ['50%', '35%'],
                        roseType: 'radius',
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: true
                            }
                        },
                        data: $scope.data
                    },
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    }

    //2.3点击事件
    $scope.onclick_23 = function () {
        $scope.GetMDMDashboardItemSummary();


    }

    //2.3宝贝月销量前十
    $scope.GetMDMDashboardItemSummary = function () {
        var url = "/api/MDMReport/GetMDMDashboardItemSummary?topN=" + $scope.topN + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ItemSummary = response;
            console.log($scope.ItemSummary);
            $scope.name = [];
            $scope.data = [];
            if (response.length < 1) {
                return
            }
            for (var i = 0; i < response.length; i++) {
                if (response[i].ItemName.length > 8) {
                    name_s = (i + 1) + response[i].ItemName.substring(0, 10) + "...";
                } else {
                    name_s = (i + 1) + response[i].ItemName;
                }
                $scope.name[i] = name_s;
                $scope.data.push({ value: "", name: "" })
                $scope.data[i].value = response[i].SoldVol
                $scope.data[i].name = name_s;
            }
            console.log($scope.data);

            var myChart = echarts.init(document.getElementById('ItemPie'));
            option = {
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    x: 'center',
                    y: 'bottom',
                    data: $scope.name
                },

                calculable: true,
                series: [
                    {
                        name: '前十店铺月销售量占比',
                        type: 'pie',
                        radius: [20, 110],
                        center: ['50%', '35%'],
                        roseType: 'radius',
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: true
                            }
                        },
                        data: $scope.data
                    },
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);

        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };



    //2.4点击事件
    $scope.onclick_24 = function () {
        $scope.GetMDMDashboardProductSummary();
    }

    //2.4产品月销量前十
    $scope.GetMDMDashboardProductSummary = function () {
        var url = "/api/MDMReport/GetMDMDashboardProductSummary?topN=" + $scope.topN + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ProductSummary = response;
            console.log($scope.ProductSummary);
            $scope.name = [];
            $scope.data = [];
            if (response.length < 1) {
                return
            }
            for (var i = 0; i < response.length; i++) {
                if (response[i].Product.length > 8) {
                    name_s = (i + 1) + response[i].Product.substring(0, 10) + "...";
                } else {
                    name_s = (i + 1) + response[i].Product;
                }
                $scope.name[i] = name_s;
                $scope.data.push({ value: "", name: "" })
                $scope.data[i].value = response[i].TotalSoldVol;
                $scope.data[i].name = name_s;
            }
            console.log($scope.data);

            var myChart = echarts.init(document.getElementById('ProductPie'));
            option = {
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    x: 'center',
                    y: 'bottom',
                    data: $scope.name
                },

                calculable: true,
                series: [
                    {
                        name: '前十店铺月销售量占比',
                        type: 'pie',
                        radius: [20, 110],
                        center: ['50%', '35%'],
                        roseType: 'radius',
                        label: {
                            normal: {
                                show: false
                            },
                            emphasis: {
                                show: true
                            }
                        },
                        data: $scope.data
                    },
                ]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);

        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };

    //3.2点击事件
    $scope.onclick_31 = function () {
        $scope.GetMDMDashboardSuspectShopSummary();

    }
    //3.2.1可疑店铺前五
    $scope.GetMDMDashboardSuspectShopSummary = function () {
        var url = "/api/MDMReport/GetMDMDashboardSuspectShopSummary?topN=" + $scope.topN2 + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.SuspectShopSummary = response;
            console.log($scope.SuspectShopSummary);
            var dataSH = [];
            for (var i = 0; i < $scope.SuspectShopSummary.length; i++) {
                dataSH[i] = [$scope.SuspectShopSummary[i].Listings, $scope.SuspectShopSummary[i].TotalComments, $scope.SuspectShopSummary[i].SoldVol / 3000, 1, 1, 1, (50 - 2 * i), $scope.SuspectShopSummary[i].ShopName]
            }

            var myChart = echarts.init(document.getElementById('qipao'));

            var schema = [
                { name: 'date', index: 0, text: '日' },
                { name: 'AQIindex', index: 1, text: '销售额' },
                { name: 'PM25', index: 2, text: '宝贝数' },
                { name: 'PM10', index: 3, text: '评论数' },
            ];


            var itemStyle = {
                normal: {
                    opacity: 0.8,
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowOffsetY: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            };

            option = {

                color: [
                    '#dd4444', '#fec42c', '#80F1BE'
                ],

                grid: {
                    x: '10%',
                    y: '10%',
                    y2: '7%'
                },
                tooltip: {
                    padding: 10,
                    backgroundColor: '#222',
                    borderColor: '#777',
                    borderWidth: 1,
                    formatter: function (obj) {
                        var value = obj.value;
                        return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">'
                            + '店铺名：'
                            + value[7]
                            + '</div>'
                            + schema[1].text + '：￥' + (value[2] * 3000).toFixed(2) + '<br>'
                            + schema[2].text + '：' + value[0] + '<br>'
                            + schema[3].text + '：' + value[1] + '<br>'
                    }
                },
                xAxis: {
                    type: 'value',
                    name: '宝贝数',
                    nameGap: 16,
                    nameTextStyle: {
                        color: '#666',
                        fontSize: 14
                    },
                    splitLine: {
                        show: false
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#666'
                        }
                    }
                },
                yAxis: {
                    type: 'value',
                    name: '评论数',
                    nameLocation: 'end',
                    nameGap: 20,
                    nameTextStyle: {
                        color: '#666',
                        fontSize: 16
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#666'
                        }
                    },
                    splitLine: {
                        show: false
                    }
                },
                visualMap: [
                    {
                        left: 'right',
                        top: '10%',
                        dimension: 2,
                        min: 0,
                        max: 250,
                        itemWidth: 30,
                        itemHeight: 120,
                        calculable: true,
                        precision: 1,
                        text: [''],
                        textGap: 30000,
                        textStyle: {
                            color: '#fff'
                        },
                        inRange: {
                            symbolSize: [20, 50]
                        },
                        outOfRange: {
                            symbolSize: [20, 50],
                            color: ['rgba(255,255,255,.2)']
                        },
                        controller: {
                            inRange: {
                                color: ['#c23531']
                            },
                            outOfRange: {
                                color: ['#444']
                            }
                        }
                    },
                    {
                        left: 'right',
                        bottom: '5%',
                        dimension: 6,
                        min: 0,
                        max: 50,
                        itemHeight: 120,
                        calculable: true,
                        precision: 0.1,
                        text: [''],
                        textGap: 3000,
                        textStyle: {
                            color: '#fff'
                        },
                        inRange: {
                            colorLightness: [1, 0.5]
                        },
                        outOfRange: {
                            color: ['rgba(255,255,255,.2)']
                        },
                        controller: {
                            inRange: {
                                color: ['#c23531']
                            },
                            outOfRange: {
                                color: ['#444']
                            }
                        }
                    }
                ],
                series: [

                    {
                        name: '上海',
                        type: 'scatter',
                        itemStyle: itemStyle,
                        data: dataSH
                    },

                ]
            };
            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };



    //3.3点击事件
    $scope.onclick_32 = function () {
        $scope.GetTopRiskStore();
        $scope.GetTopRiskProduct();
        $scope.GetTopRiskItem();
    }

    //3.3.1差评宝贝前十
    $scope.GetTopRiskStore = function () {
        var url = "/api/MDMReport/GetTopRiskStore?topN=" + $scope.topN2 + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetTopRiskStore_list = response;
            console.log($scope.GetTopRiskStore_list);
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };
    $scope.GetTopRiskProduct = function () {
        var url = "/api/MDMReport/GetTopRiskProduct?topN=" + $scope.topN2 + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetTopRiskProduct_list = response;
            console.log($scope.GetTopRiskProduct_list);
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };
    $scope.GetTopRiskItem = function () {
        var url = "/api/MDMReport/GetTopRiskItem?topN=" + $scope.topN2 + "&reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetTopRiskItem_list = response;
            console.log($scope.GetTopRiskItem_list);
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };
    //4.1获取总体设置
    $scope.GetMDMReportSetting = function () {
        var url = "/api/MDMReport/GetMDMReportSetting?";
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.IsManager = response.IsManager;
            if (response != null) {
                if ($rootScope.LangVersion == 0) {
                    $scope.ReportSetting_list = response.Data[0];
                    console.log($scope.ReportSetting_list);
                } else if ($rootScope.LangVersion == 1) {
                    $scope.ReportSetting_list = response.Data[1];
                }
                $rootScope.ID = $scope.ReportSetting_list.ID;

                $scope.CompanyName = $scope.ReportSetting_list.CompanyName;
                $scope.BrandName = $scope.ReportSetting_list.BrandName;
                $scope.Statement = $scope.ReportSetting_list.Statement;
                $scope.Title1 = $scope.ReportSetting_list.Title1;
                $scope.Title2 = $scope.ReportSetting_list.Title2;
                $scope.Title3 = $scope.ReportSetting_list.Title3;
                $scope.Title4 = $scope.ReportSetting_list.Title4;
                $scope.Title1Content = $scope.ReportSetting_list.Title1Content;
                $scope.Title2Content = $scope.ReportSetting_list.Title2Content;
                $scope.Title3Content = $scope.ReportSetting_list.Title3Content;
                $scope.Title4Content = $scope.ReportSetting_list.Title4Content;
                $scope.Ending = $scope.ReportSetting_list.Ending;
                $scope.HeadTitle = $scope.ReportSetting_list.HeadTitle;
                $scope.ShopSaleRank = $scope.ReportSetting_list.ShopSaleRank;
                $scope.Outline = $scope.ReportSetting_list.Outline;
                $scope.DataOutline = $scope.ReportSetting_list.DataOutline;
                $scope.DataChangeTrend = $scope.ReportSetting_list.DataChangeTrend;
                $scope.DataDisplay = $scope.ReportSetting_list.DataDisplay;
                $scope.DistrictSaleRank = $scope.ReportSetting_list.DistrictSaleRank;
                $scope.ItemSaleRank = $scope.ReportSetting_list.ItemSaleRank;
                $scope.ProductSaleRank = $scope.ReportSetting_list.ProductSaleRank;
                $scope.Deduce = $scope.ReportSetting_list.Deduce;
                $scope.FakeShop = $scope.ReportSetting_list.FakeShop;
                $scope.FakeItem = $scope.ReportSetting_list.FakeItem;
                $rootScope.LangVersion = $scope.ReportSetting_list.LangVersion;

                $scope.AuthorizedShopRatio = $scope.ReportSetting_list.AuthorizedShopRatio;
                $scope.AuthorizedShopTrend = $scope.ReportSetting_list.AuthorizedShopTrend;
                $scope.Conclusion = $scope.ReportSetting_list.Conclusion;
                $scope.ConclusionContent = $scope.ReportSetting_list.ConclusionContent;

            }
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };

    //4.2保存总体设置

    $scope.SaveMDMReportSetting = function () {

        $scope.paramsList = {
            ID: $rootScope.ID,
            CompanyName: $scope.CompanyName,
            BrandName: $scope.BrandName,
            Statement: $scope.Statement,
            Title1: $scope.Title1,
            Title2: $scope.Title2,
            Title3: $scope.Title3,
            Title4: $scope.Title4,
            Title1Content: $scope.Title1Content,
            Title2Content: $scope.Title2Content,
            Title3Content: $scope.Title3Content,
            Title4Content: $scope.Title4Content,
            Ending: $scope.Ending,
            HeadTitle: $scope.HeadTitle,
            ShopSaleRank: $scope.ShopSaleRank,
            Outline: $scope.Outline,
            DataOutline: $scope.DataOutline,
            DataChangeTrend: $scope.DataChangeTrend,
            DataDisplay: $scope.DataDisplay,
            DistrictSaleRank: $scope.DistrictSaleRank,
            ItemSaleRank: $scope.ItemSaleRank,
            ProductSaleRank: $scope.ProductSaleRank,
            Deduce: $scope.Deduce,
            FakeShop: $scope.FakeShop,
            FakeItem: $scope.FakeItem,
            LangVersion: $rootScope.LangVersion,

            AuthorizedShopRatio: $scope.AuthorizedShopRatio,
            AuthorizedShopTrend: $scope.AuthorizedShopTrend,
            Conclusion: $scope.Conclusion,
            ConclusionContent: $scope.ConclusionContent,

        };
        var urls = "/api/MDMReport/SaveMDMReportSetting";
        var q = $http.post(
                urls,
               JSON.stringify($scope.paramsList),
               {
                   headers: {
                       'Content-Type': 'application/json'
                   }
               }
            )
        q.success(function (response, status) {
            alert("保存成功");
            console.log(response);
            $scope.GetMDMReportSetting();
        });
        q.error(function (e) {
        });
    }

    //4.3获取细节设置

    $scope.GetMDMReportDetailSetting = function () {
        var url = "/api/MDMReport/GetMDMReportDetailSetting?reportTime=" + $scope.reportTime;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response != null) {
                if ($rootScope.LangVersion == 0) {
                    $scope.ReportDetail_list = response[0];
                    console.log($scope.ReportDetail_list);
                } else if ($rootScope.LangVersion == 1) {
                    $scope.ReportDetail_list = response[1];
                    console.log($scope.ReportDetail_list);
                }
                if ($scope.ReportDetail_list.ID) {
                    $rootScope.ID1 = $scope.ReportDetail_list.ID;
                } else {
                    $rootScope.ID1 = "";
                }
                $cookieStore.put("ID1", $rootScope.ID1)
                //$scope.IsManager = $scope.ReportDetail_list.IsManager;
                $scope.ReportTime = $scope.ReportDetail_list.ReportTime;
                $scope.OutlineDesc = $scope.ReportDetail_list.OutlineDesc;
                $scope.ShopSoldDesc = $scope.ReportDetail_list.ShopSoldDesc;
                $scope.DistrictSoldDesc = $scope.ReportDetail_list.DistrictSoldDesc;
                $scope.ItemSoldDesc = $scope.ReportDetail_list.ItemSoldDesc;
                $scope.ProductSoldDesc = $scope.ReportDetail_list.ProductSoldDesc;
                $scope.FakeShopDesc = $scope.ReportDetail_list.FakeShopDesc;
                $scope.FakeItemDesc = $scope.ReportDetail_list.FakeItemDesc;
                $scope.TrendDesc = $scope.ReportDetail_list.TrendDesc;
                $scope.PublishDate = $scope.ReportDetail_list.PublishDate;

                $scope.AuthorizedShopRatioDesc = $scope.ReportDetail_list.AuthorizedShopRatioDesc;
                $scope.AuthorizedShopTrendDesc = $scope.ReportDetail_list.AuthorizedShopTrendDesc;
                $scope.ConclusionDesc = $scope.ReportDetail_list.ConclusionDesc;
            }
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";

        });
    };

    //4.2保存细节设置
    $scope.SaveMDMReportDetailSetting = function () {
        $scope.paramsList = {
            ID: $rootScope.ID1,
            ReportTime: $scope.ReportTime,
            TrendDesc: $scope.TrendDesc,
            OutlineDesc: $scope.OutlineDesc,
            ShopSoldDesc: $scope.ShopSoldDesc,
            DistrictSoldDesc: $scope.DistrictSoldDesc,
            ItemSoldDesc: $scope.ItemSoldDesc,
            ProductSoldDesc: $scope.ProductSoldDesc,
            FakeShopDesc: $scope.FakeShopDesc,
            FakeItemDesc: $scope.FakeItemDesc,
            Status: $rootScope.Status,
            SiteName: $rootScope.SiteName_List1,
            LangVersion: $rootScope.LangVersion,
            AuthorizedShopRatioDesc: $scope.AuthorizedShopRatioDesc,
            AuthorizedShopTrendDesc: $scope.AuthorizedShopTrendDesc,
            ConclusionDesc: $scope.ConclusionDesc,
        };
        var urls = "/api/MDMReport/SaveMDMReportDetailSetting";
        var q = $http.post(
                urls,
               JSON.stringify($scope.paramsList),
               {
                   headers: {
                       'Content-Type': 'application/json'
                   }
               }
            )
        q.success(function (response, status) {
            alert("保存成功");
            console.log(response);
            $scope.GetMDMReportDetailSetting();
        });
        q.error(function (e) {
            alert("服务器连接出错");
        });
    }



    //__________________________________________________



    //++++++++++++++++++++++++++++++++++++++++++++++++++++
    $scope.gettimes();
    $scope.GetMDMReportSetting();
    $scope.GetMDMReportList();
    $scope.GetAllMap();

});