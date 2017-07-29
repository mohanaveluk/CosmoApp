var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
DashboardApp.controller('DashboardController', [
    '$scope', '$http', '$interval', function($scope, $http, $interval) {

        $scope.labels = [];
        $scope.driveFreeSpace = [];
        $scope.driveUsedSpace = [];
        $scope.driveTotalSpace = [];
        $scope.hourSpace = [];
        $scope.hourCpu = [];
        $scope.averageCpuUsage = [];
        $scope.averageAvailableMemory = [];
        $scope.averageUsedMemory = [];
        $scope.driveName = [];
        $scope.freeSpace = [];
        $scope.errorMessage = "";

        $scope.geServerPerformanceCurrent = function() {
            openModal();

            var dataObj = {
                host: $scope.hostname
            };

            try {
                $http.post('../forms/Generic.aspx/GetCurrentDriveInfos', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.ServerCurrentInfo = data.d;

                        if ($scope.ServerCurrentInfo != null && $scope.ServerCurrentInfo.Performance != null && $scope.ServerCurrentInfo.Performance.Drives != null) {
                            angular.forEach($scope.ServerCurrentInfo.Performance.Drives, function(drive) {
                                if (drive.Label === "" || drive.Label == null) drive.Label = "Local Disk";
                                drive.UsedInPercentage = (drive.UsedSpace / drive.TotalSpace) * 100;
                            });
                        }

                        closeModal();
                        $scope.handleError('');
                        $scope.geServerPerformanceForDiskSpace();
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

        $scope.geServerPerformanceForDiskSpace = function() {
            if ($scope.errorMessage !== "") return;
            openModal();
            
            var dataObj = {
                host: $scope.hostname,
                mode:'free'
            };
            try {
                $http.post('../forms/Generic.aspx/GetAverageUsedSpace', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.ServerUsedSpace = data.d;
                        angular.forEach($scope.ServerUsedSpace, function(performance) {
                            $scope.freeSpace = [];
                            angular.forEach(performance.FreeSpaceInPercent, function (fSpace) {
                                $scope.freeSpace.push(fSpace);
                            });
                            $scope.driveFreeSpace.push($scope.freeSpace);
//                            $scope.DiskUsedSpace.push(performance.UsedSpace);
//                            $scope.DiskTotalSpace.push(performance.TotalSpace);
//                            $scope.hourSpace.push(performance.Hour);
                            $scope.labels = performance.Date;
                            $scope.driveName.push(performance.Name.substring(0,1));

                        });
                        PopulateDataToChart($scope.labels, $scope.driveFreeSpace, $scope.driveName);
                        closeModal();
                        $scope.handleError('');
                        $scope.geServerPerformanceForCpuUsage();
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

        $scope.geServerPerformanceForCpuUsage = function() {
            if ($scope.errorMessage !== "") return;
            openModal();

            var dataObj = {
                host: $scope.hostname
            };
            try {
                $http.post('../forms/Generic.aspx/GetAverageCpuUsage', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.ServerCpuUsage = data.d;
                        angular.forEach($scope.ServerCpuUsage, function (performance) {
                            $scope.averageCpuUsage.push(performance.AverageCpuUsage);
                            $scope.averageAvailableMemory.push(performance.AverageAvailableMemory);
                            $scope.averageUsedMemory.push((performance.AverageTotalMemory - performance.AverageAvailableMemory) / performance.AverageTotalMemory * 100);
                            $scope.hourCpu.push(performance.Hour);

                        });
                        PopulateChartForCpuMemory($scope.hourCpu, $scope.averageCpuUsage, $scope.averageUsedMemory);
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
            if (message !== "") {
                $('#divErrorMessage').removeClass().addClass('alert alert-danger');
            }
            if (message === "") {
                $('#divErrorMessage').removeClass('alert alert-danger');
            }
        };

        $scope.hostname = $("#hidHostName").val();
        if ($scope.hostname === "" || $scope.hostname === undefined) {
            $scope.handleError("Host IP is required");
            return false;
        }

        $scope.geServerPerformanceCurrent();
            
            
    }
]);

var options = {
    width: 300,
    height: 40,
    showPoint: true,
    // Disable line smoothing
    lineSmooth: true,
    // X-Axis specific configuration
    axisX: {
        // We can disable the grid for this axis
        showGrid: false,
        // and also don't show the label
        showLabel: false
    },
    
    chartPadding: {
        top: 45
    }
   
};
var PopulateDataToChart = function(labels, response, driveName) {
    var data = {
        // A labels array that can contain any sort of values
        labels: labels,
        // Our series array that contains series objects or in this case series data arrays
        series: response
    };
    var legendStorage = document.getElementById('legStorage');
    var legend = {
        fullWidth: false,
        axisX: {
            showLabel: true,
            showGrid: false
        },
        plugins: [
            Chartist.plugins.legend({
                position: legendStorage,
                legendNames: driveName
            })
        ],
        low:0,
        high:100
    };

    var line = new Chartist.Line('#storageLast30Days', data, legend, options);
};

var PopulateChartForCpuMemory = function(labels, cpuUsage, memory) {
    var legendCpu = document.getElementById('legCpu');
    var legendMemory = document.getElementById('legMemory');
    var data = {
        // A labels array that can contain any sort of values
        labels: labels,
        // Our series array that contains series objects or in this case series data arrays
        series: [
            cpuUsage
        ]
    };

    var divLegendCpu = {
        fullWidth: false,
        axisX: {
            showLabel: true,
            showGrid: false
        },
        plugins: [
            Chartist.plugins.legend({
                position: legendCpu,
                legendNames: ["Average CPU Usage"]
            })
        ],
        low:0,
        high: 100
    };

    var divLegendMemory = {
        fullWidth: false,
        axisX: {
            showLabel: true,
            showGrid: false
        },
        plugins: [
            Chartist.plugins.legend({
                position: legendMemory,
                legendNames: ["Average Memory Usage"]
            })
        ],
        low:0,
        high:100
    };
    var data1 = {
        // A labels array that can contain any sort of values
        labels: labels,
        // Our series array that contains series objects or in this case series data arrays
        series: [
            memory
        ]
    };
        var line1 = new Chartist.Line('#CpuLast24Hours', data, divLegendCpu, options);
        var line2 = new Chartist.Line('#MemoryLast24Hours', data1, divLegendMemory, options);
    };