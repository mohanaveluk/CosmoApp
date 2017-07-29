var ServiceBuildApp = angular.module("ServiceBuildApp", []);
ServiceBuildApp.controller("ServiceBuildController", ['$scope', '$http', 'ReportService', function ($scope, $http, ReportService) {

    $scope.sortType = 'ConfigServiceType'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.searchSchedule = '';     // set the default search/filter term

    $scope.sortType_build = 'ConfigServiceType'; // set the default sort type
    $scope.sortReverse_build = false;  // set the default sort order



    $scope.envID = '0';
    $scope.searchSchedule = '';
    $scope.errorMessage = '';
    $scope.buildDetails = {};
    $scope.buildEnvironment = {};

    $scope.reportRange = '1';
    $scope.reportType = '0';
    $scope.reportTypeText = 'All Environments';
    $scope.startDate = ReportService.getDate(1);
    $scope.endDate = ReportService.getDate(0);

    $scope.parseDate = function parseDate(dateObj) {
        return ReportService.parsedDate(dateObj);
    }

    $scope.getDates = function () {
        $scope.startDate = ReportService.getDate($scope.reportRange);
        $scope.endDate = ReportService.getDate(0);
        $scope.getBuildHistory();
    };

    $scope.getAllEnvironments = function () {
        var dataObj = { envId: $scope.reportType };
        $http.post('../forms/Generic.aspx/GetAllEnvironments', dataObj)
        .success(function (data, status, headers, config) {
            $scope.buildEnvironment = data.d;
        })
        .error(function (data, status, headers, config) {
            alert("Error");
        });
    };

    $scope.getBuildHistory = function () {
        if (!CompareDates($scope.startDate, $scope.endDate)) return false;
        angular.forEach($scope.buildEnvironment, function (key, value) {
            if (key.EnvID == $scope.reportType) {
                $scope.reportTypeText = key.EnvName;
                return false;
            }
        });

        var dataObj = {
            envID: $scope.reportType,
            startDate: $scope.startDate,
            endDate: $scope.endDate
        };
        $http.post('../Forms/Generic.aspx/GetIncidentTrackingReport', dataObj)
        .success(function (data, status, headers, config) {
            $scope.buildDetails = data.d;
            if ($scope.buildDetails != null) {
                angular.forEach($scope.buildDetails, function (monitorList) {
                    //schedule.GROUP_SCH_TIME = schedule.GROUP_SCH_TIME.substr(6).replace(')/', '');
                    angular.forEach(monitorList.monitorList, function (monitor) {
                        monitor.MonitorCreatedDateTime = ReportService.parsedDate(monitor.MonitorCreatedDateTime);
                        monitor.MonitorUpdatedDateTime = ReportService.parsedDate(monitor.MonitorUpdatedDateTime);
                        monitor.LastMoniterTime = ReportService.parsedDate(monitor.LastMoniterTime);
                        if (monitor.ConfigServiceType == '1')
                            monitor.ConfigServiceType = 'Content Manager';
                        else if (monitor.ConfigServiceType == '2')
                            monitor.ConfigServiceType = 'Dispatcher';

                        if (monitor.MonitorStatus.toLowerCase() == 'stopped')
                            monitor.MonitorComments = '../images/red1_icon.jpg';
                        else if (monitor.MonitorStatus.toLowerCase() == 'running')
                            monitor.MonitorComments = '../images/green_icon.jpg';
                        else if (monitor.MonitorStatus.toLowerCase() == 'not running')
                            monitor.MonitorComments = '../images/red_icon.jpg';
                        else if (monitor.MonitorStatus.toLowerCase() == 'not ready')
                            monitor.MonitorComments = '../images/blue_icon.jpg';
                        else if (monitor.MonitorStatus.toLowerCase() == 'standby')
                            monitor.MonitorComments = '../images/orange_icon.jpg';
                        else
                            monitor.MonitorComments = '../images/gray_icon.png';

                    });
                });
            }
            else
                $scope.errorMessage = "Build details does not exists";
        })
        .error(function (data, status, headers, config) {
            alert("Error");
        });
    };
    $scope.getAllEnvironments();
    $scope.getBuildHistory();

} ]);


ServiceBuildApp.service("ReportService", dateService);

function dateService() {

    this.getDate = function (repType) {
        var currDate = new Date();
        if (repType == 1)
            currDate = this.addDays(currDate, -1);
        else if (repType == 2)
            currDate = this.addDays(currDate, -2);
        else if (repType == 3)
            currDate = this.addDays(currDate, -6);
        else if (repType == 4)
            currDate = this.addDays(currDate, -13);
        else if (repType == 5) {
            currDate = this.addDays(currDate, -30);
        }
        var sDay = currDate.getDate();
        var sMonth = (currDate.getMonth() + 1);
        var sYear = currDate.getFullYear();
        var sHour = currDate.getHours();
        var sMinute = currDate.getMinutes()
        var sSecond = currDate.getSeconds();

        if (sDay.toString().length == 1) sDay = '0' + sDay;
        if (sMonth.toString().length == 1) sMonth = '0' + sMonth;

        return sMonth + '/' + sDay + '/' + sYear + ' ' + currDate.getHours() + ':' + currDate.getMinutes() + ':' + currDate.getSeconds();

    };

    this.addDays = function (date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    this.parsedDate = function (jsonDate) {
        //--http://codeasp.net/assets/demos/blogs/convert-net-datetime-to-javascript-date.aspx
        var parsedJsDate = new Date(parseInt(jsonDate.substr(6)));
        var jsDate = new Date(parsedJsDate); //Date object

        var sDay = jsDate.getDate();
        var sMonth = (jsDate.getMonth() + 1);
        var sYear = jsDate.getFullYear();
        var sHour = jsDate.getHours();
        var sMinute = jsDate.getMinutes()
        var sSecond = jsDate.getSeconds();
        console.log(jsDate.getTimezoneOffset());
        return sMonth + '/' + sDay + '/' + sYear + ' ' + sHour + ':' + sMinute + ':' + sSecond;
    };
}


ServiceBuildApp.filter('dateTimeFilter', dateTimeFilter);

function dateTimeFilter($filter) {
    return function (monitorData) {
        var monitorDateList = [];
        if (input == null) return '';

        angular.forEach(monitorData, function (monitor) {
            var _date = $filter('date')(new Date(monitor.MonitorCreatedDateTime), 'MM/dd/yyyy HH');
            monitorDateList.push(_date);
        });
    };
}