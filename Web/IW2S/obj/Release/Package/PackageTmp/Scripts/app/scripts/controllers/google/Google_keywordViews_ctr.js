var Google_keywordViews_ctr = myApp.controller("Google_keywordViews_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal) {

    $scope.setUpIsactive = true;
    $scope.isActiveCollection2 = true;
    $scope.Title = "";
    $scope.domain = "";
    $scope.infriLawCode = "";
    $scope.page11 = 1;
    $scope.pagesize11 = 20;
    $scope.BaidukeywordId = "";
    $scope.status = "";
    $scope.isActiveLoadmore2 = true;
    $scope.activeId = '';
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    //________________________________________________________________________________

    //所有侵权类型id
    $scope.allIdArr = [];
    for (var i = 0; i < $rootScope.PrjAnalysisItemName_list.length; i++) {
        $scope.allIdArr.push($rootScope.PrjAnalysisItemName_list[i]._id);
    }
    $scope.allIdArr1 = $scope.allIdArr.join(';');
    $scope.currentInfriType = $scope.allIdArr1;

    //1.监测结果设置 切换
    $scope.changesetUp = function () {
        if ($scope.setUpIsactive == true) {
            $scope.setUpIsactive = false;
            $scope.currentInfriType = '000000000000000000000000';
            $scope.GetBaiduLevelLinksView();
        } else if ($scope.setUpIsactive == false) {
            $scope.setUpIsactive = true;
            $scope.currentInfriType = $scope.allIdArr1;
            $scope.BaidukeywordId = '';
            $scope.categoryId = '';
            $scope.GetTreeData();
            $scope.GetBaiduLevelLinksView();
        }
    }

    //词组zTree
    $scope.GetTreeData = function () {
        var url = "/api/Google/GetAllFenZhu?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.zNodes = response;
            //console.log(response);

            if (response != null) {




                //让头部展开
                $scope.zNodes[0].open = true;

                //默认加载所有关键词
                $scope.getkeywords1($scope.zNodes[0].id);


                var setting = {
                    check: {
                        enable: true,
                        chkboxType: { "Y": "s", "N": "ps" }
                    },
                    data: {
                        simpleData: {
                            enable: true
                        }
                    },
                    callback: {
                        onCheck: $scope.getkeywords
                    }

                };


                $.fn.zTree.init($("#treeDemo"), setting, $scope.zNodes);


            }



        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    //获取选中词组下的关键词
    $scope.getkeywords = function () {
        $scope.ChkIdKw = [];
        $scope.ifCheckedKw = false;
        var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
        var nodes = treeObj.getCheckedNodes(true);
        var treeNodeId = [];
        for (var i = 0, len = nodes.length; i < len; i++) {
            treeNodeId.push(nodes[i].id);
        }
        //checkbox勾选的id赋值给$scope
        $scope.treeNodeId = treeNodeId.join(";");
        var url = "/api/Google/GetFenleiKeywordsView?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&categoryId=" + $scope.treeNodeId;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetAllKeywordCategory_list = response;
            console.log($scope.GetAllKeywordCategory_list);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //默认加载
    $scope.getkeywords1 = function (treeNodeId) {
        var url = "/api/Google/GetFenleiKeywordsView?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&categoryId=" + treeNodeId;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetAllKeywordCategory_list = response;
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    //9.筛选过滤条件
    //获取选中的id
    $scope.ChkIdKw = [];
    $scope.pushIdKw = function (id, isChkKw) {
        if (isChkKw) {
            $scope.ChkIdKw.push(id);
            $scope.resultFilterlink();
        } else {
            for (var i = 0; i < $scope.ChkIdKw.length; i++) {
                if ($scope.ChkIdKw[i] == id) {
                    $scope.ChkIdKw.splice(i, 1);
                    break;
                }
            }
            $scope.resultFilterlink();
        }
    }
    $scope.allIdKw = function (ifCheckedKw) {
        if (ifCheckedKw) {
            for (var i = 0; i < $scope.GetAllKeywordCategory_list.length; i++) {
                $scope.ChkIdKw.push($scope.GetAllKeywordCategory_list[i].id);
            }
            $scope.resultFilterlink();
        } else {
            $scope.ChkIdKw = [];
            $scope.resultFilterlink();
        }
    }

    //筛选
    $scope.resultFilterlink = function () {
        $scope.BaidukeywordId = $scope.ChkIdKw.join(';');
        $scope.GetBaiduLevelLinksView();
    }

    //3.清除
    $scope.cancelChange = function () {
        $scope.GetTreeData();
        $scope.ChkIdKw = [];
        $scope.ifCheckedKw = false;
    }


    //2.3.1 百度搜索记录的结果（含侵权类型）

    $scope.GetBaiduLevelLinksView = function () {
        $scope.GetLevelLinksViewList = {
            user_id: $rootScope.userID,
            projectId: $rootScope.getProjectId,
            keywordId:  $scope.BaidukeywordId ,
            Title: $scope.Title,
            domain: $scope.domain,
            infriLawCode:  $scope.infriLawCode,
            status: $scope.status,
            page: ($scope.page11 - 1),
            pagesize:$scope.pagesize2
        }


        var urls = "api/Google/GetLevelLinksView";
        var q = $http.post(
            urls,
            JSON.stringify($scope.GetLevelLinksViewList),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        )
        q.success(function (response, status) {
            console.log(response);
            $scope.activeId = response.infriLawCode;
            $scope.resultList = response.Result;
            //分页
            $scope.CollectionCount = response.Count;
            if (!response.HasValue) {
                $rootScope.addAlert('danger', "没有相关数据！");
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "服务器连接出错");
        });
    }


    //6.收藏和删除 链接
    $scope.SetLinkStatus = function (id, type) {
        $scope.id = id;
        if (type == "delete") {
            if (confirm("确定要删除这条记录吗？")) {
                var url = "/api/Google/SetLinkStatus?id=" + $scope.id + "&status=" + 2 + "&user_id=" + $rootScope.userID;
                var q = $http.get(url);
                q.success(function (response, status) {
                    console.log('iw2s_ctr>SetLinkStatus');
                    $scope.GetBaiduLevelLinksView();
                })
                q.error(function (response) {
                    $scope.error = "服务器连接出错";
                });
            }
        } else if (type == "save") {
            angular.element('.' + id).toggleClass('fa-star');
            angular.element('.' + id).toggleClass('fa-star-o');
            var isStar = angular.element('.' + id).hasClass('fa-star');
            var isStaro = angular.element('.' + id).hasClass('fa-star-o');
            if (isStar != true) {
                var url = "/api/Google/SetLinkStatus?id=" + $scope.id + "&status=" + "&user_id=" + $rootScope.userID;
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


    //显示收藏的链接
    $scope.showbaiduCollectionLinksView = function () {
        if ($scope.isActiveCollection2 == true) {
            $scope.isActiveCollection2 = false;
            $scope.status = 1;
            $scope.infriType = $scope.currentInfriType;
            $scope.GetBaiduLevelLinksView();
        } else if ($scope.isActiveCollection2 == false) {
            $scope.isActiveCollection2 = true;
            $scope.status = "";
            $scope.infriType = $scope.currentInfriType;
            $scope.GetBaiduLevelLinksView();
        }
    }


    //14.0切换侵权

    $scope.ChangeInfriType = function (infriType) {
        $scope.BaidukeywordId = '';
        $scope.categoryId = '';
        if (infriType == 'hasChoose') {

            $scope.currentInfriType = $scope.allIdArr1;
        } else {
            $scope.currentInfriType = infriType;
        }
        $scope.GetTreeData();
        $scope.GetBaiduLevelLinksView();
    };

    //9.设置连接侵权类型
    //获取选中的id
    $scope.ChkId = [];
    $scope.pushId = function (id, isChk) {
        if (isChk) {
            $scope.ChkId.push(id);
        } else {
            for (var i = 0; i < $scope.ChkId.length; i++) {
                if ($scope.ChkId[i] == id) {
                    $scope.ChkId.splice(i, 1);
                    break;
                }
            }
        }
    }
    $scope.allId = function (ifChecked) {
        if (ifChecked) {
            for (var i = 0; i < $scope.resultList.length; i++) {
                $scope.ChkId.push($scope.resultList[i]._id);
            }
        } else {
            $scope.ChkId = [];
        }
    }

    $scope.SetLinkInfriType = function (infriType) {
        if ($scope.ChkId.length == 0) {
            alert('您还未勾选链接详情');
        } else if (!infriType) {
            alert('请选择侵权类型');
        } else {
            $scope.ChkId1 = $scope.ChkId.join(";");
            var url = "/api/Google/SetLinkInfriType?id=" + $scope.ChkId1 + "&infriType=" + infriType + "&user_id=" + $rootScope.userID;
            var q = $http.get(url);
            q.success(function (response, status) {
                if (response.IsSuccess) {
                    $rootScope.addAlert('success', "设置成功！");
                    $scope.GetBaiduLevelLinksView();
                    console.log('keywordViews_ctr>SetLinkInfriType');
                }
            })
            q.error(function (response) {
                $scope.error = "服务器连接出错";
            });
        }
    };


    //导出监测结果
    $scope.ExportLevelLinks = function () {
        $scope.paramsList = {
            user_id: $rootScope.userID,
            projectId: $rootScope.getProjectId,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            //  data:{name:'john',age:27},
            url: "/api/Google/ExportLevelLinks"
        })
            .success(function (response, status) {
                if (response != null) {
                    if (response != "没有要导出的数据") {
                        window.location.href = "Export/DownLoadExcel?path=" + response;
                        $rootScope.addAlert('success', "监测结果导出成功！");
                    } else {
                        $rootScope.addAlert('danger', "没有要导出的监测结果！");
                    }
                }
            })
            .error(function (response, status, headers, config) {
                $scope.addAlert('danger', "导出失败！");
            });
    };
    //_________________________________________________________________________________
    $scope.GetTreeData();
    $scope.GetBaiduLevelLinksView();
});