var Dnl_Google_ctr = myApp.controller("Dnl_Google_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {


    //selectStatus
    $rootScope.selectStatus = "谷歌";
    $scope.isCollection = true;
    $scope.timeStampSelect = [{ "Id": 0, "Name": "否" }, { "Id": 1, "Name": "是" }];
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
    $rootScope.resultList = [];
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
    $scope.GetAllKeywordCategory_list = "";
    $scope.Title = "";
    $scope.domain = "";
    $scope.Abstract = "";
    $scope.infriLawCode = "";
    $rootScope.BaidukeywordId = "";
    $rootScope.PrjAnalysisItemName_list = "";
    $scope.myInfriTypes = "";
    $scope.InfriTypes = [];
    $scope.allIdArr = [];
    $rootScope.isActive_change = 0;
    $rootScope.Comment = "";
    $rootScope.pinglun = false;
    $scope.url = "";
    $rootScope.ShareOutContent = "";

    //$rootScope.isActiveModale = "baidu";
    //$cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //改变信源
    //0.1微信
    $scope.changeModel_baidu = function () {
        $rootScope.isActiveModale = "baidu";
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


    $scope.changeModel_bing = function () {
        $rootScope.isActiveModale = "bing";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }


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

            var url = "/api/Google/insertKeyword?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&keywords=" + $scope.searchInput;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log('iw2s_ctr>keywordsSearch');
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
        var url = "/api/Google/GetBaiduKeyword?user_id=" + $rootScope.userID + "&page=" + $scope.pageBaidu + "&projectId=" + $rootScope.getProjectId + "&pagesize=" + $rootScope.pagesizeBaidu;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_ctr>GetBaiduKeyword');
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
        var url = "/api/Google/GetLevelLinks?user_id=" + $rootScope.userID + "&categoryId=" + $rootScope.categoryId + "&projectId=" + $rootScope.getProjectId + "&keywordId=" + $rootScope.BaidukeywordId + "&Title=" + $scope.Title +
            "&domain=" + $scope.domain + "&infriLawCode=" + $scope.infriLawCode + "&page=" + ($scope.page2 - 1) + "&pagesize=" + $scope.pagesize2 + "&status=" + $scope.status;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_ctr>GetBaiduLevelLinks');
            $scope.BaiduCount = response.Count;

            if (response != null) {
                $rootScope.resultList = response.Result;
                //console.log($rootScope.resultList)
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
        }
        $scope.GetBaiduLevelLinks()
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
                var url = "/api/Google/SetLinkStatus?id=" + $scope.id + "&status=" + 2 + "&user_id=" + $rootScope.userID;
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('iw2s_ctr>SetLinkStatus');
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
                var url = "/api/Google/SetLinkStatus?id=" + $scope.id + "&user_id=" + $rootScope.userID + "&status=";
                var q = $http.get(url);
                q.success(function (response, status) {
                    //console.log(response);
                    console.log('iw2s_ctr>SetLinkStatus');
                })
                q.error(function (response) {
                    $scope.error = "服务器连接出错";
                });
            } else {

                var url = "/api/Google/SetLinkStatus?id=" + $scope.id + "&status=" + 1 + "&user_id=" + $rootScope.userID;
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('iw2s_ctr>SetLinkStatus');
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

            var urls = "api/Google/ExcludeKeyword";
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
                console.log('iw2s_ctr>ExcludeKeyword');
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

            var urls = "api/Google/ExcludeBaiduKeyword";
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
                console.log('iw2s_ctr>ExcludeBaiduKeyword');
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
    $scope.addkeywordFilterBaidu = function () {
        var Cw_scope = $rootScope.$new();

        Cw_scope.BaidukeywordId = $rootScope.BaidukeywordId


        var kw_scope = $rootScope.$new();
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/addkeywordFilterBing.html',
            controller: addkeywordFilterBing_ctr,
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
        var url = "/api/Google/GetAllKeywordCategory?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_ctr>GetAllKeywordCategory');
            $scope.GetAllKeywordCategory_list = response;
            console.log(response);
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


    //14.下拉框

    $scope.changePrjAsIt_list = function () {
        $scope.InfriTypes = [];
        console.log($rootScope.PrjAnalysisItemName_list);
        for (var i = 0; i < $rootScope.PrjAnalysisItemName_list.length; i++) {
            $scope.InfriTypes.push({ Key: "", Value: "" });
            $scope.InfriTypes[i].Key = $rootScope.PrjAnalysisItemName_list[i]._id;
            $scope.InfriTypes[i].Value = $rootScope.PrjAnalysisItemName_list[i].Name;
        }
    };


    //15.设置连接链接标签
    $scope.SetLinkInfriType = function (id, infriType) {
        $scope.infriType = infriType
        $scope.link1ID = id;
        var url = "/api/Google/SetLinkInfriType?id=" + $scope.link1ID + "&infriType=" + $scope.infriType + "&user_id=" + $rootScope.userID;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_ctr>SetLinkInfriType');
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

    //11.2.获取项目列表

    //11.2.获取项目列表
    $scope.GetProjects = function () {
        var roleId;
        if ($rootScope.UsrRole == 0) {
            roleId = $rootScope.userID;
        } else {
            roleId = $rootScope.currentId;
        }
        var url = "/api/Google/GetProjects?usr_id=" + roleId + "&page=" + ($scope.page4 - 1) + "&pagesize=" + $scope.pagesize4;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('modelSelect_ctr>GetProjects');
            console.log($rootScope.projectsList);
            if (response != null) {
                $rootScope.projectsList = response.Result;
                //$cookieStore.put("projectsList", $rootScope.projectsList);
                myApplocalStorage.setObject('projectsList', $rootScope.projectsList);
                $scope.projectsListlength = response.Count
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    //获取设置项目分析指项
    $scope.GetPrjAnalysisItem2 = function () {
        $scope.anaItem = {
            usr_id: $rootScope.userID,
            prjId: $rootScope.getProjectId,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Google/GetPrjAnalysisItem"
        })
            .success(function (response, status) {
                console.log(response);
                if (response.Name == "" || response.Name == null) {
                    $scope.isActiveAnalysis_selected = false;
                } else {
                    $scope.isActiveAnalysis_selected = true;
                    $scope.GetPrjAnalysisItem_list = response;
                    $scope.GetPrjAnalysisItemName_list = response.ItemValues;
                    $rootScope.GetPrjAnalysisItemName_list_name = response.Name
                    $cookieStore.put("GetPrjAnalysisItemName_list_name", $rootScope.GetPrjAnalysisItemName_list_name);
                    $rootScope.PrjAnalysisItemName_list = response.ItemValues;
                    $cookieStore.put("PrjAnalysisItemName_list", $rootScope.PrjAnalysisItemName_list);
                    $scope.changePrjAsIt_list();
                }
            })
            .error(function (response, status) {
                $scope.addAlert('danger', "服务器连接出错");
            });
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
    $rootScope.change_model = function (num, num2, url) {
        if ($scope.num != num) {
            $scope.num = num;
            $scope.num2 = num2;
            $scope.url = url;
            if ($scope.url) {
                $rootScope.GetQRCode();
            } else {
                if ($rootScope.isActive_change == 3 || $rootScope.isActive_change == 5) {
                    $rootScope.isActive_change = 2;
                    $rootScope.GetOperateLog();
                }
            }
            $rootScope.GetOperateLog()
            $rootScope.GetOperateComment();
        }

    }
    //刷新
    $rootScope.refresh = function (num, num2) {
        $rootScope.GetOperateLog()
        $rootScope.GetOperateComment();
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

        if (a == 1) {
            $rootScope.isActive_change = 1;
            $rootScope.GetOperateLog()
        } else if (a == 2) {
            $rootScope.isActive_change = 2;
            $rootScope.GetOperateComment();
        } else if (a == 3) {
            $rootScope.isActive_change = 3;
            $rootScope.GetQRCode();
        } else if (a == 4) {
            $rootScope.isActive_change = 4;
            $rootScope.GetShareOutComment();
        } else if (a == 5) {
            $rootScope.isActive_change = 5;

        } else if (a == 31) {
            if ($rootScope.isActive_change == 0) {
                $rootScope.isActive_change = 1;
                $rootScope.GetOperateLog()
            } else {
                $rootScope.isActive_change = 0;
            }
        } else if (a == 32) {
            if ($rootScope.isActive_change == 0) {
                $rootScope.isActive_change = 2;
                $rootScope.GetOperateComment();
            } else {
                $rootScope.isActive_change = 0;
            }
        } else if (a == 33) {
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
    //获取操作日志评论
    $rootScope.GetOperateComment = function () {

        $scope.anaItem = {
            prjId: $rootScope.getProjectId,
            opereateType: $scope.num,
            page: 0,
            pagesize: 100,
            siteSource: $scope.num2,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetOperateComment"
        })
            .success(function (response, status) {
                console.log(response);
                $rootScope.OperateComment_list = response.Result;
                $rootScope.OperateComment_Count = response.Count;
            })
            .error(function (response, status) {
                $scope.addAlert('danger', "服务器连接出错");
            });
    }
    //获取操作日志
    $rootScope.GetOperateLog = function () {
        $scope.anaItem = {
            prjId: $rootScope.getProjectId,
            opereateType: $scope.num,
            page: 0,
            pagesize: 100,
            siteSource: $scope.num2,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetOperateLog"
        })
            .success(function (response, status) {
                console.log(response);
                $rootScope.OperateLog_list = response.Result;
                $rootScope.OperateLog_Count = response.Count;
                for (var i = 0; i < $rootScope.OperateLog_list.length; i++) {
                    $rootScope.OperateLog_list[i].ShareOperateType = $scope.ShareOperateType_list[$rootScope.OperateLog_list[i].ShareOperateType];
                }
            })
            .error(function (response, status) {
                $scope.addAlert('danger', "服务器连接出错");
            });
    }
    //新增评论
    $rootScope.InsertComment = function () {
        $scope.paramsList = {
            UserId: $rootScope.userID,
            ShareOperateType: $scope.num,
            ProjectId: $rootScope.getProjectId,
            SiteSource: $scope.num2,
            Comment: $rootScope.Comment,
        };
        var urls = "/api/Share/InsertComment";
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
                $rootScope.GetOperateComment();
                $rootScope.pinglun = false;
                $rootScope.Comment = "";
            } else {
                $scope.addAlert('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "服务器连接出错");
        });
    }
    //删除评论
    $rootScope.DelComment = function (id) {
        if (confirm("您确认删除此留言吗？")) {
            $scope.anaItem = {
                prjId: $rootScope.getProjectId,
                usr_id: $rootScope.userID,
                commentId: id,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/DelComment"
            })
                .success(function (response, status) {
                    $rootScope.addAlert('success', "评论删除成功！");
                    $rootScope.GetOperateComment($scope.num, $scope.num2);
                })
                .error(function (response, status) {
                    $scope.addAlert('danger', "服务器连接出错");
                });
        }

    }
    //获取分享评论
    $rootScope.GetShareOutComment = function () {

        $scope.anaItem = {
            prjId: $rootScope.getProjectId,
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
                $scope.addAlert('danger', "服务器连接出错");
            });
    }
    //新增分享评论
    $rootScope.InsertShareOutComment = function () {
        $scope.paramsList = {
            UserId: $rootScope.userID,
            ShareOperateType: $scope.num,
            ProjectId: $rootScope.getProjectId,
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
            $scope.addAlert('danger', "服务器连接出错");
        });
    }
    //删除分享评论
    $rootScope.DelShareOutComment = function (id) {
        if (confirm("您确认删除此评论吗？")) {
            $scope.anaItem = {
                prjId: $rootScope.getProjectId,
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
                    $scope.addAlert('danger', "服务器连接出错");
                });
        }

    }
    //微信获取二维码
    $rootScope.GetQRCode = function () {
        var QRCode_url = $location.host() + ":" + $location.port() + "/#/";
        QRCode_url = QRCode_url + $scope.url;
        QRCode_url = "http://" + QRCode_url + "?porjectid=" + $rootScope.getProjectId
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
            $scope.addAlert('danger', "服务器连接出错");
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

    //获取分享到发现
    $rootScope.GetShareToDiscover = function () {
        $scope.anaItem = {
            prjId: $rootScope.getProjectId,
            opereateType: $scope.num,
            page: 0,
            pagesize: 100,
            siteSource: $scope.num2,
            prjusrname: "",
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetShareToDiscover"
        })
            .success(function (response, status) {
                console.log(response);
                $rootScope.ShareOutComment_list = response.Result;
                $rootScope.ShareOutComment_Count = response.Count;
            })
            .error(function (response, status) {
                $scope.addAlert('danger', "服务器连接出错");
            });
    }
    //新增分享到发现
    $rootScope.InsertShareToDiscover = function () {
        $scope.paramsList = {
            UserId: $rootScope.userID,
            ShareOperateType: $scope.num,
            ProjectId: $rootScope.getProjectId,
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
            $scope.addAlert('danger', "服务器连接出错");
        });
    };

    //删除分享到发现
    $rootScope.DelShareToDiscover = function (id) {
        if (confirm("您确认删除此分享吗？")) {
            $scope.anaItem = {
                prjId: $rootScope.getProjectId,
                usr_id: $rootScope.userID,
                commentId: id,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/DelShareToDiscover"
            })
                .success(function (response, status) {
                    $rootScope.addAlert('success', "删除评论成功！");
                    $rootScope.GetShareOutComment($scope.num, $scope.num2);
                })
                .error(function (response, status) {
                    $scope.addAlert('danger', "服务器连接出错");
                });
        }

    }

    //自动加载______________________________________________________________________________

    $scope.GetPrjAnalysisItem2();
    $scope.GetProjects();
    //$scope.GetBaiduKeyword();
    $scope.GetAllKeywordCategory();


});