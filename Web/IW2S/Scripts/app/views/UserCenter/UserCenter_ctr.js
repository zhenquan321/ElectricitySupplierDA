var UserCenter_ctr = myApp.controller("UserCenter_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {

    $scope.show_list = 3;
    $scope.newPw = "";
    $scope.oldPw1 = '';
    $scope.newPw_q = '';
    $scope.COpage = 1;
    $scope.COpagesize = 10;
    $scope.NCOpage = 1;
    $scope.NCOpagesize = 10;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var str = location.href;
        var num = str.indexOf("?")
        str = str.substr(num + 1);
        var r = str.match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    $scope.show_list_fun = function (num) {
        if (num) {
            $scope.show_list = num;
            if (num == 1) {

            } else if (num == 5) {
                $scope.CompletedGetOrder1();
                $scope.NoCompletedGetOrder();
            }
        } else {
            $scope.show_list = 3;
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
            PictureSrc: $rootScope.uer_PictureSrc
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
                chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
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
        xiuxiu.setUploadURL('http://43.240.138.233:9999/api/File/ImgUpload');
        xiuxiu.setUploadType(2);
        xiuxiu.setUploadDataFieldName("upload_file");
        var cc = $rootScope.uer_PictureSrc;
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
            $scope.PictureSrc = data.substring(1, data.length-1);
            $rootScope.uer_PictureSrc = data.substring(1, data.length - 1);
            $cookieStore.put('uer_PictureSrc', $rootScope.uer_PictureSrc);
            $scope.changeXinxi();
        }
        xiuxiu.onDebug = function (data) {
            alert("错误响应" + data);
        }
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
            var url = "/api/Account/ChangePwd?uName=" + $rootScope.LoginName + "&newPwd=" + $scope.newPw + "&oldPwd=" + $scope.oldPw1;
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
    //用户详细信息

    $scope.GetUserInfoById = function () {
        $scope.anaItem = {
            userId: $rootScope.userID
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Account/GetUserInfoById"
        })
        .success(function (response, status) {
            $scope.GetUserInfoByIdList = response;
            console.log($scope.GetUserInfoByIdList);
        })
        .error(function (response, status) {
            // $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

 
    //付费页面_Gong
    $scope.getPayPage = function (x) {
  
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
    //获取订单信息
    //type：要查询的订单类型，0为所有，1为未完成，2为已完成

    $scope.CompletedGetOrder1 = function () {
        var url = "/api/Pay/GetOrder?userId=" + $rootScope.userID + "&page=" + ($scope.COpage - 1) + "&pagesize=" + $scope.COpagesize + "&type=" + 2;
        var q = $http.get(url);
        q.success(function (response, status) {

            $scope.CompletedGetOrderList = response.Result;
            $scope.CompletedGetOrderListCount = response.Count;
        });
        q.error(function (response) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        });
    };
    $scope.NoCompletedGetOrder = function () {
        var url = "/api/Pay/GetOrder?userId=" + $rootScope.userID + "&page=" + ($scope.NCOpage - 1) + "&pagesize=" + $scope.NCOpagesize + "&type=" + 1;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.NoCompletedGetOrderList = response.Result;
            $scope.NoCompletedGetOrderListCount = response.Count;
        });
        q.error(function (response) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        });
    };
    //删除订单
    $scope.DelOrder = function (id) {
        if(confirm("您确定要删除该订单吗")){
            var url = "/api/Pay/DelOrder?orderId=" + id;
            var q = $http.get(url);
            q.success(function (response, status) {
                $scope.alert_fun('success', '订单删除成功');
                $scope.NoCompletedGetOrder();
            });
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    }

    //自动调用__________________________________
    $scope.GetUserInfoById();
    $scope.show_list_fun(GetQueryString('tab'));
});