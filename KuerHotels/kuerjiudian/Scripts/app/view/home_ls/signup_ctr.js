var signup_ctr = myApp.controller("signup_ctr", function ($scope, $rootScope, $http, $location, $window, $modal, $cookieStore) {

    $scope.LoginName='';
    $scope.LoginPwd = "";
    $scope.UserEmail = "";
    $scope.VerifyCode2 = "";
    $scope.error = '';
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);

    //启动enter 事件
    $scope.EnterPress = function () {
        var e = e || window.event;
        if (e.keyCode == 13) {
             $scope.singup();
        }
    }
    var pattern = /\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}/;
    $scope.singup = function () {
        //var regMobile = /^0?1[3|4|5|8][0-9]\d{8}$/;
        //if ($scope.phonenum == "") {
        //    $scope.error = "请输入手机号码";
        //    return
        //}
        //var a = regMobile.test($scope.phonenum);
        //if(!a){
        //    $scope.error = "手机号合格不正确";
        //    return;
        //}
        if ($scope.LoginName == "") {
            $scope.error = "请输入登录名";
            return;
        }
        if ($scope.UserEmail == "") {
            $scope.error = "请输入邮箱";
            return;
        }
        if (!pattern.test($scope.UserEmail)) {
            $scope.error = "您输入的邮箱格式不正确";
            return;
        }
        if ($scope.LoginPwd == "" || $scope.LoginPwd.length < 6) {
            $scope.error = "密码长度至少为6位";
            return;
        }
        if ($scope.LoginPwd != $scope.LoginPwd2) {
            $scope.error = "两次输入密码不相同";
            return;
        }
        if (!$scope.VerifyCode2) {
            $scope.error = "请输入验证码";
            return;
        }

        
        $scope.Regsit();
    }

  

    //3.注册_________________________________

    $scope.Regsit = function () {
        $scope.paramsList = {
            LoginName: $scope.LoginName,
            LoginPwd: $scope.LoginPwd,
            UserEmail: $scope.UserEmail,
            YZM: $scope.VerifyCode2,
        };
        var urls = "/api/Account/Regsit";
        var q = $http.post(
                urls,
               JSON.stringify($scope.paramsList),
               {
                   headers: {
                       'Content-Type': 'application/json'
                   }
               }
            )
        q.success(function (response, status) {
            if (response.Error) {
                $scope.error = response.Error;
            } else {
                $rootScope.logined = true;
                $cookieStore.put("logined_nm", $rootScope.logined);
                chk_global_vars($cookieStore, $rootScope, response, null, $http);
                $location.path("/home").replace();
            }
        });
        q.error(function (e) {
            alert("服务器连接出错");
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


});