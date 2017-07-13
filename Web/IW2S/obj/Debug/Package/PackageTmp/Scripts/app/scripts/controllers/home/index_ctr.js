var index_ctr = myApp.controller("index_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


});