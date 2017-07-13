var picSearch_result_ctr = myApp.controller("picSearch_result_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal) {

    //selectStatus

    $scope.page_picSre = 1;
    $scope.pagesize_picSre = 10


    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    //获取侵权图片
    $scope.GetImgSearchLinks = function () {
        $cookieStore.put("searchSrc", $rootScope.searchSrc)
        $scope.paramsList = {
            user_id: $rootScope.userID,
            projectId: $rootScope.getProjectId,
            page: $scope.page_picSre - 1,
            pagesize: $scope.pagesize_picSre,
            status: 1,
            searchTaskId: "",
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "api/Img/GetImgSearchLinks"
        })
            .success(function (response, status) {
                $scope.Result_count = response.Count;
                $rootScope.GetImgSearchLinks_list1 = response.Result;
                console.log($scope.GetImgSearchLinks_list1);
            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "服务器连接出错");
            });

    }

    //设置图片状态
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
                    $rootScope.addAlert('success', "排除成功！");
                } else if (status == 1) {
                    $rootScope.addAlert('success', "收藏成功！");
                }

                $scope.GetImgSearchLinks();

            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "服务器连接出错");
            });

    }

    //自动加载_____________________________________________________________
    $scope.GetImgSearchLinks();
});