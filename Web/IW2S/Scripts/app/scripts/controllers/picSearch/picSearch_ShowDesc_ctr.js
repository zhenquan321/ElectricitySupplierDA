var picSearch_ShowDesc_ctr = myApp.controller("picSearch_ShowDesc_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {

    //selectStatus

    $scope.page_picS = 1;
    $scope.pagesize_picS = 10

    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    //0.4picS
    $scope.changeModel_picS = function () {
        $rootScope.isActiveModale = "picSearch";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }
    $scope.changeModel_picS();
    //1.获取搜索图
    $scope.GetImgSearchTasks_fun = function () {
        $scope.paramsList = {
            usr_id: $rootScope.userID,
            prjId: $rootScope.getProjectId,
            page: $scope.page_picS - 1,
            pagesize: $scope.pagesize_picS,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "api/Img/GetImgSearchTasks"
        })
            .success(function (response, status) {
                $scope.conut_St = response.Count;
                $rootScope.searchPic = response.Result;
                console.log(response)
            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "服务器连接出错");
            });
    }

    //2.删除搜索图
    $scope.DelImgSearchTask_fun = function (id) {
        $scope.paramsList = {
            ids: id,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "api/Img/DelImgSearchTask"
        })
            .success(function (response, status) {
                $rootScope.addAlert('success', "删除成功！");
                $scope.GetImgSearchTasks_fun()
            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "服务器连接出错");
            });
    }


    //自动加载_____________________________________________________________

});