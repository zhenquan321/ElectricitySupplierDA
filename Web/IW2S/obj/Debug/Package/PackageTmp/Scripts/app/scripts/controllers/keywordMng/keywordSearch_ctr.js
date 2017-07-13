var keywordSearch_ctr = myApp.controller("keywordSearch_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, myApplocalStorage) {
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //1.关键词管理---保存
    $scope.ExcludeKeyword = function () {
        $scope.list_submitAll = [];
        for (var i = 0; i < $rootScope.BaiduCommendKeywordList.length; i++) {
            if ($rootScope.BaiduCommendKeywordList[i].IsRemoved == true) {
                $scope.list_submitAll.push($scope.BaiduCommendKeywordList[i]);
            }
            $rootScope.BaiduCommendKeywordList[i].IsRemoved = false;
        }
        for (var i = 0; i < $rootScope.ExcludeBaiduCommendList.length; i++) {
            if ($rootScope.ExcludeBaiduCommendList[i].IsRemoved == false) {
                $scope.list_submitAll.push($rootScope.ExcludeBaiduCommendList[i]);
            }
            $rootScope.ExcludeBaiduCommendList[i].IsRemoved = true;
        }

        //console.log("list_submitAll");
        //console.log($scope.list_submitAll);

        if ($scope.list_submitAll.length > 0) {


            var urls = "api/Keyword/ExcludeKeyword";
            var q = $http.post(
                urls,
                JSON.stringify($scope.list_submitAll),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            )
            q.success(function (response, status) {
                console.log('keywordSearch_ctr>ExcludeKeyword');
                //console.log(response);

            });
            q.error(function (e) {
                alert("网络打盹了，请稍后。。。")


            });
        } else {
            $scope.error = "数据没有变化,请改变数据原本位置";
        }

    }

});