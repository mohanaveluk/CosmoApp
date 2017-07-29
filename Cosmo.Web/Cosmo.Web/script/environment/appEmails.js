var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
DashboardApp.controller('DashboardController', [
    '$scope', '$http', '$interval', '$sce', function ($scope, $http, $interval, $sce) {

        $scope.sortType = 'ConfigServiceType'; // set the default sort type
        $scope.sortReverse = false; // set the default sort order
        $scope.searchSchedule = ''; // set the default search/filter term

        $scope.tableStatus = '';
        $scope.isExpand = false;
        $scope.environments = [];
        $scope.errorMessage = '';

        $scope.getEnvironmentList = function() {
            openModal();
            var dataObj = {
                envId: '0'
            };

            $http.post('../forms/Generic.aspx/GetEnvironmentEmails', dataObj)
                .success(function(data, status, headers, config) {
                    $scope.environmentList = data.d;
                    if ($scope.environmentList != null && $scope.environmentList.length > 0) {
                        angular.forEach($scope.environmentList, function(eList) {
                            
                        });
                        $scope.handleError('');
                    } else {
                        $scope.handleError("No Notification Data Exists");
                    }
                    closeModal();
                })
                .error(function(data, status, headers, config) {
                    closeModal();
                    $scope.handleError("Error " + data);
                    
                });
        };

        $scope.handleError = function(message) {
            $scope.errorMessage = message;
            if (message !== '') {
                $('#divErrorMessage').removeClass().addClass('alert alert-danger');
            }
            if (message === '') {
                $('#divErrorMessage').removeClass('alert alert-danger');
            }
        };

        $scope.EditEnvironmentEmailConfiguration = function(envid) {
            openEmailConfig("EditEmailConfig.aspx?s=" + envid + "&t=en");
        };

        $scope.getEnvironmentList();
    }
]);

DashboardApp.filter('renderHTMLCorrectly', function($sce) {
    return function(stringToParse) {
        return $sce.trustAsHtml(stringToParse);
    }
});

function UpdateEnvironmentEmailList() {
    console.log('calling UpdategetEnvironmentEmailList');
    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function () {
        scope.getEnvironmentList();
    });
}