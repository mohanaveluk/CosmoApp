var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
DashboardApp.controller('DashboardController', [
    '$scope', '$http', '$interval', 'ReportService', function ($scope, $http, $interval, reportService) {

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
            try {
                $http.post('../forms/Generic.aspx/GetAllEnvironments', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.environmentList = data.d;
                        if ($scope.environmentList != null && $scope.environmentList.length > 0) {
                            angular.forEach($scope.environmentList, function(eList) {

                                $scope.environments.push({ id: eList.EnvID });
                                angular.forEach(eList.EnvDetailsList, function(entity) {
                                    if (entity.EnvDetCreatedDate != null || eList.EnvDetCreatedDate != undefined)
                                        entity.EnvDetCreatedDate = reportService.parsedDate(entity.EnvDetCreatedDate);
                                    if (entity.EnvDetUpdatedDate != null || eList.EnvDetUpdatedDate != undefined)
                                        entity.EnvDetUpdatedDate = reportService.parsedDate(entity.EnvDetUpdatedDate);

                                });
                            });
                            $scope.handleError('');
                        } else {
                            $scope.handleError("No Environment Data Exists");
                            $('#linkExpand').attr('visibility', 'hidden');
                        }
                        closeModal();
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        $('#linkExpand').attr('visibility', 'hidden');
                        $scope.handleError("Error " + data);
                    });
            } catch (e) {
                closeModal();
                $scope.handleError("Error " + e);
            }
        };

        $scope.DeleteConfig = function(cfgId, message, type) {
            var dataObj = {
                'type': type,
                'configID': cfgId
            };
            if (confirm(message)) {
                openModal();
                $http.post('../forms/Generic.aspx/DeleteEnvConfig', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.getEnvironmentList();
                    })
                    .error(function (data, status, headers, config) {
                        closeModal();
                        alert(data.d);
                    });
            }
        };

        $scope.toggle_it = function(itemId) {
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

        $scope.getSchedularClass = function(status) {
            if (status === "0")
                return "scheduler_na";
            else if (status === "1")
                return "scheduler";
            else if (status === "2")
                return "scheduler_tobe";
            else if (status === "3")
                return "scheduler_partial";
            else if (status === "Yet to schedule")
                return "scheduler_tobe";
            else if (status != null)
                return "scheduler";
            return "";
        }

        $scope.EditSchedule = function (obj) {
            if (!obj.IsEmailExists)
                alert("Please add email address before schedule to monitor the environment " + obj.EnvName);
            else {
                ScheduleService("" + obj.EnvName + "", "" + obj.SchedularStatus + "", "" + obj.ScheduledCount + "", "" + obj.EnvID + "");
            }
        };

        $scope.EditScheduleCSM = function (entity, env) {
            if (!env.IsEmailExists) {
                alert("Please add email address before schedule to monitor the environment " + entity.EnvName);
            } else {
                openSchedulerModal("Schedule.aspx?e=" + entity.EnvID + "&c=" + entity.EnvDetID + "&t=ed");
            }
        };

        $scope.EditEnvironmentConfiguration = function (entity, type) {

            if (type === "c")
                openEditEnvironmentModal("ServiceDetails.aspx?s=" + entity.EnvDetID + "&t=ed");
            else if (type === "e")
                openEditEnvironmentModal("ServiceDetails.aspx?s=" + entity.EnvID + "&t=en");
            else if (type === "n") {
                openEditEnvironmentModal("ServiceDetails.aspx");
            }
        };

        $scope.DeleteEnvironment = function (entity) {
            var name = entity.EnvName;
            var message = "Are you sure, you want to delete " + name + "?";
            if (entity.EnvDetailsList != null && entity.EnvDetailsList.length > 0) {
                var noOfService = entity.EnvDetailsList.length;
                if (entity.EnvDetailsList.length === 1) {
                    message = "Are you sure, you want to delete " + name + " with one service?";
                }
                if (entity.EnvDetailsList.length > 1) {
                    message = "Are you sure, you want to delete all " + noOfService + " services under " + name + "?";
                }
            }

            $scope.DeleteConfig(entity.EnvID, message, 'env');
        };

        $scope.DeleteEnvironmentConfiguration = function (entity) {
            var message = "Are you sure, you want to delete configuration detail of " + entity.EnvDetHostIP + ", port: " + entity.EnvDetPort + "?";
            $scope.DeleteConfig(entity.EnvDetID, message, 'cfg');
        };

        $scope.handleError = function (message) {
            $scope.errorMessage = message;
            if (message !== '') {
                $('#divErrorMessage').removeClass().addClass('alert alert-danger');
            }
            if (message === '') {
                $('#divErrorMessage').removeClass('alert alert-danger');
            }
        };


        $scope.getEnvironmentList();
    }
]);

DashboardApp.service("ReportService", dateService);

function dateService() {

    this.getDate = function(repType) {
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

    this.addDays = function(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    this.parsedDate = function(jsonDate) {
        return jsonDate.replace('/Date(', '').replace(')/', '');
    };
}

function UpdategetEnvironmentList() {
    console.log('calling UpdategetEnvironmentList');
    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function() {
        scope.getEnvironmentList();
    });
}