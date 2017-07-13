var publicFun_ctr = myApp.controller("publicFun_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {

    $rootScope.selected_page = 'IPRWorx';
    $rootScope.selected_page_2 = "运营管理系统";
    $scope.showAmendAndOff = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);


    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    $scope.alert_warning = false;
    $scope.alert_success = false;
    $scope.alert_danger = false;
    $scope.alert_info = false;
    $scope.alert_warning_n = '';
    $scope.alert_success_n = '';
    $scope.alert_danger_n = "";
    $scope.alert_info_n = "";

    $scope.alert_fun = function (lei, nei) {
        $scope.alert_close_fun();
        if (lei == "warning") {
            $scope.alert_warning = true;
            $scope.alert_warning_n = nei;
        } else if (lei == "danger") {
            $scope.alert_danger = true;
            $scope.alert_danger_n = nei;
        } else if (lei == "success") {
            $scope.alert_success = true;
            $scope.alert_success_n = nei;
        } else if (lei == "info") {
            $scope.alert_info = true;
            $scope.alert_info_n = nei;
        };
        $timeout(function () {
            $scope.alert_close_fun()
        }, 10000);
    };
    $scope.alert_close_fun = function () {
        $scope.alert_warning = false;
        $scope.alert_success = false;
        $scope.alert_danger = false;
        $scope.alert_info = false;
    };
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

 
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    //退出-----------------------------------------------------------------------
    $scope.signout = function () {
        $location.path("home").replace();
        $rootScope.userID = "";
        $rootScope.LoginName = "";
        $rootScope.UsrRole = "";
        $rootScope.NickName = "";
        $rootScope.UserPhone = "";
        $rootScope.UsrEmail = "";
        $rootScope.HeadIcon = "";
        $rootScope.logined = false;
        //------------------------------
        $cookieStore.remove('userID');
        $cookieStore.remove('LoginName');
        $cookieStore.remove('UsrRole');
        $cookieStore.remove('NickName');
        $cookieStore.remove('UserPhone');
        $cookieStore.remove('UsrEmail');
        $cookieStore.remove('HeadIcon');
        $cookieStore.remove('logined');
   
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++

});