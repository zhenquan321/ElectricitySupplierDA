var shareReportHeader_ctr = myApp.controller("shareReportHeader_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {
    $scope.keXiuGai = true;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);

    $scope.keXiuGaiFun = function (num, x, state) {
        $scope.keXiuGai = num;
        $scope.selsetReport = x;
        $scope.reportModal_LX = state;
        $cookieStore.put("keXiuGai", $scope.keXiuGai);
        $cookieStore.put("selsetReport", $scope.selsetReport);
        $cookieStore.put("reportModal_LX", $scope.reportModal_LX);
    }



});