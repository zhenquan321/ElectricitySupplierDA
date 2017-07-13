var UserCenter_ctr = myApp.controller("UserCenter_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore) {

    $scope.show_list = 1;
    $scope.newPw = "";
    $scope.oldPw1 = '';
    $scope.newPw_q = '';
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    $scope.show_list_fun = function (num) {
        $scope.show_list = num;
        if (num = 1) {

        }
    }
    //修改 用户信息
    var pattern = /\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}/;
    $scope.changeXinxi = function () {
       
        if (!$rootScope.Gender) {
            $scope.alert_fun('warning', '昵称不能为空！')
            return;
        }
        if (!$rootScope.MobileNo) {
            $scope.alert_fun('warning', '手机号不能为空！')
            return;
        }
        if (!$rootScope.UsrEmail) {
            $scope.alert_fun('warning', '邮箱不能为空！')
            return;
        }
        //if ($rootScope.UsrEmail) {
        //    if (!pattern.test($rootScope.Email)) {
        //        $scope.alert_fun('warning', '您输入的邮箱格式不正确！')
        //        return;
        //    }
        //}
        if (!$rootScope.Remarks) {
            $scope.alert_fun('warning', '公司信息不能为空！')
            return;
        }
        $scope.aa = {
            LoginName:$rootScope.LoginName,
            Gender:$rootScope.Gender,//昵称
            MobileNo:$rootScope.MobileNo,
            Remarks:$rootScope.Remarks,//公司
            UsrEmail: $rootScope.UsrEmail,
        };
        var urls = "/api/Account/UpdateUser?uName=" + $rootScope.LoginName;
        var q = $http.post(
                urls,
               JSON.stringify($scope.aa),
               {
                   headers: {
                       'Content-Type': 'application/json'
                   }
               }
            )
        q.success(function (response, status) {
            if (response.Gender == $rootScope.Gender) {
                $scope.alert_fun('success', '信息修改成功！')
                $scope.no_changePW();
                $cookieStore.put("Gender", $rootScope.Gender);
                $cookieStore.put("MobileNo", $rootScope.MobileNo);
                $cookieStore.put("Remarks", $rootScope.Remarks);
                $cookieStore.put("UsrEmail", $rootScope.UsrEmail);
                chk_global_vars($cookieStore, $rootScope, null, $location, $http);
            } else {
                $scope.alert_fun('danger', response)
            }

        })
        q.error(function (response, status) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        })
    }
    $scope.no_changeXinxi = function () {
    }
    //上传头像
    $scope.editImage = function () {
        xiuxiu.remove("userpic");
        $('#flashEditorOut').html("<div id='altContent'></div>");
        xiuxiu.embedSWF("altContent", 5, "100%", "100%", "userpic");
        xiuxiu.setUploadURL('http://' + window.location.host + '/post.ashx');
        xiuxiu.setUploadType(2);
        xiuxiu.setUploadDataFieldName("upload_file");
        var cc = 'http://' + window.location.host + $rootScope.uer_PictureSrc;
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
            $scope.PictureSrc = data;
            $scope.getpasspic();
        }
        xiuxiu.onDebug = function (data) {
            alert("错误响应" + data);
        }
    }
    $scope.getpasspic = function () {
        var url = "api/Account/SaveUserHeadIcon?id=" + $rootScope.userID + '&url=' + $scope.PictureSrc + "&baseurl=" + 'http://' + window.location.host;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess) {
                $rootScope.uer_PictureSrc = response.Message;
                $cookieStore.put('uer_PictureSrc', $rootScope.uer_PictureSrc);
                $scope.changeXinxi();
            } else {
                $scope.alert_fun('worning', '图片上传存在错误！');
            }
        }
        )
        q.error(function (err) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        });
    }

    //自动调用


   $scope.editImage();
    //修修改密码
    $scope.changepwd = function () {
        if ($scope.oldPw1 == "") {
            $scope.error = "请输入原密码";
            return;
        }
        if ($scope.newPw == "") {
            $scope.error = "请输入新密码";
            return;
        }
        if ($scope.newPw_q != $scope.newPw) {
            $scope.error = "两次输入密码不相同";
            return
        }
        else {
            var url = "/api/Account/ChangePwd?uName=" + $rootScope.LoginName + "&oldPwd=" + $scope.oldPw1 + "&newPwd=" + $scope.newPw;
            var q = $http.get(url);
            q.success(function (response, status) {
                console.log(response);
                console.log('signup_ctr>Regist');
                if (response.Error != null) {
                    $scope.error = response.Error
                    ;
                } else {
                    $scope.changeSucc = true;
                    $scope.alert_fun('success', '密码修改成功！');
                    $scope.error = '';
                }
            });
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    }
    $scope.no_changePW = function () {
        $scope.error = '';
        $scope.oldPw = '';
        $scope.newPw = '';
        $scope.newPw_q = '';
    }

    //alert
    //页面添加alert
    $rootScope.addAlert = function (status, messages) {
        var len = $rootScope.alerts.length + 1;
        $rootScope.alerts = [];
        $rootScope.alerts.push({ type: status, msg: messages });
    }
    //关闭alert
    $scope.closeAlert = function (index) {
        $rootScope.alerts.splice(index, 1)
    }
    //自动调用



});