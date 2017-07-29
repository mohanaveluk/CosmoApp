var ServiceBuildApp = angular.module("ServiceStatusApp", []);
ServiceBuildApp.controller("ServiceBuildController", ['$scope', '$http', 'ReportService', function ($scope, $http, ReportService) {
    var monthName = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");
    $scope.sortType = 'ConfigServiceType'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.searchSchedule = '';     // set the default search/filter term

    $scope.sortType_build = 'ConfigServiceType'; // set the default sort type
    $scope.sortReverse_build = false;  // set the default sort order



    $scope.envID = '0';

    $scope.errorMessage = '';
    $scope.buildDetails = {};
    $scope.buildEnvironment = {};
    $scope.columnDateHeaders = [];
    $scope.columnMonthHeaders = [];
    $scope.columnMonthHeadersCount = [];

    $scope.reportRange = '1';
    $scope.reportStyle = '1';
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

    $scope.getBuildHistory = function() {
        openModal();
        if (!CompareDates($scope.startDate, $scope.endDate)) return false;
//        angular.forEach($scope.buildEnvironment, function(key) {
//            if (key.EnvID == $scope.reportType) {
//                $scope.reportTypeText = key.EnvName;
//                return;
//            }
//        });
        $scope.reportTypeText = $('#drpReportType option:selected').text();
        $scope.createDateHeaders($scope.startDate, $scope.endDate);

        var dataObj = {
            envID: $scope.reportType,
            startDate: $scope.startDate,
            endDate: $scope.endDate
        };
        try {
            $http.post('../forms/Generic.aspx/GetServiceStatusHistoryReport', dataObj)
                .success(function(data, status, headers, config) {
                    $scope.buildDetails = data.d;
                    if ($scope.buildDetails != null) {
                        angular.forEach($scope.buildDetails, function(monitorList) {
                            var tempStatus = [];
                            angular.forEach(monitorList.monitorList, function(monitor) {
                                var tempstatusArr = monitor.MonitorComments.split(',');
                                var tempstatusIcon = GetMonitorStatusReportIcon(tempstatusArr);
                                //                        angular.forEach(tempstatusArr, function (lst) {
                                //                            tempStatus.push(lst.toString());
                                //                        });
                                //console.log(tempstatusArr);
                                monitor.MonitorComments = tempstatusIcon;
                            });
                        });
                        $scope.reportStyle = $scope.reportRange;
                    } else
                        $scope.errorMessage = "Build details does not exists";
                    closeModal();
                })
                .error(function(data, status, headers, config) {
                    closeModal();
                    alert("Error");
                });
        } catch (e) {
            closeModal();
        }
    };

    $scope.createDateHeaders = function (startDate, endDate) {
        $scope.columnDateHeaders = [];
        $scope.columnMonthHeaders = [];
        $scope.columnMonthHeadersCount = [];

        var dataSplit = startDate.split('/');
        var stDate = new Date(dataSplit[2].substr(0, 4), (dataSplit[0] - 1), dataSplit[1]);
        dataSplit = endDate.split('/');
        var edDate = new Date(dataSplit[2].substr(0, 4), (dataSplit[0] - 1), dataSplit[1]);
        var monthHeader = '';
        var headerColCount = 0;
        
        for (var d = stDate; d <= edDate; d.setDate(d.getDate() + 1)) {
            var dateOut = new Date(d);
            $scope.columnDateHeaders.push(dateOut);
            var monthYear = monthName[dateOut.getMonth()] + '-' + dateOut.getFullYear();

            if (monthHeader != monthYear) {
                $scope.columnMonthHeaders.push(monthYear);

                if (monthHeader != '') {
                    $scope.columnMonthHeadersCount.push(headerColCount);
                    headerColCount = 0;
                }
                monthHeader = monthYear;
            }
            headerColCount++;

        }
        if (headerColCount > 0) $scope.columnMonthHeadersCount.push(headerColCount);
        console.log($scope.columnMonthHeaders);
        console.log($scope.columnMonthHeadersCount);
    }

    $scope.SetTableSize = function () {
        return $scope.reportStyle === '1'
            ? 'table0_4'
            : ($scope.reportStyle === '2'
                ? 'table0_5'
                : ($scope.reportStyle === '3'
                    ? 'table0_6'
                    : ($scope.reportStyle === '4'
                        ? 'table0_75'
                        : 'table1')));
    }

    $scope.SetColSize = function () {
        return $scope.reportStyle === '1'
            ? 'col-sm-4'
            : ($scope.reportStyle === '2'
                ? 'col-sm-4'
                : ($scope.reportStyle === '3'
                    ? 'col-sm-3'
                    : ($scope.reportStyle === '4'
                        ? 'col-sm-2'
                        : 'col-sm-1')));
    }

    var GetMonitorStatusReportIcon = function(tempstatusArr) {
        var statusIcon = [];
        angular.forEach(tempstatusArr, function(statusArr) {
            var icon = '';
            switch (statusArr) {
            case "A":
                icon = "../images/green_icon.jpg";
                break;
            case "R":
                icon = "../images/blue_icon.png";
                break;
            case "R/D":
            case "D/R":
            case "A/D":
            case "D/A":
            case "A/D/S":
            case "A/S/D":
            case "D/S/A":
            case "D/A/S":
            case "S/D/A":
            case "S/A/D":
            case "D":
                icon = "../images/pink_icon.png";
                break;
            case "S":
            case "S/D":
            case "D/S":
                icon = "../images/orange_icon.jpg";
                break;
            case "A/S":
            case "S/A":
                icon = "../images/yellow_green.png";
                break;
            case "N/A":
                icon = "../images/gray_icon.png";
                break;
            default:
                icon = "../images/gray_icon.png";
                break;


            }
            console.log(statusArr);
            statusIcon.push(icon);
        });
        return statusIcon;
    }

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
        var sMinute = currDate.getMinutes();
        var sSecond = currDate.getSeconds();

        if (sDay.toString().length == 1) sDay = '0' + sDay;
        if (sMonth.toString().length == 1) sMonth = '0' + sMonth;

        return sMonth + '/' + sDay + '/' + sYear + ' ' + sHour + ':' + sMinute + ':' + sSecond; ;

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
        var sMinute = jsDate.getMinutes();
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