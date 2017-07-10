var modelSelect_ctr = myApp.controller("modelSelect_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $timeout, myApplocalStorage) {


    //selectStatus
    $rootScope.selectStatus = "百度";
    $scope.isCollection = true;
    $scope.timeStampSelect = [{"Id": 0, "Name": "否"}, {"Id": 1, "Name": "是"}];
    $rootScope.isSuccess = "isNull";
    $scope.searchInput = "";
    $scope.page = 1;
    $scope.pagesize = 10;
    $scope.page2 = 1;
    $scope.pagesize2 = 10;
    $scope.page3 = 0;
    $scope.pagesize3 = 10;
    $scope.Abstract = "";
    $rootScope.keyword = "";
    $scope.Title = "";
    $scope.domain = "";
    $scope.status = "";
    $rootScope.resultList = [];
    $rootScope.ZhiboresultList = [];
    $rootScope.alerts = [];
    $scope.pageBaidu = 0;
    $rootScope.pagesizeBaidu = 10;
    $scope.keywordsList1 = [];
    $scope.keywordsListCommend = [];
    $rootScope.getBaiduRecordId = "";
    $scope.isActiveKeyword = false;
    $rootScope.BaidukeywordId = "";
    $rootScope.ZhibokeywordId = "";
    $rootScope.keywordsList = [];
    $scope.isActiveUserStyle = false;
    $rootScope.isActiveLoadmore = true;
    $rootScope.isActiveLoadmoreZhiboLevelLinks = true;
    $scope.BaiduCount = "";
    $scope.id = "";
    $scope.status = "";
    $scope.isActiveCollection = true;
    $scope.isActiveCollection2 = true;
    $rootScope.categoryId = "";
    $scope.page4 = 1;
    $scope.pagesize4 = 11;
    $scope.page_group = 1;
    $scope.pagesize_group = 11;
    $rootScope.projectsList = [];
    $scope.projectsListlength = 0;
    $rootScope.logined == false;
    $scope.shareUser = 1;
    $scope.shareUser_num = 0;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //显示分享用户
    $scope.shareUser_s = function (num,num2) {
        $scope.shareUser = num;
        $scope.shareUser_num = num2;
    }
    //页面添加alert
    $rootScope.addAlert = function (status, messages) {
        var len = $rootScope.alerts.length + 1;
        $rootScope.alerts = [];
        $rootScope.alerts.push({type: status, msg: messages});
    }
    //关闭alert
    $scope.closeAlert = function (index) {
        $rootScope.alerts.splice(index, 1)
    }

    //11.添加项目弹框
    $scope.addProject = function () {
        var kw_scope = $rootScope.$new();
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/addProject.html',
            controller: addProject_ctr,
            scope: kw_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            $scope.GetProjects();
        });
    };

    //11.修改项目弹框
    $scope.changeProject = function (id, Name, Description) {

        var CP_scope = $rootScope.$new();
        CP_scope.projectId = id;
        CP_scope.projectName = Name;
        CP_scope.projectNameDescribe = Description;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/changeProject.html',
            controller: changeProject_ctr,
            scope: CP_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            $scope.GetProjects();
            $scope.GetMyShareProjects();
            $scope.GetShareToMeProjects();
        });
    };
    //11.删除项目弹框
    $scope.delProject = function (id) {

        var CP_scope = $rootScope.$new();
        CP_scope.projectId = id;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/delProject.html',
            controller: delProject_ctr,
            scope: CP_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            $scope.GetProjects();
        });
    };

    $rootScope.GetProjects1 = function () {
        $scope.GetProjects();
    }
    //11.2.获取项目列表
    $scope.GetProjects = function () {
        var roleId;
        if ($rootScope.UsrRole == 0) {
            roleId=$rootScope.userID;
        } else {
            roleId=$rootScope.currentId;
        }
        var url = "/api/Keyword/GetProjects?usr_id=" + roleId + "&page=" + ($scope.page4 - 1) + "&pagesize=" + $scope.pagesize4;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('modelSelect_ctr>GetProjects');
            console.log($rootScope.projectsList);
            if (response != null) {
                $rootScope.projectsList = response.Result;
                //$cookieStore.put("projectsList", $rootScope.projectsList);
                myApplocalStorage.setObject('projectsList', $rootScope.projectsList);
                $scope.projectsListlength = response.Count;

                //项目链接数变化图
                $timeout(function () {
                    for (var aa = 0; aa < $rootScope.projectsList.length; aa++) {
                    	var countList=$rootScope.projectsList[aa].CountList;
                        var cc = "#" + "modal" +aa;
                        $(cc).sparkline(countList, {
                            type: 'line',
                            width: '85',
                            height: '35',
                            lineColor: '#1da3a3',
                            fillColor: '#bce2e2',
                            spotColor: '#007f7f',
                            minSpotColor: '#007f7f',
                            maxSpotColor: '#007f7f',
                            highlightSpotColor: '#606060',
                            highlightLineColor: '#777777',
                            drawNormalOnTop: true
                        });
                    }
                },50)

            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };



    //11.4点击项目
    $rootScope.getProjectId = "";
    $rootScope.getPGPIdS = '';

    $scope.clickItem = function (id, name) {
        $rootScope.getProjectId = id;
        $cookieStore.put("getProjectId", $rootScope.getProjectId);
        $rootScope.getProjectName = name;
        $cookieStore.put("getProjectName", $rootScope.getProjectName);
    }
    $scope.clickItem2 = function (id, name,PGroup) {
        $rootScope.getProjectGroupId = id;
        $cookieStore.put("getProjectGroupId", $rootScope.getProjectGroupId);
        $rootScope.getProjectGroupName = name;
        $cookieStore.put("getProjectGroupName", $rootScope.getProjectGroupName);
        for (var i = 0;i<PGroup.length;i++){
            $rootScope.getPGPIdS = $rootScope.getPGPIdS + PGroup[i].ProjectId + ';';
        }
        $cookieStore.put("getPGPIdS", $rootScope.getPGPIdS);
    }
   
    //11.分享项目弹框
    $scope.shareProject = function (id, Name,email) {

        var CP_scope = $rootScope.$new();
        CP_scope.projectId = id;
        CP_scope.projectName = Name;
        CP_scope.usrEmails = email;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/shareProject.html',
            controller: shareProject_ctr,
            scope: CP_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            $scope.GetProjects();
            $scope.GetMyShareProjects();
            $scope.GetShareToMeProjects();
        });
    };
    //11.2取消分享项目

    $scope.CancelShareProject = function (id) {
        var url = "/api/Share/CancelShareProject?usr_id=" + $rootScope.userID + "&prjId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ToMeProjects = response.Result;
            console.log(response);
            $scope.GetMyShareProjects();
            $scope.GetShareToMeProjects();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };



    //12.获取分享项目
    $scope.page_share = 1;
    $scope.pagesize_share = 12;
    $scope.GetMyShareProjects = function () {
        var url = "/api/Share/GetMyShareProjects?usr_id=" + $rootScope.userID + "&page=" + ($scope.page_share-1) + "&pagesize=" + $scope.pagesize_share;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ShareProjects = response.Result;
            $scope.count_share = response.Count;
            console.log(response);
            //项目链接数变化图
                $timeout(function () {
                    for (var aa = 0; aa < $scope.ShareProjects.length; aa++) {
                    	var countList=$scope.ShareProjects[aa].CountList;
                        var cc = "#" + "myShareModal" +aa;
                        $(cc).sparkline(countList, {
                            type: 'line',
                            width: '85',
                            height: '35',
                            lineColor: '#1da3a3',
                            fillColor: '#bce2e2',
                            spotColor: '#007f7f',
                            minSpotColor: '#007f7f',
                            maxSpotColor: '#007f7f',
                            highlightSpotColor: '#606060',
                            highlightLineColor: '#777777',
                            drawNormalOnTop: true
                        });
                    }
                },50)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    //12.获取分享给我的项目
    $scope.page_shareMe = 1;
    $scope.pagesize_shareMe = 12;
    $scope.GetShareToMeProjects = function () {
        var url = "/api/Share/GetShareToMeProjects?usr_id=" + $rootScope.userID + "&page=" + ($scope.page_shareMe-1) + "&pagesize=" + $scope.pagesize_shareMe;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.ToMeProjects = response.Result;
            $scope.count_shareMe = response.Count;
            console.log(response);
            //项目链接数变化图
                $timeout(function () {
                    for (var aa = 0; aa < $scope.ToMeProjects.length; aa++) {
                    	var countList=$scope.ToMeProjects[aa].CountList;
                        var cc = "#" + "shareMeModal" +aa;
                        $(cc).sparkline(countList, {
                            type: 'line',
                            width: '85',
                            height: '35',
                            lineColor: '#1da3a3',
                            fillColor: '#bce2e2',
                            spotColor: '#007f7f',
                            minSpotColor: '#007f7f',
                            maxSpotColor: '#007f7f',
                            highlightSpotColor: '#606060',
                            highlightLineColor: '#777777',
                            drawNormalOnTop: true
                        });
                    }
                },50)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    //复制项目
    $scope.BackupProject = function (id) {
        var url = "/api/Keyword/BackupProject?projectId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.alert_fun('success','项目复制成功！');
            $scope.GetProjects();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
   

    //11.添加项目组弹框
    $scope.addProjectGroup = function () {
        var kw_scope = $rootScope.$new();
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/addProjectGroup.html',
            controller: addProjectGroup_ctr,
            scope: kw_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            $scope.GetProjectCategory();
        });
    };

    //获取项目组列表
    $scope.GetProjectCategory = function () {
        var url = "/api/ProCategory/GetProjectCategory?userId=" + $rootScope.userID + "&page=" + ($scope.page_group - 1) + "&pagesize=" + $scope.pagesize_group;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response)
            $scope.GetProjectCategoryList = response.Result;
            $scope.count_PG = response.Count;
            console.log(response);
            //项目链接数变化图
            $timeout(function () {
                for (var aa = 0; aa < $scope.GetProjectCategoryList.length; aa++) {
                    var countList = $scope.GetProjectCategoryList[aa].CountList;
                    var cc = "#" + "ProjectGroupModal" + aa;
                    $(cc).sparkline(countList, {
                        type: 'line',
                        width: '85',
                        height: '35',
                        lineColor: '#1da3a3',
                        fillColor: '#bce2e2',
                        spotColor: '#007f7f',
                        minSpotColor: '#007f7f',
                        maxSpotColor: '#007f7f',
                        highlightSpotColor: '#606060',
                        highlightLineColor: '#777777',
                        drawNormalOnTop: true
                    });
                }
            }, 50)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
    //删除项目组
    $scope.DelProjectCategory = function (id) {
        var url = "/api/ProCategory/DelProjectCategory?categoryIds="+id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess==true) {
                $scope.GetProjectCategory();
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //更新项目组
    //11.添加项目组弹框
    $scope.UpdateProjectGroup = function (x) {
        var kw_scope = $rootScope.$new();
        kw_scope.projectName = x.Name;
        kw_scope.selectPjt0bjectList = x.ProjectList;
        kw_scope.projectNameDescribe = x.Description;
        kw_scope.UpdataPG = true;
        kw_scope.categoryId = x._id;

        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/addProjectGroup.html',
            controller: addProjectGroup_ctr,
            scope: kw_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            $scope.GetProjectCategory();
        });
    };
    //克隆分享项目到本地
    $scope.CloneProject = function (id) {
        var url = "/api/Share/CloneProject?userId=" + $rootScope.userID + "&projectId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun("success", "项目克隆成功！");
                $scope.GetProjects();
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    


    //自动加载______________________________________________________________________________
    $scope.GetProjects();
    $scope.GetMyShareProjects();
    $scope.GetShareToMeProjects();
    $scope.GetProjectCategory();
});

