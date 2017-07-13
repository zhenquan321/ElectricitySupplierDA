var PurchaseService_ctr = myApp.controller("PurchaseService_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage,$modal) {

    $scope.TabListShow = 1;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //切换Tab list
    $scope.ChangeTabList = function (num) {
        $scope.TabListShow = num;
    }
  



})