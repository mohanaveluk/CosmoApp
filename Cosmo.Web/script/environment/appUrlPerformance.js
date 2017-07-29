var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
DashboardApp.controller('DashboardController', [
    '$scope', '$http', '$interval', 'ReportService', function ($scope, $http, $interval, reportService) {

        $scope.labels = [];
        $scope.response = [];
        $scope.chartData = [];
        $scope.label = "";
        $scope.responseTime = "";

        $scope.geUrlPerformanceByEnvironent = function() {
            openModal();
            var envId = $("#hidEnvId").val();
            var dataObj = {
                envId: envId
            };

            try {
                $http.post('../forms/Generic.aspx/GetUrlPerformance', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.urlPerformanceList = data.d;
                        angular.forEach($scope.urlPerformanceList, function(performance) {
                            $scope.chartData = [];
                            $scope.labels = [];
                            $scope.DisplayName = performance.DisplayName;
                            $scope.EnvName = performance.EnvName;
                            $scope.Adress = performance.Adress;
                            $scope.ResponseTimeLastPing = performance.ResponseTimeLastPing;
                            $scope.ResponseTimeLastHour = performance.ResponseTimeLastHour;
                            $scope.LastPingDateTime = performance.LastPingDateTime;

                            if (performance.LastPingDateTime != null || performance.LastPingDateTime != undefined)
                                $scope.LastPingDateTime = reportService.parsedDate(performance.LastPingDateTime);

                            angular.forEach(performance.ResponseTimeLast24Hour, function (entity) {
                                //$scope.chartData.push("{ x: " + entity.Hour + ", y: " + entity.Average + "}");
                                $scope.chartData.push({ x: entity.Hour, y: entity.Average });

                                $scope.label += "'" + entity.Hour + "', ";
                                $scope.responseTime += "'" + entity.Average + "', ";
                                $scope.labels.push(entity.Hour);
                                $scope.response.push(entity.Average);
                            });
                            PopulateDataToChart(performance.EnvId, $scope.labels, $scope.response, $scope.chartData);
                            //DisplayChartRT();
                            //$scope.labels.push($scope.chartData);

                        });

                        closeModal();
                        $scope.handleError('');
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        $scope.handleError("Error " + data.Message);
                    });
            } catch (e) {
                closeModal();
                $scope.handleError("Error " + e);
            }
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

        $scope.geUrlPerformanceByEnvironent();
    }
]);

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

var DisplayChartRT = function () {
    var line = new Chartist.Line('#divsample', {
        labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
        series: [
            [12, 9, 7, 8, 5],
            [2, 1, 3.5, 7, 3],
            [1, 3, 4, 5, 6]
        ]
    }, {
        fullWidth: true,
        chartPadding: {
            right: 40
        }
    });
}
var PopulateDataToChart = function(envId, labels, response, chartData) {
    var data = {
        // A labels array that can contain any sort of values
        labels: labels,
        // Our series array that contains series objects or in this case series data arrays
        series: [
            response
        ]
    };
    var legendRt = document.getElementById('legResponseTime');
//    var divLegendResponseTime = {
//        fullWidth: false,
//        axisX: {
//            showLabel: false,
//            showGrid: false,
//        },
//        plugins: [
//            Chartist.plugins.legend({
//                position: legendRt,
//                legendNames: ["Average Response Time"]
//            })
//        ]
//    };

    var options = {
        width: 400,
        height: 180,
        showPoint: true,
        // Disable line smoothing
        lineSmooth: true,
        // X-Axis specific configuration
        axisX: {
            showLabel: true,
            showGrid: false,
        },
        chartPadding: {
            top: 25
        }
    };
    var line = new Chartist.Line('.ct-chart', data, options);//, divLegendResponseTime, options);
    //var line1 = new Chartist.Line('#ctResponseTime', data, options);
};

var PopulateDataToChart_canvas = function (envId, labels, response, chartData) {
        //console.log("EnvId: " + envId);

        
        var data = [];
        var dataSeries = { type: "line",showInLegend: true, name:"Avg ResponseTime"  };
        dataSeries.dataPoints = chartData;//datapoints;
        data.push(dataSeries);
        
        /*
        var limit = 10;    //increase number of dataPoints by increasing this
        debugger;
        var y = 0;
        var data = []; var dataSeries = { type: "line" };
        var dataPoints = [];
        for (var i = 0; i < limit; i += 1) {
            y += (Math.random() * 10 - 5);
            dataPoints.push({
                x: i - limit / 2,
                y: y
            });
        }
        dataSeries.dataPoints = dataPoints;
        data.push(dataSeries);
        */
        var chart = new CanvasJS.Chart("chartContainer", {
            zoomEnabled: true,
            title: {
                text: "Last 24 Hours"
            },
            axisX: {
                interval: 4
            },
            toolTip: {
                shared: false
            },
            axisY: {
                interval: 0.2,
                gridColor: "#eeeeee",
                tickColor: "silver"

            },
            data: data
        });


        

        //chart.options.data = data;

    chart.render();
}

function PopulateDataToChart1(envId, labels, response) {
    var ctx = $("#myChart");
    //ctx.canvas.height = 200;
    //ctx.height = 200;
    var dataChart = {
        labels: labels.join(),
        datasets: [
            {
                label: 'Last 24 hours',
                data: [response.join()],
                borderWidth: 1
            }
        ]
    };

    var myChart = new Chart(ctx, {
        type: 'line',
        data: dataChart,
        options: {
            responsive: true
        }
    });


}