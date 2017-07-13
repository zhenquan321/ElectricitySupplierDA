var Report_ctr = myApp.controller("Report_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout, myApplocalStorage) {
    $scope.keXiuGai = true;
    
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    $scope.keXiuGaiFun = function (num,x,state) {
        $rootScope.keXiuGai = num;
        $rootScope.selsetReport = x;
        $rootScope.reportModal_LX = state;
        $cookieStore.put("keXiuGai", $rootScope.keXiuGai);
        $cookieStore.put("selsetReport", $rootScope.selsetReport);
        $cookieStore.put("reportModal_LX", $rootScope.reportModal_LX);
     
    }
   
});