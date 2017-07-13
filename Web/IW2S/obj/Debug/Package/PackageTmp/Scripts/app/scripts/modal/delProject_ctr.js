var delProject_ctr = function ($scope, $modalInstance, $rootScope, $http) {

    $scope.isactive = "";
    $scope.projectDce = "";
    $scope.projectJg = "";
    $scope.projectGx = "";
    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };


    $scope.nextStape = function () {
        $scope.isactive = true;
    };

    $scope.nextStape21 = function () {
        var url = "/api/Keyword/DelProject?ids=" + $scope.projectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('modelSelect_ctr>DelProject');
            $rootScope.addAlert('success', '项目删除成功！');
            $rootScope.GetProjects1();
        });
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
        });

    };

    $scope.ExportProject_all = function () {
        if ($scope.projectDce == true) {
            $scope.paramsList = {
                user_id: $rootScope.userID,
                projectId: $scope.projectId,
            };
            $http({
                method: 'get',
                params: $scope.paramsList,
                //  data:{name:'john',age:27},
                url: "/api/Keyword/ExportProject"
            })
                .success(function (response, status) {
                    if (response != null) {
                        if (response != "没有要导出的数据") {
                            window.location.href = "Export/DownLoadExcel?path=" + response;
                            $rootScope.addAlert('success', "监测结果导出成功！");
                        } else {
                            $rootScope.addAlert('danger', "没有要导出监测结果！");
                        }
                    }
                })
                .error(function (response, status, headers, config) {
                    $rootScope.addAlert('danger', "项目描述导出成功！");
                });
        }
        ;
        if ($scope.projectJg == true) {
            $scope.paramsList = {
                user_id: $rootScope.userID,
                projectId: $scope.projectId,
            };
            $http({
                method: 'get',
                params: $scope.paramsList,
                //  data:{name:'john',age:27},
                url: "/api/Keyword/ExportLevelLinks"
            })
                .success(function (response, status) {
                    if (response != null) {
                        if (response != "没有要导出的数据") {
                            window.location.href = "Export/DownLoadExcel?path=" + response;
                            $rootScope.addAlert('success', "监测结果导出成功！");
                        } else {
                            $rootScope.addAlert('danger', "没有要导出监测结果！");
                        }
                    }
                })
                .error(function (response, status, headers, config) {
                    $rootScope.addAlert('danger', "监测结果导出失败！");
                });
        }
        ;
        if ($scope.projectGx == true) {
            $scope.paramsList = {
                user_id: $rootScope.userID,
                projectId: $scope.projectId,
            };
            $http({
                method: 'get',
                params: $scope.paramsList,
                //  data:{name:'john',age:27},
                url: "/api/Keyword/ExportKeywordGroup"
            })
                .success(function (response, status) {
                    if (response != null) {
                        if (response != "没有要导出的数据") {
                            window.location.href = "Export/DownLoadExcel?path=" + response;
                            $rootScope.addAlert('success', "监测结果导出成功！");
                        } else {
                            $rootScope.addAlert('danger', "没有要导出监测结果！");
                        }
                    }

                })
                .error(function (response, status, headers, config) {
                    $rootScope.addAlert('danger', "数据关系导出失败！");
                });
        }
        ;
        $scope.nextStape21();

        $scope.cancel();


    };


    $scope.cancel2 = function () {

        $scope.nextStape21();

        $scope.cancel();


    }


};