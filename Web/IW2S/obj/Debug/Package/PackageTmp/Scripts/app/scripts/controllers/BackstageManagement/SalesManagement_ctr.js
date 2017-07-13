var SalesManagement_ctr = myApp.controller("SalesManagement_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, myApplocalStorage) {
   
    $scope.show_list = 1;
    $scope.InsertProduct = {
        name: '',
        keywords: '',
        price: '',
    };
    $scope.page = 0;
    $scope.pagesize = 100;
    $scope.changeProductIf = false;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    //\\________________________________________________________________

    //切换
    $scope.show_list_fun = function (num) {
        $scope.show_list = num;
        if (num == 1) {
            $scope.GetProduct();
            $scope.changeProductIf = false;
        } else if (num == 2) {

        }
    }
    //取消
    $scope.quxiao = function () {
        $scope.show_list_fun(1);
        $scope.InsertProduct = {
            name: '',
            keywords: '',
            price: '',
        };
    }
    //插入产品
    
    $scope.InsertProductFun = function () {
        if ($scope.InsertProduct.name == "" || $scope.InsertProduct.name == null) {
            $scope.alert_fun('waring', '请输入名称');
        } else if ($scope.InsertProduct.description == "" || $scope.InsertProduct.description == null) {
            $scope.alert_fun('waring', '请输入描述');
        } else if ($scope.InsertProduct.price == "" || $scope.InsertProduct.price == null) {
            $scope.alert_fun('waring', '请输入价格');
        } else {
            $scope.paramsList = {
                id: '',
                name: $scope.InsertProduct.name,
                description: $scope.InsertProduct.description,
                price: $scope.InsertProduct.price,
            };
            var urls = "/api/Pay/InsertProduct";
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
                    $scope.alert_fun('success', "添加成功！");
                    $scope.show_list_fun(1);
                } else {
                    $scope.alert_fun('danger', response.Message);
                }
            });
            q.error(function (e) {
                alert("网络打盹了，请稍后。。。");
            });
        }
    }
    //2.2获取产品
    $scope.GetProduct = function () {
        var url = "/api/Pay/GetProduct?page=" + $scope.page + "&pagesize=" + $scope.pagesize;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.GetProductList = response;
            $scope.GetProductListCount = response.length;
        });
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
        });
    }
    //更新产品
   
    $scope.changeProduct = function (x) {
        $scope.InsertProduct.name = x.Name;
        $scope.InsertProduct.description = x.Description;
        $scope.InsertProduct.price = x.Price;
        $scope.UpdateProductId = x.Id;
        $scope.changeProductIf = true;
        $scope.show_list_fun(2);
    }
    $scope.UpdateProduct = function (id) {
        if ($scope.InsertProduct.name == "" || $scope.InsertProduct.name == null) {
            $scope.alert_fun('waring', '请输入名称');
        } else if ($scope.InsertProduct.description == "" || $scope.InsertProduct.description == null) {
            $scope.alert_fun('waring', '请输入描述');
        } else if ($scope.InsertProduct.price == "" || $scope.InsertProduct.price == null) {
            $scope.alert_fun('waring', '请输入价格');
        } else {
            $scope.InsertProduct.price = parseInt($scope.InsertProduct.price)
            $scope.paramsList = {
                id: $scope.UpdateProductId,
                name: $scope.InsertProduct.name,
                description: $scope.InsertProduct.description,
                price: $scope.InsertProduct.price,
            };
            var urls = "/api/Pay/UpdateProduct";
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
                    $scope.alert_fun('success', "修改成功！");
                    $scope.changeProductIf = false;
                    $scope.show_list_fun(1);
                } else {
                    $scope.alert_fun('danger', response.Message);
                }
            });
            q.error(function (e) {
                alert("网络打盹了，请稍后。。。");
            });
        }
    }
   
    //2.2删除产品
    $scope.DelProduct = function (id) {
        if (confirm("您确定要删除该产品吗")) {
            var url = "/api/Pay/DelProduct?productId=" + id;
            var q = $http.get(url);
            q.success(function (response, status) {
                if (response.IsSuccess == true) {
                    $scope.alert_fun('success', "删除成功！");
                    $scope.GetProduct();
                } else {
                    $scope.alert_fun('danger', response.Message);
                }
            });
            q.error(function (response) {
                $scope.error = "网络打盹了，请稍后。。。";
            });
        }
    }


    //-----------------------------------------------------------------------
   
    $scope.GetProduct();
});