var home_ctr = myApp.controller("home_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    $scope.card_show = 1;
    $scope.wechart_s_k = false;
    $scope.card_plshow = 1;
    //________________________________________________
    //header 方法——————————————————————————
    $scope.showamendpwd = function (num) {
        $scope.showAmendAndOff = num;
    }
    $scope.hideamendpwd = function (num) {
        $scope.showAmendAndOff = false;
    }
    //header切换
    $scope.chang_cards = function (num, sit) {
        if (num > 3) {
            num = 1;
        }
        if (sit==true) {
        }
        $scope.card_show = num;
        num++
        $timeout(function () { $scope.chang_cards(num,false)}, 12000)
    }
    $scope.chang_cards(1);

    //评论切换
    $scope.chang_plcards = function (sit) {
       
        if (sit == true) {
            $scope.card_plshow ++;
        }else{
            $scope.card_plshow--;
        }
        if ($scope.card_plshow > 3) {
            $scope.card_plshow = 1;
        }
        $timeout(function () { $scope.chang_plcards(true) }, 8000)
    }
    $scope.chang_plcards(1);


    $scope.wechart_s = function (num) {
        if (num == 1) {
            $scope.wechart_s_k = true;
        } else {
            $scope.wechart_s_k = false;
        }
    }
    //title——active
    $scope.Onmouse = function (num) {
        if (num==1) {
            $scope.yundong = {
                'width': '36px',
                'left': '0',
                'height': '0',
                'transition': '1.5 s ease;',
            }
        } else if (num == 2) {
            $scope.yundong = {
                'width': '80px',
                'left': '132px',
                'height': '0',
                'transition': '1s ease;'
            }
        } else if (num == 3) {
            $scope.yundong = {
                'width': '80px',
                'left': '289px',
                'height': '0',
                'transition': '1s ease;'
            }
        } else if (num == 4) {
            $scope.yundong = {
                'width': '80px',
                'left': '434px',
                'height': '0',
                'transition': '1s ease;'
            }
        } else if (num == 5) {
            $scope.yundong = {
                'width': '80px',
                'left': '589px',
                'height': '0',
                'transition': '1s ease;'
            }
        }
       
    }
});