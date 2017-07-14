var PurchaseService_ctr = myApp.controller("PurchaseService_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage,$modal) {

    $scope.TabListShow = 1;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //切换Tab list
    $scope.ChangeTabList = function (num) {
        $scope.TabListShow = num;
    }
    //专业服务
    $scope.zhuanyepaypage = function () {
        var can = {
            title: '专业服务',
            money: 245,
        };
        $scope.getPayPage(can);
    }
    //企业服务
    $scope.qiyePayPage = function () {
        var can = {
            title: '企业套餐',
            money: 13600,
        };
        $scope.getPayPage(can);
    }
    //付费页面_Gong
    $scope.getPayPage = function (x) {
        var kw_scope = $rootScope.$new();
        kw_scope.title = x.title;
        kw_scope.money = x.money;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/Dashboard/modal/payPage.html',
            controller: payPage_ctr,
            scope: kw_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'md'
        });
        frm.result.then(function (response, status) {
           
        });
    };



})