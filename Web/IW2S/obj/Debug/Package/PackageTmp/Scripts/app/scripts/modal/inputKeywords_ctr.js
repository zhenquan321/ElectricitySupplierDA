var inputKeywords_ctr = function ($scope, $modalInstance, $rootScope, $http) {

    $scope.isactive = "";
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


    $scope.excelupload = function () {
        //创建FormData对象
        var data = new FormData();
        //为FormData对象添加数据
        $.each($('#fileField')[0].files, function (i, file) {
            data.append('upload_file' + i, file);
        });
        //发送数据
        $.ajax({
            url: 'Export/UpLoadFile',
            type: 'POST',
            data: data,
            cache: false,
            contentType: false,        //不可缺参数
            processData: false,        //不可缺参数
            success: function (data) {
                if (data != null && data != undefined) {
                    console.log(data);
                    $scope.singleExlUpload = data;
                    $scope.BatchImportOffImgs();
                }
            },
            error: function () {
                alert('data');
            }
        });
    };

    $scope.excelFilePath = "";
    $scope.BatchImportOffImgs = function () {
        //  $scope.excelupload();
        var excelFilePath = $("#fileField").val();
        if (excelFilePath == undefined || excelFilePath == "") {
            return;
        } else {
            var modale = $rootScope.isActiveModale;
            var baseUrl = "";
            switch (modale) {
                case "baidu":
                    baseUrl = "/api/Keyword/ImportKeywordGroup";
                    break;
                case "wechat":
                    baseUrl = "/api/Media/ImportKeywordGroup";
                    break;
                case "bing":
                    baseUrl = "/api/Bing/ImportKeywordGroup";
                    break;
                case "weibo":
                    baseUrl = "/api/Weibo/ImportKeywordGroup";
                    break;
                case "sougo_new":
                    baseUrl = "/api/Sogou/ImportKeywordGroup";
                    break;
                case "googol":
                    baseUrl = "/api/Google/ImportKeywordGroup";
                    break;
                default:
                    break;
            }
            var url = baseUrl + "?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId
                + "&excelFilePath=" + encodeURIComponent($scope.singleExlUpload);
            var p = $http.get(url);
            p.success(function (response, status) {
                console.log('picManage_ctr>BatchImportOffImgs');
                if (response == "导入成功") {
                    $("#fileField").val("");
                    $scope.cancel();
                    $rootScope.GetBaiduSearchKeyword2();
                    $rootScope.addAlert('success', "词组关系导入成功！");
                } else {
                    alert(response);
                }
            });
            p.error(function (e) {
                $scope.error = "网络打盹了，请稍后。。。";

            });
        }
    }

}