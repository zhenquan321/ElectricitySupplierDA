var home_ctr = myApp.controller("home_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    //$scope.background_show = 0;

    $rootScope.reload_LS = false;

    $scope.Login_ReloadPutC = function () {
        $rootScope.reload_LS = false;
        $cookieStore.put("reload_LS", $rootScope.reload_LS);

    }

    $scope.Login_ReloadPutC();

    //$scope.chang_background = function () {
    //    var num = Math.random();
    //    if (num<.2) {
    //        $scope.background_show = 1;
    //    } else if (.2<=num < .4) {
    //        $scope.background_show = 2;
    //    }else if(.4<=num<.6){
    //        $scope.background_show = 3;
    //    }else if(.6<=num<.8){
    //        $scope.background_show = 4;
    //    }else if(.8<=num<1){
    //        $scope.background_show = 5;
    //    }
    //}
    ////___________________________________
    //$scope.chang_background();


});