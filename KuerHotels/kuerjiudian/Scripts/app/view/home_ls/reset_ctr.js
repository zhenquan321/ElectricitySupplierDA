var reset_ctr = myApp.controller("reset_ctr", function ($scope, $rootScope, $http, $location, $window, $modal, $cookieStore) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    $scope.uName = "";
    $scope.uPwd = "";
    $scope.error = "";
    $scope.email = "";



    $rootScope.logined = false;
    //4.找回密码_________________________________
    $scope.email1 = "";
    $scope.FindPwd = function () {
        var url = "/api/Account/FindPwd?email=" + $scope.email;
        var q = $http.get(url);
        q.success(function (response, status) {
            alert(response.Error);
            $location.path("/login").replace();
        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };


    //登出
    $scope.logoff = function () {
        $rootScope.userID = "";
        $rootScope.LoginName = "";
        $rootScope.LoginPwd = "";
        $rootScope.UsrEmail = "";
        $rootScope.Token = "";
        $rootScope.UsrKey = "";
        $rootScope.UsrRole = "";
        $rootScope.applicationState = "";
        $rootScope.UsrNum = "";
        $rootScope.IsEmailConfirmed = "";
        $rootScope.logined = false;

        $cookieStore.remove("userID");
        $cookieStore.remove("LoginName");
        $cookieStore.remove("LoginPwd");
        $cookieStore.remove("logined_nm");
        $cookieStore.remove("UsrEmail");
        $cookieStore.remove("Token");
        $cookieStore.remove("UsrKey");
        $cookieStore.remove("UsrRole");
        $cookieStore.remove("applicationState");
        $cookieStore.remove("UsrNum");
        $cookieStore.remove("IsEmailConfirmed");
        $cookieStore.remove("logined_nm");
        $location.path("/home/main_home").replace();
        //location.reload();
    };

});