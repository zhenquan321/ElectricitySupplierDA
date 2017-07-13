var reset_ctr = myApp.controller("reset_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    $scope.uName = "";
    $scope.uPwd = "";
    $scope.error = "";
    $scope.email = "";

    $rootScope.logined = false;
    //4.找回密码_________________________________
    $scope.email1 = "";
    $scope.FindPwd = function () {


        if ($scope.uName == null || $scope.uName == "") {
            $scope.error = "用户名不能为空";
            return;
        }

        if ($scope.email == null || $scope.email == "") {
            $scope.error = "邮箱不能为空";
            return;
        }
        var url = "/api/Account/FindPwd?email=" + $scope.email + "&loginName=" + $scope.uName;
        var q = $http.get(url);
        q.success(function (response, status) {
            alert(response.Error);
            $location.path("/login").replace();
        })
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
        });
    };

});