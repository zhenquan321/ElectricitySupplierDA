var WC_linkCollection_ctr = myApp.controller("WC_linkCollection_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {

    $scope.Title = "";
    $scope.domain = "";
    $scope.Abstract = "";
    $scope.infriLawCode = "";
    $scope.page11 = 1;
    $scope.pagesize11 = 20;
    $rootScope.BaidukeywordId = "";
    $scope.GetBaiducCollectionLevelLinks = [];


    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //________________________________________________________________________________


    //1.加载收藏链接
    $scope.GetBaiducCollectionLevelLinks1 = function () {
        $scope.status = 1;
        $rootScope.BaidukeywordId = "";
        var url = "/api/Media/GetLevelLinks?user_id=" + $rootScope.userID + "&categoryId=" + $rootScope.categoryId + "&projectId=" + $rootScope.getProjectId + "&keywordId=" + $rootScope.BaidukeywordId + "&Title=" + $scope.Title +
            "&domain=" + $scope.domain + "&infriLawCode=" + $scope.infriLawCode + "&page=" + ($scope.page11 - 1) + "&pagesize=" + $scope.pagesize11 + "&status=" + $scope.status;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('linkCollection_ctr>GetBaiducCollectionLevelLinks1');
            if (response != null) {
                $scope.GetBaiducCollectionLevelLinks = response.Result;
                $scope.CollectionCount = response.Count;
                //console.log($scope.GetBaiducCollectionLevelLinks);
            }
        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    $scope.GetBaiducCollectionLevelLinks2 = function (id) {

    };

    $scope.ChangeInfriType = function (infriType) {
        $scope.infriLawCode = infriType;
        $scope.GetBaiducCollectionLevelLinks1();


    };

    //2.下拉框
    $scope.myInfriTypes = "";
    $scope.InfriTypes = [{ Key: 1, Value: "擅自使用知名商品特有的名称、包装、装潢行为" }, { Key: 2, Value: "商业贿赂行为" },
        { Key: 3, Value: "虚假宣传行为" }, { Key: 4, Value: "侵犯商业秘密行为" },
        { Key: 5, Value: "不正当有奖销售行为" }, { Key: 6, Value: "公用企业或独占经营者强制交易行为" },
        { Key: 7, Value: "滥用行政权力限制竞争行为" }, { Key: 8, Value: "串通招投标行为" }
    ];


    //3.设置连接链接标签
    $scope.SetLinkInfriType = function (id, infriType) {
        $scope.infriType = infriType
        $scope.link1ID = id;
        var url = "/api/Media/SetLinkInfriType?id=" + $scope.link1ID + "&infriType=" + $scope.infriType + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('linkCollection_ctr>SetLinkInfriType');
        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };


    //$scope.listGroupOnclick = function () {
    //    var listGroup = document.getElementsByClassName('list-group');
    //    listGroup.onclick = function (event) {
    //        $('.list-group').gt(1).css('list-group-item');
    //        var src = event.target || srcElement;
    //        src.className = "list-group-item heder12active";
    //    }
    //}

    //_________________________________________________________________________________
    $scope.GetBaiducCollectionLevelLinks1();
    $scope.GetBaiduKeyword();
});