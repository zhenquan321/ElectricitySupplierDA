var addkeywordFilterBing_ctr = function ($scope, $modalInstance, $rootScope, $http) {


    $scope.isActiveFilter = false;
    $scope.GetKeywordFilterWords = [];
    $scope.filterKeywords = "";
    //
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.插入推荐词的搜索过滤词

    $scope.InsertKeywordFilter = function (commendKeywordId) {

        $scope.filterKeywords2 = $scope.filterKeywords;

        if ($scope.filterKeywords == "" || $scope.filterKeywords == null) {
            alert("请输入关键词")
        } else {
            var url = "/api/Bing/InsertKeywordFilter?user_id=" + $rootScope.userID + "&commendKeywordId=" + $rootScope.BaidukeywordId + "&keywords=" + $scope.filterKeywords + "&projectId=" + $rootScope.getProjectId;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log('addkeywordFilterBaidu_ctr>InsertKeywordFilter');
                $scope.GetKeywordFilter();
            });
            q.error(function (response) {
                $scope.error = "服务器连接出错";
            });
        }

    };

    //2.获取推荐词的搜索过滤词

    $scope.GetKeywordFilter = function () {
        var url = "/api/Bing/GetKeywordFilter?user_id=" + $rootScope.userID + "&keywordId=" + $rootScope.BaidukeywordId + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('addkeywordFilterBaidu_ctr>GetKeywordFilter');
            $scope.GetKeywordFilterWords = response;
            //console.log($scope.GetKeywordFilterWords);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    //3.删除推荐词的搜索过滤词

    $scope.DelKeywordFilter = function (id) {

        $scope.filterIds = id;
        var url = "/api/Bing/DelKeywordFilter?filterIds=" + $scope.filterIds + "&user_id=" + $rootScope.userID;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('addkeywordFilterBaidu_ctr>DelKeywordFilter');
            $scope.GetKeywordFilter();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };


    //_________________________________________________________________________________________________________
    $scope.GetKeywordFilter();

}