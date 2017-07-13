var addPicture_ctr = function ($scope, $modalInstance, $rootScope, $http,$timeout) {


    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //上传头像
    $scope.editImage = function () {
        xiuxiu.remove("userpic");
        $('#flashEditorOut').html("<div id='altContent'></div>");
        xiuxiu.embedSWF("altContent", 1, "100%", "100%", "userpic");
        xiuxiu.setUploadURL('http://' + window.location.host + '/post.ashx');
        xiuxiu.setUploadType(2);
        xiuxiu.setUploadDataFieldName("upload_file");
        var cc = 'http://' + window.location.host;
        xiuxiu.onInit = function () {
            xiuxiu.loadPhoto(cc, false);
        }
        xiuxiu.onBeforeUpload = function (data, id) {
            var size = data.size;
            if (size > 2 * 1024 * 1024) {
                alert("图片不能超过2M");
                return false;
            }
            return true;
        }
        xiuxiu.onUploadResponse = function (data) {
            $rootScope.PictureSrcReport = data;
            $scope.cancel();
        }
        xiuxiu.onDebug = function (data) {
            alert("错误响应" + data);
        }
    }
    

    //自动调用

    $timeout(function () { $scope.editImage(); },100)


}