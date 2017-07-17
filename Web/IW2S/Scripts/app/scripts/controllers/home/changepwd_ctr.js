var changepwd_ctr = myApp.controller("changepwd_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {

    $scope.error = "";
    $scope.email = "";
    $rootScope.logined = false;
    $scope.uName = "";
    $scope.uPwd1 = "";
    $scope.uPwd2 = "";
    $scope.email = "";
    //$scope.YZM = "";
    $scope.isSeclected = true;//拖动验证
    $scope.isSeclectedAgree = "";
    $scope.changeSucc = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //启动enter 事件
    $scope.EnterPress = function () {
        var e = e || window.event;
        if (e.keyCode == 13) {
            $scope.changepwd();
        }
    }


    //3.注册_________________________________

    $scope.changepwd = function () {
        //var pattern = /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/;
        //if ($scope.uName == "" || $scope.uName.length < 6) {
        //    $scope.error = "用户名长度至少为6位.";
        //    return;
        //}
        if ($scope.uPwd1 == "" || $scope.uPwd1.length < 6) {
            $scope.error = "密码长度至少为6位";
            return;
        }
        if ($scope.uPwd2 == "" || $scope.uPwd2 != $scope.uPwd1) {
            $scope.error = "密码不一致";
            return;
        }
        //if ($scope.email == "") {
        //    $scope.error = "请输入邮箱";
        //    return;

        //}
        //if ($scope.email.match(pattern) == null) {
        //    $scope.error = "邮箱格式不正确";
        //    return;
        //}
        //if ($scope.isSeclected != true) {
        //    $scope.error = "请拖动滑块验证";
        //    return;
        //}
        //if ($scope.isSeclectedAgree != true) {
        //    $scope.error = "请同意服务条款与用户须知";
        //    return;
        //}
        else {
            var url = "/api/Account/ChangePwd?uName=" + $rootScope.LoginName + "&oldPwd=" + $scope.uPwd0 + "&newPwd=" + $scope.uPwd1;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log(response);
                console.log('signup_ctr>Regist');
                if (response.Error != null) {
                    $scope.error = response.Error;
                } else {
                    $scope.changeSucc = true;
                    //$rootScope.isActiveModale = "baidu";
                    //$cookieStore.put("isActiveModale", $rootScope.isActiveModale)
                    //$rootScope.pagesizeBaidu = 10;
                    //chk_global_vars($cookieStore, $rootScope, response, null, $http);
                    //$location.path("/modelSelect").replace();

                }
            });
            q.error(function (response) {
                $scope.error = "网络打盹了，请稍后。。。";
            });
        }
    }

    ////条款
    //$scope.openItemFun = function () {
    //    $scope.openItem = true;
    //}
    //$scope.closeItemFun = function () {
    //    $scope.openItem = false;
    //}


    ////2.自动刷新页面

    //$scope.singup_Reload = function () {
    //    if ($rootScope.reload_LS == false) {
    //        window.location.reload();
    //        $rootScope.reload_LS = true;
    //        $cookieStore.put("reload_LS", $rootScope.reload_LS);
    //    }


    //}


    //$scope.singup_Reload();


});