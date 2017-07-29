var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
 DashboardApp.service("ReportService", dateService);

function dateService() {

    this.getDate = function (repType) {
        var currDate = new Date();
        if (repType === 1)
            currDate = this.addDays(currDate, -1);
        else if (repType === 2)
            currDate = this.addDays(currDate, -2);
        else if (repType === 3)
            currDate = this.addDays(currDate, -6);
        else if (repType === 4)
            currDate = this.addDays(currDate, -13);
        else if (repType === 5) {
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

        return sMonth + '/' + sDay + '/' + sYear + ' ' + sHour + ':' + sMinute + ':' + sSecond; // + ' ' + ampm;

    };

    this.addDays = function (date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    this.parsedDate = function (jsonDate) {
        return jsonDate.replace('/Date(', '').replace(')/', '');
    };
}