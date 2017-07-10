var sogouNEWShowDesc_ctr = myApp.controller("sogouNEWShowDesc_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {


    //selectStatus
    $rootScope.selectStatus = "百度";
    $scope.isCollection = true;
    $scope.timeStampSelect = [{"Id": 0, "Name": "否"}, {"Id": 1, "Name": "是"}];
    $rootScope.isSuccess = "isNull";
    $scope.searchInput = "";
    $scope.page = 1;
    $scope.pagesize = 10;
    $scope.page2 = 1;
    $scope.pagesize2 = 20;
    $scope.page3 = 0;
    $scope.pagesize3 = 10;
    $scope.Abstract = "";
    $rootScope.keyword = "";
    $scope.Title = "";
    $scope.domain = "";
    $scope.status = "";
    $rootScope.SGresultList = [];
    $rootScope.ZhiboresultList = [];
    $rootScope.alerts = [];
    $scope.pageBaidu = 0;
    $rootScope.pagesizeBaidu = 10;
    $scope.keywordsList1 = [];
    $scope.keywordsListCommend = [];
    $rootScope.getBaiduRecordId = "";
    $scope.isActiveKeyword = false;
    $rootScope.BaidukeywordId = "";
    $rootScope.ZhibokeywordId = "";
    $rootScope.keywordsList = [];
    $scope.isActiveUserStyle = false;
    $rootScope.isActiveLoadmore = true;
    $rootScope.isActiveLoadmoreZhiboLevelLinks = true;
    $scope.BaiduCount = "";
    $scope.id = "";
    $scope.status = "";
    $scope.isActiveCollection = true;
    $scope.isActiveCollection2 = true;
    $rootScope.categoryId = "";
    $scope.page4 = 1;
    $scope.pagesize4 = 11;
    //$rootScope.projectsList = [];
    $scope.projectsListlength = 0;
    $scope.keyID = "";
    $scope.GetAllKeywordCategory = "";
    $scope.Title = "";
    $scope.domain = "";
    $scope.Abstract = "";
    $scope.infriLawCode = "";
    $rootScope.BaidukeywordId = "";


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

    //页面添加alert
    $rootScope.addAlert = function (status, messages) {
        var len = $rootScope.alerts.length + 1;
        $rootScope.alerts = [];
        $rootScope.alerts.push({type: status, msg: messages});
    }
    //关闭alert
    $scope.closeAlert = function (index) {
        $rootScope.alerts.splice(index, 1)
    }

    ////11.4点击项目
    //$rootScope.getProjectId = "";
    //$scope.clickItem = function (id,name) {
    //    $rootScope.getProjectId = id;
    //    $cookieStore.put("getProjectId", $rootScope.getProjectId);
    //    $rootScope.getProjectName = name;
    //    $cookieStore.put("getProjectName", $rootScope.getProjectName);
    //    $scope.GetBaiduKeyword();
    //   // chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    //}

    //1.3添加关键词
    $scope.keywordsSearch = function () {
        if ($scope.searchInput == "" || $scope.searchInput == null) {

            $rootScope.addAlert('danger', '请输入关键词');

        } else {

            var url = "/api/sogou/insertKeyword?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&keywords=" + $scope.searchInput;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log('sogoushowDesc_ctr>keywordsSearch');
                //console.log(response);
                if (response.Message == null) {
                    $scope.GetBaiduKeyword()
                    $scope.addAlert('success', "添加成功！");
                    //$rootScope.addAlert('success', '添加成功');
                } else {
                    $rootScope.addAlert('danger', response.Message);
                }
            });
            q.error(function (response) {
                $scope.error = "服务器连接出错";
            });
        }
    }


    //2.2加载百度搜索记录
    $scope.GetBaiduKeyword = function () {
        if ($rootScope.pagesizeBaidu == undefined) {
            $rootScope.pagesizeBaidu = 10;
        }
        var url = "/api/sogou/GetBaiduKeyword?user_id=" + $rootScope.userID + "&page=" + $scope.pageBaidu + "&projectId=" + $rootScope.getProjectId + "&pagesize=" + $rootScope.pagesizeBaidu;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('sogoushowDesc_ctr>GetBaiduKeyword');
            $rootScope.keywordsListRecord = response;
            //$cookieStore.put("keywordsListRecord", $rootScope.keywordsListRecord);
            if (response != null && response.length > 0) {
                $scope.keywordsListCommend = response[0].BaiduCommends;
                //console.log(response);
                if (response.length < $rootScope.pagesizeBaidu) {
                    $rootScope.isActiveLoadmore = false
                    $cookieStore.put("isActiveLoadmore", $rootScope.isActiveLoadmore);
                }
            }
            else {
                $scope.keywordsListCommend = [];
            }
            $scope.getBaiduRecord($rootScope.getBaiduRecordId);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //加载百度推荐关键词
    $scope.getBaiduRecord = function (id, keyID) {
        $rootScope.getBaiduRecordId = id;
        $cookieStore.put("getBaiduRecordId", $rootScope.getBaiduRecordId);
        for (var i = 0; i < $rootScope.keywordsListRecord.length; i++) {
            if ($rootScope.keywordsListRecord[i]._id == $rootScope.getBaiduRecordId) {
                $scope.keywordsListCommend = $rootScope.keywordsListRecord[i].BaiduCommends;
            }
        }
        $scope.keyID = keyID;
        $scope.GetBaiduLevelLinks2($scope.keyID);
    }

    //2.2.1加载更多
    $scope.GetMoreBaiduKeyword = function () {

        $rootScope.pagesizeBaidu = $rootScope.pagesizeBaidu + 10;
        $cookieStore.put("pagesizeBaidu", $rootScope.pagesizeBaidu);
        $scope.GetBaiduKeyword($rootScope.getProjectId);
    }

    //2.3搜索记录的结果
    //2.3.1 百度搜索记录的结果

    $scope.GetBaiduLevelLinks = function () {
        var url = "/api/sogou/GetLevelLinks?user_id=" + $rootScope.userID + "&categoryId=" + $rootScope.categoryId + "&projectId=" + $rootScope.getProjectId + "&keywordId=" + $rootScope.BaidukeywordId + "&Title=" + $scope.Title +
            "&domain=" + $scope.domain + "&infriLawCode=" + $scope.infriLawCode + "&page=" + ($scope.page2 - 1) + "&pagesize=" + $scope.pagesize2 + "&status=" + $scope.status;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('sogoushowDesc_ctr>GetBaiduLevelLinks');
            $scope.BaiduCount = response.Count;

            if (response != null) {
                $rootScope.SGresultList = response.Result;
                console.log($rootScope.SGresultList)
                $scope.Count = response.Count;
            }
        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    //2.3.1.1分页
    $scope.GetBaiduLevelLinks2 = function (id) {
        if (id != "" && id != null) {
            $rootScope.BaidukeywordId = id;
            $cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
            $scope.GetBaiduLevelLinks()
        } else {
            $scope.GetBaiduLevelLinks()
        }
    }

    //3刷新数据___________
    //3.1
    $scope.RefreshGetBaiduKeyword = function () {
        $scope.addAlert('success', '搜索记录刷新成功！');
        $scope.GetBaiduKeyword()
    }
    //3.2
    $scope.RefreshgetBaiduRecord = function () {
        $scope.addAlert('success', '百度推荐刷新成功！');
        $scope.getBaiduRecord($rootScope.getBaiduRecordId)
    }


    //4.下拉按钮

    $scope.openUser = function () {
        if ($scope.isActiveUserStyle == true) {
            $scope.isActiveUserStyle = false;
            $scope.openUserStyle = {
                "display": "block",
            }
        } else if ($scope.isActiveUserStyle == false) {
            $scope.isActiveUserStyle = true;
            $scope.openUserStyle = {
                "display": "none",
            }
        }
    }
    $scope.openUser();

    //6.收藏
    $scope.SetLinkStatus = function (id, type) {
        $scope.id = id;
        if (type == "delete") {
            if (confirm("确定要删除这条记录吗？")) {
                var url = "/api/sogou/SetLinkStatus?id=" + $scope.id + "&status=" + 2;
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('sogoushowDesc_ctr>SetLinkStatus');
                    $scope.GetBaiduLevelLinks2();
                })
                q.error(function (response) {
                    $scope.error = "服务器连接出错";
                });
            }
        } else if (type == "save") {
            angular.element('#' + id).toggleClass('fa-star-o');
            angular.element('#' + id).toggleClass('fa-star');
            var isStar = angular.element('#' + id).hasClass('fa-star');
            var isStaro = angular.element('#' + id).hasClass('fa-star-o');
            if (isStar != true) {
                var url = "/api/sogou/SetLinkStatus?id=" + $scope.id + "&status=";
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('sogoushowDesc_ctr>SetLinkStatus');
                    //console.log(response);
                })
                q.error(function (response) {
                    $scope.error = "服务器连接出错";
                });
            } else {

                var url = "/api/sogou/SetLinkStatus?id=" + $scope.id + "&status=" + 1;
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('sogoushowDesc_ctr>SetLinkStatus');
                    //console.log(response);
                })
                q.error(function (response) {
                    $scope.error = "服务器连接出错";
                });
            }
        }
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
    // $scope.baiduKeywordMng=function(){
    // 	if(){
    // 		
    // 	}
    // }
    //8.百度搜藏链接
    $scope.showbaiduCollectionLinks = function () {
        if ($scope.isActiveCollection2 == true) {
            $scope.isActiveCollection2 = false;
            $scope.status = 1;
            $scope.GetBaiduLevelLinks2();
        } else if ($scope.isActiveCollection2 == false) {
            $scope.isActiveCollection2 = true;
            $scope.status = "";
            $scope.GetBaiduLevelLinks2();
        }
    }
    //9.删除直搜纪录/百度推荐关键词
    $scope.ExcludeKeyword = function (_id) {
        if (confirm("确定要删除这条记录吗？")) {
            $scope.list_submitAll = [{
                IsRemoved: true,
                _id: _id
            }]

            var urls = "api/sogou/ExcludeKeyword";
            var q = $http.post(
                urls,
                JSON.stringify($scope.list_submitAll),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            )
            q.success(function (response, status) {
                console.log('sogoushowDesc_ctr>ExcludeKeyword');
                $scope.addAlert('success', "删除成功");
                $rootScope.BaidukeywordId = "";
                $cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
                $scope.GetBaiduKeyword();

                if ($rootScope.keywordsListRecord.length != 0) {
                    $scope.GetBaiduLevelLinks();
                } else {
                    $scope.keywordsListCommend = [];
                }
            });
            q.error(function (e) {
                $scope.addAlert('danger', "服务器连接出错");
            });
        }
    }

    //10.删除直搜纪录/百度推荐关键词
    $scope.ExcludeBaiduKeyword = function (_id) {
        if (confirm("确定要删除这条记录吗？")) {
            $scope.list_submitAllbaidu = [{
                IsRemoved: true,
                _id: _id
            }]

            var urls = "api/sogou/ExcludeBaiduKeyword";
            var q = $http.post(
                urls,
                JSON.stringify($scope.list_submitAllbaidu),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            )
            q.success(function (response, status) {
                console.log('sogoushowDesc_ctr>ExcludeBaiduKeyword');
                $scope.addAlert('success', "删除成功！");
                $rootScope.BaidukeywordId = "";
                $cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);

                $scope.GetBaiduKeyword();
                $scope.GetBaiduLevelLinks2();
            });
            q.error(function (e) {
                $scope.addAlert('danger', "服务器连接出错");
            });
        }


    }

    //12.百度排除关键词弹框
    $scope.addkeywordFilterSogou = function () {
        var Cw_scope = $rootScope.$new();

        Cw_scope.BaidukeywordId = $rootScope.BaidukeywordId


        var kw_scope = $rootScope.$new();
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/addkeywordFilterSogou.html',
            controller: addkeywordFilterSogou_ctr,
            scope: kw_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'lg'
        });
        frm.result.then(function (response, status) {
            $scope.GetBaiduLevelLinks2($rootScope.BaidukeywordId);
        });
    };


    //13.获取直搜第一行
    $scope.GetAllKeywordCategory = function () {
        var url = "/api/sogou/GetAllKeywordCategory?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('sogoushowDesc_ctr>GetAllKeywordCategory');
            $scope.GetAllKeywordCategory1 = response;
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    //13.2关键词词组点击事件
    $scope.keywordMngClick = function (id) {
        $rootScope.keywordMngClicked = id;
        $cookieStore.put("keywordMngClicked", $rootScope.keywordMngClicked);
        $rootScope.categoryId = $rootScope.keywordMngClicked;
        $rootScope.BaidukeywordId = "";
        $scope.GetBaiduLevelLinks();
    }


    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //________________________________________________________________________________

    //14.0切换侵权
    $scope.ChangeInfriType = function (infriType) {
        $scope.infriLawCode = infriType;
        $scope.GetBaiduLevelLinks();
    };

    //14.下拉框
    $scope.myInfriTypes = "";
    $scope.InfriTypes = [{Key: 1, Value: "擅自使用知名商品特有的名称、包装、装潢行为"}, {Key: 2, Value: "商业贿赂行为"},
        {Key: 3, Value: "虚假宣传行为"}, {Key: 4, Value: "侵犯商业秘密行为"},
        {Key: 5, Value: "不正当有奖销售行为"}, {Key: 6, Value: "公用企业或独占经营者强制交易行为"},
        {Key: 7, Value: "滥用行政权力限制竞争行为"}, {Key: 8, Value: "串通招投标行为"}
    ];


    //15.设置连接链接标签
    $scope.SetLinkInfriType = function (id, infriType) {
        $scope.infriType = infriType
        $scope.link1ID = id;
        var url = "/api/sogou/SetLinkInfriType?id=" + $scope.link1ID + "&infriType=" + $scope.infriType + "&usr_id=" + $rootScope.userID;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('sogoushowDesc_ctr>SetLinkInfriType');
        })
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
    //自动加载______________________________________________________________________________
    $scope.GetBaiduLevelLinks2($rootScope.BaidukeywordId);
    $scope.GetBaiduKeyword();
    $scope.GetAllKeywordCategory();

});