var AboutUs_ctr = myApp.controller("AboutUs_ctr", function ($scope, $rootScope, $http, $location, $window, $modal, $cookieStore,$timeout) {
  
    $scope.card_show = 1;
    $scope.wechart_s_k = false;
    $scope.card_plshow = 1;
    $scope.yundong = {
        'width': '80px',
        'left': '589px',
        'height': '0',
        'transition': '1s ease;'
    }
    //________________________________________________
    //header 方法——————————————————————————
    $scope.showamendpwd = function (num) {
        $scope.showAmendAndOff = num;
    }
    $scope.hideamendpwd = function (num) {
        $scope.showAmendAndOff = false;
    }
    //title——active
    $scope.Onmouse = function (num) {
        if (num == 1) {
            $scope.yundong = {
                'width': '36px',
                'left': '0',
                'height': '0',
                'transition': '1.5 s ease;'
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




})