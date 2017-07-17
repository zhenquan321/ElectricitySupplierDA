var picManage_ctr = myApp.controller("picManage_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, foo, myApplocalStorage) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    $scope.isActive = true;
    $scope.isActive1 = true;
    $scope.isActiveA = true;
    $scope.isActiveB = false;
    $scope.isActiveC = false;
    $scope.ImgList = [];
    $scope.imgs = {Count: 0, Result: []};
    $scope.selectall = false;
    $scope.curPrjID = "";
    $scope.isActiveC = false;
    $scope.isActiveC = false;
    $scope.isActiveC = false;

    //1.1tab切换未启动
    $scope.changestarted = function () {
        $scope.isActive = false;
    };
    //1.2tab切换已启动
    $scope.changestart = function () {
        $scope.isActive = true;
    };


    //2.2.1切换单张图片上传
    $scope.changesUpload = function () {
        $scope.isActiveA = true;
        $scope.isActiveB = false;
        $scope.isActiveC = false;
    };


    //2.2.2切换多张图片上传
    $scope.changebUpload = function () {
        $scope.isActiveA = false;
        $scope.isActiveB = true;
        $scope.isActiveC = false;
    };

    //2.2.3切换网页抓取

    $scope.changewCraping = function () {
        $scope.isActiveA = false;
        $scope.isActiveB = false;
        $scope.isActiveC = true;
    };

    //3.链接上传图片

    $scope.uploadImg = function () {
        $('#uploadPic').click();
    };
    //3.1链接图片管理

    $scope.picMagLink = function () {
        $('#uploadPic1').click();
    };


    //4.加载图片
    $scope.page = 1;
    $scope.pagesize = 20;
    $scope.imgdescripexceluploadtion = "";
    $scope.imgdescription = "";
    $scope.createdata = "";
    $scope.imgTopic = "";
    $scope.ImgResultList = [];
    $scope.ImgList = [];
    $scope.pageMach = 0;
    $scope.sum = 0;
    $scope.proCount = 0;

    $scope.loadImgs = function () {
        $rootScope.loadingTrue = true;//加载中

        var url = "/api/image/GetOfficalImgs?companyId=" + $rootScope.CompanyID + "&projectID=" + $rootScope.projectID + "&imgdescription="
            + $scope.imgdescription + "&createdata=" + $scope.createdata + "&imgTopic=" + $scope.imgTopic +
            "&page=" + ($scope.page - 1) + "&pagesize=" + $scope.pagesize;
        var p = $http.get(url);
        p.success(
            function (response, status) {
                console.log('picManage_ctr>loadImgs');
                $scope.ImgList = response;
                $scope.proCount = response.Count;

                if (response.Count >= 0) {
                    $scope.ImgResultList = response.Result;
                    $scope.pageMach = $scope.ImgResultList.length;
                    $scope.picMag();
                    $rootScope.loadingTrue = false;//加载中

                    if (response.Count >= 10) {
                        $scope.loadMore = true;
                    }
                }
            }
        );
        p.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
            $scope.isActiveList = false;
            $rootScope.loadingTrue = false;//加载中

        });
    }
    //4.1加载更多
    //$scope.loadingMore = function () {
    //    $scope.page = $scope.page + 1;
    //    var url = "/api/image/GetOfficalImgs?companyId=" + $rootScope.CompanyID + "&projectID=" + $rootScope.projectID + "&imgdescription="
    //        + $scope.imgdescription + "&createdata=" + $scope.createdata + "&imgTopic=" + $scope.imgTopic +
    //        "&page=" + $scope.page + "&pagesize=" + $scope.pagesize;
    //    var p = $http.get(url);
    //    p.success(
    //        function (response, status) {
    //            if (response.Count >=0) {
    //                $scope.ImgResultList2 = response.Result;
    //                var a = [];
    //                var b = [];
    //                var c = [];
    //                a = $scope.ImgResultList;
    //                b = $scope.ImgResultList2;
    //                c = a.concat(b);
    //                if (b.length == 0) {
    //                    $scope.loadMore = false;
    //                }
    //                $scope.ImgResultList = c;
    //                $scope.pageMach = $scope.ImgResultList.length;


    //            }
    //        }
    //        );
    //    p.error(function (response) {
    //        $scope.error = "网络打盹了，请稍后。。。";
    //    });
    //}

    //5.图片管理切换

    $scope.picMag = function () {

        if ($scope.ImgResultList == 0) {
            $scope.isActive1 = true;
        }
        else {
            $scope.isActive1 = false;
        }
    }


    // 6.图片管理
    // 6.1全选照片
    $scope.ischecked2 = "全选";
    $scope.isCheckAll = function () {
        if ($scope.ischecked2 == "全选") {
            $scope.ischecked2 = "取消";
            for (var i in $scope.ImgResultList) {
                $scope.ImgResultList[i].IsSelected = true;
            }
        } else {
            $scope.ischecked2 = "全选";
            for (var i in $scope.ImgResultList) {
                $scope.ImgResultList[i].IsSelected = false;
            }
        }
        $scope.IsChecked();
    };
    //6.2获得ids

    $scope.IsChecked = function () {
        $scope.ids = "";
        var ids = "";
        for (var i = 0; i < $scope.ImgResultList.length; i++) {
            var p = $scope.ImgResultList[i];
            if (p.IsSelected == true) {
                ids = ids + p.ID + ";";
                $scope.ids = ids;
            }
        }
    };

    //6.3删除正版图片
    $scope.deleImg = function () {
        if ($scope.ids == "") {
            alert("请勾选侵权项！");
        } else {
            var url = "/api/Image/DelOfficalImg"
                + "?companyID=" + $rootScope.CompanyID + "&projectID=" + $rootScope.projectID
                + "&ids=" + $scope.ids;
            var p = $http.get(url);
            p.success(function (response, status) {
                console.log('picManage_ctr>deleImg');
                if (response == 0) {
                    alert("删除失败");
                } else {
                    $scope.loadImgs();
                    alert("删除成功");
                }
            });
            p.error(function (e) {
                $scope.error = "网络打盹了，请稍后。。。";

            });
        }
    };


    //7.单张上传
    //7.1
    $scope.whitelist = "";
    $scope.imgID = "";
    $scope.imgtabs = "";
    $scope.imgDis = "";
    $scope.imgURL = "";
    $scope.singleImgUpload = {ImgUrl: "", isShowImgUrl: 0};
    $scope.error = "";
    $scope.fileField1 = "";
    $scope.fetchData = function () {

        $scope.imageUpload();
    }

    $scope.imageUpload = function () {

        var filectl = $('#fileField1')[0];
        //console.log($('#fileField1'))
        if (filectl.files.length <= 0) {
            alert("请选择上传图片！")
            return false;
        }

        if ($scope.imgID == "" || $scope.imgID == null) {
            alert("图片ID不能为空！")
            return
        }
        //创建FormData对象
        var data = new FormData();
        //为FormData对象添加数据
        $.each($('#fileField1')[0].files, function (i, file) {
            data.append('upload_image' + i, file);
        });
        //发送数据
        $rootScope.loadingTrue = true;//加载中

        $.ajax({
            url: 'Export/ImgUpload',
            type: 'POST',
            data: data,
            cache: false,
            contentType: false,        //不可缺参数
            processData: false,        //不可缺参数
            success: function (data) {
                if (data != null && data != undefined) {
                    $scope.singleImgUpload.ImgUrl = data;
                    $scope.singleImgUpload.isShowImgUrl = 1;
                    $scope.DimgSaveinfo();
                    $rootScope.loadingTrue = false;//加载中
                }
            },
            error: function () {
                alert('上传出错');
                $rootScope.loadingTrue = false;//加载中

            }
        });
    };

    //7.2

    $scope.DimgSaveinfo = function () {
        //  $scope.imageUpload();
        var imginfo = {
            "AuthorizedShop": $scope.whitelist,
            "ImgUrl": $scope.singleImgUpload.ImgUrl == "" ? $scope.imgURL : $scope.singleImgUpload.ImgUrl,
            "OffImgId": $scope.imgID,
            "OffImgTopic": $scope.imgtabs,
            "OffImgDesc": $scope.imgDis,
            "ProjectID": $rootScope.projectID,
            "End_At": Date.now,
            "CompanyId": $rootScope.CompanyID
        };
        $.ajax({
            type: "POST",
            url: "/api/image/SingleImportOffImgs",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            data: JSON.stringify(imginfo),
            success: function (data) {
                $scope.error = data;
                $("#fileField1").val("");
                $(".destination img").attr("src", '');
                $("#textfield").val("");
                $("#iiimg").attr("src", '');
                $scope.whitelist = "";
                $scope.imgID = "";
                $scope.imgtabs = "";
                $scope.imgDis = "";
                $scope.imgURL = "";
                alert(data);
            }
        });
        $scope.loadImgs();
    };


    //8.批量上传
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
                    $scope.singleImgUpload.ImgUrl = data;
                    $("#filepath").val(data);
                    $scope.BatchImportOffImgs();
                    $scope.DimgSaveinfo();
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
            var url = "/api/Image/BatchImportOffImgs?companyID=" + $rootScope.CompanyID + "&projectID=" + $rootScope.projectID
                + "&excelFilePath=" + encodeURIComponent($scope.singleImgUpload.ImgUrl);
            var p = $http.get(url);
            p.success(function (response, status) {
                console.log('picManage_ctr>batchImportOffImgs');
                if (response == "导入成功") {
                    $("#filepath").val("");
                    $("#fileField").val("");
                    $("#textfield").val("");
                } else {
                    alert(response);
                }
            });
            p.error(function (e) {
                $scope.error = "网络打盹了，请稍后。。。";

            });
        }
    }

    $scope.imgViewer = function (el) {
        foo.getPrivate();

    }

    //9.加载蓝V 
    $scope.page_blog = 1;
    $scope.totalpage_blog = 1;
    $scope.totalcount_blog = 0;
    $scope.blogs = [];
    $scope.pageindexs_blog = [1];
    $scope.pagesize_blog = 10;
    $scope.doneType = null;
    $scope.poster_name_kwd = "";
    $scope.ItemLVName = "";
    $scope.pageMachLan = 0;
    $scope.sumpageMachLan = 0;
    $scope.loadblogs = function () {
        $rootScope.loadingTrue = true;//加载中

        var url = "/api/Mng/GetBlogPosters?&projectID=" + $rootScope.projectID + "&poster_name_kwd="
            + $scope.ItemLVName + "&done=" + $scope.doneType + "&page=" + ($scope.page_blog - 1) +
            "&pagesize=" + $scope.pagesize_blog;
        var p = $http.get(url);
        p.success(
            function (data) {
                console.log('picManage_ctr>loadblogs');
                $scope.sumpageMachLan = data.Count;
                if (data.Count > 0) {
                    $scope.blogs = data.Result;

                    $scope.pageMachLan = $scope.blogs.length;
                    $scope.totalcount_blog = data.Count;
                    $scope.totalpage_blog = Math.round(data.Count / $scope.pagesize_blog) == 0 ? 1 : Math.ceil(data.Count / $scope.pagesize_blog);
                    if ($scope.totalpage_blog > 1) {
                        var arr = new Array();
                        if ($scope.totalpage_blog >= 5) {
                            arr = [1, 2, 3, 4, 5];
                        }
                        else {
                            var k = 0;
                            for (var i = 1; i <= $scope.totalpage_blog; i++) {
                                arr[k] = i;
                                k++;
                            }
                        }
                        $scope.pageindexs_blog = arr;
                    }
                }
                //$('#blogModal').modal('show');
                $scope.isSearchblogdata = false;
                $rootScope.loadingTrue = false;//加载中

            });
        p.error(function (response) {
            alert("网络打盹了，请稍后。。。");
            $rootScope.loadingTrue = false;//加载中

        });
    };
    //9.1加载更多蓝V
    //$scope.loadingMoreLan = function () {
    //    $scope.page_blog = $scope.page_blog + 1;

    //    var url = "/api/Mng/GetBlogPosters?&projectID=" + $rootScope.projectID + "&poster_name_kwd="
    //        + $scope.ItemLVName + "&done=" + $scope.doneType + "&page=" + $scope.page_blog +
    //        "&pagesize=" + $scope.pagesize_blog;
    //    var p = $http.get(url);
    //    p.success(
    //         function (data) {
    //             if (data.Count > 0) {
    //                 $scope.blogs2 = data.Result;
    //                 var a = [];
    //                 var b = [];
    //                 var c = [];
    //                 a = $scope.blogs;
    //                 b = $scope.blogs2;
    //                 c = a.concat(b);


    //                 if (b.length == 0) {
    //                     $scope.loadMore = false;
    //                 }
    //                 $scope.blogs = c;
    //                 $scope.pageMachLan = $scope.blogs.length;
    //             }

    //             //if (data.Count > 0) {
    //             //    $scope.blogs = data.Result;
    //             //    $scope.totalcount_blog = data.Count;
    //             //    $scope.totalpage_blog = Math.round(data.Count / $scope.pagesize_blog) == 0 ? 1 : Math.ceil(data.Count / $scope.pagesize_blog);
    //             //    if ($scope.totalpage_blog > 1) {
    //             //        var arr = new Array();
    //             //        if ($scope.totalpage_blog >= 5) {
    //             //            arr = [1, 2, 3, 4, 5];
    //             //        }
    //             //        else {
    //             //            var k = 0;
    //             //            for (var i = 1; i <= $scope.totalpage_blog; i++) {
    //             //                arr[k] = i;
    //             //                k++;
    //             //            }
    //             //        }
    //             //        $scope.pageindexs_blog = arr;
    //             //    }
    //             //}
    //             $scope.isSearchblogdata = false;
    //         });
    //    p.error(function (response) {
    //        $scope.error = "网络打盹了，请稍后。。。";
    //        $scope.isActiveList = false;
    //    });
    //};

    //9.2全选择蓝V
    $scope.ischecked3 = "全选";
    $scope.AllCheckedBlog = function () {
        //self.$data.checkedimgId = "";
        if ($scope.ischecked3 == "全选") {
            $scope.ischecked3 = "反选";
            for (var i in $scope.blogs) {
                $scope.blogs[i].IsSelected = true;
                //self.$data.checkedimgId += self.$data.imgs[i].ID + ";";
            }
        } else if ($scope.ischecked3 == "反选") {
            $scope.ischecked3 = "全选"
            for (var i in $scope.blogs) {
                $scope.blogs[i].IsSelected = false;
            }
        }
        $scope.IsCheckedBlog();
    };
    //9.3选择蓝V
    $scope.ids3 = "";
    $scope.IsCheckedBlog = function () {
        $scope.ids3 = "";
        var ids3 = "";
        ////self.$data.checkedimgId = "";
        //if (!isselected &&  $scope.selectallblog) {
        //    $scope.selectallblog = false;
        //}
        //if (isselected) {
        //    for (var i in self.$data.blogs) {
        //        if (!self.$data.blogs[i].IsSelected) {
        //            return;
        //        }
        //        //self.$data.checkedimgId += self.$data.imgs[i].ID + ";";
        //    }
        //    self.$data.selectallblog = true;
        //}
        for (var i = 0; i < $scope.blogs.length; i++) {
            var p = $scope.blogs[i];
            if (p.IsSelected == true) {
                ids3 = ids3 + p.ID + ";";
                $scope.ids3 = ids3;
            }
        }
    };

    //9.4改变蓝V启动状态
    $scope.startBlog = function (id, done) {
        $.get('/api/Mng/StartBlogPosters', {
                ids: id,//选择的ids
                isdone: done,
                prjID: $rootScope.projectID
            },
            function (rows) {
                if (rows > 0) {
                    alert("操作成功！");
                    $scope.loadblogs();
                } else {
                    alert("操作失败！");
                }
            });
    }
    //9.5全部启动
    $scope.AllStartBlog = function () {
        var checkedimgId = "";
        for (var i in $scope.blogs) {
            if ($scope.blogs[i].IsSelected) {
                checkedimgId += $scope.blogs[i].ID + ";";
            }
        }
        ;
        if (checkedimgId == "") {
            alert("请选择操作项！");
        } else {
            $.get('/api/Mng/StartBlogPosters', {
                    ids: checkedimgId,
                    isdone: 0,
                    prjID: $rootScope.projectID
                },
                function (rows) {
                    if (rows > 0) {
                        alert("启动成功！");
                        $scope.loadblogs();
                    } else {
                        alert("启动失败！");
                    }
                });
        }
    };
    //9.6全部暂停
    $scope.AllStopBlog = function () {
        var checkedimgId = "";
        for (var i in $scope.blogs) {
            if ($scope.blogs[i].IsSelected) {
                checkedimgId += $scope.blogs[i].ID + ";";
            }
        }
        ;
        if (checkedimgId == "") {
            alert("请勾选项！");
        } else {
            $.get('/api/Mng/StartBlogPosters', {
                    ids: checkedimgId,
                    isdone: null,
                    prjID: $rootScope.projectID
                },
                function (rows) {
                    if (rows > 0) {
                        alert("停止成功！");
                        $scope.loadblogs();
                    } else {
                        alert("停止失败！");
                    }
                });
        }
    };
    //9.7搜索
    $scope.Searchbloglist = function () {
        if ($scope.ItemLVName != "") {
            $scope.pagesize_blog = null;
        } else {
            $scope.pagesize_blog = 20;
        }
        $scope.loadblogs();
    };

    $scope.loadImgs();
    $scope.loadblogs();
    //滚动周
    $scope.scrollListen = function () {
        //获取要定位元素距离浏览器顶部的距离

        var navH = $(".nb").offset().top;

        //滚动条事件

        $(window).scroll(function () {

            //获取滚动条的滑动距离

            var scroH = $(this).scrollTop();
            //滚动条的滑动距离大于等于定位元素距离浏览器顶部的距离，就固定，反之就不固定

            if (scroH >= navH) {

                $(".nb").css({"position": "fixed", "top": 48, "right": 25});

            } else if (scroH < navH) {


                $(".nb").css({"position": "static"});

            }

        })
    }
    //loadingMoreLan$scope.scrollListen();
    //

});