var getKWSArticle_ctr = function ($scope, $modalInstance, $rootScope, $http) {
    $scope.keywords = '';
    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.获取文章
    $scope.GetKeyInfoByName = function () {
        var url = "/api/Media/GetKeyInfoByName?nameId=" + $scope.nameId + '&projectId=' + $scope.projectId + '&categoryIds=' + $scope.categoryIds + '&page=' + ($scope.page - 1) + '&pagesize=' + $scope.pagesize;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.KAlist = response.Result;
            $scope.KAlistCount = response.Count;
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
    $scope.GetKeyInfoByName();










}