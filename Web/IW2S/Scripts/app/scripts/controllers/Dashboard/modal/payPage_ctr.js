var payPage_ctr = function ($scope, $modalInstance, $rootScope, $http, $timeout, $interval) {


    $scope.ok = function () {
        $modalInstance.close($scope.selected);
        $scope.$on('$destroy', function () {
            $interval.cancel(timeoutGOS);
        })
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
        if (!$scope.Order.ifadd) {
            $scope.DelOrder($scope.Order.order.Id);
        }
        $scope.$on('$destroy', function () {
            $interval.cancel(timeoutGOS);
        })
    };

    //判断是否支付完成
    $scope.GetOrderStatus = function () {
        var url = "/api/Pay/GetOrderStatus?orderId=" + $scope.Order.order.Id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response) {
                $scope.ok();
                $rootScope.addAlert('success', "支付完成，感谢您的支持！我们将为您提供最优质的服务！");
            }else{
                timeoutGOS;
            }
        });
        q.error(function (response) {
            $scope.error = "接口调用连接出错";
        });
    };
    var timeoutGOS = $interval(function () { $scope.GetOrderStatus(); }, 500);

    $scope.GetOrderStatus();

    //删除订单
    $scope.DelOrder = function (id) {
        if (confirm("您需要将此产品列入待支付订单吗？")) {
            $scope.alert_fun('success', '加入成功，之后您可在订单服务处再次选择支付；');
        } else {
            var url = "/api/Pay/DelOrder?orderId=" + id;
            var q = $http.get(url);
            q.success(function (response, status) {
            });
            q.error(function (response) {
                $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
            });
        }
    }

}