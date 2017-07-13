/// <reference path="" />
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
   



});

var chk_global_vars = function ($cookieStore, $rootScope, usr, $location, $http) {
  if (usr != null) {
        $rootScope.ID = usr.ID;
        $rootScope.user_Id = usr.Uid;
        $rootScope.UserCompanyID = usr.UserCompanyID;
        $rootScope.uer_PictureSrc = usr.HeadIcon;
        $rootScope.LoginName = usr.FriendlyName;
        $cookieStore.put("NickName", usr.NickName);
        $rootScope.Email = usr.Email;
        $rootScope.CompanyName = usr.CompanyName;
        $rootScope.Phone = usr.Phone;
        $rootScope.IsIPRSEESLUser = usr.IsIPRSEESLUser;
        $rootScope.Role = usr.Role;
        $rootScope.Position = usr.Position;
        $rootScope.companyID = usr.UserCompanyID;
        $rootScope.VerifyCode = usr.VerifyCode;
        $rootScope.logined_nm = true;
      
      // $http.defaults.headers.common['Authorization'] = 'Basic ' + usr.Token;
        $cookieStore.put("NickName", usr.NickName);
        $cookieStore.put("ID", usr.ID);
        $cookieStore.put("user_Id", usr.Uid);
        $cookieStore.put("UserCompanyID", usr.UserCompanyID);
        $cookieStore.put("uer_PictureSrc", usr.HeadIcon);
        $cookieStore.put("LoginName", usr.FriendlyName);
        $cookieStore.put("Email", usr.Email);
        $cookieStore.put("CompanyName", usr.CompanyName);
        $cookieStore.put("Phone", usr.Phone);
        $cookieStore.put("IsIPRSEESLUser", usr.IsIPRSEESLUser);
        $cookieStore.put("applicationState", usr.applicationState);
        $cookieStore.put("Role", usr.Role);
        $cookieStore.put("Position", usr.Position);
        $cookieStore.put('logined', true);
        $cookieStore.put('companyID',  usr.UserCompanyID);
        $cookieStore.put("IsCustAdmin", usr.IsCustAdmin);
        $cookieStore.put("IsMDMAdmin", usr.IsMDMAdmin);
        $cookieStore.put("IsSinoFaithUser", usr.IsSinoFaithUser);
        $cookieStore.put("IsVendor", usr.IsVendor);
        $cookieStore.put("IsIPSUser", usr.IsIPSUser);
        $cookieStore.put("IsWorx", usr.IsWorx);
        $cookieStore.put("IsConsoleUser", usr.IsConsoleUser);
        $cookieStore.put("IsConsoleAdmin", usr.IsConsoleAdmin);
        $cookieStore.put("logined_nm",true);
       

  } else if (typeof $rootScope.logined_nm == "undefined" || $rootScope.logined_nm == null || $rootScope.logined_nm == false) {
   
    //用户-------------------------------------------------------------------------
      $rootScope.ID = $cookieStore.get("ID");
      $rootScope.NickName = $cookieStore.get("NickName");
        $rootScope.user_Id = $cookieStore.get("user_Id");
        $rootScope.LoginName = $cookieStore.get("LoginName");
        $rootScope.UserCompanyID = $cookieStore.get("UserCompanyID");
        $rootScope.uer_PictureSrc = $cookieStore.get("uer_PictureSrc");
        $rootScope.Email = $cookieStore.get("Email");
        $rootScope.CompanyName = $cookieStore.get("CompanyName");
        $rootScope.Phone = $cookieStore.get("Phone");
        $rootScope.IsIPRSEESLUser = $cookieStore.get("IsIPRSEESLUser");
        $rootScope.Role = $cookieStore.get("Role");
        $rootScope.Position = $cookieStore.get("Position");
        $rootScope.UsrRole = $cookieStore.get("UsrRole");
        $rootScope.applicationState = $cookieStore.get("applicationState");
        $rootScope.UsrNum = $cookieStore.get("UsrNum");
        $rootScope.IsEmailConfirmed = $cookieStore.get("IsEmailConfirmed");
        $rootScope.logined_nm = $cookieStore.get("logined_nm");
        $rootScope.add_keywords_112 = $cookieStore.get("add_keywords_112");
        $rootScope.IsCustAdmin = $cookieStore.get("IsCustAdmin");
        $rootScope.IsMDMAdmin = $cookieStore.get("IsMDMAdmin");
        $rootScope.IsSinoFaithUser = $cookieStore.get("IsSinoFaithUser");
        $rootScope.IsVendor = $cookieStore.get("IsVendor");
        $rootScope.IsIPSUser = $cookieStore.get("IsIPSUser");
        $rootScope.IsWorx = $cookieStore.get("IsWorx");
        $rootScope.IsConsoleUser = $cookieStore.get("IsConsoleUser");
        $rootScope.IsConsoleAdmin = $cookieStore.get("IsConsoleAdmin");
        $rootScope.companyID = $cookieStore.get("companyID");
      //其他---------------------------------------------------------------------------
       
    }


    //判断有没有登录&&是否有cookie
    var str = window.location.href;

    var tag = '/main/';
    if (str.indexOf(tag) != -1) {
        if ($rootScope.logined_nm == "undefined" || $rootScope.logined_nm == null || $rootScope.logined_nm == false) {
            $location.path("/login").replace();
            $rootScope.user_Id = "";
            $rootScope.LoginName = "";
            $rootScope.LoginPwd = "";
            $rootScope.UsrEmail = "";
            $rootScope.Token = "";
            $rootScope.UsrKey = "";
            $rootScope.UsrRole = "";
            $rootScope.applicationState = "";
            $rootScope.UsrNum = "";
            $rootScope.IsEmailConfirmed = "";
            $rootScope.logined_nm = false;
            $cookieStore.remove("iskeyNull");
            $cookieStore.remove("keyIds");
            $cookieStore.remove("keyNames");

            $cookieStore.remove("user_Id");
            $cookieStore.remove("LoginName");
            $cookieStore.remove("LoginPwd");
            $cookieStore.remove("logined_nm");
            $cookieStore.remove("UsrEmail");
            $cookieStore.remove("Token");
            $cookieStore.remove("UsrKey");
            $cookieStore.remove("UsrRole");
            $cookieStore.remove("applicationState");
            $cookieStore.remove("UsrNum");
            $cookieStore.remove("IsEmailConfirmed");
            $cookieStore.remove("logined_nm");
        }
  }

}
//$.goup({
//  trigger: 100,
//  bottomOffset: 20,
//  locationOffset: 30,
//  title: '回到顶部',
//  titleAsText: false
//});



