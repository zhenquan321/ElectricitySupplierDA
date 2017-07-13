var login_ctr = myApp.controller("login_ctr", function ($scope, $rootScope, $http, $location, $window, $modal, $cookieStore) {


    $scope.phonenum = "";
    $scope.uPwd1 = "";
    $scope.yanzheg = "";
    $scope.p_mas = "";
    $rootScope.logined = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);


    //1.登陆___________________________________


    //启动enter 事件
    $scope.EnterPress = function () {
        var e = e || window.event;
        if (e.keyCode == 13) {
            $scope.login();
        }
    }
    

    $scope.login = function () {
        $scope.error = "";
        if ($scope.phonenum == "") {
            $scope.error = "请输入用户名";
            return
        }
        if ($scope.uPwd1 == "") {
            $scope.error = "请输入密码";
            return;
        }
      
        $scope.callLogin();
    }
    $scope.callLogin = function () {
        var url = "/api/Account/Login?usr_name=" + $scope.phonenum + "&pwd=" + $scope.uPwd1;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response)
            if (response.Error != null) {
                $scope.error = response.Error;
            } else {
                $rootScope.logined = true;
                $cookieStore.put("logined", $rootScope.logined);
                chk_global_vars($cookieStore, $rootScope, response, null, $http);
                if (response.RoleId==1) {
                    $location.path("main/PublishArticles").replace();
                } else {
                    window.location.href = document.referrer;
                }
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
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
        $location.path("/home").replace();
        //location.reload();
    };


    //微信登录
    $scope.wxlogin = function () {
        var q2 = $http.get("/api/Account/WxLogin");

        q2.success(function (response, status) {

            window.location.href = response;
        });
        q2.error(function (response) {
            $scope.logoTrue = false;
            $scope.error = "服务器连接出错";
        });
    };
    $scope.GetQueryString = function (name) {
        var url = window.location.href;
        if (url.indexOf("?") > 0) {
            url = url.substr(url.indexOf("?") + 1);
        }
        var reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)', 'i');
        var r = url.match(reg);
        if (r != null) {
            return unescape(r[2]);
        }
        return null;
    };
    $scope.init = function () {

        var un = $scope.GetQueryString("u");
        if (un) {            
            var url = "/api/User/WXIPRSEELogin?uName=" + un;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log(response)
                if (response.Error != null) {
                    $scope.error = response.Error;
                } else {
                    $rootScope.logined = true;
                    $cookieStore.put("logined_nm", $rootScope.logined);
                    chk_global_vars($cookieStore, $rootScope, response, null, $http);
                    $location.path("/home/main_home").replace();
                }
            });
            q.error(function (response) {
                $scope.error = "服务器连接出错";
            });
        }
    }
    $scope.init();

});