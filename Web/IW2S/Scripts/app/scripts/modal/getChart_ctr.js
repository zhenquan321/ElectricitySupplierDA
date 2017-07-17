var getChart_ctr = function($scope, $modalInstance, $rootScope, $http) {

	$scope.isactive = "";
	//______________________________________________________________________
	$scope.ok = function() {
		$modalInstance.close($scope.selected);
	};
	$scope.cancel = function() {
		$modalInstance.dismiss('cancel');
	};

	$scope.GetChart = function () {
	    var url = "";
	    if ($scope.isReport) {
	        url = "/api/Report/GetChart?user_id=" + $rootScope.userID + "&reportId=" + $rootScope.selsetReport._id + "&sourceType=baidu";
	    }
	    else {
	        url = "/api/Keyword/GetChart?prjId=" + $rootScope.getProjectId + "&sourceType=" + $rootScope.isActiveModale + "&user_id=" + $rootScope.userID;
	    }
		var q = $http.get(url);
		q.success(function(response, status) {
			console.log('getChart_ctr>GetChart');
			$scope.ChartList = response;
		});
		q.error(function(e) {
		    $scope.alert_fun('danger', "网络打盹了，请稍后。。。");
		});
	}

	$scope.ShowChart = function() {
		for (var i = 0; i < $scope.ChartList.length; i++) {
			if ($scope.ChartList[i]._id == ChartConfigcheckedId[0]) {
//				$scope.startTime = $scope.ChartList[i].startTime;
//				$scope.endTime = $scope.ChartList[i].endTime;
				$scope.data.percent = $scope.ChartList[i].percent;
				$scope.data.topNum = $scope.ChartList[i].topNum;
				$scope.categoryId = $scope.ChartList[i].categoryId;

				if (!$scope.isReport) {
				    $scope.GetTimeLinkList($scope.ChartList[i].categoryId);
				}
				$scope.D_lineChart(null, $scope.ChartList[i]);
				$scope.ok();
				break;
			}
		}
	}

	//排除关键词
	var ChartConfigcheckedId = [];
	$scope.ChartConfigChk = function(id, aa) {
		if (aa) {
			ChartConfigcheckedId.push(id);
		} else {
			for (var i = 0; i < ChartConfigcheckedId.length; i++) {
				if (ChartConfigcheckedId[i] == id) {
					ChartConfigcheckedId.splice(i, 1);
					break;
				}
			}
		}
		console.log(ChartConfigcheckedId);
	}

	$scope.ChartConfigAllchk = function(cal) {
		if (cal) {
			ChartConfigcheckedId = [];
			for (var i = 0; i < $scope.ChartList.length; i++) {
				ChartConfigcheckedId.push($scope.ChartList[i]._id);
			}
		} else {
			ChartConfigcheckedId = [];
		}
	}

	$scope.DelChartConfig = function() {
		if (confirm("确定要删除记录吗？")) {
			var ids = ChartConfigcheckedId.join(";");
			var IsRemoved = true;
			var url = "api/Keyword/DelChart?ids=" + ids;
			var q = $http.get(url);
			q.success(function(response, status) {
				if (response.IsSuccess == true) {
				    $scope.alert_fun('success', "删除成功！");
					ChartConfigcheckedId = [];
					$scope.GetChart();
				}
			});
			q.error(function(e) {
			    $scope.alert_fun('danger', "网络打盹了，请稍后。。。");
			});
		}
	}

	//自动加载
	$scope.GetChart();

}