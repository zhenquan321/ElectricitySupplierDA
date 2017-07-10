// <reference path="../../views/kmean.aspx" />
var eMarketNow_ctr = myApp.controller("eMarketNow_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $templateCache, $timeout, myApplocalStorage) {

    //if ($rootScope.logined == true) {
    $scope.keywordList = [];
    $scope.keywords = "";
    $scope.isActive = true;
    $scope.KeywordRecord = [];
    $scope.keywordList = [];
    $scope.xinghao = "";
    $scope.liebie = "";
    $scope.junjia = 0;
    $scope.pinpai = "";
    $scope.isActiveditu = true;
    //控制名称添加提示
    $scope.xinghao1 = false;
    $scope.liebie1 = false;
    $scope.pinpai1 = false;
    $scope.isActiveoffset1 = true;
    $scope.isActiveoffset2 = false;
    $scope.isActiveoffset3 = false;
    $scope.iskeyNull = false;
    $scope.pagesize = 10;
    $scope.pagesize1 = 10;
    $scope.pagesize2 = 10;
    $scope.times = 24;
    //加载更多
    //$scope.recordConut = 10;
    //$scope.isActiveLoadmore = true;
    //控制启动监测按钮loading
    $scope.loadingStartKeyword = false;
    //控制loading
    $scope.isActiveAdd = false;
    $scope.isActiveAdd50 = false;
    $scope.isActiveStart = false;
    $scope.isActiveList = false;
    $scope.GetImgLink = "Scripts/app/images/square-image.png";
    $scope.yzmUrl = 'http://' + window.location.host + '/Scripts/app/views/kmean.aspx';
    $scope.page_eMN = 1;
    $scope.pagesize_eMN = 100;
    $scope.projectsList = [];
    $scope.isActive_pic = "";

    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //改变信源
    //0.1微信
    $scope.changeModel_baidu = function () {
        $rootScope.isActiveModale = "baidu";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    }


    $scope.changeModel_bing = function () {
        $rootScope.isActiveModale = "bing";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }


    //0.2微信
    $scope.changeModel_sougou = function () {
        $rootScope.isActiveModale = "wechat";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    }
    //0.3eMN

    $scope.changeModel_eMN = function () {
        $rootScope.isActiveModale = "eMarketNow";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    }
    //0.4picS

    $scope.changeModel_picS = function () {
        $rootScope.isActiveModale = "picSearch";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }

    //0.5搜狗

    $scope.changeModel_weibo = function () {
        $rootScope.isActiveModale = "weibo";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }
    //0.6微博

    $scope.changeModel_sougo_new = function () {
        $rootScope.isActiveModale = "sougo_new";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }
    //0.7谷歌

    $scope.changeModel_googol = function () {
        $rootScope.isActiveModale = "googol";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }

    $(document)
        .ready(function () {
            $('.special.cards .image .dropdown').dimmer({
                on: 'hover'
            });

            $('.dropdown ').dropdown({
                on: 'hover'
            });


            $('.special.cards .image').dimmer({
                on: 'hover'
            });
            $('.accordion ')
                .accordion({
                    selector: {
                        trigger: '.title'
                    }
                })
            ;

        })
    ;


    //启动监测
    $scope.startKeyword = function (kids) {

        $scope.isActiveStart = true;

        $scope.loadingStartKeyword = true;
        var url = "/api/EMN/startKeyword?kids=" + kids + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>startKeyword');
            $scope.GetKeyword();
        });
        q.error(function (status) {
            $scope.error = "服务器连接出错";
        });

    };
    //tab切换SUK
    $scope.changeSUK = function () {
        $scope.isActive = true;
    };
    //tab切换Name
    $scope.changeName = function () {
        $scope.isActive = false;
    };
    //启动enter 事件
    $scope.EnterPress = function () {
        var e = e || window.event;
        if (e.keyCode == 13) {
            $scope.insertKeyword();
        }
    }

    //改变启动频率
    $scope.changeTimes = function (sum) {
        $scope.times = sum;
    };
    $scope.jiage == ""

    //启动监测按钮
    //$rootScope.insertKeyword = function () {
    //    $scope.imglinks = "Scripts/app/images/jiance.gif";
    //    $scope.showStart = true;
    //    $("#txt22").text('');
    //    $scope.isActiveAdd = true;
    //    if ($scope.isActive == false) {
    //        $scope.xinghao1 = false;
    //        $scope.liebie1 = false;
    //        $scope.pinpai1 = false;
    //        if ($scope.xinghao == "" || $scope.pinpai == "" || $scope.liebie == "") {
    //            $scope.isActiveAdd = false;
    //            if ($scope.pinpai == "") {
    //                $scope.pinpai1 = true;

    //            }
    //            if ($scope.xinghao == "") {
    //                $scope.xinghao1 = true;
    //            }
    //            if ($scope.liebie == "") {
    //                $scope.liebie1 = true;
    //            }
    //            alert("品牌、型号或类别不能为空");
    //        } else {
    //            $scope.GetImgLink = "Scripts/app/images/square-image.png";

    //            $scope.keywords = $scope.pinpai + $scope.liebie + $scope.xinghao;
    //            $scope.keywords111 = $scope.pinpai + $scope.liebie + $scope.xinghao;

    //            var url = "/api/EMN/insertKeyword?user_id=" + $rootScope.userID + "&keywords=" + encodeURIComponent($scope.keywords) + "&MLP=" + $scope.junjia + "&IntervalHours=" + $scope.times + "&projectId=" + $rootScope.getProjectId;
    //            var q = $http.get(url);
    //            q.success(function (response, status) {
    //                console.log('eMarketNow_ctr>insertKeyword');
    //                if (response.Error == null) {
    //                    $scope.GetKeyword();
    //                    // $scope.startKeyword(response._id);

    //                    $('#qidong').addClass('visible active');
    //                    setTimeout(function () {
    //                        $scope.GetBotResultList(response._id);
    //                    }, 3500);

    //                    // $scope.details(response._id, $scope.keywords);

    //                    setTimeout(function () {
    //                        $scope.detailsFor(response._id, $scope.keywords);
    //                    }, 5000);
    //                    $scope.isActiveAdd = false;


    //                } else {
    //                    alert(response.Error);
    //                    $scope.isActiveAdd = false;
    //                }
    //            });
    //            q.error(function (response) {
    //                $scope.error = "服务器连接出错";
    //                $scope.isActiveAdd = false;

    //            });
    //        }
    //    } else {
    //        $scope.isActiveAdd = true;
    //        if ($scope.SKU == "" || $scope.SKU == null) {
    //            $rootScope.addAlert('danger', '请输入SKU！');
    //            $scope.isActiveAdd = false;
    //        } else {
    //            $scope.keywords111 = $scope.SKU;
    //            $scope.GetImgLink = "Scripts/app/images/square-image.png";

    //            var url = "/api/EMN/insertKeyword?user_id=" + $rootScope.userID + "&keywords=" + encodeURIComponent($scope.SKU) + "&MLP=" + $scope.jiage + "&IntervalHours=" + $scope.times + "&projectId=" + $rootScope.getProjectId;


    //            var q = $http.get(url);
    //            q.success(function (response, status) {
    //                console.log('eMarketNow_ctr>insertKeyword');
    //                if (response.Error == null) {
    //                    $scope.GetKeyword();
    //                    //$scope.startKeyword(response._id);

    //                    $('#qidong').addClass('visible active');

    //                    setTimeout(function () {
    //                        $scope.GetBotResultList(response._id);
    //                    }, 3500);

    //                    // $scope.details(response._id, $scope.SKU);


    //                    setTimeout(function () {
    //                        $scope.detailsFor(response._id, $scope.SKU);
    //                    }, 5000);


    //                    $scope.isActiveAdd = false;

    //                } else {
    //                    alert(response.Error);
    //                    $scope.isActiveAdd = false;
    //                }

    //            });
    //            q.error(function (response) {
    //                $scope.error = "服务器连接出错";
    //                $scope.isActiveAdd = false;
    //            });
    //        }
    //    }
    //};


    $rootScope.insertKeyword = function () {
            if ($scope.SKU == "" || $scope.SKU == null) {
                $rootScope.addAlert('danger', '请输入SKU！');
                $scope.alertfun('worning','请输入搜索关键词！') 
            } else {
                $scope.keywords111 = $scope.SKU;
                var url = "/api/EMN/insertKeyword?user_id=" + $rootScope.userID + "&keywords=" + encodeURIComponent($scope.SKU) + "&MLP=" + $scope.jiage + "&IntervalHours=" + $scope.times + "&projectId=" + $rootScope.getProjectId;
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('eMarketNow_ctr>insertKeyword');
                    if (response.Error == null) {
                        // $scope.details(response._id, $scope.SKU);
                        $scope.GetKeyword3();
                        $scope.isActiveAdd = false;
                    } else {
                        alert(response.Error);
                        $scope.isActiveAdd = false;
                    }

                });
                q.error(function (response) {
                    $scope.error = "服务器连接出错";
                    $scope.isActiveAdd = false;
                });
            }
        
    };
    //启动循环监测20次
    var times = 0;
    $scope.detailsFor = function (id, keyname) {
        times++;
        $scope.details(id, keyname);
        if (times < 8) {
            setTimeout(function () {
                $scope.detailsFor(id, keyname);
            }, 4000);

        }
    }
    //搜索时加载
    $scope.GetKeyword3 = function () {
        $scope.isActiveAdd50 = true;
        var url = "/api/EMN/GetKeyword?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>Getkeyword');
            $scope.isActiveAdd50 = false;
            $scope.keywordList = response;
            $scope.recordConut = $scope.keywordList.length;
            if ($scope.keywordList.length > 0) {
                $scope.keyId = $scope.keywordList[0]._id;
                $scope.keyName = $scope.keywordList[0].TaskName;
                setTimeout(function () {
                    $scope.detailsFor($scope.keyId, $scope.keyName);
                },2000);
            }

            //console.log($scope.keywordList.length);
        });
        q.error(function (response) {
            $scope.isActiveAdd50 = false;

            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };
    //加载左边keywords
    $scope.GetKeyword = function () {
        $scope.isActiveAdd50 = true;
        var url = "/api/EMN/GetKeyword?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>Getkeyword');
            $scope.isActiveAdd50 = false;
            $scope.keywordList = response;
            $scope.recordConut = $scope.keywordList.length;

            if ($scope.keywordList.length > 0) {
                $scope.keyId = $scope.keywordList[0]._id;

                $scope.keyName = $scope.keywordList[0].TaskName;

                $scope.details($scope.keyId, $scope.keyName);
           
            }

            //console.log($scope.keywordList.length);
        });
        q.error(function (response) {
            $scope.isActiveAdd50 = false;

            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };


    //加载更多

    //$scope.GetMoreTaobaoKeyword = function () {
    //    $scope.recordConut = $scope.recordConut + 10;

    //    $scope.GetKeyword();

    //}

    //加载左边keywords//首次加载关键词
    $scope.GetKeyword2 = function () {

        $scope.isActiveAdd50 = true;
        var url = "/api/EMN/GetKeyword?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>GetKeyword2');
            $scope.isActiveAdd50 = false;

            $scope.keywordList = response;

            if ($scope.keywordList.length > 0) {
                $scope.keyId = $scope.keywordList[0]._id;

                $scope.keyName = $scope.keywordList[0].TaskName;

                $scope.details($scope.keyId, $scope.keyName);
            }


        });
        q.error(function (response) {
            $scope.isActiveAdd50 = false;

            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };


    //判断是否是第一次加载关键词
    $scope.firstLoading = function () {

        var sss = typeof ($rootScope.keyId);


        if ($rootScope.keyId == "" || sss == "undefined") {
            $scope.GetKeyword2();
        } else {
            $scope.GetKeyword();
            $scope.details($rootScope.keyId, $rootScope.keyName);
        }
    };


    $scope.isActiveFive = false;
    $scope.GetKeywordByTimeList = [];


    $scope.GetTotalItemsAccount = "";
    $scope.GetTotalShopsAccount = "";
    $scope.Get30SaleNumAccount = "";
    $scope.Get30VgePriceAccount = "";
    $scope.Get30SaleVolAccount = "";
    $scope.isActiveTop10 = false;


    var options = {
        useEasing: true,
        useGrouping: true,
        separator: ',',
        decimal: '.',
        prefix: '',
        suffix: ''
    };


    $scope.GetTotalItemsAccount = 0;

    //监测到的总链接数
    $scope.GetTotalItems = function () {

        var url = "/api/Dashboard/GetTotalItems?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;
        $scope.GetTotalItemsAccount1 = 0;

        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>GetTotalItems');
            if ($scope.GetTotalItemsAccount != response) {
                $scope.GetTotalItemsAccount1 = response;
                var demo1 = new CountUp("content_value1", $scope.GetTotalItemsAccount, $scope.GetTotalItemsAccount1, 0, 6, options);
                demo1.start();
                $scope.GetTotalItemsAccount = $scope.GetTotalItemsAccount1;
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    //监测到的总店铺数
    $scope.GetTotalShopsAccount = 0;
    $scope.GetTotalShops = function () {
        var url = "/api/Dashboard/GetTotalShops?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;
        $scope.GetTotalShopsAccount1 = 0;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>GetTotalShops');
            if ($scope.GetTotalShopsAccount != response) {
                $scope.GetTotalShopsAccount1 = response;
                var demo2 = new CountUp("content_value2", $scope.GetTotalShopsAccount, $scope.GetTotalShopsAccount1, 0, 6, options);
                demo2.start();
                $scope.GetTotalShopsAccount = $scope.GetTotalShopsAccount1;
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    //30天销售量by关键词
    $scope.Get30SaleNumAccount = 0;

    $scope.Get30SaleNum = function () {

        var url = "/api/Dashboard/Get30SaleNum?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;
        $scope.Get30SaleNumAccount1 = 0;

        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>Get30SaleNum');
            if ($scope.Get30SaleNumAccount != response) {
                $scope.Get30SaleNumAccount1 = response;

                var demo3 = new CountUp("content_value3", $scope.Get30SaleNumAccount, $scope.Get30SaleNumAccount1, 0, 6, options);

                demo3.start();
                $scope.Get30SaleNumAccount = $scope.Get30SaleNumAccount1;
            }


        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    //30天销售额by关键词
    $scope.Get30SaleVolAccount = 0;

    $scope.Get30SaleVol = function () {
        var url = "/api/Dashboard/Get30SaleVol?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;
        $scope.Get30SaleVolAccount1 = 0;

        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>Get30SaleVol');
            if ($scope.Get30SaleVolAccount != response) {
                $scope.Get30SaleVolAccount1 = response;

                var demo4 = new CountUp("content_value4", $scope.Get30SaleVolAccount, $scope.Get30SaleVolAccount1, 0, 6, options);

                demo4.start();
                $scope.Get30SaleVolAccount = $scope.Get30SaleVolAccount1;
            }


        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    //30天销售产品均价
    $scope.Get30VgePriceAccount = 0;

    $scope.Get30VgePrice = function () {
        var url = "/api/Dashboard/Get30VgePrice?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;
        $scope.Get30VgePriceAccount1 = 0;

        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>Get30VgePrice');
            if ($scope.Get30VgePriceAccount != response) {
                $scope.Get30VgePriceAccount1 = response;

                var demo5 = new CountUp("content_value5", $scope.Get30VgePriceAccount, $scope.Get30VgePriceAccount1, 0, 6, options);

                demo5.start();
                $scope.Get30VgePriceAccount = $scope.Get30VgePriceAccount1;
            }


        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    //有销量占比
    $scope.GetRatioAccount = 0

    $scope.GetRatio = function () {
        var url = "/api/Dashboard/GetRatio?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;

        $scope.GetRatioAccount1 = 0;

        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>GetRatio');
            if ($scope.GetRatioAccount != response) {
                $scope.GetRatioAccount1 = response;

                var demo6 = new CountUp("content_value6", $scope.GetRatioAccount, $scope.GetRatioAccount1, 0, 6, options);

                demo6.start();
                $scope.GetRatioAccount = $scope.GetRatioAccount1;
            }


        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }
    //获取图片
    $scope.GetImg = function () {
        var url = "/api/Dashboard/GetImg?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>GetImg');
            $scope.GetImgLink_all = response;
            $scope.GetImgLink = response[0];
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";

        });
    }

    $scope.changePic_show = function () {

        $scope.isActive_pic = true;

    };
    $scope.changePic_hide = function () {

        $scope.isActive_pic = false;

    };
    $scope.changePic_d = function (scr) {
        $scope.GetImgLink = scr

    }

    $scope.AllAccount = function () {

        if (typeof ($rootScope.keyId) != "undefined") {
            $scope.isActiveFive = true;
            $scope.GetTotalItems();
            $scope.GetTotalShops();
            $scope.Get30SaleNum();
            $scope.Get30SaleVol();
            $scope.Get30VgePrice();
            $scope.GetRatio();
            $scope.GetImg();
            $scope.isActiveFive = false;
        }
        
    }
    var data3 = [];

    $scope.RefreshGetBubbleList = function () {
        $scope.pagesize2 = $scope.pagesize2
        $scope.GetBubbleList2();
    }
    //气泡图
    $scope.GetBubbleList = function (num) {

        if (typeof ($rootScope.keyId) != "undefined") {

            $scope.TimeKey = [];
            $scope.seriesOptions = [];
            $scope.pagesize2 = num;
            $scope.GetBubbleList2();

        }

        
    }

    $scope.GetBubbleList2 = function () {
        var url = "/api/Dashboard/GetBubbleList?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&pagesize=" + $scope.pagesize2 + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetBubbleList_list = response[0].FreeBotItemList;
            console.log($scope.GetBubbleList_list);
            console.log(response);
            var myChart = echarts.init(document.getElementById('GetBubbleList'));
            var schema = [
                { name: 'date', index: 0, text: '日' },
                { name: 'AQIindex', index: 1, text: '综合排名' },
                { name: 'PM10', index: 2, text: '30天销量' },
                { name: 'CO', index: 3, text: '累计评论数' },
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
            for (var i = 0; i < response.length; i++) {
                $scope.TimeKey.push(response[i].TimeKey);
                var data1 = [];
                var TotalCommentsMax = 0;
                var Recent30DaysSoldNummax = 0;
                var Recent30DaysSoldNumSize = 0;
                var PositionMax = 0;
                for (var j = 0; j < response[i].FreeBotItemList.length; j++) {
                    if (response[i].FreeBotItemList[j].TotalComments > TotalCommentsMax) {
                        TotalCommentsMax = response[i].FreeBotItemList[j].TotalComments + 10;
                    }
                    if (response[i].FreeBotItemList[j].Position > PositionMax) {
                        PositionMax = response[i].FreeBotItemList[j].Position;
                    }
                    if (response[i].FreeBotItemList[j].Recent30DaysSoldNum > Recent30DaysSoldNummax) {
                        Recent30DaysSoldNummax = response[i].FreeBotItemList[j].Recent30DaysSoldNum;
                    }
                    Recent30DaysSoldNumSize = response[i].FreeBotItemList[j].Recent30DaysSoldNum / Recent30DaysSoldNummax + 0.2
                    response[i].FreeBotItemList[j].Recent30DaysSoldNum
                    var responseA = [response[i].FreeBotItemList[j].TotalComments, -response[i].FreeBotItemList[j].Position, Recent30DaysSoldNumSize, response[i].FreeBotItemList[j].TotalComments, response[i].FreeBotItemList[j].ShopName, response[i].FreeBotItemList[j].Position, 50, response[i].FreeBotItemList[j].Recent30DaysSoldNum];

                    data1.push(responseA);
                }

                var seriseOne = {
                    animation: false,
                    legend: {
                        y: -999,
                        data: ['scatter']
                    },
                    tooltip: {
                        padding: 10,
                        backgroundColor: '#222',
                        borderColor: '#777',
                        borderWidth: 1,
                        formatter: function (obj) {
                            var value = obj.value;
                            return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">'
                                + value[4]
                                + '</div>'
                                + schema[1].text + '：' + value[5] + '<br>'
                                + schema[2].text + '：' + value[7] + '<br>'
                                + schema[3].text + '：' + value[3] + '<br>'
                        }
                    },
                    xAxis: {
                        name: '累计评论数',
                        type: 'value',
                        min: 'dataMin',
                        max: 'dataMax',
                        splitLine: {
                            show: true
                        }
                    },
                    yAxis: {
                        name: '淘宝综合排名',

                        type: 'value',
                        min: 'dataMin',
                        max: 'dataMax',
                        axisLabel: {
                            formatter: function (v) {
                                return -v;
                            }
                        },
                        splitLine: {
                            show: true
                        }
                    },
                    dataZoom: [
                        {
                            type: 'slider',
                            show: true,
                            xAxisIndex: [0],
                            start: 0,
                            top: '93%',
                            height: 10,
                            end: 100,

                        },
                        {
                            type: 'slider',
                            show: true,
                            yAxisIndex: [0],
                            left: '93%',
                            start: 0,
                            end: 100,
                            width: 10,

                        },
                        {
                            type: 'inside',
                            xAxisIndex: [0],
                            start: 0,
                            end: 100,

                        },
                        {
                            type: 'inside',
                            yAxisIndex: [0],
                            start: 0,
                            end: 100,
                            x: 0,
                        }
                    ],
                    series: [
                        {
                            name: 'scatter',
                            type: 'scatter',
                            itemStyle: {
                                normal: {
                                    opacity: 0.8,
                                    color: '#16b38e'
                                }
                            },
                            symbolSize: function (val) {
                                return val[2] * 40;
                            },
                            data: data1
                        },


                    ]
                }
                $scope.seriesOptions.push(seriseOne);
            }
            option = {
                baseOption: {
                    timeline: {
                        // y: 0,
                        axisType: 'category',
                        realtime: false,
                        loop: true,
                        autoPlay: false,
                        currentIndex: 0,
                        playInterval: 1000,
                        // controlStyle: {
                        //     position: 'left'
                        // },
                        x: 10,
                        x2: 10,
                        bottom: -80,
                        padding: 15,
                        data: $scope.TimeKey,
                        label: {
                            formatter: function (s) {
                                return (new Date(s)).getFullYear() + '-' + ((new Date(s)).getMonth() + 1) + '-' + ((new Date(s)).getDate());
                            },
                            rotate: 25
                        }
                    },
                    calculable: true,
                    grid: {
                        top: 50,
                        bottom: 60
                    },
                    lineStyle: {
                        color: '#666',
                        width: 1,
                        type: 'dashed'
                    },
                    checkpointStyle: {
                        symbol: 'auto',
                        symbolSize: 'auto',
                        color: 'auto',
                        type: 'time',
                        borderColor: 'auto',
                        borderWidth: 'auto',
                        label: {
                            show: false,
                            textStyle: {
                                color: 'auto'
                            }
                        }
                    },
                },
                options: $scope.seriesOptions
            }
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;

        });
    };


    $scope.RefreshchainMap = function () {
        $scope.pagesize = $scope.pagesize;
        $scope.chainMap2();
    }

    //世界地图
    var myChart = echarts.init(document.getElementById('worldMap'));


    var latlong = {};
    latlong.AD = {'latitude': 42.5, 'longitude': 1.5};
    latlong.AE = {'latitude': 24, 'longitude': 54};
    latlong.AF = {'latitude': 33, 'longitude': 65};
    latlong.AG = {'latitude': 17.05, 'longitude': -61.8};
    latlong.AI = {'latitude': 18.25, 'longitude': -63.1667};
    latlong.AL = {'latitude': 41, 'longitude': 20};
    latlong.AM = {'latitude': 40, 'longitude': 45};
    latlong.AN = {'latitude': 12.25, 'longitude': -68.75};
    latlong.AO = {'latitude': -12.5, 'longitude': 18.5};
    latlong.AP = {'latitude': 35, 'longitude': 105};
    latlong.AQ = {'latitude': -90, 'longitude': 0};
    latlong.AR = {'latitude': -34, 'longitude': -64};
    latlong.AS = {'latitude': -14.3333, 'longitude': -170};
    latlong.AT = {'latitude': 47.3333, 'longitude': 13.3333};
    latlong.AU = {'latitude': -27, 'longitude': 133};
    latlong.AW = {'latitude': 12.5, 'longitude': -69.9667};
    latlong.AZ = {'latitude': 40.5, 'longitude': 47.5};
    latlong.BA = {'latitude': 44, 'longitude': 18};
    latlong.BB = {'latitude': 13.1667, 'longitude': -59.5333};
    latlong.BD = {'latitude': 24, 'longitude': 90};
    latlong.BE = {'latitude': 50.8333, 'longitude': 4};
    latlong.BF = {'latitude': 13, 'longitude': -2};
    latlong.BG = {'latitude': 43, 'longitude': 25};
    latlong.BH = {'latitude': 26, 'longitude': 50.55};
    latlong.BI = {'latitude': -3.5, 'longitude': 30};
    latlong.BJ = {'latitude': 9.5, 'longitude': 2.25};
    latlong.BM = {'latitude': 32.3333, 'longitude': -64.75};
    latlong.BN = {'latitude': 4.5, 'longitude': 114.6667};
    latlong.BO = {'latitude': -17, 'longitude': -65};
    latlong.BR = {'latitude': -10, 'longitude': -55};
    latlong.BS = {'latitude': 24.25, 'longitude': -76};
    latlong.BT = {'latitude': 27.5, 'longitude': 90.5};
    latlong.BV = {'latitude': -54.4333, 'longitude': 3.4};
    latlong.BW = {'latitude': -22, 'longitude': 24};
    latlong.BY = {'latitude': 53, 'longitude': 28};
    latlong.BZ = {'latitude': 17.25, 'longitude': -88.75};
    latlong.CA = {'latitude': 54, 'longitude': -100};
    latlong.CC = {'latitude': -12.5, 'longitude': 96.8333};
    latlong.CD = {'latitude': 0, 'longitude': 25};
    latlong.CF = {'latitude': 7, 'longitude': 21};
    latlong.CG = {'latitude': -1, 'longitude': 15};
    latlong.CH = {'latitude': 47, 'longitude': 8};
    latlong.CI = {'latitude': 8, 'longitude': -5};
    latlong.CK = {'latitude': -21.2333, 'longitude': -159.7667};
    latlong.CL = {'latitude': -30, 'longitude': -71};
    latlong.CM = {'latitude': 6, 'longitude': 12};
    latlong.CN = {'latitude': 35, 'longitude': 105};
    latlong.CO = {'latitude': 4, 'longitude': -72};
    latlong.CR = {'latitude': 10, 'longitude': -84};
    latlong.CU = {'latitude': 21.5, 'longitude': -80};
    latlong.CV = {'latitude': 16, 'longitude': -24};
    latlong.CX = {'latitude': -10.5, 'longitude': 105.6667};
    latlong.CY = {'latitude': 35, 'longitude': 33};
    latlong.CZ = {'latitude': 49.75, 'longitude': 15.5};
    latlong.DE = {'latitude': 51, 'longitude': 9};
    latlong.DJ = {'latitude': 11.5, 'longitude': 43};
    latlong.DK = {'latitude': 56, 'longitude': 10};
    latlong.DM = {'latitude': 15.4167, 'longitude': -61.3333};
    latlong.DO = {'latitude': 19, 'longitude': -70.6667};
    latlong.DZ = {'latitude': 28, 'longitude': 3};
    latlong.EC = {'latitude': -2, 'longitude': -77.5};
    latlong.EE = {'latitude': 59, 'longitude': 26};
    latlong.EG = {'latitude': 27, 'longitude': 30};
    latlong.EH = {'latitude': 24.5, 'longitude': -13};
    latlong.ER = {'latitude': 15, 'longitude': 39};
    latlong.ES = {'latitude': 40, 'longitude': -4};
    latlong.ET = {'latitude': 8, 'longitude': 38};
    latlong.EU = {'latitude': 47, 'longitude': 8};
    latlong.FI = {'latitude': 62, 'longitude': 26};
    latlong.FJ = {'latitude': -18, 'longitude': 175};
    latlong.FK = {'latitude': -51.75, 'longitude': -59};
    latlong.FM = {'latitude': 6.9167, 'longitude': 158.25};
    latlong.FO = {'latitude': 62, 'longitude': -7};
    latlong.FR = {'latitude': 46, 'longitude': 2};
    latlong.GA = {'latitude': -1, 'longitude': 11.75};
    latlong.GB = {'latitude': 54, 'longitude': -2};
    latlong.GD = {'latitude': 12.1167, 'longitude': -61.6667};
    latlong.GE = {'latitude': 42, 'longitude': 43.5};
    latlong.GF = {'latitude': 4, 'longitude': -53};
    latlong.GH = {'latitude': 8, 'longitude': -2};
    latlong.GI = {'latitude': 36.1833, 'longitude': -5.3667};
    latlong.GL = {'latitude': 72, 'longitude': -40};
    latlong.GM = {'latitude': 13.4667, 'longitude': -16.5667};
    latlong.GN = {'latitude': 11, 'longitude': -10};
    latlong.GP = {'latitude': 16.25, 'longitude': -61.5833};
    latlong.GQ = {'latitude': 2, 'longitude': 10};
    latlong.GR = {'latitude': 39, 'longitude': 22};
    latlong.GS = {'latitude': -54.5, 'longitude': -37};
    latlong.GT = {'latitude': 15.5, 'longitude': -90.25};
    latlong.GU = {'latitude': 13.4667, 'longitude': 144.7833};
    latlong.GW = {'latitude': 12, 'longitude': -15};
    latlong.GY = {'latitude': 5, 'longitude': -59};
    latlong.HK = {'latitude': 22.25, 'longitude': 114.1667};
    latlong.HM = {'latitude': -53.1, 'longitude': 72.5167};
    latlong.HN = {'latitude': 15, 'longitude': -86.5};
    latlong.HR = {'latitude': 45.1667, 'longitude': 15.5};
    latlong.HT = {'latitude': 19, 'longitude': -72.4167};
    latlong.HU = {'latitude': 47, 'longitude': 20};
    latlong.ID = {'latitude': -5, 'longitude': 120};
    latlong.IE = {'latitude': 53, 'longitude': -8};
    latlong.IL = {'latitude': 31.5, 'longitude': 34.75};
    latlong.IN = {'latitude': 20, 'longitude': 77};
    latlong.IO = {'latitude': -6, 'longitude': 71.5};
    latlong.IQ = {'latitude': 33, 'longitude': 44};
    latlong.IR = {'latitude': 32, 'longitude': 53};
    latlong.IS = {'latitude': 65, 'longitude': -18};
    latlong.IT = {'latitude': 42.8333, 'longitude': 12.8333};
    latlong.JM = {'latitude': 18.25, 'longitude': -77.5};
    latlong.JO = {'latitude': 31, 'longitude': 36};
    latlong.JP = {'latitude': 36, 'longitude': 138};
    latlong.KE = {'latitude': 1, 'longitude': 38};
    latlong.KG = {'latitude': 41, 'longitude': 75};
    latlong.KH = {'latitude': 13, 'longitude': 105};
    latlong.KI = {'latitude': 1.4167, 'longitude': 173};
    latlong.KM = {'latitude': -12.1667, 'longitude': 44.25};
    latlong.KN = {'latitude': 17.3333, 'longitude': -62.75};
    latlong.KP = {'latitude': 40, 'longitude': 127};
    latlong.KR = {'latitude': 37, 'longitude': 127.5};
    latlong.KW = {'latitude': 29.3375, 'longitude': 47.6581};
    latlong.KY = {'latitude': 19.5, 'longitude': -80.5};
    latlong.KZ = {'latitude': 48, 'longitude': 68};
    latlong.LA = {'latitude': 18, 'longitude': 105};
    latlong.LB = {'latitude': 33.8333, 'longitude': 35.8333};
    latlong.LC = {'latitude': 13.8833, 'longitude': -61.1333};
    latlong.LI = {'latitude': 47.1667, 'longitude': 9.5333};
    latlong.LK = {'latitude': 7, 'longitude': 81};
    latlong.LR = {'latitude': 6.5, 'longitude': -9.5};
    latlong.LS = {'latitude': -29.5, 'longitude': 28.5};
    latlong.LT = {'latitude': 55, 'longitude': 24};
    latlong.LU = {'latitude': 49.75, 'longitude': 6};
    latlong.LV = {'latitude': 57, 'longitude': 25};
    latlong.LY = {'latitude': 25, 'longitude': 17};
    latlong.MA = {'latitude': 32, 'longitude': -5};
    latlong.MC = {'latitude': 43.7333, 'longitude': 7.4};
    latlong.MD = {'latitude': 47, 'longitude': 29};
    latlong.ME = {'latitude': 42.5, 'longitude': 19.4};
    latlong.MG = {'latitude': -20, 'longitude': 47};
    latlong.MH = {'latitude': 9, 'longitude': 168};
    latlong.MK = {'latitude': 41.8333, 'longitude': 22};
    latlong.ML = {'latitude': 17, 'longitude': -4};
    latlong.MM = {'latitude': 22, 'longitude': 98};
    latlong.MN = {'latitude': 46, 'longitude': 105};
    latlong.MO = {'latitude': 22.1667, 'longitude': 113.55};
    latlong.MP = {'latitude': 15.2, 'longitude': 145.75};
    latlong.MQ = {'latitude': 14.6667, 'longitude': -61};
    latlong.MR = {'latitude': 20, 'longitude': -12};
    latlong.MS = {'latitude': 16.75, 'longitude': -62.2};
    latlong.MT = {'latitude': 35.8333, 'longitude': 14.5833};
    latlong.MU = {'latitude': -20.2833, 'longitude': 57.55};
    latlong.MV = {'latitude': 3.25, 'longitude': 73};
    latlong.MW = {'latitude': -13.5, 'longitude': 34};
    latlong.MX = {'latitude': 23, 'longitude': -102};
    latlong.MY = {'latitude': 2.5, 'longitude': 112.5};
    latlong.MZ = {'latitude': -18.25, 'longitude': 35};
    latlong.NA = {'latitude': -22, 'longitude': 17};
    latlong.NC = {'latitude': -21.5, 'longitude': 165.5};
    latlong.NE = {'latitude': 16, 'longitude': 8};
    latlong.NF = {'latitude': -29.0333, 'longitude': 167.95};
    latlong.NG = {'latitude': 10, 'longitude': 8};
    latlong.NI = {'latitude': 13, 'longitude': -85};
    latlong.NL = {'latitude': 52.5, 'longitude': 5.75};
    latlong.NO = {'latitude': 62, 'longitude': 10};
    latlong.NP = {'latitude': 28, 'longitude': 84};
    latlong.NR = {'latitude': -0.5333, 'longitude': 166.9167};
    latlong.NU = {'latitude': -19.0333, 'longitude': -169.8667};
    latlong.NZ = {'latitude': -41, 'longitude': 174};
    latlong.OM = {'latitude': 21, 'longitude': 57};
    latlong.PA = {'latitude': 9, 'longitude': -80};
    latlong.PE = {'latitude': -10, 'longitude': -76};
    latlong.PF = {'latitude': -15, 'longitude': -140};
    latlong.PG = {'latitude': -6, 'longitude': 147};
    latlong.PH = {'latitude': 13, 'longitude': 122};
    latlong.PK = {'latitude': 30, 'longitude': 70};
    latlong.PL = {'latitude': 52, 'longitude': 20};
    latlong.PM = {'latitude': 46.8333, 'longitude': -56.3333};
    latlong.PR = {'latitude': 18.25, 'longitude': -66.5};
    latlong.PS = {'latitude': 32, 'longitude': 35.25};
    latlong.PT = {'latitude': 39.5, 'longitude': -8};
    latlong.PW = {'latitude': 7.5, 'longitude': 134.5};
    latlong.PY = {'latitude': -23, 'longitude': -58};
    latlong.QA = {'latitude': 25.5, 'longitude': 51.25};
    latlong.RE = {'latitude': -21.1, 'longitude': 55.6};
    latlong.RO = {'latitude': 46, 'longitude': 25};
    latlong.RS = {'latitude': 44, 'longitude': 21};
    latlong.RU = {'latitude': 60, 'longitude': 100};
    latlong.RW = {'latitude': -2, 'longitude': 30};
    latlong.SA = {'latitude': 25, 'longitude': 45};
    latlong.SB = {'latitude': -8, 'longitude': 159};
    latlong.SC = {'latitude': -4.5833, 'longitude': 55.6667};
    latlong.SD = {'latitude': 15, 'longitude': 30};
    latlong.SE = {'latitude': 62, 'longitude': 15};
    latlong.SG = {'latitude': 1.3667, 'longitude': 103.8};
    latlong.SH = {'latitude': -15.9333, 'longitude': -5.7};
    latlong.SI = {'latitude': 46, 'longitude': 15};
    latlong.SJ = {'latitude': 78, 'longitude': 20};
    latlong.SK = {'latitude': 48.6667, 'longitude': 19.5};
    latlong.SL = {'latitude': 8.5, 'longitude': -11.5};
    latlong.SM = {'latitude': 43.7667, 'longitude': 12.4167};
    latlong.SN = {'latitude': 14, 'longitude': -14};
    latlong.SO = {'latitude': 10, 'longitude': 49};
    latlong.SR = {'latitude': 4, 'longitude': -56};
    latlong.ST = {'latitude': 1, 'longitude': 7};
    latlong.SV = {'latitude': 13.8333, 'longitude': -88.9167};
    latlong.SY = {'latitude': 35, 'longitude': 38};
    latlong.SZ = {'latitude': -26.5, 'longitude': 31.5};
    latlong.TC = {'latitude': 21.75, 'longitude': -71.5833};
    latlong.TD = {'latitude': 15, 'longitude': 19};
    latlong.TF = {'latitude': -43, 'longitude': 67};
    latlong.TG = {'latitude': 8, 'longitude': 1.1667};
    latlong.TH = {'latitude': 15, 'longitude': 100};
    latlong.TJ = {'latitude': 39, 'longitude': 71};
    latlong.TK = {'latitude': -9, 'longitude': -172};
    latlong.TM = {'latitude': 40, 'longitude': 60};
    latlong.TN = {'latitude': 34, 'longitude': 9};
    latlong.TO = {'latitude': -20, 'longitude': -175};
    latlong.TR = {'latitude': 39, 'longitude': 35};
    latlong.TT = {'latitude': 11, 'longitude': -61};
    latlong.TV = {'latitude': -8, 'longitude': 178};
    latlong.TW = {'latitude': 23.5, 'longitude': 121};
    latlong.TZ = {'latitude': -6, 'longitude': 35};
    latlong.UA = {'latitude': 49, 'longitude': 32};
    latlong.UG = {'latitude': 1, 'longitude': 32};
    latlong.UM = {'latitude': 19.2833, 'longitude': 166.6};
    latlong.US = {'latitude': 38, 'longitude': -97};
    latlong.UY = {'latitude': -33, 'longitude': -56};
    latlong.UZ = {'latitude': 41, 'longitude': 64};
    latlong.VA = {'latitude': 41.9, 'longitude': 12.45};
    latlong.VC = {'latitude': 13.25, 'longitude': -61.2};
    latlong.VE = {'latitude': 8, 'longitude': -66};
    latlong.VG = {'latitude': 18.5, 'longitude': -64.5};
    latlong.VI = {'latitude': 18.3333, 'longitude': -64.8333};
    latlong.VN = {'latitude': 16, 'longitude': 106};
    latlong.VU = {'latitude': -16, 'longitude': 167};
    latlong.WF = {'latitude': -13.3, 'longitude': -176.2};
    latlong.WS = {'latitude': -13.5833, 'longitude': -172.3333};
    latlong.YE = {'latitude': 15, 'longitude': 48};
    latlong.YT = {'latitude': -12.8333, 'longitude': 45.1667};
    latlong.ZA = {'latitude': -29, 'longitude': 24};
    latlong.ZM = {'latitude': -15, 'longitude': 30};
    latlong.ZW = {'latitude': -20, 'longitude': 30};

    var mapData = [
        {'code': 'AF', 'name': 'Afghanistan', 'value': 32358260, 'color': '#eea638'},
        {'code': 'AL', 'name': 'Albania', 'value': 3215988, 'color': '#d8854f'},
        {'code': 'DZ', 'name': 'Algeria', 'value': 35980193, 'color': '#de4c4f'},
        {'code': 'AO', 'name': 'Angola', 'value': 19618432, 'color': '#de4c4f'},
        {'code': 'AR', 'name': 'Argentina', 'value': 40764561, 'color': '#86a965'},
        {'code': 'AM', 'name': 'Armenia', 'value': 3100236, 'color': '#d8854f'},
        {'code': 'AU', 'name': 'Australia', 'value': 22605732, 'color': '#8aabb0'},
        {'code': 'AT', 'name': 'Austria', 'value': 8413429, 'color': '#d8854f'},
        {'code': 'AZ', 'name': 'Azerbaijan', 'value': 9306023, 'color': '#d8854f'},
        {'code': 'BH', 'name': 'Bahrain', 'value': 1323535, 'color': '#eea638'},
        {'code': 'BD', 'name': 'Bangladesh', 'value': 150493658, 'color': '#eea638'},
        {'code': 'BY', 'name': 'Belarus', 'value': 9559441, 'color': '#d8854f'},
        {'code': 'BE', 'name': 'Belgium', 'value': 10754056, 'color': '#d8854f'},
        {'code': 'BJ', 'name': 'Benin', 'value': 9099922, 'color': '#de4c4f'},
        {'code': 'BT', 'name': 'Bhutan', 'value': 738267, 'color': '#eea638'},
        {'code': 'BO', 'name': 'Bolivia', 'value': 10088108, 'color': '#86a965'},
        {'code': 'BA', 'name': 'Bosnia and Herzegovina', 'value': 3752228, 'color': '#d8854f'},
        {'code': 'BW', 'name': 'Botswana', 'value': 2030738, 'color': '#de4c4f'},
        {'code': 'BR', 'name': 'Brazil', 'value': 196655014, 'color': '#86a965'},
        {'code': 'BN', 'name': 'Brunei', 'value': 405938, 'color': '#eea638'},
        {'code': 'BG', 'name': 'Bulgaria', 'value': 7446135, 'color': '#d8854f'},
        {'code': 'BF', 'name': 'Burkina Faso', 'value': 16967845, 'color': '#de4c4f'},
        {'code': 'BI', 'name': 'Burundi', 'value': 8575172, 'color': '#de4c4f'},
        {'code': 'KH', 'name': 'Cambodia', 'value': 14305183, 'color': '#eea638'},
        {'code': 'CM', 'name': 'Cameroon', 'value': 20030362, 'color': '#de4c4f'},
        {'code': 'CA', 'name': 'Canada', 'value': 34349561, 'color': '#a7a737'},
        {'code': 'CV', 'name': 'Cape Verde', 'value': 500585, 'color': '#de4c4f'},
        {'code': 'CF', 'name': 'Central African Rep.', 'value': 4486837, 'color': '#de4c4f'},
        {'code': 'TD', 'name': 'Chad', 'value': 11525496, 'color': '#de4c4f'},
        {'code': 'CL', 'name': 'Chile', 'value': 17269525, 'color': '#86a965'},
        {'code': 'CN', 'name': 'China', 'value': 1347565324, 'color': '#eea638'},
        {'code': 'CO', 'name': 'Colombia', 'value': 46927125, 'color': '#86a965'},
        {'code': 'KM', 'name': 'Comoros', 'value': 753943, 'color': '#de4c4f'},
        {'code': 'CD', 'name': 'Congo, Dem. Rep.', 'value': 67757577, 'color': '#de4c4f'},
        {'code': 'CG', 'name': 'Congo, Rep.', 'value': 4139748, 'color': '#de4c4f'},
        {'code': 'CR', 'name': 'Costa Rica', 'value': 4726575, 'color': '#a7a737'},
        {'code': 'CI', 'name': 'Cote d\'Ivoire', 'value': 20152894, 'color': '#de4c4f'},
        {'code': 'HR', 'name': 'Croatia', 'value': 4395560, 'color': '#d8854f'},
        {'code': 'CU', 'name': 'Cuba', 'value': 11253665, 'color': '#a7a737'},
        {'code': 'CY', 'name': 'Cyprus', 'value': 1116564, 'color': '#d8854f'},
        {'code': 'CZ', 'name': 'Czech Rep.', 'value': 10534293, 'color': '#d8854f'},
        {'code': 'DK', 'name': 'Denmark', 'value': 5572594, 'color': '#d8854f'},
        {'code': 'DJ', 'name': 'Djibouti', 'value': 905564, 'color': '#de4c4f'},
        {'code': 'DO', 'name': 'Dominican Rep.', 'value': 10056181, 'color': '#a7a737'},
        {'code': 'EC', 'name': 'Ecuador', 'value': 14666055, 'color': '#86a965'},
        {'code': 'EG', 'name': 'Egypt', 'value': 82536770, 'color': '#de4c4f'},
        {'code': 'SV', 'name': 'El Salvador', 'value': 6227491, 'color': '#a7a737'},
        {'code': 'GQ', 'name': 'Equatorial Guinea', 'value': 720213, 'color': '#de4c4f'},
        {'code': 'ER', 'name': 'Eritrea', 'value': 5415280, 'color': '#de4c4f'},
        {'code': 'EE', 'name': 'Estonia', 'value': 1340537, 'color': '#d8854f'},
        {'code': 'ET', 'name': 'Ethiopia', 'value': 84734262, 'color': '#de4c4f'},
        {'code': 'FJ', 'name': 'Fiji', 'value': 868406, 'color': '#8aabb0'},
        {'code': 'FI', 'name': 'Finland', 'value': 5384770, 'color': '#d8854f'},
        {'code': 'FR', 'name': 'France', 'value': 63125894, 'color': '#d8854f'},
        {'code': 'GA', 'name': 'Gabon', 'value': 1534262, 'color': '#de4c4f'},
        {'code': 'GM', 'name': 'Gambia', 'value': 1776103, 'color': '#de4c4f'},
        {'code': 'GE', 'name': 'Georgia', 'value': 4329026, 'color': '#d8854f'},
        {'code': 'DE', 'name': 'Germany', 'value': 82162512, 'color': '#d8854f'},
        {'code': 'GH', 'name': 'Ghana', 'value': 24965816, 'color': '#de4c4f'},
        {'code': 'GR', 'name': 'Greece', 'value': 11390031, 'color': '#d8854f'},
        {'code': 'GT', 'name': 'Guatemala', 'value': 14757316, 'color': '#a7a737'},
        {'code': 'GN', 'name': 'Guinea', 'value': 10221808, 'color': '#de4c4f'},
        {'code': 'GW', 'name': 'Guinea-Bissau', 'value': 1547061, 'color': '#de4c4f'},
        {'code': 'GY', 'name': 'Guyana', 'value': 756040, 'color': '#86a965'},
        {'code': 'HT', 'name': 'Haiti', 'value': 10123787, 'color': '#a7a737'},
        {'code': 'HN', 'name': 'Honduras', 'value': 7754687, 'color': '#a7a737'},
        {'code': 'HK', 'name': 'Hong Kong, China', 'value': 7122187, 'color': '#eea638'},
        {'code': 'HU', 'name': 'Hungary', 'value': 9966116, 'color': '#d8854f'},
        {'code': 'IS', 'name': 'Iceland', 'value': 324366, 'color': '#d8854f'},
        {'code': 'IN', 'name': 'India', 'value': 1241491960, 'color': '#eea638'},
        {'code': 'ID', 'name': 'Indonesia', 'value': 242325638, 'color': '#eea638'},
        {'code': 'IR', 'name': 'Iran', 'value': 74798599, 'color': '#eea638'},
        {'code': 'IQ', 'name': 'Iraq', 'value': 32664942, 'color': '#eea638'},
        {'code': 'IE', 'name': 'Ireland', 'value': 4525802, 'color': '#d8854f'},
        {'code': 'IL', 'name': 'Israel', 'value': 7562194, 'color': '#eea638'},
        {'code': 'IT', 'name': 'Italy', 'value': 60788694, 'color': '#d8854f'},
        {'code': 'JM', 'name': 'Jamaica', 'value': 2751273, 'color': '#a7a737'},
        {'code': 'JP', 'name': 'Japan', 'value': 126497241, 'color': '#eea638'},
        {'code': 'JO', 'name': 'Jordan', 'value': 6330169, 'color': '#eea638'},
        {'code': 'KZ', 'name': 'Kazakhstan', 'value': 16206750, 'color': '#eea638'},
        {'code': 'KE', 'name': 'Kenya', 'value': 41609728, 'color': '#de4c4f'},
        {'code': 'KP', 'name': 'Korea, Dem. Rep.', 'value': 24451285, 'color': '#eea638'},
        {'code': 'KR', 'name': 'Korea, Rep.', 'value': 48391343, 'color': '#eea638'},
        {'code': 'KW', 'name': 'Kuwait', 'value': 2818042, 'color': '#eea638'},
        {'code': 'KG', 'name': 'Kyrgyzstan', 'value': 5392580, 'color': '#eea638'},
        {'code': 'LA', 'name': 'Laos', 'value': 6288037, 'color': '#eea638'},
        {'code': 'LV', 'name': 'Latvia', 'value': 2243142, 'color': '#d8854f'},
        {'code': 'LB', 'name': 'Lebanon', 'value': 4259405, 'color': '#eea638'},
        {'code': 'LS', 'name': 'Lesotho', 'value': 2193843, 'color': '#de4c4f'},
        {'code': 'LR', 'name': 'Liberia', 'value': 4128572, 'color': '#de4c4f'},
        {'code': 'LY', 'name': 'Libya', 'value': 6422772, 'color': '#de4c4f'},
        {'code': 'LT', 'name': 'Lithuania', 'value': 3307481, 'color': '#d8854f'},
        {'code': 'LU', 'name': 'Luxembourg', 'value': 515941, 'color': '#d8854f'},
        {'code': 'MK', 'name': 'Macedonia, FYR', 'value': 2063893, 'color': '#d8854f'},
        {'code': 'MG', 'name': 'Madagascar', 'value': 21315135, 'color': '#de4c4f'},
        {'code': 'MW', 'name': 'Malawi', 'value': 15380888, 'color': '#de4c4f'},
        {'code': 'MY', 'name': 'Malaysia', 'value': 28859154, 'color': '#eea638'},
        {'code': 'ML', 'name': 'Mali', 'value': 15839538, 'color': '#de4c4f'},
        {'code': 'MR', 'name': 'Mauritania', 'value': 3541540, 'color': '#de4c4f'},
        {'code': 'MU', 'name': 'Mauritius', 'value': 1306593, 'color': '#de4c4f'},
        {'code': 'MX', 'name': 'Mexico', 'value': 114793341, 'color': '#a7a737'},
        {'code': 'MD', 'name': 'Moldova', 'value': 3544864, 'color': '#d8854f'},
        {'code': 'MN', 'name': 'Mongolia', 'value': 2800114, 'color': '#eea638'},
        {'code': 'ME', 'name': 'Montenegro', 'value': 632261, 'color': '#d8854f'},
        {'code': 'MA', 'name': 'Morocco', 'value': 32272974, 'color': '#de4c4f'},
        {'code': 'MZ', 'name': 'Mozambique', 'value': 23929708, 'color': '#de4c4f'},
        {'code': 'MM', 'name': 'Myanmar', 'value': 48336763, 'color': '#eea638'},
        {'code': 'NA', 'name': 'Namibia', 'value': 2324004, 'color': '#de4c4f'},
        {'code': 'NP', 'name': 'Nepal', 'value': 30485798, 'color': '#eea638'},
        {'code': 'NL', 'name': 'Netherlands', 'value': 16664746, 'color': '#d8854f'},
        {'code': 'NZ', 'name': 'New Zealand', 'value': 4414509, 'color': '#8aabb0'},
        {'code': 'NI', 'name': 'Nicaragua', 'value': 5869859, 'color': '#a7a737'},
        {'code': 'NE', 'name': 'Niger', 'value': 16068994, 'color': '#de4c4f'},
        {'code': 'NG', 'name': 'Nigeria', 'value': 162470737, 'color': '#de4c4f'},
        {'code': 'NO', 'name': 'Norway', 'value': 4924848, 'color': '#d8854f'},
        {'code': 'OM', 'name': 'Oman', 'value': 2846145, 'color': '#eea638'},
        {'code': 'PK', 'name': 'Pakistan', 'value': 176745364, 'color': '#eea638'},
        {'code': 'PA', 'name': 'Panama', 'value': 3571185, 'color': '#a7a737'},
        {'code': 'PG', 'name': 'Papua New Guinea', 'value': 7013829, 'color': '#8aabb0'},
        {'code': 'PY', 'name': 'Paraguay', 'value': 6568290, 'color': '#86a965'},
        {'code': 'PE', 'name': 'Peru', 'value': 29399817, 'color': '#86a965'},
        {'code': 'PH', 'name': 'Philippines', 'value': 94852030, 'color': '#eea638'},
        {'code': 'PL', 'name': 'Poland', 'value': 38298949, 'color': '#d8854f'},
        {'code': 'PT', 'name': 'Portugal', 'value': 10689663, 'color': '#d8854f'},
        {'code': 'PR', 'name': 'Puerto Rico', 'value': 3745526, 'color': '#a7a737'},
        {'code': 'QA', 'name': 'Qatar', 'value': 1870041, 'color': '#eea638'},
        {'code': 'RO', 'name': 'Romania', 'value': 21436495, 'color': '#d8854f'},
        {'code': 'RU', 'name': 'Russia', 'value': 142835555, 'color': '#d8854f'},
        {'code': 'RW', 'name': 'Rwanda', 'value': 10942950, 'color': '#de4c4f'},
        {'code': 'SA', 'name': 'Saudi Arabia', 'value': 28082541, 'color': '#eea638'},
        {'code': 'SN', 'name': 'Senegal', 'value': 12767556, 'color': '#de4c4f'},
        {'code': 'RS', 'name': 'Serbia', 'value': 9853969, 'color': '#d8854f'},
        {'code': 'SL', 'name': 'Sierra Leone', 'value': 5997486, 'color': '#de4c4f'},
        {'code': 'SG', 'name': 'Singapore', 'value': 5187933, 'color': '#eea638'},
        {'code': 'SK', 'name': 'Slovak Republic', 'value': 5471502, 'color': '#d8854f'},
        {'code': 'SI', 'name': 'Slovenia', 'value': 2035012, 'color': '#d8854f'},
        {'code': 'SB', 'name': 'Solomon Islands', 'value': 552267, 'color': '#8aabb0'},
        {'code': 'SO', 'name': 'Somalia', 'value': 9556873, 'color': '#de4c4f'},
        {'code': 'ZA', 'name': 'South Africa', 'value': 50459978, 'color': '#de4c4f'},
        {'code': 'ES', 'name': 'Spain', 'value': 46454895, 'color': '#d8854f'},
        {'code': 'LK', 'name': 'Sri Lanka', 'value': 21045394, 'color': '#eea638'},
        {'code': 'SD', 'name': 'Sudan', 'value': 34735288, 'color': '#de4c4f'},
        {'code': 'SR', 'name': 'Suriname', 'value': 529419, 'color': '#86a965'},
        {'code': 'SZ', 'name': 'Swaziland', 'value': 1203330, 'color': '#de4c4f'},
        {'code': 'SE', 'name': 'Sweden', 'value': 9440747, 'color': '#d8854f'},
        {'code': 'CH', 'name': 'Switzerland', 'value': 7701690, 'color': '#d8854f'},
        {'code': 'SY', 'name': 'Syria', 'value': 20766037, 'color': '#eea638'},
        {'code': 'TW', 'name': 'Taiwan', 'value': 23072000, 'color': '#eea638'},
        {'code': 'TJ', 'name': 'Tajikistan', 'value': 6976958, 'color': '#eea638'},
        {'code': 'TZ', 'name': 'Tanzania', 'value': 46218486, 'color': '#de4c4f'},
        {'code': 'TH', 'name': 'Thailand', 'value': 69518555, 'color': '#eea638'},
        {'code': 'TG', 'name': 'Togo', 'value': 6154813, 'color': '#de4c4f'},
        {'code': 'TT', 'name': 'Trinidad and Tobago', 'value': 1346350, 'color': '#a7a737'},
        {'code': 'TN', 'name': 'Tunisia', 'value': 10594057, 'color': '#de4c4f'},
        {'code': 'TR', 'name': 'Turkey', 'value': 73639596, 'color': '#d8854f'},
        {'code': 'TM', 'name': 'Turkmenistan', 'value': 5105301, 'color': '#eea638'},
        {'code': 'UG', 'name': 'Uganda', 'value': 34509205, 'color': '#de4c4f'},
        {'code': 'UA', 'name': 'Ukraine', 'value': 45190180, 'color': '#d8854f'},
        {'code': 'AE', 'name': 'United Arab Emirates', 'value': 7890924, 'color': '#eea638'},
        {'code': 'GB', 'name': 'United Kingdom', 'value': 62417431, 'color': '#d8854f'},
        {'code': 'US', 'name': 'United States', 'value': 313085380, 'color': '#a7a737'},
        {'code': 'UY', 'name': 'Uruguay', 'value': 3380008, 'color': '#86a965'},
        {'code': 'UZ', 'name': 'Uzbekistan', 'value': 27760267, 'color': '#eea638'},
        {'code': 'VE', 'name': 'Venezuela', 'value': 29436891, 'color': '#86a965'},
        {'code': 'PS', 'name': 'West Bank and Gaza', 'value': 4152369, 'color': '#eea638'},
        {'code': 'VN', 'name': 'Vietnam', 'value': 88791996, 'color': '#eea638'},
        {'code': 'YE', 'name': 'Yemen, Rep.', 'value': 24799880, 'color': '#eea638'},
        {'code': 'ZM', 'name': 'Zambia', 'value': 13474959, 'color': '#de4c4f'},
        {'code': 'ZW', 'name': 'Zimbabwe', 'value': 12754378, 'color': '#de4c4f'}];

    var max = -Infinity;
    var min = Infinity;
    mapData.forEach(function (itemOpt) {
        if (itemOpt.value > max) {
            max = itemOpt.value;
        }
        if (itemOpt.value < min) {
            min = itemOpt.value;
        }
    });

    option = {
        backgroundColor: '#fff',
        title: {
            text: 'World Population (2011)',
            subtext: 'From Gapminder',
            left: 'center',
            top: 'top',
            textStyle: {
                color: '#fff'
            }
        },
        tooltip: {
            trigger: 'item',
            formatter: function (params) {
                var value = (params.value + '').split('.');
                value = value[0].replace(/(\d{1,3})(?=(?:\d{3})+(?!\d))/g, '$1,')
                + '.' + value[1];
                return params.seriesName + '<br/>' + params.name + ' : ' + value;
            }
        },
        visualMap: {
            show: false,
            min: 0,
            max: max,
            inRange: {
                symbolSize: [6, 60]
            }
        },
        geo: {
            name: 'World Population (2010)',
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
                    areaColor: '#323c48',
                    borderColor: '#111'
                },
                emphasis: {
                    areaColor: '#2a333d'
                }
            }
        },
        series: [
            {
                type: 'scatter',
                coordinateSystem: 'geo',
                data: mapData.map(function (itemOpt) {
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

    myChart.setOption(option);

    //切换地图
    $scope.changeMap_show = function () {
        $scope.isActiveditu = true;
    }
    $scope.changeMap_hide = function () {
        $scope.isActiveditu = false;
    }

    //中国地图
    $scope.chinaMapData = [];
    $scope.chainMap = function (num) {
        $scope.pagesize = num;
        $scope.GetciteLists = [];
        $scope.chainMap2();
    }
    $scope.chainMap2 = function () {
        var url = "/api/Dashboard/GetItemList?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&pagesize=" + $scope.pagesize + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>chainMap2');
            console.log(response);
            var myChart = echarts.init(document.getElementById('chainMap'));
            myChart.showLoading();

            $scope.GetciteLists = response;
            var data22 = [];
            data22 = response;
            var dataMax = data22[0].salesAmount;
            var dataSize = 400 / dataMax;


            // 基于准备好的dom，初始化echarts实例
            // 指定图表的配置项和数据
            var geoCoordMap = {
                '海门': [121.15, 31.89],
                '鄂尔多斯': [109.781327, 39.608266],
                '招远': [120.38, 37.35],
                '舟山': [122.207216, 29.985295],
                '齐齐哈尔': [123.97, 47.33],
                '盐城': [120.13, 33.38],
                '赤峰': [118.87, 42.28],
                '青岛': [120.33, 36.07],
                '乳山': [121.52, 36.89],
                '金昌': [102.188043, 38.520089],
                '泉州': [118.58, 24.93],
                '莱西': [120.53, 36.86],
                '日照': [119.46, 35.42],
                '胶南': [119.97, 35.88],
                '南通': [121.05, 32.08],
                '拉萨': [91.11, 29.97],
                '云浮': [112.02, 22.93],
                '梅州': [116.1, 24.55],
                '文登': [122.05, 37.2],
                '上海': [121.48, 31.22],
                '攀枝花': [101.718637, 26.582347],
                '威海': [122.1, 37.5],
                '承德': [117.93, 40.97],
                '厦门': [118.1, 24.46],
                '汕尾': [115.375279, 22.786211],
                '潮州': [116.63, 23.68],
                '丹东': [124.37, 40.13],
                '太仓': [121.1, 31.45],
                '曲靖': [103.79, 25.51],
                '烟台': [121.39, 37.52],
                '福州': [119.3, 26.08],
                '瓦房店': [121.979603, 39.627114],
                '即墨': [120.45, 36.38],
                '抚顺': [123.97, 41.97],
                '玉溪': [102.52, 24.35],
                '张家口': [114.87, 40.82],
                '阳泉': [113.57, 37.85],
                '莱州': [119.942327, 37.177017],
                '湖州': [120.1, 30.86],
                '汕头': [116.69, 23.39],
                '昆山': [120.95, 31.39],
                '宁波': [121.56, 29.86],
                '湛江': [110.359377, 21.270708],
                '揭阳': [116.35, 23.55],
                '荣成': [122.41, 37.16],
                '连云港': [119.16, 34.59],
                '葫芦岛': [120.836932, 40.711052],
                '常熟': [120.74, 31.64],
                '东莞': [113.75, 23.04],
                '河源': [114.68, 23.73],
                '淮安': [119.15, 33.5],
                '泰州': [119.9, 32.49],
                '南宁': [108.33, 22.84],
                '营口': [122.18, 40.65],
                '惠州': [114.4, 23.09],
                '江阴': [120.26, 31.91],
                '蓬莱': [120.75, 37.8],
                '韶关': [113.62, 24.84],
                '嘉峪关': [98.289152, 39.77313],
                '广州': [113.23, 23.16],
                '延安': [109.47, 36.6],
                '太原': [112.53, 37.87],
                '清远': [113.01, 23.7],
                '中山': [113.38, 22.52],
                '昆明': [102.73, 25.04],
                '寿光': [118.73, 36.86],
                '盘锦': [122.070714, 41.119997],
                '长治': [113.08, 36.18],
                '深圳': [114.07, 22.62],
                '珠海': [113.52, 22.3],
                '宿迁': [118.3, 33.96],
                '咸阳': [108.72, 34.36],
                '铜川': [109.11, 35.09],
                '平度': [119.97, 36.77],
                '佛山': [113.11, 23.05],
                '海口': [110.35, 20.02],
                '江门': [113.06, 22.61],
                '章丘': [117.53, 36.72],
                '肇庆': [112.44, 23.05],
                '大连': [121.62, 38.92],
                '临汾': [111.5, 36.08],
                '吴江': [120.63, 31.16],
                '石嘴山': [106.39, 39.04],
                '沈阳': [123.38, 41.8],
                '苏州': [120.62, 31.32],
                '茂名': [110.88, 21.68],
                '嘉兴': [120.76, 30.77],
                '长春': [125.35, 43.88],
                '胶州': [120.03336, 36.264622],
                '银川': [106.27, 38.47],
                '张家港': [120.555821, 31.875428],
                '三门峡': [111.19, 34.76],
                '锦州': [121.15, 41.13],
                '南昌': [115.89, 28.68],
                '柳州': [109.4, 24.33],
                '三亚': [109.511909, 18.252847],
                '自贡': [104.778442, 29.33903],
                '吉林': [126.57, 43.87],
                '阳江': [111.95, 21.85],
                '泸州': [105.39, 28.91],
                '西宁': [101.74, 36.56],
                '宜宾': [104.56, 29.77],
                '呼和浩特': [111.65, 40.82],
                '成都': [104.06, 30.67],
                '大同': [113.3, 40.12],
                '镇江': [119.44, 32.2],
                '桂林': [110.28, 25.29],
                '张家界': [110.479191, 29.117096],
                '宜兴': [119.82, 31.36],
                '北海': [109.12, 21.49],
                '西安': [108.95, 34.27],
                '金坛': [119.56, 31.74],
                '东营': [118.49, 37.46],
                '牡丹江': [129.58, 44.6],
                '遵义': [106.9, 27.7],
                '绍兴': [120.58, 30.01],
                '扬州': [119.42, 32.39],
                '常州': [119.95, 31.79],
                '潍坊': [119.1, 36.62],
                '重庆': [106.54, 29.59],
                '台州': [121.420757, 28.656386],
                '南京': [118.78, 32.04],
                '滨州': [118.03, 37.36],
                '贵阳': [106.71, 26.57],
                '无锡': [120.29, 31.59],
                '本溪': [123.73, 41.3],
                '克拉玛依': [84.77, 45.59],
                '渭南': [109.5, 34.52],
                '马鞍山': [118.48, 31.56],
                '宝鸡': [107.15, 34.38],
                '焦作': [113.21, 35.24],
                '句容': [119.16, 31.95],
                '北京': [116.46, 39.92],
                '徐州': [117.2, 34.26],
                '衡水': [115.72, 37.72],
                '包头': [110, 40.58],
                '绵阳': [104.73, 31.48],
                '乌鲁木齐': [87.68, 43.77],
                '枣庄': [117.57, 34.86],
                '杭州': [120.19, 30.26],
                '淄博': [118.05, 36.78],
                '鞍山': [122.85, 41.12],
                '溧阳': [119.48, 31.43],
                '库尔勒': [86.06, 41.68],
                '安阳': [114.35, 36.1],
                '开封': [114.35, 34.79],
                '济南': [117, 36.65],
                '德阳': [104.37, 31.13],
                '温州': [120.65, 28.01],
                '九江': [115.97, 29.71],
                '邯郸': [114.47, 36.6],
                '临安': [119.72, 30.23],
                '兰州': [103.73, 36.03],
                '沧州': [116.83, 38.33],
                '临沂': [118.35, 35.05],
                '南充': [106.110698, 30.837793],
                '天津': [117.2, 39.13],
                '富阳': [119.95, 30.07],
                '泰安': [117.13, 36.18],
                '诸暨': [120.23, 29.71],
                '郑州': [113.65, 34.76],
                '哈尔滨': [126.63, 45.75],
                '聊城': [115.97, 36.45],
                '芜湖': [118.38, 31.33],
                '唐山': [118.02, 39.63],
                '平顶山': [113.29, 33.75],
                '邢台': [114.48, 37.05],
                '德州': [116.29, 37.45],
                '济宁': [116.59, 35.38],
                '荆州': [112.239741, 30.335165],
                '宜昌': [111.3, 30.7],
                '义乌': [120.06, 29.32],
                '丽水': [119.92, 28.45],
                '洛阳': [112.44, 34.7],
                '秦皇岛': [119.57, 39.95],
                '株洲': [113.16, 27.83],
                '石家庄': [114.48, 38.03],
                '莱芜': [117.67, 36.19],
                '常德': [111.69, 29.05],
                '保定': [115.48, 38.85],
                '湘潭': [112.91, 27.87],
                '金华': [119.64, 29.12],
                '岳阳': [113.09, 29.37],
                '长沙': [113, 28.21],
                '衢州': [118.88, 28.97],
                '廊坊': [116.7, 39.53],
                '菏泽': [115.480656, 35.23375],
                '合肥': [117.27, 31.86],
                '武汉': [114.31, 30.52],
                '大庆': [125.03, 46.58]
            };

            var convertData = function (data22) {
                var res = [];
                for (var i = 0; i < data22.length; i++) {
                    var geoCoord = geoCoordMap[data22[i].Location];
                    if (geoCoord) {
                        res.push({
                            name: data22[i].Location,
                            value: geoCoord.concat((data22[i].salesAmount) * dataSize + 30)
                        });
                    }
                }

                return res;

            };
            myChart.hideLoading();

            option = {
                backgroundColor: '#FFF',
                title: {
                    text: '',
                    //subtext: 'data from PM25.in',
                    //sublink: 'http://www.pm25.in',
                    left: 'center',
                    textStyle: {
                        color: '#35BDB2'
                    }
                },
                tooltip: {

                    formatter: function (obj) {
                        return obj.name + ':' + data22[obj.dataIndex].SalesVolume + '<br/>' + '销售额排名:' + (obj.dataIndex + 1)


                    }
                },

                legend: {
                    orient: 'vertical',
                    y: 'bottom',
                    x: 'right',
                    data: ['pm2.5'],
                    textStyle: {
                        color: '#fff'
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
                            areaColor: '#ccc',
                            borderColor: '#fff'
                        },
                        emphasis: {
                            areaColor: '#888585'
                        }
                    }
                },
                series: [
                    {
                        name: 'pm2.5',
                        type: 'scatter',
                        coordinateSystem: 'geo',
                        data: 1000,
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
                        //name: 'Top 6',
                        type: 'effectScatter',
                        coordinateSystem: 'geo',
                        data: convertData(data22.slice(0, $scope.pagesize)),
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
                                color: '#35BDB2',
                                shadowBlur: 0,
                                shadowColor: '#44AFA6'
                            }
                        },
                        zlevel: 1
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

    //导出表格
    //$scope.tableExcel = function () {
    //    $(".table2excel").table2excel({
    //        exclude: ".noExl",
    //        name: "Excel Document Name",
    //        filename: "myFileName",
    //        exclude_img: true,
    //        exclude_links: true,
    //        exclude_inputs: true
    //    });

    //}
    //展示关键词纪录（按时间分组）
    //$scope.GetKeywordByTime = function () {
    //    var url = "/api/Keyword/GetKeywordByTime?user_id=" + $rootScope.userId;
    //    var q = $http.get(url);
    //    q.success(function (response, status) {
    //        $scope.GetKeywordByTimeList = response;


    //    });
    //    q.error(function (response) {
    //        $scope.error = "服务器连接出错";

    //    });
    //};

    $scope.RefreshGetShopList = function () {
        $scope.pagesize1 = $scope.pagesize1;
        $scope.GetShopList2();
    }

    //加载连接数
    $scope.GetShopList = function (num) {
        $scope.pagesize1 = num;
        $scope.GetShopList2();
    }
    $scope.GetShopList2 = function () {
        //$scope.isActiveTop10 = true;
        $scope.isActiveList = true;
        var url = "/api/Dashboard/GetShopList?user_id=" + $rootScope.userID + "&kid=" + $rootScope.keyId + "&pagesize=" + $scope.pagesize1 + "&projectId=" + $rootScope.getProjectId;
        var p = $http.get(url);
        p.success(
            function (response, status) {
                console.log('eMarketNow_ctr>GetShopList2');
                $scope.GetShopLists = response;
                $scope.isActiveList = false;

            }
        );
        p.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveList = false;
        });
    }


    $scope.details = function (id, name) {
        //$("html,body").animate({ scrollTop: $("#qy_name4").offset().top - 70 }, 1000);//1000是ms,也可以用slow代替 
        $rootScope.keyId = id;
        $rootScope.keyName = name;
        $rootScope.iskeyNull = true;
        $cookieStore.put("iskeyNull", $rootScope.iskeyNull);
        $cookieStore.put("keyIds", $rootScope.keyId);
        $cookieStore.put("keyNames", $rootScope.keyName);
        //chk_global_vars($cookieStore, $rootScope, null, $location, $http);
        $scope.detailsadd();
        $scope.GetShopList($scope.pagesize1);
        $scope.AllAccount();
        $scope.chainMap($scope.pagesize);
        $scope.GetBubbleList($scope.pagesize2);
    }

    $scope.detailsadd = function () {


       
    }
    // $scope.detailsadd();
    //向上家
    //切换添加关键词
    $scope.isActive = true;
    $scope.changeaddkeywords = function () {
        $scope.isActive1 = false;
        $("html,body").animate({scrollTop: $("#qy_name4").offset().bottom - 70}, 1000);//1000是ms,也可以用slow代替
    };


    $scope.offset2 = function () {
        $("html,body").animate({scrollTop: $("#qy_name").offset().top - 70}, 1000);//1000是ms,也可以用slow代替
    };
    //切换搜索日志
    $scope.changerecord = function () {
        $scope.isActive1 = true;
    };

    var dataNew = [];
    //监测效果
    $scope.GetBotResultList = function (id) {
        $scope.keyId2 = id;
        $scope.isActiveAdd = false;
        var url = "/api/Dashboard/GetBotResultList?user_id=" + $rootScope.userID + "&kid=" + $scope.keyId2 + "&projectId=" + $rootScope.getProjectId;
        var p = $http.get(url);
        p.success(
            function (response, status) {
                console.log('eMarketNow_ctr>GetBotResultList');
                $scope.ii = 0;
                dataNew = response;
                $scope.type();
            }
        );
        p.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    $scope.ii = 0;

    $scope.type = function () {

        if ($scope.ii < dataNew.length) {
            $("#txt22").prepend(dataNew[$scope.ii].ItemName + '\n');
            $("#txt33").text('正在监测:' + dataNew[$scope.ii].ItemName);
            $scope.ii++;
            if (dataNew.length == $scope.ii) {
                $scope.imglinks = "Scripts/app/images/jcsuccess.jpg";

                $scope.showStart = false;
                $("#txt22").prepend('加载完成:' + '\n');
            }
            setTimeout(function () {
                $scope.type();
            }, 20);
        }

    }


    $scope.chakan = function () {
        $('#qidong').removeClass('visible active');


    }

    $('#closeModal').click(function () {
        $('.jiance.modals').removeClass('visible active');
    });

    $('.button.guanbi').click(function () {
        $('.jiance.modals').removeClass('visible active');
    });


    //调节高度
    $scope.windowsize = function () {
        $scope.winHeight = 0;
        //获取窗口高度
        var i = document.body.scrollHeight
        if (window.innerHeight) {
            winHeight = window.innerHeight;
        }
        else if ((document.body) && (document.body.clientHeight)) {
            winHeight = document.body.clientHeight;
        }
        //通过深入Document内部对body进行监测，获取窗口大小
        if (document.documentElement && document.documentElement.clientHeight && document.documentElement.clientWidth) {
            winHeight1 = document.documentElement.clientHeight - 60;
            winHeight2 = document.documentElement.clientHeight - 170;
        }
        //设置div的具体宽度=窗口的宽度的%
        if (document.getElementById("KeyMenu")) {
            $scope.size1 = winHeight1 + "px";
            $scope.KeyMenu = {
                "min-height": $scope.size1,
                "position": "relative",
            };
        }
        if (document.getElementById("maSize")) {
            //document.getElementById("maSize").style.min-height = winHeight2 + "px";
            $scope.size2 = winHeight2 + "px";
            $scope.masize1 = {
                "max-height": $scope.size2,
                "position": "relative",
            };
        }
        ;
    }

    $scope.windowsize();

    $scope.$watch(function () {
        return $window.innerWidth;
    }, function (value) {
        $scope.windowsize();
    });

    //删除关键词
    $scope.removeKeyword = function (id) {
        $scope.isActiveStart = true;
        var url = "/api/EMN/removeKeyword?user_id=" + $rootScope.userID + "&keyid=" + id + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>removeKeyword');
            $scope.alert_fun('success','搜索词删除成功！')
            $scope.isActiveStart = false;
            $scope.GetKeyword();

        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;

        });
    };


    $scope.info = function () {
        //刷新加载数据
        if (typeof ($rootScope.keyId) != "undefined") {
            $scope.AllAccount();
            $scope.GetShopList($scope.pagesize1);
            $scope.chainMap($scope.pagesize);
            $scope.GetBubbleList($scope.pagesize2);
        }
    }


    //

    $scope.at = function () {
        $('#qidong').addClass('visible active');
    };
    //$scope.at();


    $scope.LoadCompanies = function () {
        if ($rootScope.UsrRole == 1) {
            var url = "/api/Account/GetUserList" + "&projectId=" + $rootScope.getProjectId;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log('eMarketNow_ctr>LoadCompanies');
                $scope.companies = response;
                if (response.length > 0) {
                    $scope.CID = response[0]._id;
                    $scope.firstLoading();
                    //$scope.info();

                }
            });
            q.error(function (e) {
                alert("获取用户信息失败");
            });
        } else {
            $scope.companies = [{_id: $rootScope.userID, LoginName: $rootScope.LoginName}];
            $scope.CID = $rootScope.userID;
            $scope.firstLoading();
            //$scope.info();

        }

    };


    $scope.usr_id_change = function (id) {

        for (var i = 0; i < $scope.companies.length; i++) {
            var p = $scope.companies[i];
            if (p._id == id) {
                $scope.CID = p._id;
            }
        }
        //
        $scope.GetKeyword();
        $scope.info();
    };
    //}


    $scope.addAlert = function () {
        $scope.alerts.push({msg: 'Another alert!'});
    };

    $scope.closeAlert = function (index) {
        $scope.alerts.splice(index, 1);
    };

    /*显示搜索框*/
    $scope.isActiveLink = false;
    $scope.isActiveLinkfun = function () {
        if ($scope.isActiveLink == false) {
            $scope.isActiveLink = true;
        } else {
            $scope.isActiveLink = false;
        }
    }

    $scope.isActiveJia = false;
    $scope.isActiveJiafun = function () {
        if ($scope.isActiveJia == false) {
            $scope.isActiveJia = true;
        } else {
            $scope.isActiveJia = false;
        }
    }


    $scope.isActiveShou = false;
    $scope.isActiveShouFun = function () {
        if ($scope.isActiveShou == false) {
            $scope.isActiveShou = true;
        } else {
            $scope.isActiveShou = false;
        }
    }

    $scope.isActiveSuo = false;
    $scope.isActiveSuofun = function () {
        if ($scope.isActiveSuo == false) {
            $scope.isActiveSuo = true;
        } else {
            $scope.isActiveSuo = false;
        }
    }

    $scope.isActiveDi = false;
    $scope.isActiveDifun = function () {
        if ($scope.isActiveDi == false) {
            $scope.isActiveDi = true;
        } else {
            $scope.isActiveDi = false;
        }
    }

    $scope.isActiveDD = false;
    $scope.isActiveDDFun = function () {
        if ($scope.isActiveDD == false) {
            $scope.isActiveDD = true;
        } else {
            $scope.isActiveDD = false;
        }
    }

    $scope.isActiveSS = false;
    $scope.isActiveSSFun = function () {
        if ($scope.isActiveSS == false) {
            $scope.isActiveSS = true;
        } else {
            $scope.isActiveSS = false;
        }
    }

    //获取项目列表
    $scope.GetProjects = function () {
        var url = "/api/Keyword/GetProjects?usr_id=" + $rootScope.userID + "&page=" + $scope.page_eMN + "&pagesize=" + $scope.pagesize_eMN + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('eMarketNow_ctr>GetProjects');
            if (response != null) {
                $scope.projectsList = response.Result;
                //console.log($scope.projectsList);
                $scope.projectsListlength = response.Count
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    //16切换项目
    $scope.clickItem = function (id, name) {
        $rootScope.getProjectId = id;
        $cookieStore.put("getProjectId", $rootScope.getProjectId);
        $rootScope.getProjectName = name;
        $cookieStore.put("getProjectName", $rootScope.getProjectName);
        window.location.reload();

    }

    //判断加载
    $rootScope.laoding_1 = function () {
        if ($scope.first == true) {
            $scope.LoadCompanies();

        } else {
            $rootScope.insertKeyword();
            $scope.first = true;
        }
    }


    //自动加载_________________________________________________________________________
    $rootScope.laoding_1();

    $scope.GetProjects();
});



