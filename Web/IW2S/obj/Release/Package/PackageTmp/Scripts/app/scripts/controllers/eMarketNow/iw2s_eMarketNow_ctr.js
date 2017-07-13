var iw2s_eMarketNow_ctr = myApp.controller("iw2s_eMarketNow_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal) {

    $scope.first = true;
    $scope.page4 = 1;
    $scope.pagesize4 = 100;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    //11.2.获取项目列表


    //退出
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


    $scope.GetProjects = function () {
        var roleId;
        if ($rootScope.UsrRole == 0) {
            roleId = $rootScope.userID;
        } else {
            roleId = $rootScope.currentId;
        }
        var url = "/api/Keyword/GetProjects?usr_id=" + roleId + "&page=" + ($scope.page4 - 1) + "&pagesize=" + $scope.pagesize4;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('modelSelect_ctr>GetProjects');
            console.log($rootScope.projectsList);
            if (response != null) {
                $rootScope.projectsList = response.Result;
                $cookieStore.put("projectsList", $rootScope.projectsList);
                $scope.projectsListlength = response.Count
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    //切换项目
    $scope.clickItem = function (id, name) {
        $rootScope.getProjectId = id;
        $cookieStore.put("getProjectId", $rootScope.getProjectId);
        $rootScope.getProjectName = name;
        $cookieStore.put("getProjectName", $rootScope.getProjectName);
        window.location.reload();

    }


    $scope.insertKeywordNew = function () {
        $scope.first = false;
        $rootScope.laoding_1();
    }


    //10.删除直搜纪录/百度推荐关键词
    $scope.ExcludeBaiduKeyword = function (_id) {
        if (confirm("确定要删除这条记录吗？")) {
            $scope.list_submitAllbaidu = [{
                IsRemoved: true,
                _id: _id
            }]

            var urls = "api/weibo/ExcludeBaiduKeyword";
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
                console.log('iw2s_sogou_ctr>ExcludeBaiduKeyword');
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


    //自动加载______________________________________________________________________________
    $scope.GetProjects();
});
