var Dashboard_ctr = myApp.controller("Dashboard_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {
    $scope.selected_page = '';

    $scope.side_bar_show = true;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);

    //header 方法——————————————————————————
    $scope.showamendpwd = function (num) {
        $scope.showAmendAndOff = num;
    }
    $scope.hideamendpwd = function (num) {
        $scope.showAmendAndOff = false;
    }
    //位置active
    $scope.selected_page_fun = function () {
        $timeout(function () {
            $scope.selected_page_fun_1();

        }, 200);
    }
    $scope.selected_page_fun();
    $scope.selected_page_fun_1 = function () {
        var dizhi = window.location.hash;
        var arr = dizhi.split('/');
        if (arr[1] == "main") {
            $scope.selected_w = 'main';
        }
    }

    //折叠side_bar
    $scope.side_bar_fun = function () {
        $scope.side_bar_show = !$scope.side_bar_show;
    }

    //自动获取当前页面
    //退出-------------------------------------------------------------
    $scope.logoff = function () {
        $rootScope.userId = "";
        $rootScope.LoginName = "";
        $rootScope.LoginPwd = "";
        $rootScope.UsrEmail = "";
        $rootScope.Token = "";
        $rootScope.UsrKey = "";
        $rootScope.UsrRole = "";
        $rootScope.applicationState = "";
        $rootScope.UsrNum = "";
        $rootScope.IsEmailConfirmed = "";
        $rootScope.uer_PictureSrc = "";
        $rootScope.logined_nm = false;

        $cookieStore.remove("userId");
        $cookieStore.remove("LoginName");
        $cookieStore.remove("LoginPwd");
        $cookieStore.remove("logined_nm");
        $cookieStore.remove("UsrEmail");
        $cookieStore.remove("Token");
        $cookieStore.remove("UsrKey");
        $cookieStore.remove("UsrRole");
        $cookieStore.remove("applicationState");
        $cookieStore.remove("UsrNum");
        $cookieStore.remove("uer_PictureSrc");
        $cookieStore.remove("logined_nm");
        $location.path("/home/main_home").replace();
        //location.reload();
    };
    //++++++++++++++++++++++++++++++++++++++++++++++++++++

    // backtop:方法  ++++++++++++++++++++++++
    $scope.phone_on = function () {
        $scope.phone_on_show = true;
    }
    $scope.phone_close = function () {
        $scope.phone_on_show = false;
    }
    $scope.backtop_fun = function () {
        document.documentElement.scrollTop = document.body.scrollTop = 0;

    }

});