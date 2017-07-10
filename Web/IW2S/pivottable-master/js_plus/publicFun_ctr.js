var publicFun_ctr = myApp.controller("publicFun_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {
  

    $scope.showAmendAndOff = false;
    $scope.CaseInquiry = "";
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    $scope.alert_warning = false;
    $scope.alert_success = false;
    $scope.alert_danger = false;
    $scope.alert_info = false;
    $scope.alert_warning_n = '';
    $scope.alert_success_n = '';
    $scope.alert_danger_n = "";
    $scope.alert_info_n = "";

    $scope.alert_fun = function (lei, nei) {
        $scope.alert_close_fun();
        if (lei == "warning") {
            $scope.alert_warning = true;
            $scope.alert_warning_n = nei;
        } else if (lei == "danger") {
            $scope.alert_danger = true;
            $scope.alert_danger_n = nei;
        } else if (lei == "success") {
            $scope.alert_success = true;
            $scope.alert_success_n = nei;
        } else if (lei == "info") {
            $scope.alert_info = true;
            $scope.alert_info_n = nei;
        };
        $timeout(function () {
            $scope.alert_close_fun()
        }, 10000);
    };
    $scope.alert_close_fun = function () {
        $scope.alert_warning = false;
        $scope.alert_success = false;
        $scope.alert_danger = false;
        $scope.alert_info = false;
    };
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //切换项目
    // <a class="btn cc ">电商数据分析</a>  1
    // <a class="btn cc ">测试购买流程</a>  2
    // <a class="btn cc ">目标策略规划</a>  3
    // <a class="btn cc ">情报分享讨论</a>  4
    // <a class="btn cc ">案件数据查询</a>  5
    // <a class="btn cc ">执行绩效管理</a>  6
    //跳转子系统
    $scope.in_xitong = function (num) {
        var domain = window.location.host;
        domain = 'http://' + domain;
        if ($rootScope.logined_nm) {
            if (num == 1) {
                window.location.href=domain + "/#/home/main";
            } else if (num == 2) {

            } else if (num == 3) {
                $scope.alert_fun('info', '非常抱歉，此功能我们还在奋力开发中，尚没有开通，我们会尽快发布，敬请期待！');
            } else if (num == 4) {
                domain =domain + "/#/ShareDiscussion";
                window.location.href=domain;
            } else if (num == 41) {
                window.location.replace( domain + "/#/ShareDiscussion");
            } else if (num == 5) {
                window.location.replace(domain + "/#/CaseMag_main/CaseInquiry");
            } else if (num == 51) {
                window.location.replace(domain + "/#/CaseMag_main/DataImport");
            } else if (num == 6) {
                window.location.replace(domain + "/#/OMD_main/achievement");
            }
            else if (num == 7) {   //数据清洗
                window.location.replace(domain + "/#/Data_clear_main/ClearMain");
            }  
            else if (num == 8) {   //测试购买
                var test = window.open("http://worx2.iprsee.cn/#/home/login?id=" + $rootScope.user_Id, "_blank", "");
            }
            else if (num == 9) {   //console
                var test = window.open("http://console.iprsee.cn/#/login?id=" + $rootScope.user_Id, "_blank", "");
            }
            else if (num == 10) {   //案件流程管理
                window.location.replace(domain + "/#/CasePcs_main/CasePcs_CaseInquiry");
            }
            else if (num == 11) {   //客户服务登记表
                window.location.href = '/pivottable-master/examples/c4.html';
            }
            else {
                $scope.alert_fun('info', '非常抱歉，此功能我们还在奋力开发中，尚没有开通，我们会尽快发布，敬请期待！');
            }
        } else {
            window.location.replace = domain + "/#/home/login";
        }

        $scope.selected_page_fun();
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    //header 方法————————————————————————————————
    $scope.showamendpwd = function (num) {
        $rootScope.showAmendAndOff = num;
    }
    $scope.hideamendpwd = function (num) {
        $rootScope.showAmendAndOff = false;
    }

    //选择自系统
    $scope.selected_page_fun = function () {
        $timeout(function () {
            $scope.selected_page_fun_1()
        }, 600);
    }
    $scope.selected_page_fun();
    $scope.selected_page_fun_1 = function () {
        var dizhi = window.location.hash;
        var arr = dizhi.split('/');
        if (arr[1] == "OMD_main") {
            $rootScope.selected_page_2 = '业绩分析';
            $rootScope.selected_page = 'IPRWorx';
        } else if (arr[1] == 'home') {
            $rootScope.selected_page = 'IPRWorx';
            $rootScope.selected_page_2 = '运营管理系统';
        } else if (arr[1] == 'CaseMag_main') {
            $rootScope.selected_page = '案件数据查询';
            $rootScope.selected_page_2 = '运营管理系统';
        } else if (arr[1] == 'ShareDiscussion') {
            $rootScope.selected_page = '情报分享讨论';
            $rootScope.selected_page_2 = '运营管理系统';
        } else if (arr[0] == 'pivottable-master') {
            $rootScope.selected_page = '数据分析系统';
            $rootScope.selected_page_2 = '运营管理系统';
        } else if (arr[1] == "Data_clear_main") {
            $rootScope.selected_page = 'IPRWorx';
            $rootScope.selected_page_2 = '运营管理系统';
        }
    }

    //退出-----------------------------------------------------------------------
    $rootScope.signout = function () {
      
        $rootScope.ID = '';
        $rootScope.user_Id = '';
        $rootScope.LoginName = '';
        $rootScope.UserCompanyID = '';
        $rootScope.uer_PictureSrc = '';
        $rootScope.Email = '';
        $rootScope.CompanyName = '';
        $rootScope.Phone = '';
        $rootScope.IsIPRSEESLUser = '';
        $rootScope.Role = '';
        $rootScope.Position = '';
        $rootScope.UsrRole = '';
        $rootScope.applicationState = '';
        $rootScope.UsrNum = '';
        $rootScope.IsEmailConfirmed = '';
        $rootScope.logined_nm = '';
        $rootScope.add_keywords_112 = '';
        $rootScope.IsCustAdmin = '';
        $rootScope.IsMDMAdmin = '';
        $rootScope.IsSinoFaithUser = '';
        $rootScope.IsVendor = '';
        $rootScope.IsIPSUser = '';
        $rootScope.IsWorx = '';
        $rootScope.IsConsoleUser = '';
        $rootScope.IsConsoleAdmin = '';
        $rootScope.companyID = '';
        //------------------------------
        $cookieStore.remove('ID');
        $cookieStore.remove('user_Id');
        $cookieStore.remove('LoginName');
        $cookieStore.remove('UserCompanyID');
        $cookieStore.remove('uer_PictureSrc');
        $cookieStore.remove('Email');
        $cookieStore.remove('CompanyName');
        $cookieStore.remove('Phone');
        $cookieStore.remove('IsIPRSEESLUser');
        $cookieStore.remove('Role');
        $cookieStore.remove('Position');
        $cookieStore.remove('UsrRole');
        $cookieStore.remove('applicationState');
        $cookieStore.remove('UsrNum');
        $cookieStore.remove('IsEmailConfirmed');
        $cookieStore.remove('logined_nm');
        $cookieStore.remove('add_keywords_112');
        $cookieStore.remove('IsCustAdmin');
        $cookieStore.remove('IsMDMAdmin');
        $cookieStore.remove('IsSinoFaithUser');
        $cookieStore.remove('IsVendor');
        $cookieStore.remove('IsIPSUser');
        $cookieStore.remove('IsWorx');
        $cookieStore.remove('IsConsoleUser');
        $cookieStore.remove('IsConsoleAdmin');
        $cookieStore.remove('companyID');
        $scope.in_xitong(1);
        chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++
 
});