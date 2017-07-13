var picSuspected_ctr = myApp.controller("picSuspected_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, foo) {



    //1.加载侵权图片
    /// <param name="companyID">账号所属公司id</param>
    /// <param name="poster_name_kwd">博主昵称或公司名关键词</param>
    /// <param name="minMatchDegree">最小匹配度</param>
    /// <param name="maxMatchDegree">最大匹配度</param>
    /// <param name="status">状态，0是嫌疑，1是侵权，2是白名单，3.是被删除 4.发邮件催买,5.排除，6.高度疑似</param>
    /// <param name="project_id">所属项目id</param>
    /// <param name="usr_id">如果是个体户，个体户的id</param>
    /// <param name="poster_id">博主id</param>
    /// <param name="page"></param>
    /// <param name="pagesize"></param>
    //$scope.poster_name_kwd = "";
    //$scope.minMatchDegree = 0;
    //$scope.maxMatchDegree = 100;
    //$scope.imgTopic = "";
    //$scope.imgdescription = "";
    //$scope.createdata = "";
    //$scope.SuspectedList = [];
    //$scope.foo = foo;
    //$scope.status = 0;
    //$scope.poster_id = "";
    //$scope.page = 1;
    //$scope.pagesize = 10;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);


    //$scope.GetImgsV2 = function () {
    //    $.get('/api/image/GetImgsV2', {
    //        companyID: $rootScope.CompanyID,
    //        poster_name_kwd: $scope.poster_name_kwd,
    //        imgTopic: $scope.imgTopic,
    //        imgdescription: $scope.imgdescription,
    //        createdata: $scope.createdata,
    //        minMatchDegree: $scope.minMatchDegree,
    //        maxMatchDegree: $scope.maxMatchDegree,
    //        status: $scope.status,
    //        project_id: $rootScope.projectID,
    //        usr_id: null,
    //        poster_id: null,
    //        page: $scope.page,
    //        pagesize: $scope.pagesize
    //    },
    //              function (data, status) {
    //                  if (status == "success") {
    //                      if (data.Count > 0) {
    //                          $scope.SuspectedList = data.Result;
    //                      } else {
    //                          alert("没有搜索到结果！")
    //                      }
    //                  } else {
    //                      $scope.error = "服务器连接出错";
    //                  }

    //              }
    //)}

    //$scope.GetImgsV2 = function () {
    //    var url = "/api/image/GetImgsV2"
    //        + "?companyID=" + $rootScope.CompanyID + "&projectID=" + $rootScope.projectID + "&poster_name_kwd=" + $scope.poster_name_kwd
    //        + "&imgTopic=" + $scope.imgTopic + "&imgdescription=" + $scope.imgdescription + "&createdata=" + $scope.createdata + "&minMatchDegree=" + $scope.minMatchDegree
    //    + "&maxMatchDegree=" + $scope.maxMatchDegree + "&status=" + $scope.status + "&usr_id=" + null + "&poster_id=" + null + "&page=" + $scope.page + "&pagesize=" + $scope.pagesize;
    //    var p = $http.get(url);
    //    p.success(function (response, status) {
    //        if (status == "success") {
    //            if (data.Count > 0) {
    //                $scope.SuspectedList = data.Result;
    //            }
    //        } else {
    //            $scope.error = "服务器连接出错";
    //            $scope.loadImgs();
    //        }
    //    });
    //    p.error(function (e) {
    //        $scope.error = "服务器连接出错";

    //    });
    //}

    //2.全选
    $scope.ischecked4 = "全选";
    $scope.AllChecked = function () {
        if ($scope.ischecked4 == "全选") {
            $scope.ischecked4 = "取消";
            for (var i in $scope.SuspectedList) {
                $scope.SuspectedList[i].IsSelected = true;
            }
        } else {
            $scope.ischecked4 = "全选";
            for (var i in $scope.SuspectedList) {
                $scope.SuspectedList[i].IsSelected = false;
            }
        }
        $scope.IsChecked4();
    };
    //3.单选
    $scope.IsChecked4 = function () {
        $scope.ids4 = "";
        var ids4 = "";
        for (var i = 0; i < $scope.SuspectedList.length; i++) {
            var p = $scope.SuspectedList[i];
            if (p.IsSelected == true) {
                ids4 = ids4 + p.ID + ";";
                $scope.ids4 = ids4;
            }
        }
    };
    //4.设为侵权
    $scope.setIsQinqian = function () {
        var checkedimgId = "";
        for (var i in $scope.SuspectedList) {
            if ($scope.SuspectedList[i].IsSelected) {
                checkedimgId += $scope.SuspectedList[i].ID + ";";
            }
        }
        ;
        var url = "/api/image/SetImgStatus?companyID=" + $rootScope.CompanyID + "&ids=" + $scope.ids4 + "&status=" + 1;
        var p = $http.get(url);
        p.success(
            function (response) {
                console.log('picSuspected_ctr>setIsQinqian');
                if (response != null && response > 0) {
                    alert("成功设置侵权！");
                    $scope.showLV();
                } else {
                    alert("设置超时！");
                }
            });
        p.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    };
    //5.排除侵权
    $scope.Exclued = function () {
        var checkedimgId = "";
        for (var i in $scope.SuspectedList) {
            if ($scope.SuspectedList[i].IsSelected) {
                checkedimgId += $scope.SuspectedList[i].ID + ";";
            }
        }
        ;
        $.get('/api/image/SetImgStatus', {
                companyId: $rootScope.CompanyID,
                ids: checkedimgId,
                status: 5
            },
            function (rows) {
                if (rows > 0) {
                    alert("成功排除！");
                    $scope.showLV();
                } else {
                    alert("设置超时！");
                }
            });
    }


    //6.以蓝v加载图片
    $scope.imgViewer = function (el) {
        foo.getPrivate();
    }

    $rootScope.poster_name_kwd = "";
    $scope.minMathValue = 0;
    $scope.maxMathValue = 1;
    $scope.createdata = "";
    $scope.imgTopic = "";
    $scope.imgdescription = "";
    $scope.status = 0;
    $scope.page1 = 1;
    $scope.pagesize1 = 20;
    $scope.SuspectedList = [];
    $scope.proCount = 0;
    $scope.showLV = function () {
        $rootScope.loadingTrue = true;//加载中

        var url = "/api/image/GetImgsV2?companyID=" + $rootScope.CompanyID + "&poster_name_kwd=" + encodeURIComponent($rootScope.poster_name_kwd)
            + "&minMatchDegree=" + $scope.minMathValue + "&maxMatchDegree=" + $scope.maxMathValue
            + "&status=" + $scope.status + "&project_id=" + $rootScope.projectID + "&usr_id=" + null + "&poster_id=" + null
            + "&imgTopic=" + $scope.imgTopic + "&imgdescription=" + $scope.imgdescription + "&createdata=" + $scope.createdata
            + "&page=" + ($scope.page1 - 1) + "&pagesize=" + $scope.pagesize1;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('picSuspected_ctr>showLV');
            $scope.proCount = response.Count;

            if (response.Count >= 0) {
                $scope.SuspectedList = response.Result;
            }
            $rootScope.loadingTrue = false;//加载中

        });
        q.error(function (response) {
            alert("服务器连接出错");
            $rootScope.loadingTrue = false;//加载中

        });
    }
    //7.相似度切换
    $scope.changeBFB = function (min, max) {
        $scope.minMathValue = min;
        $scope.maxMathValue = max;
        $scope.showLV();
    }


    //_________________________________________


    //  $scope.GetImgsV2();
    //$scope.SuspectedList2 = foo.suspectList;

    $scope.showLV();

});