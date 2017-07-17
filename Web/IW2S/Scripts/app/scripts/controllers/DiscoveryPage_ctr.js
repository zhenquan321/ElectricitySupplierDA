var DiscoveryPage_ctr = myApp.controller("DiscoveryPage_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {
    $scope.num = 0;
    $scope.num2 = 0;
    $rootScope.isActive_change = 0;
    $rootScope.Comment = "";
    $rootScope.pinglun = false;
    $scope.url = "";
    $rootScope.ShareOutContent = "";
    $scope.prjusrname = "";
    $rootScope.alerts = [];
    $scope.page_Discover = 0;
    $scope.pagesize_Discover = 10;
    $scope.active_more = true;
    $rootScope.ShareToDiscover_list_2 = [];
    $scope.SelSTNum = 1;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    $rootScope.userID = $cookieStore.get("userID");

    //页面添加alert
    $rootScope.addAlert = function (status, messages) {
        var len = $rootScope.alerts.length + 1;
        $rootScope.alerts = [];
        $rootScope.alerts.push({ type: status, msg: messages });
    }
    //关闭alert
    $scope.closeAlert = function (index) {
        $rootScope.alerts.splice(index, 1)
    }
    //切换不同分享模块
    $scope.SelSType=function(num){
        $scope.SelSTNum = num;
        if (num == 1) {

        }
    }


    //不同模块
    $scope.share_modal_n = [
        [ 12, "监测结果链接详情" ],
        [13, "监测结果详情" ],
        [ 14,"监测结果详情" ],
        [ 15, "监测结果详情" ],
        [ 51, "关键词共现" ],
        [ 17, "监测结果链接详情" ],
        [11, "词组关系树状图"],
         [59, "矩阵图"],
    ];

    $scope.share_modal_url = [
        [12, "rizhi/baidu_share_1"],
        [13, "rizhi/baidu_share_2"],
        [14, "rizhi/baidu_share_3"],
        [15, "rizhi/baidu_share_4"],
        [51, "rizhi/baidu_share_5"],
        [17, "rizhi/baidu_share_6"],
        [11, "rizhi/baidu_share_7"],
         [59, "rizhi/baidu_share_8"],
    ];

    //获取分享到发现
    $rootScope.GetShareToDiscover = function () {
        $scope.anaItem = {
            prjId:"",
            opereateType: "",
            page: $scope.page_Discover,
            pagesize: $scope.pagesize_Discover,
            siteSource: "",
            prjusrname: $scope.prjusrname,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetShareToDiscover"
        })
            .success(function (response, status) {
                console.log(response);
                $scope.aaa2 = response.Result;
                if ($scope.pagesize_Discover > $scope.aaa2.length) {
                    $scope.active_more = false;
                }
                var QRCode_url = $location.host() + ":" + $location.port() + "/#/";
                QRCode_url = "http://" + QRCode_url;
                
                for (var i = 0; i < $scope.aaa2.length; i++) {
                    for (var z = 0; z < $scope.share_modal_n.length;z++){
                        if ($scope.aaa2[i].ShareOperateType == $scope.share_modal_n[z][0]) {
                            $scope.aaa2[i].title_a = $scope.share_modal_n[z][1];
                            $scope.aaa2[i].modelUrl =QRCode_url+ $scope.share_modal_url[z][1];
                        }
                       }
                    }
                $rootScope.ShareToDiscover_list = $scope.aaa2;

                var cc = 0;
                for (var a = 0; a < $rootScope.ShareToDiscover_list.length;a++){
                    if (a%2==0) {
                        $rootScope.ShareToDiscover_list_2[cc] = $rootScope.ShareToDiscover_list[a];
                        cc++;
                    }
                }
                

                $rootScope.ShareToDiscover_Count = response.Count;
            })
            .error(function (response, status) {
               // $scope.addAlert('danger', "网络打盹了，请稍后。。。");
            });
    }
    //加载更多
    $rootScope.GetShareToDiscover_more = function () {
        $scope.pagesize_Discover = $scope.pagesize_Discover + 10;
        $rootScope.GetShareToDiscover();
    }
    //5.退出清cookie
    $scope.off = function () {
        $location.path("/home/main_1").replace();
        $rootScope.userID = "";
        $rootScope.LoginName = "";
        $rootScope.UsrRole = "";
        $rootScope.UsrKey = "";
        $rootScope.UsrNum = "";
        $rootScope.UsrEmail = "";
        //页面
        $rootScope.keyword = "";
        $rootScope.keywordName = "";
        //changexinyuan
        $rootScope.selectStatus = "";
        $rootScope.ZhibokeywordId = "";
        $rootScope.BaidukeywordId = "";
        $rootScope.keywordsListRecord = "";
        $rootScope.getBaiduRecordId = "";
        $cookieStore.remove("userID");
        $cookieStore.remove("LoginName");
        $cookieStore.remove("UsrRole");
        $cookieStore.remove("UsrKey");
        $cookieStore.remove("UsrNum");
        $cookieStore.remove("UsrEmail");
        $cookieStore.remove("keyword");
        $cookieStore.remove("keywordName");
        $cookieStore.remove("ZhibokeywordId");
        $cookieStore.remove("BaidukeywordId");
        $cookieStore.remove("keywordsListRecord");
        $cookieStore.remove("getBaiduRecordId");
        $cookieStore.remove("selectStatus");
    }
  




    //2016-7-27 分享功能____________________________________________________________________________________________________________________________

    // ShareOperateType 1_______________
    //    AddKeyword = 0,//添加搜索词
    //    DelKeyword = 1,//删除搜索词
    //    FilterConfig = 2,//过滤设置
    //    CollectConfig = 3,//收藏设置
    //    ManageGroup = 4,//创建和管理词组
    //    ManageAnalysisItem = 5,//分析指向管理
    //    SearchResultConfig = 6,//监测结果设置
    //    SetLinkAnalysisItem = 7,//设置连接分析指项
    //    ImportKeywordGroup = 8,//导入分组
    //    ReSearchKeyword =9,//重新搜索关键词
    //    SaveDomainCategory = 10,//保存域名分类
    //                        =11//搜索管理——树状图


    //SiteSource 2_____________
    //    Baidu = 0,//百度
    //    BaiduWeibo = 1,//百度微博
    //    Weichat = 2,//微信
    //    Sogou = 3,//搜狗
    //    BaiduImg = 4,//百度图片
    //    emarketnow = 5,//市场快照

    //获取当前分区
    $rootScope.change_model = function (num, num2, id) {
       
            $scope.num = num;
            $scope.num2 = num2;
            $scope.projectid = id;
            //if ($scope.url) {
            //    $rootScope.GetQRCode();
            //} else {
            //    if ($rootScope.isActive_change == 3 || $rootScope.isActive_change == 5) {
            //        $rootScope.isActive_change = 2;
            //        $rootScope.GetOperateLog();
            //    }
            //}
            //$rootScope.GetShareOutComment();
    }
    //鼠标离去全部不显示

    $scope.allHide = function () {
        $rootScope.isActive_change = 0;


    }



    //刷新
    $rootScope.refresh = function (num, num2) {
        $rootScope.GetOperateLog()
        $scope.addAlert('success', "刷新成功！");
    }

    $scope.ShareOperateType_list = [
    "添加搜索词",
    "删除搜索词",
    "过滤设置",
    "收藏设置",
    "创建和管理词组",
    "分析指向管理",
    "监测结果设置",
    "设置连接分析指项",
    "导入分组",
    "重新搜索关键词",
    "保存域名分类",
    ];

    //切换日志、评论
    $rootScope.changeRzPl = function (a) {
        var aa = a.toString().substring(1, 3);
        $rootScope.isActive_change = a;
        if (a == 1) {
            $rootScope.GetOperateLog()
        } else if (aa == 2) {

        } else if (aa == 3) {
            $rootScope.GetQRCode();
        } else if (aa == 4) {
            $rootScope.GetShareOutComment();
        } else if (aa == 5) {

        } else if (aa == 31) {
            if ($rootScope.isActive_change == 0) {
                $rootScope.isActive_change = 1;
                $rootScope.GetShareOutComment();
            } else {
                $rootScope.isActive_change = 0;
            }
        } else if (aa == 32) {
            if ($rootScope.isActive_change == 0) {
                $rootScope.isActive_change = 2;
            } else {
                $rootScope.isActive_change = 0;
            }
        } else if (aa == 33) {
            if ($rootScope.isActive_change == 0) {
                $rootScope.isActive_change = 4;
                $rootScope.GetShareOutComment();
            } else {
                $rootScope.isActive_change = 0;
            }
        }
    }
    //评论
    $rootScope.huifu = function () {
        $rootScope.pinglun = true;
        $("#textarea_ping").focus();
    }
    $rootScope.huifu_close = function () {
        $rootScope.pinglun = false;
    }
    //获取分享评论
    $rootScope.GetShareOutComment = function () {

        $scope.anaItem = {
            prjId: $scope.projectid,
            opereateType: $scope.num,
            page: 0,
            pagesize: 100,
            siteSource: $scope.num2,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetShareOutComment"
        })
            .success(function (response, status) {
                console.log(response);
                $rootScope.ShareOutComment_list = response.Result;
                $rootScope.ShareOutComment_Count = response.Count;
            })
            .error(function (response, status) {
                $scope.addAlert('danger', "网络打盹了，请稍后。。。");
            });
    }
    //新增分享评论
    $rootScope.InsertShareOutComment = function () {
        $scope.paramsList = {
            UserId: $rootScope.userID,
            ShareOperateType: $scope.num,
            ProjectId: $scope.projectid,
            SiteSource: $scope.num2,
            Comment: $rootScope.ShareOutComment,
        };
        var urls = "/api/Share/InsertShareOutComment";
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
            if (response.IsSuccess == true) {
                $rootScope.GetShareOutComment();
                $rootScope.pinglun = false;
                $rootScope.ShareOutComment = "";
            } else {
                $scope.addAlert('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }
    //删除分享评论
    $rootScope.DelShareOutComment = function (id) {
        if (confirm("您确认删除此评论吗？")) {
            $scope.anaItem = {
                prjId: $scope.projectid,
                usr_id: $rootScope.userID,
                commentId: id,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/DelShareOutComment"
            })
                .success(function (response, status) {
                    $rootScope.addAlert('success', "删除评论成功！");
                    $rootScope.GetShareOutComment($scope.num, $scope.num2);
                })
                .error(function (response, status) {
                    $scope.addAlert('danger', "网络打盹了，请稍后。。。");
                });
        }

    }
    //微信获取二维码
    $rootScope.GetQRCode = function () {
        var QRCode_url = $location.host() + ":" + $location.port() + "/#/";
        QRCode_url = QRCode_url + $scope.url;
        QRCode_url = "http://" + QRCode_url + "?porjectid=" + $scope.projectid
        $scope.anaItem = {
            url: QRCode_url,
            baseurl:  'http://' + window.location.host
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetQRCode"
        })
        .success(function (response, status) {
            $rootScope.erweima = response.Message;
        })
        .error(function (response, status) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });

    }

    //内部分享
    //切换分享
    $rootScope.share_out = true;
    $rootScope.chang_shareout = function (num) {
        if (num == 1) {
            $rootScope.share_out = true;
        } else if (num == 2) {
            $rootScope.share_out = false;
        }
    }
   
    //新增分享到发现
    $rootScope.InsertShareToDiscover = function () {
        $scope.paramsList = {
            UserId: $rootScope.userID,
            ShareOperateType: $scope.num,
            ProjectId: $scope.projectid,
            SiteSource: $scope.num2,
            Content: $rootScope.ShareOutContent,
        };
        var urls = "/api/Share/InsertShareToDiscover";
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
            if (response.IsSuccess == true) {
                $rootScope.GetShareOutComment();
                $scope.addAlert('success', "分享成功！");
                $rootScope.isActive_change = 0;
                $rootScope.ShareOutComment = "";
            } else {
                $scope.addAlert('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    };

    //删除分享到发现
    $rootScope.DelShareToDiscover = function (id,id2,id3) {
        if (confirm("您确认删除此分享吗？")) {
            $scope.anaItem = {
                prjId: id2,
                usr_id: id3,
                commentId: id,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/DelShareToDiscover"
            })
                .success(function (response, status) {
                    $rootScope.GetShareToDiscover();
                    $rootScope.addAlert('success', "删除评论成功！");
                    $rootScope.GetShareOutComment($scope.num, $scope.num2);
                })
                .error(function (response, status) {
                    $scope.addAlert('danger', "网络打盹了，请稍后。。。");
                });
        }

    }



    //自动加载_____________________________________________________________

    $rootScope.GetShareToDiscover();



});