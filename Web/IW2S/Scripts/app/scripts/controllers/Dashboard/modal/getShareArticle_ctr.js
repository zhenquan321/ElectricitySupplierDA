var getShareArticle_ctr = function ($scope, $modalInstance, $rootScope, $http) {
    $scope.keywords = '';
    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.获取文章
    $scope.GetLinkInfoByName = function () {
        var url = "/api/Media/GetLinkInfoByName?nameId=" + $scope.nameId + '&projectId=' + $scope.projectId + '&categoryIds=' + $scope.categoryIds + '&page=' + ($scope.page - 1) + '&pagesize=' + $scope.pagesize;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.SAlist = response.Result;
            $scope.SAlistCount = response.Count;
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
    $scope.GetLinkInfoByName();

    






}