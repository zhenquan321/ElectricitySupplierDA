var picSearch_ctr = myApp.controller("picSearch_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {


    //selectStatus
    $rootScope.selectStatus = "百度";
    $scope.isCollection = true;
    $scope.timeStampSelect = [{"Id": 0, "Name": "否"}, {"Id": 1, "Name": "是"}];
    $rootScope.isSuccess = "isNull";
    $scope.searchInput = "";
    $scope.page = 1;
    $scope.pagesize = 10;
    $scope.page2 = 1;
    $scope.pagesize2 = 20;
    $scope.page3 = 0;
    $scope.pagesize3 = 10;
    $scope.Abstract = "";
    $rootScope.keyword = "";
    $scope.Title = "";
    $scope.domain = "";
    $scope.status = "";
    $rootScope.resultList = [];
    $rootScope.ZhiboresultList = [];
    $rootScope.alerts = [];
    $scope.pageBaidu = 0;
    $rootScope.pagesizeBaidu = 10;
    $scope.keywordsList1 = [];
    $scope.keywordsListCommend = [];
    $rootScope.getBaiduRecordId = "";
    $scope.isActiveKeyword = false;
    $rootScope.BaidukeywordId = "";
    $rootScope.ZhibokeywordId = "";
    $rootScope.keywordsList = [];
    $scope.isActiveUserStyle = false;
    $rootScope.isActiveLoadmore = true;
    $rootScope.isActiveLoadmoreZhiboLevelLinks = true;
    $scope.BaiduCount = "";
    $scope.id = "";
    $scope.status = "";
    $scope.isActiveCollection = true;
    $scope.isActiveCollection2 = true;
    $rootScope.categoryId = "";
    $scope.page4 = 1;
    $scope.pagesize4 = 11;
    //$rootScope.projectsList = [];
    $scope.projectsListlength = 0;
    $scope.keyID = "";
    $scope.GetAllKeywordCategory_list = "";
    $scope.Title = "";
    $scope.domain = "";
    $scope.Abstract = "";
    $scope.infriLawCode = "";
    $rootScope.BaidukeywordId = "";
    $scope.page_picSr = 1;
    $scope.pagesize_picSr = 10;

    $scope.page4 = 1;
    $scope.pagesize4 = 100;
    $scope.page_picS = 1;
    $scope.pagesize_picS = 20;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //改变信源
    //0.1微信
    $scope.changeModel_baidu = function () {
        $rootScope.isActiveModale = "baidu";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    }

    $scope.changeModel_bing = function () {
        $rootScope.isActiveModale = "bing";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }

    //0.2微信
    $scope.changeModel_sougou = function () {
        $rootScope.isActiveModale = "wechat";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    }
    //0.3eMN

    $scope.changeModel_eMN = function () {
        $rootScope.isActiveModale = "eMarketNow";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)

    }
    //0.4picS

    $scope.changeModel_picS = function () {
        $rootScope.isActiveModale = "picSearch";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }
    $scope.changeModel_picS();
    //0.5搜狗

    $scope.changeModel_weibo = function () {
        $rootScope.isActiveModale = "weibo";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }
    //0.6微博

    $scope.changeModel_sougo_new = function () {
        $rootScope.isActiveModale = "sougo_new";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }
    //0.7谷歌

    $scope.changeModel_googol = function () {
        $rootScope.isActiveModale = "googol";
        $cookieStore.put("isActiveModale", $rootScope.isActiveModale)
    }


    //页面添加alert
    $rootScope.addAlert = function (status, messages) {
        var len = $rootScope.alerts.length + 1;
        $rootScope.alerts = [];
        $rootScope.alerts.push({type: status, msg: messages});
    }
    //关闭alert
    $scope.closeAlert = function (index) {
        $rootScope.alerts.splice(index, 1)
    }
    //5.退出清cookie
    $scope.off = function () {
        $location.path("/home/main_1").replace();
        $rootScope.userID = "";
        $rootScope.LoginName = "";
        $rootScope.UsrRole = "";
        $rootScope.UsrKey = "";
        $rootScope.UsrNum = "";
        $rootScope.UsrEmail = "";
        //页面
        $rootScope.keyword = "";
        $rootScope.keywordName = "";
        //changexinyuan
        $rootScope.selectStatus = "";
        $rootScope.ZhibokeywordId = "";
        $rootScope.BaidukeywordId = "";
        $rootScope.keywordsListRecord = "";
        $rootScope.getBaiduRecordId = "";
        $cookieStore.remove("userID");
        $cookieStore.remove("LoginName");
        $cookieStore.remove("UsrRole");
        $cookieStore.remove("UsrKey");
        $cookieStore.remove("UsrNum");
        $cookieStore.remove("UsrEmail");
        $cookieStore.remove("keyword");
        $cookieStore.remove("keywordName");
        $cookieStore.remove("ZhibokeywordId");
        $cookieStore.remove("BaidukeywordId");
        $cookieStore.remove("keywordsListRecord");
        $cookieStore.remove("getBaiduRecordId");
        $cookieStore.remove("selectStatus");
    }

    //7.单张上传
    $scope.whitelist = "";
    $scope.imgID = "";
    $scope.imgtabs = "";
    $scope.imgDis = "";
    $scope.imgURL = "";
    $scope.singleImgUpload = {ImgUrl: "", isShowImgUrl: 0};
    $scope.error = "";
    $scope.fileImage = "";
    $scope.input_pic_address = "";
    $scope.fetchData = function () {
        $scope.imageUpload();
    }

    $scope.imageUpload = function () {

        if ($scope.input_pic_address != "" && $scope.input_pic_address != null) {
            $scope.singleImgUpload.ImgUrl = $scope.input_pic_address;
            $scope.DimgSaveinfo();
        }
        else {
            var filectl = $('#fileImage')[0];
            if (filectl.files.length <= 0) {
                alert("请选择上传图片！")
                return false;
            }
            //创建FormData对象
            var data = new FormData();
            //为FormData对象添加数据
            $.each($('#fileImage')[0].files, function (i, file) {
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
        }
    };

    //7.2

    $scope.DimgSaveinfo = function () {
        //  $scope.imageUpload();
        var imginfo = {
            "Src": $scope.singleImgUpload.ImgUrl == "" ? $scope.imgURL : $scope.singleImgUpload.ImgUrl,
            "ProjectID": $rootScope.getProjectId,
            "UsrId": $rootScope.userID,

        };
        $.ajax({
            type: "POST",
            url: "/api/Img/InsertImgSearchTask",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            data: JSON.stringify(imginfo),
            success: function (data) {
                $scope.error = data;
                $("#fileImage").val("");
                $(".upload_append_list").remove();
                $scope.GetImgSearchTasks_fun();
                $rootScope.addAlert('success', "添加成功！");
                $scope.input_pic_address = ""
            }
        });
    };


    //1.获取搜索图
    $scope.GetImgSearchTasks_fun = function () {
        $scope.paramsList = {
            usr_id: $rootScope.userID,
            prjId: $rootScope.getProjectId,
            page: $scope.page_picS - 1,
            pagesize: $scope.pagesize_picS,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "api/Img/GetImgSearchTasks"
        })
            .success(function (response, status) {
                $scope.conut_St = response.Count;
                $rootScope.searchPic = response.Result;
                console.log(response)
            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "服务器连接出错");
            });
    }

    //3.获取百度搜索图片
    $scope.GetImgSearchLinks = function (id, Src) {
        $rootScope.searchSrc = Src;
        $rootScope.picRs_id = id;
        $cookieStore.put("picRs_id", $rootScope.picRs_id);
        $cookieStore.put("searchSrc", $rootScope.searchSrc)
        $scope.paramsList = {
            user_id: $rootScope.userID,
            projectId: $rootScope.getProjectId,
            page: $scope.page_picSr - 1,
            pagesize: $scope.pagesize_picSr,
            status: "",
            searchTaskId: id,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "api/Img/GetImgSearchLinks"
        })
            .success(function (response, status) {

                $scope.count_psr = response.Count;
                $rootScope.GetImgSearchLinks_list = response.Result;
                console.log(response);
            })
            .error(function (response, status) {
                $rootScope.addAlert('danger', "服务器连接出错");
            });

    }


    $scope.GetImgSearchLinks_fun = function (page) {
        $scope.page_picSr = page;
        $scope.GetImgSearchLinks($rootScope.picRs_id, $rootScope.searchSrc);
    }

    //11.2.获取项目列表
    $scope.GetProjects = function () {
        var roleId;
        if ($rootScope.UsrRole == 0) {
            roleId = $rootScope.userID;
        } else {
            roleId = $rootScope.currentId;
        }
        var url = "/api/Keyword/GetProjects?usr_id=" + roleId + "&page=" + ($scope.page4 - 1) + "&pagesize=" + $scope.pagesize4;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('modelSelect_ctr>GetProjects');
            console.log($rootScope.projectsList);
            if (response != null) {
                $rootScope.projectsList = response.Result;
                // $cookieStore.put("projectsList", $rootScope.projectsList);
                myApplocalStorage.setObject('projectsList', $rootScope.projectsList);
                $scope.projectsListlength = response.Count
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    //切换项目
    $scope.clickItem = function (id, name) {
        $rootScope.getProjectId = id;
        $cookieStore.put("getProjectId", $rootScope.getProjectId);
        $rootScope.getProjectName = name;
        $cookieStore.put("getProjectName", $rootScope.getProjectName);
        window.location.reload();

    }

    //自动加载______________________________________________________________________________
    $scope.GetProjects();
    $scope.GetImgSearchTasks_fun();
});