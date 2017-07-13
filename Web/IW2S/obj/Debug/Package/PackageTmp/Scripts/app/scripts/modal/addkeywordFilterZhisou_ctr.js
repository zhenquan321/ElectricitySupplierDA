var addkeywordFilterZhisou_ctr = function ($scope, $modalInstance, $rootScope, $http) {
    $scope.ok = function () {
        $modalInstance.close($scope.selected);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

}