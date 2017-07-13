/// <reference path="app/view/keywords/keywordcConstrast.html" />
/// <reference path="app/view/keywords/keywordcConstrast.html" />
/// <reference path="app/view/keywordMng/keywordSearch.html" />

var myApp = angular.module("myApp", [
  'ngAnimate',
  "ngCookies",
  'ui.router',
  "ngMessages",
  'angular-loading-bar',
  "ui.bootstrap"]);

myApp.filter('htmlContent', ['$sce', function ($sce) {
    return function (input) {
        return $sce.trustAsHtml(input);
    }
}]);

//angular路由
myApp.config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/home");
    $stateProvider
    .state("home", {
        url: "/home",
        controller: "home_ctr",
        templateUrl: "Scripts/app/view/home/home.html"
    })
    .state("login", {
        url: "/login",
        controller: "login_ctr",
        templateUrl: "Scripts/app/view/home_ls/login.html"
    })
    .state("signup", {
        url: "/signup",
        controller: "signup_ctr",
        templateUrl: "Scripts/app/view/home_ls/signup.html"
    })
     .state("reset", {
         url: "/reset",
         controller: "reset_ctr",
         templateUrl: "Scripts/app/view/home_ls/reset.html"
     })

     .state("main", {
         url: "/main",
         controller: "main_ctr",
         templateUrl: "Scripts/app/view/dashboard/main.html"
     })
     //articles
    .state("main.PublishArticles", {
        url: "/PublishArticles",
        controller: "PublishArticles_ctr",
        templateUrl: "Scripts/app/view/dashboard/articles/PublishArticles.html"
    })
    .state("main.Published_Articles", {
        url: "/Published_Articles",
        controller: "Published_Articles_ctr",
        templateUrl: "Scripts/app/view/dashboard/articles/Published_Articles.html"
    })
    .state("main.PArticlesCun", {
        url: "/PArticlesCun",
        controller: "PArticlesCun_ctr",
        templateUrl: "Scripts/app/view/dashboard/articles/PArticlesCun.html"
    })
    //userService
     .state("main.OptimalEvaluation", {
         url: "/OptimalEvaluation",
         controller: "OptimalEvaluation_ctr",
         templateUrl: "Scripts/app/view/dashboard/userService/OptimalEvaluation.html"
     })
    .state("main.UserShare", {
        url: "/UserShare",
        controller: "UserShare_ctr",
        templateUrl: "Scripts/app/view/dashboard/userService/UserShare.html"
    })
  
    //TwoLevelPage
     .state("TwoLevelPage", {
         url: "/TwoLevelPage",
         controller: "TwoLevelPage_ctr",
         templateUrl: "Scripts/app/view/TwoLevelPage/TwoLevelPage.html"
     })
      .state("TwoLevelPage.HotelFeatures", {
          url: "/HotelFeatures",
          controller: "HotelFeatures_ctr",
          templateUrl: "Scripts/app/view/TwoLevelPage/HotelFeatures.html"
      })
     .state("TwoLevelPage.LocalStyle", {
         url: "/LocalStyle",
         controller: "LocalStyle_ctr",
         templateUrl: "Scripts/app/view/TwoLevelPage/LocalStyle.html"
     })
     .state("TwoLevelPage.SharedFeeling", {
         url: "/SharedFeeling",
         controller: "SharedFeeling_ctr",
         templateUrl: "Scripts/app/view/TwoLevelPage/SharedFeeling.html"
     })
     .state("AboutUs", {
         url: "/AboutUs",
         controller: "AboutUs_ctr",
         templateUrl: "Scripts/app/view/TwoLevelPage/AboutUs.html"
     })
      //用户中心
    .state("TwoLevelPage.userCenter", {
        url: "/userCenter",
        controller: "UserCenter_ctr",
        templateUrl: "Scripts/app/view/TwoLevelPage/userCenter.html"
    })


  
});


var chk_global_vars = function ($cookieStore, $rootScope, usr, $location, $http) {


    $rootScope.reload_LS = $cookieStore.get("reload_LS");
    if (usr != null) {
        $rootScope.userID = usr.ID;
        $rootScope.LoginName = usr.LoginName;
        $rootScope.UsrRole = usr.RoleId;
        $rootScope.NickName = usr.NickName;
        $rootScope.UserPhone = usr.UserPhone;
        $rootScope.UserEmail = usr.UserEmail;
        $rootScope.HeadIcon = usr.HeadIcon;
        $rootScope.CreatedAt_user = usr.CreatedAt;
        
        $rootScope.logined = true;
        
        //_____________________________________
        $cookieStore.put("userID", usr.ID);
        $cookieStore.put("LoginName", usr.LoginName);
        $cookieStore.put("UsrRole", usr.RoleId);
        $cookieStore.put("NickName", usr.NickName);
        $cookieStore.put("UserPhone", usr.UserPhone);
        $cookieStore.put("UsrEmail", usr.UserEmail);
        $cookieStore.put("HeadIcon", usr.HeadIcon);
        $cookieStore.put("CreatedAt_user", usr.CreatedAt);
        $cookieStore.put("logined", true);




    } else if (typeof $rootScope.logined == "undefined" || $rootScope.logined == null || $rootScope.logined == false) {
       //用户
        $rootScope.userID = $cookieStore.get("userID");
        $rootScope.LoginName = $cookieStore.get("LoginName");
        $rootScope.UsrRole = $cookieStore.get("UsrRole");
        $rootScope.NickName = $cookieStore.get("NickName");
        $rootScope.UserPhone = $cookieStore.get("UserPhone");
        $rootScope.UserEmail = $cookieStore.get("UserEmail");
        $rootScope.HeadIcon = $cookieStore.get("HeadIcon");
        $rootScope.CreatedAt_user = $cookieStore.get("CreatedAt_user");
        $rootScope.logined = $cookieStore.get("logined");
        //页面
      

    }




}



