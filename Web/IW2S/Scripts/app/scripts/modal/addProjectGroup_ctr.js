var addProjectGroup_ctr = function ($scope, $modalInstance, $rootScope, $http) {

    $scope.error = "";
    $scope.selectPjt0bject = {};
    $scope.SelReportSw = 0;

    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };


    //项目列表显示
    $scope.SelReportSwFun = function (num) {
        $scope.SelReportSw = num;
    }

    //选择项目
    var selectPjt0bject = [];

    $scope.selectPjt = function (x) {
        if (selectPjt0bject.length>0) {
            var cc=true;
            for (var i = 0; i < selectPjt0bject.length; i++) {
                if (selectPjt0bject[i]._id == x._id) {
                    cc = false;
                } 
            }
            if (cc) {
                selectPjt0bject.push(x);
                $scope.selectPjt0bjectList = selectPjt0bject;
            }
        } else {
            selectPjt0bject.push(x);
            $scope.selectPjt0bjectList = selectPjt0bject;
        }
    }

    $scope.delPjt = function (x) {
        for (var i = 0; i < $scope.selectPjt0bjectList.length; i++) {
            if ($scope.selectPjt0bjectList[i]._id == x._id) {
                $scope.selectPjt0bjectList.splice(i, 1);
                break;
            }
        }
    }

    //1.创建项目组
    $scope.InsertProjectCategory = function () {
        $scope.projectIds = "";
        for (var i = 0;i<$scope.selectPjt0bjectList.length;i++){
            $scope.projectIds = $scope.projectIds + $scope.selectPjt0bjectList[i]._id + ';';
        }
        if (!$scope.projectName) {
            $scope.error = "项目组名称不能为空";
            return
        }
        if (!$scope.projectIds) {
            $scope.error = "请选择项目";
            return
        }
        if (!$scope.projectNameDescribe) {
            $scope.error = "请选择输入项目描述";
            return
        }
        var url = "/api/ProCategory/InsertProjectCategory?userId=" + $rootScope.userID + "&Name=" + $scope.projectName + "&description=" + $scope.projectNameDescribe + "&projectIds=" + $scope.projectIds;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('addProject_ctr>InsertProject');
            $scope.massage = response.Message;
            if (response.IsSuccess != true) {
                $scope.error = $scope.massage;
            } else {
                $scope.ok();
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    //更新项目组
    $scope.UpdateProjectCategory = function () {
        $scope.projectIds = "";
        for (var i = 0; i < $scope.selectPjt0bjectList.length; i++) {
            $scope.projectIds = $scope.projectIds + $scope.selectPjt0bjectList[i]._id + ';';
        }
        if (!$scope.projectName) {
            $scope.error = "项目组名称不能为空";
            return
        }
        if (!$scope.projectIds) {
            $scope.error = "请选择项目";
            return
        }
        if (!$scope.projectNameDescribe) {
            $scope.error = "请选择输入项目描述";
            return
        }
        var url = "/api/ProCategory/UpdateProjectCategory?categoryId=" + $scope.categoryId + "&Name=" + $scope.projectName + "&description=" + $scope.projectNameDescribe + "&projectIds=" + $scope.projectIds;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('addProject_ctr>InsertProject');
            $scope.massage = response.Message;
            if (response.IsSuccess != true) {
                $scope.error = $scope.massage;
            } else {
                $scope.ok();
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    //获取项目列表
    $scope.GetProjects2 = function () {
        var url = "/api/Keyword/GetProjects?usr_id=" + $rootScope.userID + "&page=" + 0 + "&pagesize=" + 1000;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('modelSelect_ctr>GetProjects');
            if (response != null) {
                $rootScope.projectsList2 = response.Result;
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    $scope.GetProjects2();


}