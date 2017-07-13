var addGroupModal_ctr = function ($scope, $modalInstance, $rootScope, $http) {

    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.allkeywords = $scope.GetBaiduSearchKeyword;
    /// <param name="usr_id"></param>
    /// <param name="groupName"></param>
    /// <param name="lawCode"></param>
    /// <param name="weight"></param>
    /// <param name="groupType">

    $scope.newGroup = function () {
        var url = "/api/Keyword/InsertKeywordGroup?usr_id=" + $rootScope.userID + "&groupName=" + encodeURIComponent($scope.groupName)
            + "&lawCode=" + $scope.lawCode + "&weight=" + $scope.weight + "&groupType=" + $scope.groupType
            + "&parentGroupId=" + $scope.parentGroupId + "&keywordIds=" + $scope.keywordIds;
        var q = $http.get(url);

        $scope.list_submitAll = [{
            usr_id: $rootScope.userID,
            groupName: encodeURIComponent($scope.groupName),
            lawCode: $scope.InfriType,
            weight: $scope.weight,
            groupType: groupType,
            parentGroupId: $scope.parentGroupId,
            keywordIds: keywordIds,
        }]

        var urls = "api/Keyword/InsertKeywordGroup";
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
            console.log('addGroupModal_ctr>newGroup');
            if (response.Error) {
                $scope.error = response.Error;
            } else {
                $location.path("/login").replace();
            }
        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    $scope.newGroup();

}