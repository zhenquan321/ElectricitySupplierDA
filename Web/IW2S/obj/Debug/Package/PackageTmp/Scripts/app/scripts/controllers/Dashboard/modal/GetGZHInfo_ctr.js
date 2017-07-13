var GetGZHInfo_ctr = function ($scope, $modalInstance, $rootScope, $http) {
    $scope.keywords = '';
    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.获取文章
    $scope.GetNameInfo = function () {
        var url = "/api/Media/GetNameInfo?nameId=" + $scope.nameId ;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetNameInfoList = response;
            console.log(response)
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
    $scope.GetNameInfo();

    






}