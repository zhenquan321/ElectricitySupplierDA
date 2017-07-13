var Home_New_ctr = myApp.controller("Home_New_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal,$timeout) {
    $scope.head_active = false;
    $scope.scooled_top = true;
    $scope.hover_li_t = 1;
    $scope.ptSever_show = 0;
    $scope.erwema = 1;
    //header onmouse
    $scope.head_hover = function (num) {
        if (num==1) {
            $scope.head_active = true;
        } else {
            $scope.head_active = false;
        }
    }
    //监测鼠标高度
    window.onscroll = function () {
        var t = document.documentElement.scrollTop || document.body.scrollTop;
        if (window.innerWidth) {
            winWidth = window.innerWidth;
        }
        else if ((document.body) && (document.body.clientWidth)) {
            winWidth = document.body.clientWidth;
        }
        var height = winWidth * 0.37 - 150;
        if (t >= height) {
            $scope.scooled_top = false;
            $scope.$apply();
        } else {
            $scope.scooled_top = true;
            $scope.$apply();
        }
    }

    //header_card onmouseover fun
    $scope.hover_li_fun = function (num1, num2) {
        if ($scope.hover_li_t_c == num1) {
            $scope.hover_li_t = num2;
        } else {
            $scope.hover_li_t_c = num1;
            $scope.hover_li_t = 1;
        }
    }
    //
    $scope.ptSever_fun = function (num) {
        if( $scope.ptSever_show != num){
            $scope.hover_li_t = 1;
        }
        $scope.ptSever_show = num;
    }

    //二维码显示
    $scope.erwema_fun = function (num) {
        $scope.erwema = num;
    }










})