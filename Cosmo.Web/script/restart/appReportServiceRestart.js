var ServiceRestartApp = angular.module("ServiceRestartApp", []);
ServiceRestartApp.controller("ServiceRestartController", ['$scope', '$http', 'ReportService', function ($scope, $http, ReportService) {

    $scope.sortType = 'GROUP_NAME'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.searchSchedule = '';     // set the default search/filter term

    $scope.reportRange = '1';
    $scope.reportType = 'O';
    $scope.schedules = {};

    $scope.startDate = ReportService.getDate(1);
    $scope.endDate = ReportService.getDate(0);

    $scope.parseDate = function parseDate(dateObj) {
        return ReportService.parsedDate(dateObj);
    }

    $scope.getDates = function () {
        console.log($scope.reportRange);
        $scope.startDate = ReportService.getDate($scope.reportRange);
        $scope.endDate = ReportService.getDate(0);
        $scope.getServiceScheduleReport();
    };

    $scope.getServiceScheduleReport = function () {
        showAlertFadeOut();
        if (!CompareDates($scope.startDate, $scope.endDate)) return false;
        //        $http.get('Generic.aspx/GetGroupScheduleReport',{
        //            params: {schType:1, schType:2, endDate:3}
        //        })
        $scope.sortType = 'GROUP_NAME'; // set the default sort type
        var dataObj = {
            schType: $scope.reportType,
            startDate: $scope.startDate,
            endDate: $scope.endDate
        };
        try {
            $http.post('../forms/Generic.aspx/GetGroupScheduleReport', dataObj)
                .success(function (data, status, headers, config) {
                    if (data.d === null) {
                        showAlert("No data found! Please refine your search");
                        closeModal();
                    }

                    $scope.schedules = data.d;
                    if (data.d.length > 0) {
                        angular.forEach($scope.schedules, function(schedule) {
                            //schedule.GROUP_SCH_TIME = ReportService.parsedDate(schedule.GROUP_SCH_TIME);
                            schedule.GROUP_SCH_CREATED_DATETIME = schedule.GROUP_SCH_CREATED_DATETIME.substr(6).replace(')/', '');
                            schedule.GROUP_SCH_TIME = schedule.GROUP_SCH_TIME.substr(6).replace(')/', '');
                            if (schedule.GROUP_SCH_COMPLETED_TIME !== null)
                                schedule.GROUP_SCH_COMPLETED_TIME = schedule.GROUP_SCH_COMPLETED_TIME.substr(6).replace(')/', '');
                            if (schedule.ServiceStartedTime !== null)
                                schedule.ServiceStartedTime = schedule.ServiceStartedTime.substr(6).replace(')/', '');
                            if (schedule.ServiceCompletionTime !== null)
                                schedule.ServiceCompletionTime = schedule.ServiceCompletionTime.substr(6).replace(')/', '');
                        });
                    }
                    else
                    { showAlert("No data found! Please refine your search", "nodata"); }
                    closeModal();
                })
                .error(function(data, status, headers, config) {
                    closeModal();
                    showAlert(data.message);
                    console.log("Error: " + data);
                });
                closeModal();

        } catch (e) {
                showAlert(data.message);
                closeModal();
        }
    };

    $scope.getServiceScheduleReport();
} ]);

ServiceRestartApp.service("ReportService", dateService);

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

        var ampm = sHour >= 12 ? 'PM' : 'AM';

        if (sDay.toString().length === 1) sDay = '0' + sDay;
        if (sMonth.toString().length === 1) sMonth = '0' + sMonth;

        if (sHour.toString().length === 1) sHour = '0' + sHour;
        if (sMinute.toString().length === 1) sMinute = '0' + sMinute;
        if (sSecond.toString().length === 1) sSecond = '0' + sSecond;

        return sMonth + '/' + sDay + '/' + sYear + ' ' + sHour + ':' + sMinute + ':' + sSecond;

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

        if (sHour.toString().length === 1) sHour = '0' + sHour;
        if (sMinute.toString().length === 1) sMinute = '0' + sMinute;
        if (sSecond.toString().length === 1) sSecond = '0' + sSecond;

        console.log(jsDate.getTimezoneOffset());
        return sMonth + '/' + sDay + '/' + sYear + ' ' + sHour + ':' + sMinute + ':' + sSecond;
    };
}

function showAlert(text, alertMode) {
    $('#div-alert').show();

    if (alertMode === "error") {
        $('#div-alert').removeClass().addClass('alert alert-danger');
    } else if (alertMode === "success") {
        $('#div-alert').removeClass().addClass('alert alert-success');
    } else {
        $('#div-alert').removeClass().addClass('alert alert-warning');
    }
    $('#label-alert').html(text).fadeIn('fast');
}

function showAlertFadeOut() {
    $('#div-alert').hide();
 }