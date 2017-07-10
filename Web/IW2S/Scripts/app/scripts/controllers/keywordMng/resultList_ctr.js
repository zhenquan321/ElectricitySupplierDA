var resultList_ctr = myApp.controller("resultList_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {
    $scope.page = 1;
    $scope.pagesize = 10;
    $scope.Abstract = "";
    $scope.infriLawCode = "";
    $rootScope.keyword = "";
    $scope.Title = "";
    $scope.domain = "";
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //1.获取详情
    $scope.GetLevelLinks = function () {

        var url = "/api/Keyword/GetLevelLinks?user_id=" + $rootScope.userID + "&keyword=" + $rootScope.keyword + "&Title=" + $scope.Title
            + "&domain=" + $scope.domain + "&infriLawCode=" + $scope.infriLawCode + "&page=" + ($scope.page - 1) + "&pagesize=" + $scope.pagesize;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('resultList_ctr>GetLevelLinks');
            if (response != null) {
                $rootScope.resultList = response.Result;
                //console.log($rootScope.resultList);
                $scope.Count = response.Count;

            }


        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetLevelLinks();
});