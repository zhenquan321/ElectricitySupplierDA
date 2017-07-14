/// <reference path="app/views/keywords/keywordcConstrast.html" />
/// <reference path="app/views/keywords/keywordcConstrast.html" />
/// <reference path="app/views/keywordMng/keywordSearch.html" />

var myApp = angular.module("myApp", [
    'ngAnimate',
    "ngCookies",
    'ui.router',
    "ngMessages",
    'angular-loading-bar',
    "ui.bootstrap",
    'ngDragDrop'])
    .run(['$anchorScroll', function ($anchorScroll) {
        $anchorScroll.yOffset =100;
    }]);

myApp.filter('htmlContent', ['$sce', function ($sce) {
    return function (input) {
        return $sce.trustAsHtml(input);
    }
}]);

//angular路由
myApp.config(function ($stateProvider, $urlRouterProvider) {
    //$urlRouterProvider.when("", "/home");
    $urlRouterProvider.when("", "/home/main_1");
    $stateProvider

        .state("home", {
            url: "/home",
            controller: "Home_New_ctr",
            templateUrl: "Scripts/app/views/Home_New/Home_New.html"
        })

        //.state("home", {
        //    url: "/home",
        //    controller: "login_ctr",
        //    templateUrl: "Scripts/app/views/home/login.html"
        //})

         .state("home.main_1", {
             url: "/main_1",
             controller: "main_1_ctr",
             templateUrl: "Scripts/app/views/Home_New/main/main_1.html"
         })
        .state("login", {
            url: "/login",
            controller: "login_ctr",
            templateUrl: "Scripts/app/views/home/login.html"
        })
        .state("signup", {
            url: "/signup",
            controller: "signup_ctr",
            templateUrl: "Scripts/app/views/home/signup.html"
        })
        .state("reset", {
            url: "/reset",
            controller: "reset_ctr",
            templateUrl: "Scripts/app/views/home/reset.html"
        })
        .state("changepwd", {
            url: "/changepwd",
            controller: "changepwd_ctr",
            templateUrl: "Scripts/app/views/home/changepwd.html"
        })

        //index
        .state("modelSelect", {
            url: "/modelSelect",
            controller: "modelSelect_ctr",
            templateUrl: "Scripts/app/views/modelSelect.html"
        })
      
        //eMarketNow
        .state("DnL_eMarketNow", {
            url: "/DnL_eMarketNow",
            controller: "iw2s_eMarketNow_ctr",
            templateUrl: "Scripts/app/views/eMarketNow/iw2s_eMarketNow.html"
        })
        .state("DnL_eMarketNow.eMarketNow", {
            url: "/eMarketNow",
            controller: "eMarketNow_ctr",
            templateUrl: "Scripts/app/views/eMarketNow/eMarketNow.html"
        })
        .state("DnL_eMarketNow.eMarketNowMng", {
            url: "/eMarketNowMng",
            controller: "eMarketNowMng_ctr",
            templateUrl: "Scripts/app/views/eMarketNow/eMarketNowMng.html"
        })
        .state("DnL_eMarketNow.eMarketNowDashboard", {
            url: "/eMarketNowDashboard",
            controller: "eMarketNowDashboard_ctr",
            templateUrl: "Scripts/app/views/eMarketNow/eMarketNowDashboard.html"
        })
        //weixin(sogou)
        .state("DnL_sogou", {
            url: "/DnL_sogou",
            controller: "iw2s_sogou_ctr",
            templateUrl: "Scripts/app/views/sogou/iw2s_sogou.html"
        })
        .state("DnL_sogou.sogouShowDesc", {
            url: "/sogouShowDesc",
            controller: "sogouShowDesc_ctr",
            templateUrl: "Scripts/app/views/sogou/sogouShowDesc.html"
        })
        .state("DnL_sogou.sogouMng", {
            url: "/sogouMng",
            controller: "sogouMng_ctr",
            templateUrl: "Scripts/app/views/sogou/sogouMng.html"
        })
        .state("DnL_sogou.sogouViews", {
            url: "/sogouViews",
            controller: "sogouViews_ctr",
            templateUrl: "Scripts/app/views/sogou/sogouViews.html"
        })
        .state("DnL_sogou.sogouConstrast", {
            url: "/sogouConstrast",
            controller: "sogouConstrast_ctr",
            templateUrl: "Scripts/app/views/sogou/sogouConstrast.html"
        })
        .state("DnL_sogou.sogouDashboard", {
            url: "/sogouDashboard",
            controller: "sogouDashboard_ctr",
            templateUrl: "Scripts/app/views/sogou/sogouDashboard.html"
        })

        //picSearch
        .state("picSearch", {
            url: "/picSearch?name",
            controller: "picSearch_ctr",
            templateUrl: "Scripts/app/views/picSearch/picSearch.html"
        })
        .state("picSearch.picSearch_ShowDesc", {
            url: "/picSearch_ShowDesc",
            controller: "picSearch_ShowDesc_ctr",
            templateUrl: "Scripts/app/views/picSearch/picSearch_ShowDesc.html"
        })
        .state("picSearch.picSearch_sr", {
            url: "/picSearch_sr",
            controller: "picSearch_sr_ctr",
            templateUrl: "Scripts/app/views/picSearch/picSearch_sr.html"
        })
        .state("picSearch.picSearch_result", {
            url: "/picSearch_result",
            controller: "picSearch_result_ctr",
            templateUrl: "Scripts/app/views/picSearch/picSearch_result.html"
        })
        //weibo
        .state("weibo", {
            url: "/weibo?name",
            controller: "weibo_ctr",
            templateUrl: "Scripts/app/views/weibo/weibo.html"
        })
        .state("weibo.weiboShowDesc", {
            url: "/weiboShowDesc",
            controller: "weiboShowDesc_ctr",
            templateUrl: "Scripts/app/views/weibo/weiboShowDesc.html"
        })
        .state("weibo.weiboConstrast", {
            url: "/weiboConstrast",
            controller: "weiboConstrast_ctr",
            templateUrl: "Scripts/app/views/weibo/weiboConstrast.html"
        })
        .state("weibo.weiboMng", {
            url: "/weiboMng",
            controller: "weiboMng_ctr",
            templateUrl: "Scripts/app/views/weibo/weiboMng.html"
        })
        .state("weibo.weiboViews", {
            url: "/weiboViews",
            controller: "weiboViews_ctr",
            templateUrl: "Scripts/app/views/weibo/weiboViews.html"
        })
        .state("weibo.weiboDashboard", {
            url: "/weiboDashboard",
            controller: "weiboDashboard_ctr",
            templateUrl: "Scripts/app/views/weibo/weiboDashboard.html"
        })
        //googol
        .state("googol", {
            url: "/googol?name",
            controller: "googol_ctr",
            templateUrl: "Scripts/app/views/googol/googol.html"
        })
        .state("googol.googol_ShowDesc", {
            url: "/googol_ShowDesc",
            controller: "googol_ShowDesc_ctr",
            templateUrl: "Scripts/app/views/googol/googol_ShowDesc.html"
        })
        //sogou(sogouNEW)
        .state("DnL_sogouNEW", {
            url: "/DnL_sogouNEW",
            controller: "iw2s_sogouNEW_ctr",
            templateUrl: "Scripts/app/views/sogouNEW/iw2s_sogouNEW.html"
        })
        .state("DnL_sogouNEW.sogouNEWShowDesc", {
            url: "/sogouNEWShowDesc",
            controller: "sogouNEWShowDesc_ctr",
            templateUrl: "Scripts/app/views/sogouNEW/sogouNEWShowDesc.html"
        })
        .state("DnL_sogouNEW.sogouMngNEW", {
            url: "/sogouMngNEW",
            controller: "sogouNEWMng_ctr",
            templateUrl: "Scripts/app/views/sogouNEW/sogouNEWMng.html"
        })
        .state("DnL_sogouNEW.sogouNEWViews", {
            url: "/sogouNEWViews",
            controller: "sogouNEWViews_ctr",
            templateUrl: "Scripts/app/views/sogouNEW/sogouNEWViews.html"
        })
        .state("DnL_sogouNEW.sogouNEWConstrast", {
            url: "/sogouNEWConstrast",
            controller: "sogouNEWConstrast_ctr",
            templateUrl: "Scripts/app/views/sogouNEW/sogouNEWConstrast.html"
        })
        .state("DnL_sogouNEW.sogouNEWDashboard", {
            url: "/sogouNEWDashboard",
            controller: "sogouNEWDashboard_ctr",
            templateUrl: "Scripts/app/views/sogouNEW/sogouNEWDashboard.html"
        })

        //用户管理后台__________________________________________________________________
        .state("computingResources", {
            url: "/computingResources",
            controller: "computingResources_ctr",
            templateUrl: "Scripts/app/views/BackstageManagement/computingResources.html"
        })
         .state("UserManage", {
             url: "/UserManage",
             controller: "UserManage_ctr",
             templateUrl: "Scripts/app/views/BackstageManagement/UserManage.html"
         })
         .state("SalesManagement", {
             url: "/SalesManagement",
             controller: "SalesManagement_ctr",
             templateUrl: "Scripts/app/views/BackstageManagement/SalesManagement.html"
         })
        .state("PurchaseService", {
            url: "/PurchaseService",
            controller: "PurchaseService_ctr",
            templateUrl: "Scripts/app/views/UserCenter/PurchaseService.html"
        })
    
        //________________________________________________________________________________
        .state("DnL", {
            url: "/DnL?name",
            controller: "iw2s_ctr",
            templateUrl: "Scripts/app/views/iw2s.html"
        })

        //main 关键词管理
        .state("DnL.addProject", {
            url: "/addProject",
            controller: "addProject_ctr",
            templateUrl: "Scripts/app/views/addProject/addProject.html"
        })
        //baidu_dashboard_________________________________________________________________
        .state("DnL.DnL_dashboard", {
            url: "/DnL_dashboard",
            controller: "iw2s_dashboard_ctr",
            templateUrl: "Scripts/app/views/keywords/iw2s_dashboard.html"
        })
        .state("rizhi", {
            url: "/rizhi",
            controller: "rizhi_ctr",
            templateUrl: "Scripts/app/views/rizhi.html"
        })
           //发现
        .state("DiscoveryPage", {
            url: "/DiscoveryPage",
            controller: "DiscoveryPage_ctr",
            templateUrl: "Scripts/app/views/DiscoveryPage.html"
        })
          .state("rizhi.baidu_share_1", {
              url: "/baidu_share_1",
              controller: "baidu_share_ctr",
              templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_1.html"
          })
          .state("rizhi.baidu_share_2", {
              url: "/baidu_share_2",
              controller: "baidu_share_ctr",
              templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_2.html"
          })
           .state("rizhi.baidu_share_3", {
               url: "/baidu_share_3",
               controller: "baidu_share_ctr",
               templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_3.html"
           })
           
            .state("rizhi.baidu_share_6", {
                url: "/baidu_share_6",
                controller: "baidu_share_ctr",
                templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_6.html"
            })
            .state("rizhi.baidu_share_4", {
                url: "/baidu_share_4",
                controller: "baidu_share_2_ctr",
                templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_4.html"
            })
            .state("rizhi.baidu_share_5", {
                url: "/baidu_share_5",
                controller: "baidu_share_2_ctr",
                templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_5.html"
            })
          .state("rizhi.baidu_share_7", {
              url: "/baidu_share_7",
              controller: "baidu_share_2_ctr",
              templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_7.html"
          })
           .state("rizhi.baidu_share_8", {
               url: "/baidu_share_8",
               controller: "baidu_share_2_ctr",
               templateUrl: "Scripts/app/views/keywords/baidu_dashboard/baidu_share_8.html"
           })
        .state("DnL.DnL_domaincategory", {
            url: "/DnL_domaincategory",
            controller: "iw2s_domaincategory_ctr",
            templateUrl: "Scripts/app/views/keywords/iw2s_domaincategory.html"
        })
        .state("DnL.showDesc", {
            url: "/showDesc",
            controller: "showDesc_ctr",
            templateUrl: "Scripts/app/views/keywords/showDesc.html"
        })
        .state("DnL.keywordMng", {
            url: "/keywordMng",
            controller: "keywordMng_ctr",
            templateUrl: "Scripts/app/views/keywords/keywordMng.html"
        })
        .state("DnL.keywordViews", {
            url: "/keywordViews",
            controller: "keywordViews_ctr",
            templateUrl: "Scripts/app/views/keywords/keywordViews.html"
        })
        .state("DnL.linkCollection", {
            url: "/linkCollection",
            controller: "linkCollection_ctr",
            templateUrl: "Scripts/app/views/keywords/linkCollection.html"
        })
        .state("DnL.keywordConstrast", {
            url: "/keywordConstrast",
            controller: "keywordConstrast_ctr",
            templateUrl: "Scripts/app/views/keywords/keywordcConstrast.html"
        })
        .state("DnL.wordtree", {
            url: "/wordtree",
            controller: "wordtree_ctr",
            templateUrl: "Scripts/app/views/wordtree/wordtree2.html"
        })
        //数据分析
        .state("DnL.DataAnalysis", {
            url: "/DataAnalysis",
            controller: "DataAnalysis_ctr",
            templateUrl: "Scripts/app/views/Dashboard/DataAnalysis/DataAnalysis.html"
        })



        //bing

         .state("DnL_bing", {
             url: "/DnL_bing",
             controller: "iw2s_bing_ctr",
             templateUrl: "Scripts/app/views/bing/iw2s.html"
         })


     .state("DnL_bing.Bing_DnL_dashboard", {
         url: "/Bing_DnL_dashboard",
         controller: "Bing_dashboard_ctr",
         templateUrl: "Scripts/app/views/bing/iw2s_dashboard.html"
     })
     .state("DnL_bing.Bing_domaincategory", {
         url: "/Bing_domaincategory",
         controller: "Bing_domaincategory_ctr",
         templateUrl: "Scripts/app/views/bing/Bing_domaincategory.html"
     })
     .state("DnL_bing.Bing_keywordConstrast", {
         url: "/Bing_keywordConstrast",
         controller: "Bing_keywordConstrast_ctr",
         templateUrl: "Scripts/app/views/bing/keywordcConstrast.html"
     })
     .state("DnL_bing.Bing_keywordMng", {
         url: "/Bing_keywordMng",
         controller: "Bing_keywordMng_ctr",
         templateUrl: "Scripts/app/views/bing/keywordMng.html"
     })

     .state("DnL_bing.Bing_keywordViews", {
         url: "/Bing_keywordViews",
         controller: "Bing_keywordViews_ctr",
         templateUrl: "Scripts/app/views/bing/keywordViews.html"
     })

     .state("DnL_bing.Bing_linkCollection", {
         url: "/Bing_linkCollection",
         controller: "Bing_linkCollection_ctr",
         templateUrl: "Scripts/app/views/bing/linkCollection.html"
     })

     .state("DnL_bing.Bing_showDesc", {
         url: "/Bing_showDesc",
         controller: "Bing_showDesc_ctr",
         templateUrl: "Scripts/app/views/bing/showDesc.html"
     })
     .state("DnL_bing.Bing_iw2s_domaincategory", {
         url: "/Bing_iw2s_domaincategory",
         controller: "Google_domaincategory_ctr",
         templateUrl: "Scripts/app/views/google/iw2s_domaincategory.html"
     })
     .state("UserCenter", {
         url: "/UserCenter",
         controller: "UserCenter_ctr",
         templateUrl: "Scripts/app/views/UserCenter/UserCenter.html"
     })
    //新版用户后台
       .state("Dashboard", {
           url: "/Dashboard",
           controller: "Dashboard_ctr",
           templateUrl: "Scripts/app/views/Dashboard/Dashboard.html"
       })
        .state("Dashboard.Report", {
            url: "/Report",
            controller: "Report_ctr",
            templateUrl: "Scripts/app/views/Dashboard/Report/Report.html"
        })
       .state("Dashboard.Report.ReportList", {
           url: "/ReportList",
           controller: "ReportList_ctr",
           templateUrl: "Scripts/app/views/Dashboard/Report/ReportList.html"
       })
       .state("Dashboard.Report.ReportContents", {
           url: "/ReportContents",
           controller: "ReportContents_ctr",
           templateUrl: "Scripts/app/views/Dashboard/Report/ReportContents.html"
       })
         .state("Dashboard.addReport", {
             url: "/addReport",
             controller: "addReport_ctr",
             templateUrl: "Scripts/app/views/Dashboard/Report/addReport.html"
         })

        .state("Dashboard.PGDashboard", {
            url: "/PGDashboard",
            controller: "PGDashboard_ctr",
            templateUrl: "Scripts/app/views/Dashboard/projectGroup/PGDashboard.html"
        })
        .state("Dashboard.ProjectGroupList", {
            url: "/ProjectGroupList",
            controller: "ProjectGroupList_ctr",
            templateUrl: "Scripts/app/views/Dashboard/projectGroup/ProjectGroupList.html"
        })


        .state("shareReportHeader", {
            url: "/shareReportHeader",
            controller: "shareReportHeader_ctr",
            templateUrl: "Scripts/app/views/Dashboard/Report/shareReportHeader.html"
        })
        .state("shareReportHeader.ReportContentsShare", {
            url: "/ReportContentsShare",
            controller: "ReportContents_ctr",
            templateUrl: "Scripts/app/views/Dashboard/Report/ReportContentsShare.html"
        })




    //////////////////// Google

         .state("DnL_Google", {
             url: "/DnL_Google",
             controller: "Dnl_Google_ctr",
             templateUrl: "Scripts/app/views/google/iw2s.html"
         })


     .state("DnL_Google.Google_DnL_dashboard", {
         url: "/Google_DnL_dashboard",
         controller: "Google_dashboard_ctr",
         templateUrl: "Scripts/app/views/google/iw2s_dashboard.html"
     })
     .state("DnL_Google.Google_domaincategory", {
         url: "/Google_domaincategory",
         controller: "Google_domaincategory_ctr",
         templateUrl: "Scripts/app/views/google/Bing_domaincategory.html"
     })
     .state("DnL_Google.Google_keywordConstrast", {
         url: "/Google_keywordConstrast",
         controller: "Google_keywordConstrast_ctr",
         templateUrl: "Scripts/app/views/google/keywordcConstrast.html"
     })
     .state("DnL_Google.Google_keywordMng", {
         url: "/Google_keywordMng",
         controller: "Google_keywordMng_ctr",
         templateUrl: "Scripts/app/views/google/keywordMng.html"
     })

     .state("DnL_Google.Google_keywordViews", {
         url: "/Google_keywordViews",
         controller: "Google_keywordViews_ctr",
         templateUrl: "Scripts/app/views/google/keywordViews.html"
     })

     .state("DnL_Google.Google_linkCollection", {
         url: "/Google_linkCollection",
         controller: "Google_linkCollection_ctr",
         templateUrl: "Scripts/app/views/google/linkCollection.html"
     })

     .state("DnL_Google.Google_showDesc", {
         url: "/Google_showDesc",
         controller: "Google_showDesc_ctr",
         templateUrl: "Scripts/app/views/google/showDesc.html"
     })
     .state("DnL_Google.Google_iw2s_domaincategory", {
         url: "/Google_iw2s_domaincategory",
         controller: "Google_domaincategory_ctr",
         templateUrl: "Scripts/app/views/google/iw2s_domaincategory.html"
     })

    //////////// Goofle 结束

   // WeChat
        .state("WeChat", {
            url: "/WeChat",
            controller: "WeChat_ctr",
            templateUrl: "Scripts/app/views/Dashboard/WeChat/WeChat.html"
        })
        .state("WeChat.WC_dashboard", {
            url: "/WC_dashboard",
            controller: "WC_dashboard_ctr",
            templateUrl: "Scripts/app/views/Dashboard/WeChat/WC_dashboard.html"
        })
        .state("WeChat.WC_keywordMng", {
            url: "/WC_keywordMng",
            controller: "WC_keywordMng_ctr",
            templateUrl: "Scripts/app/views/Dashboard/WeChat/WC_keywordMng.html"
        })
        .state("WeChat.WC_keywordViews", {
            url: "/WC_keywordViews",
            controller: "WC_keywordViews_ctr",
            templateUrl: "Scripts/app/views/Dashboard/WeChat/WC_keywordViews.html"
        })
        .state("WeChat.WC_keywordcConstrast", {
            url: "/WC_keywordcConstrast",
            controller: "WC_keywordcConstrast_ctr",
            templateUrl: "Scripts/app/views/Dashboard/WeChat/WC_keywordcConstrast.html"
        })
        .state("WeChat.WC_domaincategory", {
            url: "/WC_domaincategory",
            controller: "WC_domaincategory_ctr",
            templateUrl: "Scripts/app/views/Dashboard/WeChat/WC_domaincategory.html"
        })
       .state("WeChat.WC_showDesc", {
           url: "/WC_showDesc",
           controller: "WC_showDesc_ctr",
           templateUrl: "Scripts/app/views/Dashboard/WeChat/WC_showDesc.html"
       })
      .state("WeChat.wordtree", {
          url: "/wordtree",
          controller: "wordtree_ctr",
          templateUrl: "Scripts/app/views/wordtree/wordtree2.html"
      })

      .state("WeChat.WC_DataAnalysis", {
          url: "/WC_DataAnalysis",
          controller: "DataAnalysis_ctr",
          templateUrl: "Scripts/app/views/Dashboard/DataAnalysis/WC_DataAnalysis.html"
      })
   
});


var chk_global_vars = function ($cookieStore, $rootScope, usr, $location, $http, myApplocalStorage) {

    $rootScope.userID = $cookieStore.get("userID");
    $rootScope.reload_LS = $cookieStore.get("reload_LS");
    if (usr != null) {
        $rootScope.userID = usr._id;
        $rootScope.LoginName = usr.LoginName;
        $rootScope.UsrRole = usr.UsrRole;
        $rootScope.UsrKey = usr.UsrKey;
        $rootScope.UsrNum = usr.UsrNum;
        $rootScope.UsrEmail = usr.UsrEmail;
        $rootScope.uer_PictureSrc = usr.PictureSrc;
        $rootScope.logined = true;

        $rootScope.Gender = usr.Gender;
        $rootScope.MobileNo = usr.MobileNo;
        $rootScope.Remarks = usr.Remarks;

        // $rootScope.getProjectId = "";
        // $http.defaults.headers.common['Authorization'] = 'Basic ' + usr.Token;
        $cookieStore.put("userID", usr._id);
        $cookieStore.put("LoginName", usr.LoginName);
        $cookieStore.put("UsrRole", usr.UsrRole);
        $cookieStore.put("UsrKey", usr.UsrKey);
        $cookieStore.put("UsrNum", usr.UsrNum);
        $cookieStore.put("UsrEmail", usr.UsrEmail);
        $cookieStore.put("uer_PictureSrc", usr.PictureSrc);
        $cookieStore.put("logined", true);

        $cookieStore.put("Gender", $rootScope.Gender);
        $cookieStore.put("MobileNo", $rootScope.MobileNo);
        $cookieStore.put("Remarks", $rootScope.Remarks);

        //
        $rootScope.selectStatus = "直搜"
        $rootScope.pagesizeBaidu = 10;

        $cookieStore.put("selectStatus", $rootScope.selectStatus);
        $cookieStore.put("pagesizeBaidu", $rootScope.pagesizeBaidu);
        $rootScope.reload_LS = $cookieStore.get("reload_LS");

    } else if (typeof $rootScope.logined == "undefined" || $rootScope.logined == null || $rootScope.logined == false) {
        //用户
        $rootScope.userID = $cookieStore.get("userID");
        $rootScope.LoginName = $cookieStore.get("LoginName");
        $rootScope.UsrRole = $cookieStore.get("UsrRole");
        $rootScope.UsrKey = $cookieStore.get("UsrKey");
        $rootScope.UsrNum = $cookieStore.get("UsrNum");
        $rootScope.UsrEmail = $cookieStore.get("UsrEmail");
        $rootScope.uer_PictureSrc = $cookieStore.get("uer_PictureSrc");
        $rootScope.Gender = $cookieStore.get("Gender");
        $rootScope.MobileNo = $cookieStore.get("MobileNo");
        $rootScope.Remarks = $cookieStore.get("Remarks");

        //页面
        $rootScope.keyword = $cookieStore.get("keyword");
        $rootScope.keywordName = $cookieStore.get("keywordName");
        //changexinyuan
        $rootScope.BaidukeywordId = $cookieStore.get("BaidukeywordId");
        $rootScope.keywordsListRecord = $cookieStore.get("keywordsListRecord");
        $rootScope.getBaiduRecordId = $cookieStore.get("getBaiduRecordId");
        $rootScope.pagesizeBaidu = $cookieStore.get("pagesizeBaidu");
        $rootScope.isActiveLoadmore = $cookieStore.get("isActiveLoadmore");
        $rootScope.getBaiduRecordName = $cookieStore.get("getBaiduRecordName");
        $rootScope.getProjectId = $cookieStore.get("getProjectId");
        $rootScope.getProjectName = $cookieStore.get("getProjectName");
        $rootScope.keywordMngClicked = $cookieStore.get("keywordMngClicked");
        $rootScope.logined = $cookieStore.get("logined");
        $rootScope.keyId = $cookieStore.get("keyIds");
        $rootScope.keyName = $cookieStore.get("keyNames");
        $rootScope.reload_LS = $cookieStore.get("reload_LS");
        $rootScope.projectsList = myApplocalStorage.getObject("projectsList");//$cookieStore.get("projectsList");
        $rootScope.isActiveModale = $cookieStore.get("isActiveModale");
        $rootScope.PrjAnalysisItemName_list = $cookieStore.get("PrjAnalysisItemName_list");
        $rootScope.GetPrjAnalysisItemName_list_name = $cookieStore.get("GetPrjAnalysisItemName_list_name");
        $rootScope.searchSrc = $cookieStore.get("searchSrc");
        $rootScope.picRs_id = $cookieStore.get("picRs_id");
        $rootScope.currentId = $cookieStore.get("currentId");
        $rootScope.currentName = $cookieStore.get("currentName");
        //report
        $rootScope.keXiuGai = $cookieStore.get("keXiuGai");
        $rootScope.selsetReport = $cookieStore.get("selsetReport");
        $rootScope.reportModal_LX = $cookieStore.get("reportModal_LX");
        $rootScope.getProjectGroupId = $cookieStore.get("getProjectGroupId");
        $rootScope.getProjectGroupName = $cookieStore.get("getProjectGroupName");
        $rootScope.getPGPIdS = $cookieStore.get("getPGPIdS");
        
    }

    //判断有没有登录&&是否有cookie
    //var str = window.location.href;

    //var tag = '/project/';
    //if (str.indexOf(tag) != -1) {
    //    if ($rootScope.logined == "undefined" || $rootScope.logined == null || $rootScope.logined == false) {
    //        alert("登录过期,请重新登陆!!");
    //        $location.path("/home/login").replace();
    //        $rootScope.projectID = "";
    //        $rootScope.poster_name_kwd = ""
    //        $rootScope.projectName = ""
    //        $rootScope.projectName2 = ""
    //        $rootScope.isProject = ""
    //        $rootScope.timeChange = ""
    //        $rootScope.type = ""
    //        $rootScope.CompanyID = ""
    //        $rootScope.CompanyName = ""
    //        $rootScope.Email = ""
    //        $rootScope.FriendlyName = ""
    //        $rootScope.IsApproved = ""
    //        $rootScope.IsCustAdmin = ""
    //        $rootScope.IsIPRSEESLUser = ""
    //        $rootScope.IsMDMAdmin = ""
    //        $rootScope.IsNoRoleUser = ""
    //        $rootScope.IsSinoFaithUser = ""
    //        $rootScope.IsSuccess = ""
    //        $rootScope.IsVendor = ""
    //        $rootScope.Position = ""
    //        $rootScope.Role = ""
    //        $rootScope.logined = ""
    //        $cookieStore.remove("projectID");
    //        $cookieStore.remove("userChange");
    //        $cookieStore.remove("poster_name_kwd");
    //        $cookieStore.remove("projectName");
    //        $cookieStore.remove("projectName2");
    //        $cookieStore.remove("isProject");
    //        $cookieStore.remove("timeChange");
    //        $cookieStore.remove("type");
    //        $cookieStore.remove("CompanyID");
    //        $cookieStore.remove("CompanyName");
    //        $cookieStore.remove("Email");
    //        $cookieStore.remove("FriendlyName");
    //        $cookieStore.remove("IsApproved");
    //        $cookieStore.remove("IsCustAdmin");
    //        $cookieStore.remove("IsIPRSEESLUser");
    //        $cookieStore.remove("IsMDMAdmin");
    //        $cookieStore.remove("IsNoRoleUser");
    //        $cookieStore.remove("IsSinoFaithUser");
    //        $cookieStore.remove("IsSuccess");
    //        $cookieStore.remove("IsVendor");
    //        $cookieStore.remove("Position");
    //        $cookieStore.remove("Role");
    //        $cookieStore.remove("logined");

    //    }
    //}


}
$.goup({
    trigger: 100,
    bottomOffset: 20,
    locationOffset: 30,
    title: '回到顶部',
    titleAsText: false
});



