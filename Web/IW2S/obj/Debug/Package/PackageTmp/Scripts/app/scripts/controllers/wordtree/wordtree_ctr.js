var wordtree_ctr = myApp.controller("wordtree_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, myApplocalStorage) {
<<<<<<< HEAD

    $scope.duibiShow = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

=======
    $scope.GetLinkContentN = '';
    $scope.duibiShow = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    document.documentElement.scrollTop = document.body.scrollTop = 0;
>>>>>>> c26f92d240a523a1903a8e87db204683ad299860

    $scope.duibiShowFun = function () {
        $scope.duibiShow = !$scope.duibiShow;

<<<<<<< HEAD



    }













=======
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var str = location.href; 
        var num = str.indexOf("?")
        str = str.substr(num + 1);
        var r = str.match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    //2.2获取话语分析文章
    $scope.GetBaiduKeyword = function () {
        var linkId = GetQueryString('id');
        var score = GetQueryString('score');
        if (!linkId) {
            return
        }
        if (score==1) {
            var url = "/api/Keyword/GetLinkContent?linkId=" + linkId;
        } else if (score == 2) {
            var url = "/api/Media/GetLinkContent?linkId=" + linkId;
        }
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetLinkContentN = response;
        });
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
        });
    }


    //_________________________________________________________________________
    $scope.GetBaiduKeyword();
>>>>>>> c26f92d240a523a1903a8e87db204683ad299860


});