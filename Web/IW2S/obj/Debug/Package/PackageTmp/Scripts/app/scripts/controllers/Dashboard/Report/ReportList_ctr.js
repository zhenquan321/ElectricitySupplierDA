var ReportList_ctr = myApp.controller("ReportList_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout, $modal, myApplocalStorage) {

    $scope.page = 0;
    $scope.pagesize = 20;
    $scope.pageMS = 0;
    $scope.pagesizeMS = 20;
    $scope.pageSM = 0;
    $scope.pagesizeSM = 20;
    $rootScope.UpdateReportModal = '';
    $scope.reportModal = 1;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //切换获取报告
    $scope.changeReportModal = function (num) {
        $scope.reportModal = num;
        if (num == 1) {
            $scope.GetMyReport();
        }else if(num==2){
            $scope.getMyShareReport();
        }else if(num==3){
            $scope.GetShareToMeReport();
        }
    }
    //获取项目列表
    $scope.GetMyReport = function () {
        var url = "/api/Report/GetMyReport?usr_id=" + $rootScope.userID + '&page=' + $scope.page + '&pagesize=' + $scope.pagesize;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.MyReportList = response.Result;
            console.log($scope.MyReportList);
            $timeout(function () {
                $scope.changePicHeight();
            }, 200)
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
    //调整图片高度
    $scope.changePicHeight = function () {
        if (document.getElementById('imgHeight')) {
            var widthRight = document.getElementById('imgHeight').offsetWidth;
            $scope.imgHeightStyle = {
                "height": widthRight * 0.6,
                "overflow": "hidden",
            }
        }
    }
    window.onresize = function () {
        $scope.changePicHeight();
        $scope.$apply();
    }
    //删除简报
    $scope.DelReport = function (id) {
        var url = "/api/Report/DelReport?ids=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response == '成功！') {
                $scope.alert_fun('success', '项目删除成功！');
                $scope.GetMyReport();
            }
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };

    //修改简报封面
    $scope.UpdateReport_Fun = function (x) {
        $rootScope.UpdateReportModal = x;
        if(x){
            $location.path("/Dashboard/addReport").replace();
        }
    }

    //分享报告

    $scope.shareReport = function (id,name) {
        var SR_scope = $rootScope.$new();
        SR_scope.shareReportId = id;
        SR_scope.shareReportName = name;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/Dashboard/modal/shareReport.html',
            controller: shareReport_ctr,
            scope: SR_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'md'
        });
        frm.result.then(function (response, status) {
            $scope.alert_fun('success', '分享成功！');
        });
    }
    //取消分享

    $scope.CancelShareReport = function (id) {
        var url = "/api/Report/CancelShareReport?usr_id=" + $rootScope.userID + "&reportId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess==true) {
                $scope.alert_fun("success", '取消成功！')
                $scope.getMyShareReport();
            } else {
                $scope.alert_fun("worning", 'response.Message')
            }
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    }









    //获取分享报告
    $scope.getMyShareReport = function () {
        var url = "/api/Report/GetMyShareReport?usr_id=" + $rootScope.userID + "&page=" + $scope.pageMS + "&pagesize=" + $scope.pagesizeMS;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetMyShareReportList = response.Result;
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    }
    //获取分享给我的报告
    $scope.GetShareToMeReport = function () {
        var url = "/api/Report/GetShareToMeReport?usr_id=" + $rootScope.userID + "&page=" + $scope.pageSM + "&pagesize=" + $scope.pagesizeSM;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetShareToMeReportList = response.Result;
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    }



    
    //___________________________________________________________________________________________________________________________
    $scope.changeReportModal(1);
});