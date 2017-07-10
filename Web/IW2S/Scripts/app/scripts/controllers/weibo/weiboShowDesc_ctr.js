var weiboShowDesc_ctr = myApp.controller("weiboShowDesc_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {

    $scope.isActiveCollection2 = true;
    //selectStatus

    $scope.page_picS = 1;
    $scope.pagesize_picS = 20


    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    //8.百度搜藏链接
    $scope.showbaiduCollectionLinks = function () {
        if ($scope.isActiveCollection2 == true) {
            $scope.isActiveCollection2 = false;
            $scope.status = 1;
            $scope.GetBaiduLevelLinks2();
        } else if ($scope.isActiveCollection2 == false) {
            $scope.isActiveCollection2 = true;
            $scope.status = "";
            $scope.GetBaiduLevelLinks2();
        }
    }
    //2.删除搜索图
    //$scope.DelImgSearchTask_fun = function (id) {
    //    $scope.paramsList = {
    //        ids: id,
    //    };
    //    $http({
    //        method: 'get',
    //        params: $scope.paramsList,
    //        url: "api/Img/DelImgSearchTask"
    //    })
    //    .success(function (response, status) {
    //        $rootScope.addAlert('success', "删除成功！");
    //        $scope.GetImgSearchTasks_fun()
    //    })
    //    .error(function (response, status) {
    //        $rootScope.addAlert('danger', "服务器连接出错");
    //    });
    //}


    //自动加载_____________________________________________________________

    $scope.GetBaiduKeyword();
});