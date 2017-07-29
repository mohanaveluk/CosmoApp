var DashboardApp = angular.module('DashboardApp', ['ngMessages']);
DashboardApp.controller('DashboardController', [
    '$scope', '$http', '$interval', 'ReportService', function ($scope, $http, $interval, reportService) {

        $scope.sortType = 'ConfigServiceType'; // set the default sort type
        $scope.sortTypeGroup = 'Group_Name'; // set the default sort type
        $scope.sortReverse = false; // set the default sort order
        $scope.searchSchedule = ''; // set the default search/filter term

        $scope.tableStatus = '';
        $scope.isExpand = false;
        $scope.environments = [];
        $scope.lastRefreshedTime = reportService.getDate(0);
        $scope.reloadInterval = $scope.reloadTime = 180;
        $scope.selectedServicesList = [];
        $scope.scheduleFilter = "";
        $scope.StatusClass = "";
        $scope.buildDetails = [];

        $scope.getCurrentMonitorList = function () {
            openModal();
            var dataObj = {
                envId: '0',
                isWithServiceName : true
            };

            $scope.getPersonalizeSetting();

            $scope.reloadTime = $scope.reloadInterval === 0 ? 180 : $scope.reloadInterval; // Reset reload time
            openModal();
            $http.post('../forms/Generic.aspx/GetAllMonitorsJson', dataObj)
                .success(function(data, status, headers, config) {
                    $scope.lastRefreshedTime = reportService.getDate(0);
                    $scope.buildDetails = JSON.parse(data.d);
                    try {

                        if ($scope.buildDetails != null && $scope.buildDetails.length > 0) {
                            angular.forEach($scope.buildDetails, function(monitorList) {

                                $scope.environments.push({ id: monitorList.EnvID });

                                angular.forEach(monitorList.monitorList, function(monitor) {
                                    if (monitor.MonitorCreatedDateTime != null)
                                        monitor.MonitorCreatedDateTime = reportService.parsedDate(monitor.MonitorCreatedDateTime);
                                    if (monitor.MonitorUpdatedDateTime != null)
                                        monitor.MonitorUpdatedDateTime = reportService.parsedDate(monitor.MonitorUpdatedDateTime);
                                    if (monitor.LastMoniterTime != null && monitor.LastMoniterTime !== '')
                                        monitor.LastMoniterTime = reportService.parsedDate(monitor.LastMoniterTime);
                                    if (monitor.ConfigServiceType === '1')
                                        monitor.ConfigServiceType = 'Content Manager';
                                    else if (monitor.ConfigServiceType === '2')
                                        monitor.ConfigServiceType = 'Dispatcher';
                                });
                            });
                            $scope.errorMessage = '';
                            $('#divErrorMessage').hide();
                        } else {
                            $scope.errorMessage = "No Service Data Exists";
                            $('#divErrorMessage').show();
                            $('#linkExpand').attr('visibility', 'hidden');
                        }
                        closeModal();
                    } catch (e) {
                        console.log(e);
                        closeModal();
                    }

                })
                .error(function(data, status, headers, config) {
                    closeModal();
                    alert("Error: " + data.Message + " - " + status);
                });
        };

        $scope.LoadServiceStatus = function() {
            if ($scope.buildDetails != null && $scope.buildDetails.length > 0) {
                angular.forEach($scope.buildDetails, function(monitorList, index) {
                    var stringMonitor = angular.toJson(monitorList).replace("");
                    var dataObj = {
                        envId: monitorList.EnvID
                    };
                    $http.post('../forms/Generic.aspx/GetServiceStatusByEnvironment', dataObj)
                        .success(function(data, status, headers, config) {
                            try {
                                monitorList = data.d;
                                angular.forEach(monitorList.monitorList, function(monitor) {
                                    if (monitor.MonitorCreatedDateTime != null)
                                        monitor.MonitorCreatedDateTime = reportService.parsedDate(monitor.MonitorCreatedDateTime);
                                    if (monitor.MonitorUpdatedDateTime != null)
                                        monitor.MonitorUpdatedDateTime = reportService.parsedDate(monitor.MonitorUpdatedDateTime);
                                    if (monitor.LastMoniterTime != null && monitor.LastMoniterTime !== '')
                                        monitor.LastMoniterTime = reportService.parsedDate(monitor.LastMoniterTime);
                                    if (monitor.ConfigServiceType === '1')
                                        monitor.ConfigServiceType = 'Content Manager';
                                    else if (monitor.ConfigServiceType === '2')
                                        monitor.ConfigServiceType = 'Dispatcher';

                                    monitor.StatusClass = $scope.GetStatusClass(monitor.WindowsServiceStatus);
                                });
                                $scope.buildDetails[index] = monitorList;
                                //$scope.apply();
                            } catch (e) {
                                console.log("Error: " + e);
                            }
                        })
                        .error(function(data, status, headers, config) {
                            closeModal();
                            console.log("Error: " + data.Message + " - " + status);
                        });
                });
            }
        };

        $scope.GetAvailableGroupWithSchedules = function(groupId, groupScheduleId, serviceStatus) {
            openModal();
            try {

                var dataObj = {
                    groupID: groupId,
                    grp_sch_ID: groupScheduleId,
                    startTime: DateFormat(new Date),
                    serviceStatus: serviceStatus
                };
                $http.post('../forms/Generic.aspx/GetOpenGroupSchedule', dataObj)
                    .success(function(data, status, headers, config) {

                        //$scope.groupWithSchedule = $.parseJSON(data.d);
                        $scope.groupWithSchedule = data.d;
                        if ($scope.groupWithSchedule != null && $scope.groupWithSchedule.length > 0) {

                            angular.forEach($scope.groupWithSchedule, function(groupSchedule) {
                                if (groupSchedule.Group_Schedule_Datatime != null) {
                                    groupSchedule.Group_Schedule_Datatime = reportService.parsedDate(groupSchedule.Group_Schedule_Datatime);
                                }
                            });

                            $scope.errorMessage = "";
                            $('#divErrorMessage').hide();
                        } else {
                            //$('#divErrorMessage').show();
                            //$scope.errorMessage = "No service data exists";
                        }

                        closeModal();
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        alert("Error: " + data.Message);
                    });

            } catch (e) {
                alert(e);
                closeModal();
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

        var reloadPage = function () {

            $scope.reloadTime--;

            if ($scope.reloadTime <= 0) {
                $scope.reloadTime = $scope.reloadInterval;
                $scope.getCurrentMonitorList();
                $scope.callLoadServiceStatus();
            }
            var time = "Monitor status will update in ";
            time += $scope.reloadTime > 1 ? $scope.reloadTime + ' seconds' : $scope.reloadTime + ' second';
            $scope.ReloadInTime = time;
            GetUpdateServiceStatus();

        };

        $scope.reloadMonitor = function () {
            $interval(function () {
                reloadPage();
            }, 1000);
        };

        $scope.callLoadServiceStatus = function () {
            $interval(function () {
                $scope.LoadServiceStatus();
            }, 1000);
        };

        $scope.getPersonalizeSetting = function () {
            $http.post("../forms/Generic.aspx/GetPersonaliseSetting", {})
                .success(function (data, status, headers, config) {
                    if (data.d === "0" || data.d === undefined) window.location.href = "../login/Default.aspx";
                    $scope.reloadInterval = $scope.reloadTime = data.d * 60;
                })
                .error(function (data, status, headers, config) {
                    console.log(data.d);
                });

        };

        $scope.Notify = function (monitor) {
            openNotifyAlert("NotifyAlert.aspx?mid=" + monitor.MonID + "&e=" + monitor.EnvID + "&s=" + monitor.ConfigID + "&t=en&n=" + monitor.ConfigHostIP + " port: " + monitor.ConfigPort + "&m=st");
        };

        $scope.InvokeService = function(entity, ctrl, mode) {
            InvokeService(entity.WindowsServiceID, entity.WindowsServiceName, mode, entity.ConfigHostIP, $(ctrl.target));
        };

        $scope.AddServiceToSchedule = function (monitor, type, ctrl) {
            var windowsServiceName = "exists";
            // ReSharper disable once AssignedValueIsNeverUsed
            var result = true;

            if (type === 'e') {
                if (ctrl.currentTarget.checked) {
                    angular.forEach(monitor.monitorList, function(mon) {
                        if (mon.WindowsServiceName === null || mon.WindowsServiceName === '') {
                            windowsServiceName = '';
                            return false;
                        }
                    });
                }
                result = GetServiceToAdd(monitor.EnvID, monitor.ConfigID, type, windowsServiceName, ctrl.currentTarget.checked);
                if (!result)
                    ctrl.currentTarget.checked = false;
                else {
                    SelectServices(monitor, ctrl, type);
                }
            } else if (type === 'c') {
                result = GetServiceToAdd(monitor.EnvID, monitor.ConfigID, type, monitor.WindowsServiceName, ctrl.currentTarget.checked);
                if (!result)
                    ctrl.currentTarget.checked = false;
                SelectServices(monitor, ctrl, type);
            }

            SetRowActive(monitor,type);
        }

        $scope.ServiceContainer = function(monitor, isChecked) {
            var obj = { eId: monitor.EnvID, cId: monitor.ConfigID, win: monitor.WindowsServiceID };
            if (isChecked) {
                
                if (idx < 0)
                    $scope.selectedServicesList.push(obj);
            } else {
                
            }
        };

        $scope.EditGroupSchedule = function(groupSchedule) {
            PopulateSelectedGroup("" + groupSchedule.Group_ID + "", "" + groupSchedule.Group_Name + "", "" + formatJSONDate(groupSchedule.Group_Schedule_Datatime) + "", "" + groupSchedule.Group_Schedule_Action + "");
        };


        $scope.CancelGroupSchedule = function(groupScheduleId) {
            console.log(groupScheduleId);
            var confirmCancel = "Are you sure you want to cancel the schedule?";

            if (!confirm(confirmCancel)) return;

            var dataObj = {
                type: 'grpsch',
                groupScheduleId: groupScheduleId,
            };
            openModal();
            try {
                $http.post('../forms/Generic.aspx/CancelGroupSchedule', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.GetAvailableGroupWithSchedules("0", "0", "O");
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        alert("Error: " + data.Message);
                        console.log("Error: " + data.Message);
                    });
            } catch (e) {
                closeModal();
                alert("Error: " + e);
                console.log("Error: " + e);
            }
        };

        $scope.GetStatusClass = function(status) {
            if (status === "Not Exists!" || status === "Not Reachable" || status === "Stopped")
                return "label label-danger";
            else if (status.indexOf('Pending') >= 0)
                return "label label-warning";
            else if (status.indexOf('Loading') >= 0)
                return "label label-default";

            return "label label-success";
        };

        $scope.OpenCognosPortal = function(id, name, page) {
            $("#divMicroProgress").attr("style", "display:block");
            console.log(id, name);
            var dataObj = {
                envId: id,
                urlId: 0
            };

            try {
                openModal();
                $http.post('../forms/Generic.aspx/LogCognosPortalResponse', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.portalResponse = data.d;

                        ShowCognosPortaSuccessModal($scope.portalResponse, name, page);
                            
                        closeModal();
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        alert("Error: " + data.Message);
                        console.log("Error: " + data.Message);
                    });

            } catch (e) {
                closeModal();
                alert("Error: " + e);
                console.log("Error: " + e);
            }
        };

        //$scope.reloadMonitor();
        $scope.getCurrentMonitorList();
        $scope.GetAvailableGroupWithSchedules("0", "0", "O");
    }
]);

DashboardApp.filter('ScheduleStatus', function() {
    return function (items, status) {
        if (status === '') return items;
        return items.filter(function (element) {
            if (status === element.Group_Schedule_Status) {
                return true;
            }
            else if (status === "null" && element.Group_Schedule_Status === null) {
                return true;
            }
        });
    }
});

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


function UpdateMonitorList() {
    console.log('calling UpdategetEnvironmentList');
    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function () {
        scope.getCurrentMonitorList();
        scope.LoadServiceStatus();
    });
}

function GetAvailableGroupWithSchedules() {
    console.log('calling UpdategetEnvironmentList');
    var scope = angular.element(document.getElementById("tblGroupSchedules")).scope();
    scope.$apply(function () {
        scope.GetAvailableGroupWithSchedules("0", "0", "O");
    });
}

function GetUpdateServiceStatus() {
    console.log('calling GetUpdateServiceStatus');
    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function () {
        scope.LoadServiceStatus();
    });
}

var SelectServices = function (monitor, ctrl, type) {

    if (type === "e") {
        var table = $(ctrl.target).closest('table');
        $('td input:checkbox', table).prop('checked', ctrl.currentTarget.checked);
    } else if (type === "c") {
        if (!ctrl.currentTarget.checked) {
            $('#chk_env_' + monitor.EnvID).prop('checked', false);
        } else if (ctrl.currentTarget.checked) {
            $('#chk_env_' + monitor.EnvID).prop('checked', CheckServices($(ctrl.target), monitor.EnvID));
        }
    }
};

var CheckServices = function(ctrl, id) {
    var allChecked = true;
    var table = $('#tblService_' + id + ' tbody');
    table.find('tr').each(function(key, val) {
        var $tds = $(this).find('td'),
            check = $tds.eq(0).find('input[type="checkbox"]').is(':checked');
        if (!check) {
            allChecked = false;
            return false;
        }
    });
    return allChecked;
};

function SetRowActive(monitor, type) {
    $('#tblService_'+ monitor.EnvID + ' tbody').find(':checkbox').each(function () {
        if ($(this).is(':checked'))
            $(this).closest('tr').removeClass().addClass('warning');
        else
            $(this).closest('tr').removeClass();
    });
}

var errorMessage = "";
var selectedPageURL = "";
var homePageURL = "";
function ShowCognosPortaSuccessModal(response, name, page) {
    errorMessage = response.Message;
    
    $(".modal-title").html(page.toProperCase() + " - " + name);
    $("#microErrorMessage").html("<span class='glyphicon glyphicon-info-sign'></span> " + response.Message);
    if (response.Status === "Failure" || response.Status === "") {
        $("#microErrorMessage").attr("style", "display:block");
        $("#iframMicro").attr("style", "display:none");
        $("#divMicroProgress").attr("style", "display:none");
    } else {
        $("#microErrorMessage").attr("style", "display:none");
        $("#iframMicro").attr("style", "display:none");
        homePageURL = response.PortalUrl;
        selectedPageURL = GetPageURL(response, page);
    }
    $('#modalMicroScreen').modal('show');
}

var GetPageURL = function(response, page) {
    var baseUrl = "";
    if (response.PortalUrl.indexOf("cognos.cgi") !== -1) {
        baseUrl = response.PortalUrl;
        homePageURL += "?b_action=xts.run&m=portal/main.xts&CAMUsername=" + response.UserName + "&CAMPassword=" + response.Password;
        if (page === "system") {
            baseUrl += "/cogadmin/cogadminFragments/system";
        } else if (page === "relationships") {
            baseUrl += "/cogadmin/cogadminFragments/relationships";
        } else if (page === "settings") {
            baseUrl += "/cogadmin/cogadminFragments/settings";
        } else if (page === "metrics") {
            baseUrl += "/cogadmin/cogadminFragments/metrics";
        } else if (page === "styles") {
            baseUrl += "/cogadmin/cogadminFragments/styles";
        } else if (page === "capabilities") {
            baseUrl += "/cogadmin/cogadminFragments/capabilities";
        } else if (page === "connection") {
            baseUrl = homePageURL;
        }
    }
    return baseUrl;
};

$(function() {
    // Handler for .ready() called.
    setTimeout(function() { console.log('Calling GetUpdateServiceStatus js function');
        GetUpdateServiceStatus();
    }, 1000);
    $('#modalMicroScreen').on('shown.bs.modal', function() {
        var frameWindow = $(this).find('iframe');
        frameWindow.attr("style", "background:#ccc");
        frameWindow.attr('src', homePageURL);

        setTimeout(
            function() {
                //PopupCognosPage($(this).find('iframe'));//.attr('src', selectedPageURL);
                frameWindow.prop('src', selectedPageURL);
                frameWindow.attr("style", "visibility:visible");
                $("#iframMicro").attr("style", "display:block");
                $("#divMicroProgress").attr("style", "display:none");
            }, 1000);
    });
});

function PopupCognosPage(element) {
    element.attr('src', selectedPageURL);
}

String.prototype.toProperCase = function () {
    return this.replace(/\w\S*/g, function(txt){return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();});
};