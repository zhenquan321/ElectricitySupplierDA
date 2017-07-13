var addReport_ctr = myApp.controller("addReport_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout, $modal, myApplocalStorage) {
    $scope.SelReportSw = 0;
    $scope.selectPjt0bject = {};
    $scope.name='';
    $scope.description='';
    $scope.iconUrl='';
    $scope.gengxin = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    //项目列表显示
    $scope.SelReportSwFun = function (num) {
        $scope.SelReportSw = num;
    }
    //1.获取项目列表
    $scope.GetProjects = function () {
        var url = "/api/Keyword/GetProjects?usr_id=" + $rootScope.userID + "&page=" + 0 + "&pagesize=" + 1000;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.projectsList = response.Result;
            console.log($scope.projectsList);
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };

    //选择项目
    $scope.selectPjt = function (x) {
        $scope.selectPjt0bject = x;
        $scope.SelReportSwFun(0);
    }
    //1.创建报告
    $scope.InsertReport = function () {
       
        if (!$scope.name) {
            $scope.alert_fun('warning', '报告标题不能为空！')
            return
        }
        if (!$scope.description) {
            $scope.alert_fun('warning', '简述不能为空！')
            return
        }
        if (!$scope.selectPjt0bject._id) {
            $scope.alert_fun('warning', '请选择项目！')
            return
        }
        if (!$rootScope.PictureSrcReport) {
            $scope.alert_fun('warning', '请选择图片！')
            return
        }
        var url = "/api/Report/InsertReport?usr_id=" + $rootScope.userID + "&name=" + $scope.name + "&description=" + $scope.description + "&projectId=" + $scope.selectPjt0bject._id + "&iconUrl=" + $rootScope.PictureSrcReport;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess==true) {
                $scope.name = '';
                $scope.description = '';
                $rootScope.PictureSrcReport = '';
                $scope.SelReportSwFun(0);
                $scope.selectPjt0bject = {};
                $scope.alert_fun('success', '创建成功！');
                $location.path("/Dashboard/Report/ReportList").replace();
            } else {
                $scope.alert_fun('danger', response.Message);
            }
          
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };

    $scope.unInsertReport = function () {
        $scope.name = '';
        $scope.description = '';
        $rootScope.PictureSrcReport = '';
        $scope.selectPjt0bject = {};
        $scope.SelReportSwFun(0);
    };
    //3.分享项目弹框
    $scope.openPictureModal = function (id, Name, email) {

        var CP_scope = $rootScope.$new();
        CP_scope.projectId = id;
        CP_scope.projectName = Name;
        CP_scope.usrEmails = email;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/Dashboard/modal/addPicture.html',
            controller: addPicture_ctr,
            scope: CP_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'lg'
        });
        frm.result.then(function (response, status) {
        });
    };
    //获取创建日志列表
    $scope.GetReportLog = function () {
        var url = "/api/Report/GetReportLog?usr_id=" + $rootScope.userID ;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ReportLogList = response;
            console.log($scope.ReportLogList);
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
    //判断是否是更新report
    $scope.ifUpdataReport = function () {
        if ($rootScope.UpdateReportModal) {
            console.log($rootScope.UpdateReportModal)
            $scope.gengxin = true;
            $scope.UpdataReportId = $rootScope.UpdateReportModal._id;
            $scope.name = $rootScope.UpdateReportModal.Name;
            $scope.description = $rootScope.UpdateReportModal.Description;
            $rootScope.PictureSrcReport = $rootScope.UpdateReportModal.IconUrl;
            $scope.selectPjt0bject.Name = $rootScope.UpdateReportModal.ProjectName;
            $scope.selectPjt0bject._id = $rootScope.UpdateReportModal.ProjectId;
        }
    }
    $scope.ifUpdataReport();

    //更新report封面信息
    $scope.UpdateReport = function () {
        var url = "/api/Report/UpdateReport?reportId=" + $scope.UpdataReportId + "&name=" + $scope.name + "&description=" + $scope.description + "&iconUrl=" + $rootScope.PictureSrcReport;
        var q=$http.get(url)
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.name = '';
                $scope.description = '';
                $rootScope.PictureSrcReport = '';
                $scope.SelReportSwFun(0);
                $scope.selectPjt0bject = {};
                $scope.alert_fun('success', '修改成功！');
                $location.path("/Dashboard/Report/ReportList").replace();
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (response) {
            $scope.error = '接口调用连接出错'
        });
    }




    //自动加载
    $scope.GetProjects();
    $scope.GetReportLog();
    
});