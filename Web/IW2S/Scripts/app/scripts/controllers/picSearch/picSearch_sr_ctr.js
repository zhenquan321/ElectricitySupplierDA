var picSearch_sr_ctr = myApp.controller("picSearch_sr_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {

    //selectStatus

    $scope.page_picS = 1;
    $scope.pagesize_picS = 10;


    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //设置搜索图状态
    $scope.SetLinkStatus = function (id, status) {

        $scope.paramsList = {
            status: status,
            id: id,
            user_id: $rootScope.userID,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "api/Img/SetLinkStatus"
        })
            .success(function (response, status) {
                if (status == 2) {
                    $scope.addAlert('success', "排除成功！");
                } else if (status == 1) {
                    $scope.addAlert('success', "收藏成功！");
                }
                $scope.GetImgSearchLinks($rootScope.picRs_id, $rootScope.searchSrc);
            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "网络打盹了，请稍后。。。");
            });

    }
    //
    //$scope.GetImgSearchTasks_fun_all = function () {


    //    $scope.pagesize_picS = $scope.pagesize_picS+10;

    //    $scope.GetImgSearchTasks_fun();
    //}


    //自动加载_____________________________________________________________

    $scope.GetImgSearchLinks($rootScope.picRs_id, $rootScope.searchSrc);
    //$scope.GetImgSearchTasks_fun_all();
});