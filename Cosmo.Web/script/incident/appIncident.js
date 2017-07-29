var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
DashboardApp.controller('DashboardController', [
    '$scope', '$http', '$interval', 'ReportService', function($scope, $http, $interval, reportService) {

        $scope.sortType = 'ConfigServiceType'; // set the default sort type
        $scope.sortReverse = false; // set the default sort order
        $scope.searchSchedule = ''; // set the default search/filter term

        $scope.tableStatus = '';
        $scope.isExpand = false;
        $scope.environments = [];
        $scope.lastRefreshedTime = reportService.getDate(0);
        $scope.reloadInterval = $scope.reloadTime = 180;

        $scope.getCurrentIncidentList = function() {
            openModal();
            var dataObj = {
                envId: '0' //$scope.envId
            };

            $scope.reloadTime = $scope.reloadInterval === 0 ? 180 : $scope.reloadInterval; // Reset reload time
            try {
                $http.post('../forms/Generic.aspx/GetAllIncident', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.buildDetails = data.d;
                        if ($scope.buildDetails != null && $scope.buildDetails.length > 0) {
                            angular.forEach($scope.buildDetails, function(monitorList) {

                                $scope.environments.push({ id: monitorList.EnvID });
                                $scope.incidentAvailability = '';
                                if (monitorList.monitorList.length > 0) {
                                    angular.forEach(monitorList.monitorList, function(monitor) {
                                        if (monitor.MonitorCreatedDateTime != null)
                                            monitor.MonitorCreatedDateTime = reportService.parsedDate(monitor.MonitorCreatedDateTime);
                                        if (monitor.MonitorUpdatedDateTime != null)
                                            monitor.MonitorUpdatedDateTime = reportService.parsedDate(monitor.MonitorUpdatedDateTime);
                                        if (monitor.LastMoniterTime != null)
                                            monitor.LastMoniterTime = reportService.parsedDate(monitor.LastMoniterTime);

                                        if (monitor.ConfigServiceType === '1')
                                            monitor.ConfigServiceType = 'Content Manager';
                                        else if (monitor.ConfigServiceType === '2')
                                            monitor.ConfigServiceType = 'Dispatcher';
                                    });
                                } else {

                                    $scope.incidentAvailability = 'No Incident Exists for ' + monitorList.EnvName;
                                }
                            });
                            $scope.errorMessage = '';
                            $('#divErrorMessage').hide();
                        } else {
                            $scope.errorMessage = "No Incident Exists";
                            $('#linkExpand').attr('visibility', 'hidden');
                            $('#divErrorMessage').show();
                        }
                        closeModal();
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        console.log(data.Message);
                        console.log(data.StackTrace);
                        alert("Error: " + data.Message);
                    });
            } catch (e) {
                    closeModal();
                    $scope.errorMessage = "Error " + e;
                    $('#divErrorMessage').show();
            }
        };

            $scope.toggle_it = function (itemId) {
                var allCollapsed = true;
                var allExpanded = true;

                if (document.getElementById("row_" + itemId) != null || document.getElementById("row_" + itemId) == undefined) {
                    if ((document.getElementById("row_" + itemId).style.display == 'none')) {
                        document.getElementById("row_" + itemId).style.display = '';
                        $('#arrowImg_' + itemId + ' > span').removeClass('glyphicon glyphicon-triangle-right').addClass('glyphicon glyphicon-triangle-bottom');
                        $scope.tableStatus = "open";
                    } else {
                        document.getElementById("row_" + itemId).style.display = 'none';
                        $('#arrowImg_' + itemId + ' > span').removeClass('glyphicon glyphicon-triangle-bottom').addClass('glyphicon glyphicon-triangle-right');
                        $scope.tableStatus = "close";
                    }
                }

                for (var iEn = 0; iEn < $scope.environments.length; iEn++) {
                    if (document.getElementById("row_" + $scope.environments[iEn].id).style.display !== 'none') {
                        allCollapsed = false;
                        break;
                    }
                }
                for (var iEx = 0; iEx < $scope.environments.length; iEx++) {
                    if (document.getElementById("row_" + $scope.environments[iEx].id).style.display === 'none') {
                        allExpanded = false;
                        break;
                    }
                }
                if (allCollapsed) {
                    $scope.isExpand = false;
                    $scope.setExpandCollapse();
                } else if (allExpanded) {
                    $scope.isExpand = true;
                    $scope.setExpandCollapse();
                }
            };

            var toggleAll;
            $scope.setExpandCollapse = function (itemId) {
                if ($scope.environments !== 'undefined') {
                    for (var iEn = 0; iEn < $scope.environments.length; iEn++) {
                        toggleAll($scope.environments[iEn].id, $scope.isExpand);
                    }
                    $scope.isExpand = !$scope.isExpand;
                    if ($scope.tableStatus === "open") {
                        $("#linkExpand").html("<span class='glyphicon glyphicon-collapse-down'></span> Collapse All");
                    } else {
                        $("#linkExpand").html("<span class='glyphicon glyphicon-expand'></span> Expand All");
                    }
                }
                return false;
            };

            toggleAll = function (itemId, mode) {
                if (document.getElementById("row_" + itemId) != null || document.getElementById("row_" + itemId) == undefined) {
                    if (mode) {
                        document.getElementById("row_" + itemId).style.display = '';
                        $('#arrowImg_' + itemId + ' > span').removeClass('glyphicon glyphicon-triangle-right').addClass('glyphicon glyphicon-triangle-bottom');
                        $scope.tableStatus = "open";
                    } else {
                        document.getElementById("row_" + itemId).style.display = 'none';
                        $('#arrowImg_' + itemId + ' > span').removeClass('glyphicon glyphicon-triangle-bottom').addClass('glyphicon glyphicon-triangle-right');
                        $scope.tableStatus = "close";
                    }
                }
            };

        $scope.ngSetIncidetDetails = function(thisCtrl, monId, mode) {
            if (mode)
                PopulateAll(thisCtrl, 'envDetails_' + monId);
            else
                setIncidetDetails(monId, 'envDetails_' + monId);
        };

        $scope.SubmitIncident = function (monId, envId, configId, $event) {
            $event.preventDefault();
//            var incidentId = 2;
//            var issueDesc = "sss";
//            var solutionDesc = "sdsd";

//            var parameter = {
//                monitorId: incidentId,
//                environmentId: envId,
//                serviceId: configId,
//                serviceIssue: issueDesc,
//                serviceSolution: solutionDesc
//            };

//            $http.post('../forms/Generic.aspx/IncidentTracking', parameter)
//                .success(function(data, status, headers, config) {
//                    console.log("Success: " + data);
//                })
//                .error(function(data, status, headers, config) {
//                    console.log("Error :" + data);
//                });

            SubmitIncident(monId, envId, configId);
        };

        $scope.getCurrentIncidentList();
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