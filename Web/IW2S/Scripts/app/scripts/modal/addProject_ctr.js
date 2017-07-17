var addProject_ctr = function ($scope, $modalInstance, $rootScope, $http) {


    $scope.projectName = "";
    $scope.error = "";
    $scope.projectNameDescribe = "";

    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.创建项目
    $scope.InsertProject = function () {
        var url = "/api/Keyword/InsertProject?usr_id=" + $rootScope.userID + "&name=" + $scope.projectName + "&description=" + $scope.projectNameDescribe;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('addProject_ctr>InsertProject');
            $scope.massage = response.Message;
            if (response.IsSuccess != true) {
                $scope.error = $scope.massage;

            } else {
                $scope.ok();
            }
        });
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
        });
    };


}