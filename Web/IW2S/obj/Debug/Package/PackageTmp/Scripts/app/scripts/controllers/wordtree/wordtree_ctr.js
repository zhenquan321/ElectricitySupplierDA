var wordtree_ctr = myApp.controller("wordtree_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {

    $scope.duibiShow = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    $scope.duibiShowFun = function () {
        $scope.duibiShow = !$scope.duibiShow;




    }















});