var main_1_ctr = myApp.controller("main_1_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $timeout, myApplocalStorage) {

    $scope.select_st = true;
    $scope.sel_inbiao = 1;


    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //title_show
    $scope.xijie_show = function (num) {
        if (num == 1) {
            $scope.xijie_show_a = true;

        } else if (num == 2) {
            $scope.xijie_show_b = true;

        }
    }
    $scope.xijie_hide = function (num) {
        if (num == 1) {
            $scope.xijie_show_a = false;

        } else if (num == 2) {
            $scope.xijie_show_b = false;

        }
    }
    $scope.kuandu = {
        "width": "100%",
        "height": "690px",
    }
    $scope.top_title = {
        "top": "265px",
    }
    //获取高度
    $scope.fangda_fun = function () {
        $scope.famhfa_s = !$scope.famhfa_s;
        var winWidth = 0;
        var winWidth = 0;
        // 获取窗口宽度 
        if (window.innerWidth) {
            winWidth = window.innerWidth;
        }
        else if ((document.body) && (document.body.clientWidth)) {
            winWidth = document.body.clientWidth;
        }
        // 获取窗口高度 
        if (window.innerHeight) {
            winHeight = window.innerHeight;
        }
        else if ((document.body) && (document.body.clientHeight)) {
            winHeight = document.body.clientHeight;
        }
        $scope.kuandu = {
            "width": winWidth,
            "height": winWidth * 0.37,
        }
        $scope.top_title = {
            "top": winWidth * 0.15,
        }
    }
    $scope.fangda_fun();
    $(window).resize(function () {
        $scope.$apply(function () {
            $scope.fangda_fun();
        });
    });
    ////监测鼠标高度
    //window.onscroll = function () {
    //    var t = document.documentElement.scrollTop || document.body.scrollTop;
    //    if (window.innerWidth) {
    //        winWidth = window.innerWidth;
    //    }
    //    else if ((document.body) && (document.body.clientWidth)) {
    //        winWidth = document.body.clientWidth;
    //    }
    //    var height=winWidth * 0.37-120;
    //    if (t >= height) {
    //        $scope.scooled_top = false;
    //        $scope.$apply();
    //    } else {
    //        $scope.scooled_top = true;
    //        $scope.$apply();
    //    }
    //}

    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //切换查询条件
    $scope.changeSelT = function (num) {
        if (num == 1) {
            $scope.select_st = true;
        } else if (num == 2) {
            $scope.select_st = false;
        }

    }
    //幻灯片
    $scope.changeSelHDP = function (num) {
        $scope.select_hdp = num;
        num = num + 1;
        if (num == 4) {
            num = 1;
        }
        $timeout(function () {
            $scope.$apply(function () {
                $scope.changeSelHDP(num);
            });
        }, 6000);
    }

    $scope.xianzebiange = function (num) {
        $scope.sel_inbiao = num;
    }


    $scope.changeSelHDP(1);












})