var SelectKws_ctr = function ($scope, $modalInstance, $rootScope, $http) {
    $scope.keywords = '';
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
   var selectedKws = [];
   
    $scope.addKeywords = function (x) {
        if (selectedKws) {
            var addCould = true;
            for (var i = 0; i < selectedKws.length;i++) {
                if (x == selectedKws[i]) {
                    var addCould = false;
                }
            }
            if (addCould) {
                selectedKws.push(x);
            }
        } else {
            selectedKws.push(x);
        }
        $scope.selkeywordsL = selectedKws;
    }
    $scope.delKeywords = function (x) {
        for (var i = 0; i < selectedKws.length; i++) {
            if (x == selectedKws[i]) {
                selectedKws.splice(i, 1)
            }
        }
        $scope.selkeywordsL = selectedKws;
    }

    //1.3添加关键词
    //$scope.keywordsSearch = function () {
      
    //    var url = "/api/Keyword/insertKeyword?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&keywords=" + $scope.keywords + "&isCommend=true";
    //        var q = $http.get(url);
    //        q.success(function (response, status) {
    //            console.log('iw2s_ctr>keywordsSearch');
    //                $scope.keywords = ''
    //                selectedKws = [];
    //                $scope.ok();
    //        });
    //        q.error(function (response) {
    //            $scope.error = "网络打盹了，请稍后。。。";
    //        });
     
    //}


    $scope.keywordsSearch = function () {
       
            for (var i = 0; i < selectedKws.length; i++) {
                $scope.keywords = $scope.keywords + selectedKws[i] + ';'
            }
            $scope.paramsList = {
                user_id: $rootScope.userID,
                projectId: $rootScope.getProjectId,
                keywords: $scope.keywords,
                isCommend: true,
                cateId: ''
            };
            var urls = "/api/Keyword/insertKeyword";
            var q = $http.post(
                    urls,
                   JSON.stringify($scope.paramsList),
                   {
                       headers: {
                           'Content-Type': 'application/json'
                       }
                   }
                )
            q.success(function (response, status) {
                //console.log(response);
                if (response.IsSuccess == true) {
                    $scope.keywords = ''
                    selectedKws = [];
                    $scope.ok();
                    //$rootScope.addAlert('success', '添加成功');
                    $scope.alert_fun('success', '添加成功！');
                } else {
                    $scope.alert_fun('danger', response.Message);
                }
            });
            q.error(function (e) {
                alert("网络打盹了，请稍后。。。");
            });
    }

    //全部添加
    $scope.selAllKws = function () {
        $scope.selkeywordsL = $scope.TJKws;
        selectedKws = $scope.selkeywordsL;
    }

    //全部取消

    $scope.delAllKws = function () {
        $scope.selkeywordsL = [];
        selectedKws = [];
    }








}