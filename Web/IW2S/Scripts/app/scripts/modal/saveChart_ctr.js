var saveChart_ctr = function($scope, $modalInstance, $rootScope, $http) {

	$scope.isactive = "";
	//______________________________________________________________________
	$scope.ok = function() {
		$modalInstance.close($scope.selected);
	};
	$scope.cancel = function() {
		$modalInstance.dismiss('cancel');
	};

	$scope.EnterPress = function() {
		var e = e || window.event;
		if (e.keyCode == 13) {
			$scope.SaveChart();
		}
	}

	$scope.SaveChart = function () {
	    var url = "";
	    if ($scope.isReport) {
	        url = "/api/Report/SaveChart?categoryId=" + $scope.categoryId + "&reportId=" + $rootScope.selsetReport._id + "&startTime=" + $scope.startTime + "&endTime=" + $scope.endTime + "&percent=" + $scope.data.percent + "&topNum=" + $scope.data.topNum + "&sumNum=" + 25 + "&timeInterval=" + $scope.timeInter + "&sourceType=baidu" + "&name=" + $scope.chartName + "&user_id=" + $rootScope.userID;
	    }
	    else {
	        url = "/api/Keyword/SaveChart?categoryId=" + $scope.categoryId + "&prjId=" + $rootScope.getProjectId + "&startTime=" + $scope.startTime + "&endTime=" + $scope.endTime + "&percent=" + $scope.data.percent + "&topNum=" + $scope.data.topNum + "&sumNum=" + 25 + "&timeInterval=" + $scope.timeInter + "&sourceType=" + $rootScope.isActiveModale + "&name=" + $scope.chartName + "&user_id=" + $rootScope.userID;
	    }
	    var q = $http.get(url);
	    q.success(function (response, status) {
	        $scope.massage = response.Message;
	        if (response.IsSuccess != true) {
	            $scope.error = $scope.massage;
	        } else {
	            $scope.alert_fun('success', "保存成功！");

	            $scope.ok();
	        }
	    });
	    q.error(function (e) {
	        $scope.alert_fun('danger', "网络打盹了，请稍后。。。");
	    });
	}
	
}