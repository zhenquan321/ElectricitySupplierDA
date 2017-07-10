var login_ctr = myApp.controller("login_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {


    $scope.uName = "";
    $scope.uPwd = "";
    $scope.error = "";
    $scope.email = "";
    $rootScope.reload_LS = false;
    $rootScope.logined = false;
    $scope.isChecked = true;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //1.登陆___________________________________
    $scope.uName=$cookieStore.get('uName');
    $scope.uPwd=$cookieStore.get('uPwd');

    //启动enter 事件
    $scope.EnterPress = function () {
        var e = e || window.event;
        if (e.keyCode == 13) {
            $scope.Login();
        }
    }


    $scope.Login = function () {
        $scope.error = "";
        if ($scope.uName == null || $scope.uName == "") {
            $scope.error = "用户名不能为空";
        } else if ($scope.uPwd == null || $scope.uPwd == "") {
            $scope.error = "密码不能为空";
        } else {
            var ip = returnCitySN["cip"];
            var url = "/api/Account/Login?uName=" + $scope.uName + "&uPwd=" + $scope.uPwd + "&ip="+ip;
            var q = $http.get(url);
            q.success(function (response, status) {
                //记住密码
                if($scope.isChecked){
                    $cookieStore.put("uName", $scope.uName);
                    $cookieStore.put("uPwd", $scope.uPwd);
                }else{
                    $cookieStore.put("uName", '');
                    $cookieStore.put("uPwd", '');
                }

                console.log('login_ctr>Login');
                if (response.Error != null) {
                    $scope.error = response.Error;
                    $rootScope.logined = true;
                    $cookieStore.put("logined", $rootScope.logined);
                    $rootScope.isActiveModale = "baidu";
                    $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
                } else {
                    if (response.UsrRole == 0) {
                        chk_global_vars($cookieStore, $rootScope, response, null, $http);
                        $location.path("/modelSelect").replace();
                      
                    } else {
                        chk_global_vars($cookieStore, $rootScope, response, null, $http);
                        $location.path("/computingResources").replace();
                    }
                }
            });
            q.error(function (response) {
                $scope.error = "服务器连接出错";
            });
        }
        ;

    }
    //记住密码后自动输入密码
    $scope.autoPwd = function () {
        if ($cookieStore.get('uName') == $scope.uName) {
            $scope.uPwd = $cookieStore.get('uPwd');
        }else{
            $scope.uPwd='';
        }
    }

    //2.自动刷新页面

    //$scope.Login_Reload = function () {
    //    if ($rootScope.reload_LS == false) {
    //        window.location.reload();
    //        $rootScope.reload_LS = true;
    //        $cookieStore.put("reload_LS", $rootScope.reload_LS);
    //    }

    //}


    //$scope.Login_Reload();


});