var Published_Articles_ctr = myApp.controller("Published_Articles_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {
        $scope.showAmendAndOff = false;
        $scope.ed_show = false;
        $scope.list_show = true;
        $scope.wen_xq = false;
        $scope.dainzan = false;
        $scope.Love = 0;
        $scope.userIds = "";
        $scope.biaoqian = "";
        $scope.editer_nei = "";
        $scope.editer_bian = "";
        $scope.editer = $scope;
        $scope.Title = "";
        $scope.Label = "";
        $scope.titlelabel = "";
        $scope.page = 1;
        $scope.pagesize = 20;
        $scope.page1 = 1;
        $scope.pagesize1 = 20;
        $scope.page_11 = 1;
        $scope.pagesize_11 =10;
        $scope.if_iprseeU = false;
        $scope.if_OMDU = false;
        $scope.if_TestbuyU = false;
        $scope.if_ConsoleU = false;
        $scope.fabu = true;
        $scope.allfabu = false;
        $scope.yaoqing = true;
        $scope.userName = true;
        $scope.page_13 = 1;
        $scope.pagesize_13 = 200;
        $scope.selected_name = [];
        $scope.user_list_s = false;
        $scope.aaa = true;
        chk_global_vars($cookieStore, $rootScope, null, $location, $http);
        //________________________________________________________________
        console.log($rootScope.userID)
        $scope.showamendpwd = function () {
            $scope.showAmendAndOff = true;
        }
        $scope.hideamendpwd = function () {
            $scope.showAmendAndOff = false;
        }
        //切换到编辑
        $scope.ed_show_fun = function () {
            document.getElementById('customized-buttonpane').innerHTML = '';
            $scope.ed_show = true;
            $scope.list_show = false;
            $scope.wen_xq = false;
            $scope.GetMyShare();
            $scope.GetShare();
            $scope.GetSharetoMe();
            //$timeout(function () {
            //       $scope.GetAllUser();
            //    }, 1000)
        }
        //切换到list
        $scope.list_show_fun = function () {
            $scope.ed_show = false;
            $scope.list_show = true;
            $scope.wen_xq = false;
            $scope.GetShare();
            $scope.GetMyShare();
            $scope.GetSharetoMe();
        }
        //切换到文章
        $scope.wen_xq_fun = function () {
            $scope.ed_show = false;
            $scope.list_show = false;
            $scope.wen_xq = true;

        }

    //获取点赞的状态
        $scope.dianzanZT = function (x) {
            var url = "api/Share/LoveState?shareId=" + $scope.wenzhang.ID + "&Replyer=" + $rootScope.userID;
            var q = $http.get(url);
            q.success(function (response, status) {
                if (response.Love == 1) {
                    $scope.dainzan = true;
                }
                else {
                    $scope.dainzan = false;
                }
            });
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });

        }

    //点赞
        $scope.dianzan_fun = function () {
            if ($scope.dainzan = !$scope.dainzan) {
                $scope.Love = 1;
            }
            else {
                $scope.Love = 0;
            }
            $scope.paramsList = {
                C_Love: $scope.Love,
                ShareId: $scope.wenzhang.ID,
                Replyer: $rootScope.userID,
            };
            var urls = "/api/Share/LoveShareReply";
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
                if (response.IsSuccess == true) {
                    $scope.GetLoveCount($scope.wenzhang);
                }
            });
            q.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }

    //评论添加图片
        $scope.addpicture = function () {
            $scope.editer_nei = $scope.editer_nei + "<img src='请在这里输入图片地址' alt=''/>"
        }

    //添加标签
    //发布
        $scope.fabu_fun = function () {
            $scope.editer_bian = $("textarea[name='customized-buttonpane']").val()
            $scope.paramsList = {
                Title: $scope.Title,
                Description: $scope.editer_bian,
                Sender: $rootScope.userID,
                Label: $scope.Label,
            };
            var urls = "/api/Share/SaveShare";
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
                console.log(response);
                if (response.IsSuccess == true) {
                    $scope.Title = "";
                    $scope.Label = "";
                    $("textarea[name='customized-buttonpane']").val('');
                    $scope.shareId_f = response.Message;
                    $scope.ShareToUsers();
                    $scope.alert_fun('success', '发布成功！');
                    //$scope.GetCreatedAtCount();
                    $scope.GetShareReply2($scope.shareId_f);
                    $scope.GetShare();
                    $scope.GetMyShare();
                } else {
                    publicFunc.showAlert("温馨提示", '哎呀，网络打盹了，请重试一下！', "我知道了");
                    $scope.alert_fun('danger', response.Message);
                }
            });
            q.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    //获取所有
        $scope.GetShare = function () {
            var url = "/api/Share/GetShare?usrId=" + "" + "&titlelabel=" + "" + "&page=" + ($scope.page_11 - 1) + "&pagesize=" + $scope.pagesize_11 + "&type=" + 3;
            var p = $http.get(url);
            p.success(function (response, status) {
                console.log(response)
                $scope.GetShare_list = response.Result;
                for (var a = 0; a < $scope.GetShare_list.length; a++) {
                    var table_c = $scope.GetShare_list[a].Label.replace(/；/g, ';');
                    table_c = table_c.split(";");
                    for (var i = 0 ; i < table_c.length; i++) {
                        if (table_c[i] == "" || typeof (table_c[i]) == "undefined") {
                            table_c.splice(i, 1);
                            i = i - 1;
                        }
                    }
                    $scope.GetShare_list[a].LabelList = table_c;
                }
                $scope.GetShare_count = response.Count;
                $scope.GetShare2();
                //$scope.GetCreatedAtCount();
            });
            p.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
        $scope.GetShare_list2 = [];
        $scope.GetShare2 = function () {
            if ($scope.GetShare_list.length > 6) {
                for (var i = 0; i < 6; i++) {
                    $scope.GetShare_list2[i] = $scope.GetShare_list[i];
                }
            } else {
                $scope.GetShare_list2 = $scope.GetShare_list;
            }
        }
    //获取我分享的
        $scope.GetMyShare = function () {
            $scope.anaItem = {
                usrId: $rootScope.userID,
                titlelabel: $scope.titlelabel,
                page: ($scope.page - 1),
                pagesize: 6,
                type: 1,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/GetShare"
            })
            .success(function (response, status) {
                console.log(response);
                $scope.GetMyShareList = response.Result;
            })
            .error(function (response, status) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }


    //获取分享给我的
        $scope.GetSharetoMe = function () {
            $scope.anaItem = {
                usrId: $rootScope.userID,
                titlelabel: $scope.titlelabel,
                page: ($scope.page - 1),
                pagesize: $scope.pagesize_11,
                type: 2,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/GetShare"
            })
            .success(function (response, status) {
                console.log(response)
                //$scope.GetCreatedAtCount();
                $scope.getMyShareToCount = response.Count;
                $scope.GetSharetoMeList = response.Result;

            })
            .error(function (response, status) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }

    //搜索发布信息
        $scope.searchGetShare = function () {
            var url = "/api/Share/GetShare?usrId=" + "" + "&titlelabel=" + $scope.titlelabel + "&page=" + ($scope.page_11 - 1) + "&pagesize=" + $scope.pagesize_11 + "&type=" + 3;
            var p = $http.get(url);
            p.success(function (response, status) {
                console.log(response)
                $scope.GetShare_list = response.Result;
                for (var a = 0; a < $scope.GetShare_list.length; a++) {
                    var table_c = $scope.GetShare_list[a].Label.replace(/；/g, ';');
                    table_c = table_c.split(";");
                    for (var i = 0 ; i < table_c.length; i++) {
                        if (table_c[i] == "" || typeof (table_c[i]) == "undefined") {
                            table_c.splice(i, 1);
                            i = i - 1;
                        }
                    }
                    $scope.GetShare_list[a].LabelList = table_c;
                }
                $scope.GetShare_count = response.Count;
                $scope.GetShare2();
                //$scope.GetCreatedAtCount();
            });
            p.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    //删除发布信息
        $scope.DelShare = function (id) {
            if (confirm("您确定要删除此贴么？")) {
                $scope.anaItem = {
                    id: id,
                    isDel: true,
                };
                $http({
                    method: 'get',
                    params: $scope.anaItem,
                    url: "/api/Share/DelShare"
                })
                    .success(function (response, status) {
                        console.log(response)
                        $scope.alert_fun('success', '讨论帖删除成功');
                        //$scope.GetAllUser();
                        //$scope.GetCreatedAtCount();
                        $scope.GetShare();
                        $scope.GetSharetoMe();
                        $scope.GetMyShare();
                    })
                    .error(function (response, status) {
                        $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
                    });
            }
        }



    //邀请用户
        $scope.ShareToUsers = function () {
            $scope.anaItem = {
                shareId: $scope.shareId_f,
                userIds: $scope.userIds,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/ShareToUsers"
            })
            .success(function (response, status) {
                console.log(response);
                $scope.userIds = "";
                $scope.selected_name = [];
            })
            .error(function (response, status) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }

    /// 设置邀请用户信息已读
        $scope.SetShareUserRead = function (x) {
            $scope.anaItem = {
                id: x,
                usrId: $rootScope.userID,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/SetShareUserRead"
            })
            .success(function (response, status) {
                console.log(response);
                $scope.GetSharetoMe();
            })
            .error(function (response, status) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }

    //获取详情
        $scope.GetShareReply2 = function (x) {
            var url = "/api/Share/GetShareDetails?id=" + x;
            var q = $http.get(url);
            q.success(function (response, status) {
                // $scope.SetShareUserRead(x);
                $scope.wenzhang = response.Result[0];
                var table_c = $scope.wenzhang.Label.replace(/；/g, ';');
                table_c = table_c.split(";");
                for (var i = 0 ; i < table_c.length; i++) {
                    if (table_c[i] == "" || typeof (table_c[i]) == "undefined") {
                        table_c.splice(i, 1);
                        i = i - 1;
                    }
                }
                $scope.wenzhang.LabelList = table_c;
                $scope.wen_xq_fun();
                console.log($scope.wenzhang);
                $scope.GetShareReply_fun($scope.wenzhang);

            })
            q.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        };

        $scope.GetShareReply = function (x) {
            console.log(x)
            // x调的结果赋值
            var url = "/api/Share/GetShareDetails?id=" + x.ID;

            var q = $http.get(url);
            q.success(function (response, status) {
                //$scope.SetShareUserRead(x.ID);
                $scope.wenzhang = response.Result[0];
                var table_c = $scope.wenzhang.Label.replace(/；/g, ';');
                table_c = table_c.split(";");
                for (var i = 0 ; i < table_c.length; i++) {
                    if (table_c[i] == "" || typeof (table_c[i]) == "undefined") {
                        table_c.splice(i, 1);
                        i = i - 1;
                    }
                }
                $scope.wenzhang.LabelList = table_c;
                $scope.wen_xq_fun();
                console.log($scope.wenzhang);
                $scope.dianzanZT();
                $scope.GetShareReply_fun($scope.wenzhang);
                $scope.GetLoveCount($scope.wenzhang);
            })
            q.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    //获取分享的评论
        $scope.GetShareReply_fun = function (x) {
            $scope.anaItem = {
                shareId: x.ID,
                page: ($scope.page1 - 1),
                pagesize: $scope.pagesize1,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/GetShareReply"
            })
            .success(function (response, status) {
                console.log(response)
                $scope.GetLoveCount($scope.wenzhang);
                if (!response.Result) {
                    return
                }
                for (var i = 0; i < response.Result.length; i++) {
                    response.Result[i].Description = AnalyticEmotion(response.Result[i].Description)
                }
                $scope.GetShareReply_fun_list = response.Result;
                console.log($scope.GetShareReply_fun_list);
            })
            .error(function (response, status) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }

    /// 保存分享的评论
        $scope.SaveShareReply = function () {
            $scope.Description_ly = $('.text').val();
            if ($scope.Description_ly == "") {
                $scope.alert_fun('danger', '提交的评论不能为空哦！');
            }
            else {
                $scope.paramsList = {
                    ShareId: $scope.wenzhang.ID,
                    Replyer: $rootScope.userID,
                    //Love:$scope.Love,
                    Description: $scope.Description_ly,
                };
                var urls = "/api/Share/SaveShareReply";
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
                    if (response.IsSuccess == true) {
                        $scope.editer_nei = "";
                        $scope.GetShareReply_fun($scope.wenzhang);
                        $scope.GetLoveCount($scope.wenzhang);
                        $('.text').val('');
                    }
                    console.log(response);
                });
                q.error(function (e) {
                    $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
                });
            }

        }

    /// 删除分享的评论
        $scope.delShareReply = function (x) {
            if (confirm("您确定要删除此评论么？")) {
                $scope.Description_ly = $('.text').val();
                $scope.paramsList = {
                    Id: x.ID,
                    ShareId: x.ShareId,
                    Replyer: x.Replyer,
                    Description: x.Description,
                    IsDel: true,
                };
                var urls = "/api/Share/SaveShareReply";
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
                    if (response.IsSuccess == true) {
                        $scope.GetShareReply_fun($scope.wenzhang);
                        $scope.alert_fun('success', '评论删除成功！');
                        $scope.GetLoveCount();
                    }
                    console.log(response);
                });
                q.error(function (e) {
                    $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
                });
            }
        }
    //获取所有用户信息------------------------------------------------------------------------------------------------------
        $scope.GetAllUser = function () {
            var url = "/api/Share/GetAllIdName";
            var q = $http.get(url);
            q.success(function (response, status) {
                $scope.GetAllUserInfoList1 = response[0].Names;
                $scope.GetAllUserInfoList2 = response[1].Names;
                $scope.GetAllUserInfoList3 = response[2].Names;
                $scope.GetAllUserInfoList4 = response[3].Names;
                $scope.GetAllUserInfoList1Ids = [];
                for (var k = 0; k < $scope.GetAllUserInfoList1.length; k++) {
                    $scope.GetAllUserInfoList1Ids.push($scope.GetAllUserInfoList1[k].ID);
                }
                $scope.count_usrInfo = response.length;
                //$scope.getAll = [];
                $scope.getAl3 = [];

                for (var i = 0; i < response.length; i++) {
                    $scope.getAll2 = [];
                    $scope.getAll2 = response[i].Names;
                    for (var j = 0; j < $scope.getAll2.length; j++) {
                        $scope.getAl3.push($scope.getAll2[j].ID);
                    }
                }
                for (var k = 0; k < $scope.getAl3.length; k++) {
                    $scope.userIds = $scope.getAl3.join(';');
                }


                $scope.getAl4 = [];
                for (var i = 1; i < response.length; i++) {
                    $scope.getAll5 = [];
                    $scope.getAll5 = response[i].Names;
                    for (var j = 0; j < $scope.getAll5.length; j++) {
                        $scope.getAl4.push($scope.getAll5[j].ID);
                    }
                }
                for (var k = 0; k < $scope.getAl4.length; k++) {
                    $scope.userIdsl = $scope.getAl4.join(';');
                }

                console.log(response);
                //console.log($scope.getAl3);
                console.log($scope.userIds);
                console.log($scope.userIdsl);

            });
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    //
    //用户列表
        $scope.user_list_show = function () {
            $scope.user_list_s = !$scope.user_list_s;
        }

        $scope.Usershow = function (num) {
            if (num == 1) {
                $scope.if_iprseeU = !$scope.if_iprseeU;
            } else if (num == 2) {
                $scope.if_OMDU = !$scope.if_OMDU;
            } else if (num == 3) {
                $scope.if_TestbuyU = !$scope.if_TestbuyU;
            } else if (num == 4) {
                $scope.if_ConsoleU = !$scope.if_ConsoleU;
            }
        }


    //选择单个id

        $scope.xuanz_id = function (x) {

            if ($scope.selected_name.length > 0) {
                if ($scope.selected_name[0].Name == "所有用户") {
                    $scope.selected_name.splice(0, 1);
                    $scope.pin_id();
                }
                for (var i = 0; i < $scope.selected_name.length; i++) {
                    if ($scope.selected_name[i].Name == "所有内部用户") {
                        if ($scope.getAl4.indexOf(x.ID) != -1) {
                            $scope.selected_name.splice(i, 1);
                            $scope.selected_name.push(x);
                        }
                        $scope.pin_id();
                    }
                }
            }
            //$scope.selected_name.push(x);
            var dd = [];
            var aa = 0;
            for (var c = 0; c < $scope.selected_name.length; c++) {
                if ($scope.selected_name[c]) {
                    dd[aa] = $scope.selected_name[c];
                    aa++;
                }
            }
            $scope.selected_name = dd;

            $scope.aaa = true;
            for (var j = 0; j < $scope.selected_name.length; j++) {
                if ($scope.selected_name[j] == x) {
                    $scope.aaa = false;
                }
            }
            if ($scope.aaa) {
                $scope.selected_name.push(x);
                $scope.pin_id();
            }
        }
        $scope.allxuanze = function () {
            $scope.userIds = "";
            $scope.fabu = false;
            $scope.Noallfabu = false;
            $scope.allfabu = true;
            $scope.if_iprseeU = false;
            $scope.if_OMDU = false;
            $scope.if_TestbuyU = false;
            $scope.if_ConsoleU = false;
            $scope.aaa = true;
            $scope.userIds = $scope.getAl3.join(';');
            $scope.selected_name = [];
            $scope.selected_name.push({ Name: "所有用户", ID: $scope.getAl3.join(';') })
            $scope.pin_id();
        }


        $scope.noallxuanze = function () {
            $scope.fabu = false;
            $scope.allfabu = false;
            $scope.Noallfabu = true;
            $scope.if_iprseeU = false;
            $scope.if_OMDU = false;
            $scope.if_TestbuyU = false;
            $scope.if_ConsoleU = false;

            for (var i = 0; i < $scope.selected_name.length; i++) {
                if ($scope.GetAllUserInfoList1Ids.indexOf($scope.selected_name[i].ID) == -1) {
                    $scope.selected_name.splice(i, 1);
                    i--;
                }
            }
            $scope.selected_name.push({ Name: "所有内部用户", ID: $scope.userIdsl })

            $scope.pin_id();
            ////$scope.aaa = true;
            //$scope.userIds = $scope.userIdsl;

        }
        //选择所有邀请人

        $scope.allfabu_fun = function () {

            $scope.editer_bian = $("textarea[name='customized-buttonpane']").val()
            $scope.paramsList = {
                Title: $scope.Title,
                Description: $scope.editer_bian,
                Sender: $rootScope.userID,
                Label: $scope.Label,
            };
            var urls = "/api/Share/AllSaveShare?usrId=" + $scope.userIds;
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
                console.log(response);
                if (response.IsSuccess == true) {
                    $scope.Title = "";
                    $scope.Label = "";
                    $("textarea[name='customized-buttonpane']").val('');
                    $scope.GetMyShare();
                    $scope.shareId_f = response.Message;
                    $scope.ShareToUsers();
                    $scope.alert_fun('success', '发布成功！');
                    //$scope.GetCreatedAtCount();
                    $scope.GetShareReply2($scope.shareId_f);
                    $scope.GetShare();
                }
            });
            q.error(function (e) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });

        }


        $scope.xuanz_id_dl = function (x) {
            //$scope.selected_name_2 = [];
            //for (var i = 0; i < $scope.selected_name.length; i++) {
            //    var temp = $scope.selected_name[i];
            //    if (!isNaN(x)) {
            //        temp = i;
            //    }
            //    if (temp == x) {
            //        for (var j = i; j < $scope.selected_name.length; j++) {
            //            $scope.selected_name[j] = $scope.selected_name[j + 1];
            //        }
            //        $scope.selected_name.length = $scope.selected_name.length - 1;
            //    }
            //}
            for (var i = 0; i < $scope.selected_name.length; i++) {
                if (x.ID == $scope.selected_name[i].ID) {
                    $scope.selected_name.splice(i, 1);
                }
            }
        }
        //拼写id
        $scope.pin_id = function () {
            $scope.userIds = "";
            for (var i = 0; i < $scope.selected_name.length; i++) {
                $scope.userIds = $scope.userIds + ";" + $scope.selected_name[i].ID;
                if (i > 0) {
                    $scope.Noallfabu = false;
                    $scope.fabu = false;
                    $scope.allfabu = true;
                }
            }
            console.log($scope.userIds);
            console.log($scope.selected_name);
        }

        //讨论总计和新增
        $scope.GetCreatedAtCount = function () {
            var url = "/api/Share/GetCreatedAtCount?usrId=" + $rootScope.userID;
            var q = $http.get(url);
            q.success(function (response, status) {
                $scope.GetCount = response.count;
                $scope.GetYesterdayCount = response.yesterdayCount;
                if ($scope.GetShare_count == null) {
                    $scope.MeCount = 0;
                }
                else {
                    $scope.MeCount = $scope.GetShare_count;
                }
                console.log(response);
            })
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }


    //获取注册用户数
        $scope.GetUserNum = function () {
            var url = "/api/Account/GetUserNum";
            var q = $http.get(url);
            q.success(function (response, status) {
                $scope.GetUserNum_list = response;
                console.log(response);
            })
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }


        //评论数和点赞数 
        $scope.GetLoveCount = function (x) {
            var url = "api/Share/GetLoveCount?shareId=" + x.ID;
            var q = $http.get(url);
            q.success(function (response, status) {

                $scope.GetReplyer = response.ReplyerCount;
                $scope.GetLove = response.loveCount;
                console.log(response);


            });
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
        //__________________________________________________________________
        $scope.GetShare();
        $scope.GetCreatedAtCount();
        $scope.GetUserNum();
});