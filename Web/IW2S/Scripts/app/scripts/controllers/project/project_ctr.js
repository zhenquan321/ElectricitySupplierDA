var project_ctr = myApp.controller("project_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {
    //tab切换项目总揽
    $scope.isActive = false;
    $scope.ProjectName = "";
    $scope.ProjectDesc = "";
    $scope.ComparePeriod = "3个月";
    $rootScope.loadingTrue = false;//加载中
    $scope.isActiveList = false;
    $scope.isActiveList2 = false;
    //1.tab切换项目总览
    $scope.changeOverview = function () {
        $scope.isActive = true;
    };
    //2.tab切换添加项目
    $scope.changeAdd = function () {
        $scope.isActive = false;
    };
    //3.添加项目
    $scope.AddProject = function () {

        if ($scope.ProjectName == null || $scope.ProjectName == "") {
            alert("项目名不能为空")
        } else if ($scope.ProjectDesc == null || $scope.ProjectDesc == "") {
            alert("项目描述不能为空")
        } else {
            $rootScope.loadingTrue = true;//加载中

            $scope.filter = {
                ProjectName: $scope.ProjectName,
                CompanyID: $rootScope.CompanyID,
                ProjectDesc: $scope.ProjectDesc,
                ComparePeriod: $scope.ComparePeriod,
            };
            var urls = "api/Mng/AddProject?project=" + $scope.filter;
            var q = $http.post(
                urls,
                JSON.stringify($scope.filter),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            )
            $scope.isLoading = true;
            q.success(function (response, status) {
                console.log('project_ctr>AddProject');
                $rootScope.loadingTrue = false;//加载中

                if (response.Message != "添加成功") {
                    alert(response.Message)
                } else {
                    alert("添加成功!");
                    $scope.ProjectName = "";
                    $scope.ProjectDesc = "";
                }
            });
            q.error(function (e) {
                alert("服务器连接出错")

                $rootScope.loadingTrue = false;//加载中

            });
        }
        ;
    }

    //4.加载项目

    $scope.page = 1;

    $scope.pagesize = 10,
        $scope.ProjectName = "";
    $scope.project = [];
    $rootScope.projectList = [];
    $scope.proCount = 0;


    $scope.loadProjects = function () {
        $rootScope.loadingTrue = true;//加载中
        var url = "/api/Mng/GetProjects?companyID=" + $rootScope.CompanyID + "&companyName=" + "&projectName="
            + encodeURIComponent($scope.ProjectName) + "&page=" + ($scope.page - 1) + "&pagesize=" + $scope.pagesize;
        var p = $http.get(url);
        p.success(function (response, status) {
            console.log('project_ctr>loadProjects');
            $rootScope.projectList = response.Result;

            $scope.proCount = response.Count;
            if ($rootScope.projectList.length != 0) {
                $scope.isActiveList = true;
                $scope.isActiveList2 = true;
            }
            $rootScope.loadingTrue = false;//加载中

        });
        p.error(function (e) {
            $scope.error = "连接出错" + e;
            $rootScope.loadingTrue = false;//加载中

        });
    }


    //4.1调节表格高度
    $scope.windowsize = function () {
        $scope.winHeight = 0;
        //获取窗口高度
        var i = document.body.scrollHeight
        if (window.innerHeight) {
            winHeight = window.innerHeight;
        }
        else if ((document.body) && (document.body.clientHeight)) {
            winHeight = document.body.clientHeight;
        }
        //通过深入Document内部对body进行监测，获取窗口大小
        if (document.documentElement && document.documentElement.clientHeight && document.documentElement.clientWidth) {
            winHeight1 = document.documentElement.clientHeight - 275;
        }
        if (document.getElementById("maSize")) {
            $scope.size1 = winHeight1 + "px";
            $scope.maSize = {
                "height": $scope.size1,
                "position": "relative",
                "overflow": "auto",
            };
        }
        ;
    }
    $scope.windowsize();
    //5.删除项目
    $scope.DeleteProject = function (id) {
        var url = "/api/Mng/DeleteProject?projectID=" + id;
        var p = $http.get(url);
        p.success(function (response, status) {
            console.log('project_ctr>DeleteProject');
            alert(response.Message);
            $scope.loadProjects();
        });
        p.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }

    //6.启动项目
    $scope.startPrj = function (projectID, isApproved) {
        if (isApproved == false) {
            alert("项目没有批准不能启动");
            return;
        }
        var url = "/api/Mng/StartProject?projectID=" + projectID + "&isStart=" + true;
        var p = $http.get(url);
        p.success(function (response, status) {
            console.log('project_ctr>startPrj');
            if (response.IsSuccess) {
                alert("项目启动成功!");
                $scope.loadProjects();
            }
            else {
                alert(data.Message);
            }
            $scope.loadProjects();
        });
        p.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }
    //7.停止项目
    $scope.stopPrj = function (projectID) {
        $.get('/api/Mng/StartProject', {
                projectID: projectID,
                isStart: false
            },
            function (data) {
                if (data.IsSuccess) {
                    alert("项目停止成功!");
                    $scope.loadProjects();
                }
                else {
                    alert(data.Message);
                }
            });
    }
    //设置滚动轴长度
    $scope.set_width = function (payment) {

        return 'style="width:100%"'

    }
    //批准项目
    $scope.approvePrj = function (projectID) {
        $.get('/api/Mng/ApproveProject', {
                projectID: projectID
            },
            function (data) {
                if (data.IsSuccess) {
                    alert("项目批准成功!");
                    $scope.loadProjects();

                }
                else {
                    alert(data.Message);
                }
            });
    }


    $scope.loadProjects();
    $scope.windowsize();

});