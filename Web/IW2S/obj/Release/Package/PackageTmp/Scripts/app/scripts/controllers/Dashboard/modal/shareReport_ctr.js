var shareReport_ctr = function ($scope, $modalInstance, $rootScope, $http) {

    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.分享项目
    $scope.SendShareEmailReport = function () {
        var pattern = /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/;
        if ($scope.usrEmails=="") {
            $scope.error = "请输入分享目标用户邮箱";
            return
        }else if ($scope.usrEmails.match(pattern) == null) {
            $scope.error = "您输入的邮箱格式不正确";
            return
        }
        if ($scope.content == "" || $scope.content ==undefined) {
            $scope.error = "您输入邀请信息";
            return
        }
        var url = "/api/Report/SendShareEmail?usr_id=" + $rootScope.userID + '&reportId=' + $scope.shareReportId + '&usrEmails=' + $scope.usrEmails + '&content=' + $scope.content;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
            
                $scope.usrEmails = "";
                $scope.content = "";
                $scope.ok();
            } else {
                $scope.error=response.Message;
            }
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
















}