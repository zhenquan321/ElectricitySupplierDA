var picConfirmation_ctr = myApp.controller("picConfirmation_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, foo, myApplocalStorage) {


    $scope.minMathValue = null;
    $scope.maxMathValue = null;
    $scope.createdata = "";
    $scope.imgTopic = "";
    $scope.imgdescription = "";
    $scope.status = 1;
    $scope.page1 = 1;
    $scope.poster_name_kwd = "";
    $scope.pagesize1 = 10;
    $scope.SuspectedList = [];
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    $scope.showpicConfirmation = function () {
        $rootScope.loadingTrue = true;//加载中

        var url = "/api/image/GetImgsV2?companyID=" + $rootScope.CompanyID + "&poster_name_kwd=" + encodeURIComponent($scope.poster_name_kwd)
            + "&minMatchDegree=" + $scope.minMathValue + "&maxMatchDegree=" + $scope.maxMathValue
            + "&status=" + $scope.status + "&project_id=" + $rootScope.projectID + "&usr_id=" + null + "&poster_id=" + null
            + "&imgTopic=" + $scope.imgTopic + "&imgdescription=" + $scope.imgdescription + "&createdata=" + $scope.createdata
            + "&page=" + ($scope.page1 - 1) + "&pagesize=" + $scope.pagesize1;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('picConfirmation_ctr>showpicConfirmation');
            $scope.proCount = response.Count;

            if (response.Count >= 0) {
                $scope.picConfirmationList = response.Result;
                // $scope.loadData();
            }
            $rootScope.loadingTrue = false;//加载中

        });
        q.error(function (response) {
            alert("网络打盹了，请稍后。。。");
            $rootScope.loadingTrue = false;//加载中

        });
    }

    //kemdo

    //$scope.loadData = function () {
    //    angular.element(document).ready(function () {
    //        $("#grid").kendoGrid({
    //            dataSource: {
    //                data: $scope.picConfirmationList,
    //                schema: {
    //                    model: {
    //                        fields: {
    //                            PosterNickName: { type: "string" },
    //                            PosterCompanyName: { type: "string" },
    //                            PostUrl: { type: "string" },


    //                        }
    //                    }
    //                },
    //                pageSize: 20
    //            },
    //            groupable: true,
    //            sortable: false,
    //            pageable: {
    //                refresh: true,
    //                pageSizes: true,
    //                buttonCount: 5
    //            },
    //            columns: [
    //                       { field: "PosterNickName", title: "蓝V账号", width: "130px" },
    //                     { field: "PosterCompanyName", title: "公司名称", width: "130px" },
    //                     { field: "PostUrl", title: "博文链接", width: "130px" },
    //                        { command: "destroy", title: "操作", width: "150px" }],
    //                     //{ field: "UnitsInStock", title: "Units In Stock", width: "130px" },
    //                     //{ field: "Discontinued", width: "130px" }

    //        });
    //    })
    //};

    //导出蓝V
    $scope.ExportInfLVsInfo = function () {
        $.get('/api/Image/ExportInfLVsInfo', {
                companyID: $rootScope.CompanyID
            },
            function (data) {
                if (data != null) {
                    window.location.href = "Export/DownLoadExcel?path=" + data;
                }
            });
    }

    //导出图片
    $scope.ExportInfImgsInfo = function () {
        $.get('/api/Image/ExportInfImgsInfo', {
                //$.get('/Export/ExportInfImgsInfo', {
                companyID: $rootScope.CompanyID
            },
            function (data) {
                if (data != null) {
                    window.location.href = "Export/DownLoadExcel?path=" + data;
                }
            });
    }
    //预览
    $scope.imgViewer = function (el) {
        foo.getPrivate();
    }
    $scope.showpicConfirmation();

});