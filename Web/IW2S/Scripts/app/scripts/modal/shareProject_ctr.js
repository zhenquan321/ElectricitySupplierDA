var shareProject_ctr = function ($scope, $modalInstance, $rootScope, $http) {

    $scope.content = "";
    //______________________________________________________________________
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //1.分享项目
    $scope.SendShareEmail = function () {
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
        var url = "/api/Share/SendShareEmail?prjId=" + $scope.projectId + "&usr_id=" + $rootScope.userID + "&usrEmails=" + $scope.usrEmails + "&content=" + $scope.content;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.massage = response.Message;
            $scope.error = $scope.massage;
            $scope.ok();
            $rootScope.addAlert('success', "项目分享成功！");
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };
















}