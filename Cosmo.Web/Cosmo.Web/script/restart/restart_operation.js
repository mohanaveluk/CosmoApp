"use strict";
var startInterval;
var currentServiceStatus;
var IsServiceAddedInTempTable = false;
var ServiceHostPort = '';
var availableGroups = [];
var availableGroupsId = [];

var isExistingGroupSelected = false;

$(document).ready(function () {
    BindControls();
    //Helper function to keep table row from collapsing when being sorted
    $('[data-toggle="tooltip"]').tooltip()
    var fixHelperModified = function (e, tr) {
        var $originals = tr.children();
        var $helper = tr.clone();
        $helper.children().each(function (index) {
            $(this).width($originals.eq(index).width());
        });
        return $helper;
    };

    //Delete button in table rows
    $('table').on('click', '.btn-delete', function () {
        tableID = '#' + $(this).closest('table').attr('id');
        r = confirm('Delete this item?');
        if (r) {
            $(this).closest('tr').remove();
            renumber_table(tableID);
        }
    });

    $("input:radio[name=rdoServiceMode]").blur(function () {
        if ($("input:radio[name=rdoServiceMode]:checked").val() != undefined) {
            $('.errors').remove();
        }
    });
    $("input:radio[name=rdoServiceMode]").click(function () {
        if ($("input:radio[name=rdoServiceMode]:checked").val() != undefined) {
            $('.errors').remove();
        }
    });

    $('#btnSubmitSchedule').click(function() {
        var data = [];
        var Errors = false;
        $('.errors').remove();
        clearInterval(startInterval);
        var sWinServiceID = '';
        var sEnvID = '';
        var sConfigID = '';

        //Get group id / name
        //var sGroup = $('#<%=drpGroup.ClientID %> option:selected').val();
        var sGroupName = $('#txtGroup').val();
        var sGroupID = GetGroupId(sGroupName);

        var sScheduleAction = $("input:radio[name=rdoServiceMode]:checked").val();
        var sScheduleTime = $("#txtStartDate").val();
        var table = $("#PrioritywiseBreached tbody");

        //Check if service action is selected
        if (sScheduleAction == undefined) {
            //$("#divError").html("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select service action'></span>");
            //$('#saveButton').after("<div style='background-color:white;margin-top:5px'><span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select service action'></span></div>");
            $("input:radio[name=rdoServiceMode]").closest('.form-group').removeClass().addClass('form-group has-error');
            $("input:radio[name=rdoServiceMode]").focus();
            Errors = true;
        } else {
            $("input:radio[name=rdoServiceMode]").closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
        }

        if (sScheduleTime === '') {
            $('#lblDateError').html("Please select date and time for service action");
            Errors = true;
        }

        if (!CompareScheduleTime($('#lblCurrentTime').text(), sScheduleTime) && !Errors) {
            $("#div-schedule-error").show();
            $("#txtStartDate").closest('.form-group').removeClass().addClass('form-group has-error');
            $('#lblSchedule-error').html("Oops. It seems the schedule time is earlier than the current time");
            $("#div-schedule-error").removeClass().addClass("alert alert-danger");
            $("#txtStartDate").focus();
            Errors = true;
        } else {
            $("#txtStartDate").closest('.form-group').removeClass().addClass('form-group');
            $('#lblSchedule-error').html("");
            $("#div-schedule-error").hide();
        }

        if (sScheduleTime == '' && !Errors) {
            showAlert("Please select date and time for service action.", "error");
            Errors = true;
        }
        if (!Errors) {
            openModal();
            table.find('tr').each(function(key, val) {
                var cols = [];
                /*$(this).find('th,td').each(function (colIndex, c) {
                cols.push(c.textContent);
                });*/
                var $tds = $(this).find('td'),
                    wsID = $tds.eq(0).text(),
                    eID = $tds.eq(1).text(),
                    cID = $tds.eq(2).text();
                //data.push(cols);
                sWinServiceID += wsID + ",";
                sEnvID += eID + ",";
                sConfigID += cID + ",";
            });

            if (sConfigID != '') {
                $.ajax({
                    type: "POST",
                    url: "../forms/Generic.aspx/InsUpdateGroupSchedule",
                    data: "{'groupId':'" + sGroupID + "','groupName':'" + sGroupName + "','envID':'" + sEnvID + "','configID':'" + sConfigID + "','winServiceID':'" + sWinServiceID + "','serviceAction':'" + sScheduleAction + "','actionDateTime':'" + sScheduleTime + "','requestSource':'Desktop'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccessWinService,
                    failure: function(response) {
                        closeModal();
                        alert(response.d);
                    }
                });

                IsServiceAddedInTempTable = false; // remove the flag of changes that happened on adding service for schedule
                $('#hidGroupID').val("");
                $('#hidGroupName').val("");
                isExistingGroupSelected = false;

            }
        } else {
            return false;
        }
    });

    $('#btnGroupSubmit').click(function () {
        $('.errors').remove();
        if (IsServiceAddedInTempTable) {
            if (confirm("Do you want to cancel the change?") == true) {
                IsServiceAddedInTempTable = false;
            }
            else
                return false;
        }
        var sGroupId = '';
        var sGroupName = $('#txtGroup').val().trim();

        sGroupId = GetGroupId(sGroupName);
        $('#<%=hidGroupID.ClientID %>').val(sGroupId);
        $('#<%=hidGroupName.ClientID %>').val(sGroupName);
        //alert(sGroupId + ' ' + sGroupName);

        if (sGroupId == '0' || sGroupName == 'Select' || sGroupName == 'select') {
            $("#PrioritywiseBreached tbody > tr").remove();
            $('#addSchedule').css("visibility", "hidden");
            return false;
        }
        else {
            return true;
        }
    });

    $('.btnDelete').click(function () {
        $(this).closest('tr').remove();
    });


    $('#txtGroup').on('input blur', function () {
        if ($.trim($('#txtGroup').val()) !== '')
            showAlertFadeOut();
        //else
        //    showAlert("Invalid Group Name", "error");
    });

    $("input:radio[name=rdoServiceMode]").change(function() {
        $("input:radio[name=rdoServiceMode]").closest('.form-group').removeClass().addClass('form-group ');
    });

    $("#txtStartDate").on('input blur', function() {
        var sScheduleTime = $("#txtStartDate").val();
        if (!CompareScheduleTime($('#lblCurrentTime').html(), sScheduleTime)) {
            $("#div-schedule-error").show();
            $("#txtStartDate").closest('.form-group').removeClass().addClass('form-group has-error');
            $('#lblSchedule-error').html("Oops. It seems the schedule time is earlier than the current time");
            $("#div-schedule-error").removeClass().addClass("alert alert-danger");
        } else {
            if ($('#lblSchedule-error').html() === '') return;
            $("#txtStartDate").closest('.form-group').removeClass().addClass('form-group has-success');
            $('#lblSchedule-error').html("Yes, You got it");
            $("#div-schedule-error").show();
            $("#div-schedule-error").removeClass().addClass("alert alert-success");
        }
    });
    $('#btnCancel').click(function() {
        ClearAllServices();
        $('#txtGroup').val('');
    });
});

var GetGroupId = function (grpName) {
    //alert(GroupArr.length);
//    for (var i = 0; i < GroupArr.length; i++) {
//        if (GroupArr[i][0] == grpName) {
//            return GroupArr[i][1];
//        }
//    }
    //    return '0';

    for (var i = 0; i < availableGroups.length; i++) {
        if (availableGroups[i] === grpName) {
            return availableGroupsId[i];
        }
    }
    return 0;
}


var GetGroupName = function (grpId) {
    //alert(GroupArr.length);
    for (var i = 0; i < GroupArr.length; i++) {
        if (GroupArr[i][1] == grpId) {
            return GroupArr[i][0];
        }
    }
    return '0';
}

//Renumber table rows
function renumber_table(tableID) {
    $(tableID + " tr").each(function () {
        count = $(this).parent().children().index($(this)) + 1;
        $(this).find('.priority').html(count);
    });
}

function ScheduleWindow(mode) {
    if (mode) {
        $('#addSchedule').css("visibility", "visible");
        if ($('#<%=hidGroupName.ClientID %>').val() !== '')
            $('#txtGroup').val($('#<%=hidGroupName.ClientID %>').val());

    } else {
        $('#addSchedule').css("visibility", "hidden");
    }
}

function OnSuccessWinService(response) {
    //alert(response.d);
    try {

        $('.errors').remove();
        if (response.d !== '') {
            var jsonResult = $.parseJSON(response.d);
            //PopulateGroupSchedule(jsonResult);
            GetAvailableGroupWithSchedules();

            IsServiceAddedInTempTable = false;
            showAlert("Request has been Scheduled successfully.", "success");
            $('#WindownServiceScheduleModal').modal('hide');

            //Delete / clear the selected records
            ClearAllServices();
            var table = $("#PrioritywiseBreached tbody");
            table.find('tr').each(function(key, val) {
                $(this).remove();
            });

            closeModal();
            GetAllGroups();
        } else {
            closeModal();
            showAlert("Schedule has not updated.", "error");
        }
    } catch (e) {
        showAlert(e, "error");
        closeModal();
    }

}

function PopulateGroupSchedule(data) {
    var bodybg = '';
    $('#records_table').html('');
    var $tr = $('<tr class="reportTableHeadChild">').append(
                    $('<th>').text('#'),
                    $('<th>').text('Group'),
                    $('<th>').text('Schedule time'),
                    $('<th>').text('Action'),
                    $('<th>').text('Status')
                ).appendTo('#records_table');
    $.each(data, function (i, item) {
        //                alert(item.Group_Name);
        //var param = 'PopulateSelectedGroup("' + item.Group_ID + '","' + item.Group_Name + '")';
        $tr = $('<tr>').append(
                    $('<td>').text(i + 1),
                    $('<td class="alignLeft">').html('<a id="groupName" href="#" onclick="PopulateSelectedGroup(\'' + item.Group_ID + '\',\'' + item.Group_Name + '\',\'' + formatJSONDate(item.Group_Schedule_Datatime) + '\',\'' + item.Group_Schedule_Action + '\')">' + item.Group_Name + '</a>'),
                    $('<td class="alignLeft">').text(formatJSONDate(item.Group_Schedule_Datatime)),
                    $('<td class="alignLeft">').text(item.Group_Schedule_Action),
                    $('<td class="alignLeft">').text(item.Group_Schedule_Status)
                ).appendTo('#records_table');
    });


}

function formatJSONDate(dateAsFromServerSide) {
    //http://codeasp.net/assets/demos/blogs/convert-net-datetime-to-javascript-date.aspx
    //var newDate = dateFormat(jsonDate, "mm/dd/yyyy HH:MM");

    var jsonparsedDate = dateAsFromServerSide === null
        ? new Date()
        : (dateAsFromServerSide.indexOf('/Date') >= 0
            ? new Date(parseInt(dateAsFromServerSide.substr(6)))
            : new Date(parseInt(dateAsFromServerSide)));
    var jsDate = new Date(jsonparsedDate); //Date object
    var cMonth = (jsDate.getMonth() + 1).toString().length == 1 ? '0' + parseInt(jsDate.getMonth() + 1) : parseInt(jsDate.getMonth() + 1);
    var cDate = jsDate.getDate().toString().length == 1 ? '0' + jsDate.getDate() : jsDate.getDate();
    var cYear = jsDate.getFullYear().toString().length == 1 ? '0' + jsDate.getFullYear() : jsDate.getFullYear();

    var cHour = jsDate.getHours().toString().length == 1 ? '0' + jsDate.getHours() : jsDate.getHours();
    var cMin = jsDate.getMinutes().toString().length == 1 ? '0' + jsDate.getMinutes() : jsDate.getMinutes();
    var cSec = jsDate.getSeconds().toString().length == 1 ? '0' + jsDate.getSeconds() : jsDate.getSeconds();
    var cSec = jsDate.getSeconds().toString().length == 1 ? '0' + jsDate.getSeconds() : jsDate.getSeconds();


    var convertedDateTime = cMonth + '/' + cDate + '/' + cYear + ' ' + cHour + ':' + cMin + ':' + cSec;
    return convertedDateTime;
}

function GetOpenSchedule(sGroup, grp_sch_ID, serviceStatus) {
    $.ajax({
        type: "POST",
        url: "../forms/Generic.aspx/GetOpenGroupSchedule",
        data: "{'groupId':'" + sGroup + "','grp_sch_ID':'" + grp_sch_ID + "','startTime':'" + DateFormat(new Date) + "','serviceStatus':'" + serviceStatus + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessGetOpenSchedule,
        failure: function (response) {
            alert(response.d);
        }
    });
}

function OnSuccessGetOpenSchedule(response) {
    var jsonResult = $.parseJSON(response.d);
    PopulateGroupSchedule(jsonResult);
}

function AssignOpenSchedule(action, schTime) {
    if (action == "Start")
        $('input:radio[name="rdoServiceMode"][value="1"]').prop('checked', true);
    else if (action == "Stop")
        $('input:radio[name="rdoServiceMode"][value="2"]').prop('checked', true);
    else if (action == "Restart")
        $('input:radio[name="rdoServiceMode"][value="3"]').prop('checked', true);

    var schDateTime = new Date(schTime);
    if (schDateTime.toString() != 'Invalid Date') {
        var sMonth = schDateTime.getMonth() + 1;
        var sDate = schDateTime.getDate();
        var sYear = schDateTime.getFullYear();
        var sHour = schDateTime.getHours();
        if (sMonth.toString().length == 1) sMonth = '0' + sMonth;
        if (sDate.toString().length == 1) sDate = '0' + sDate;
        if (sHour >= 12) sHour -= 12;


        $('#<%=txtFromDateTime.ClientID %>').val(sMonth + '/' + sDate + '/' + sYear);
        document.getElementById('ctl00$ContentPlaceHolder1$txtTimeSelector_txtHour').value = sHour.toString().length == 1 ? '0' + sHour : sHour;
        document.getElementById('ctl00$ContentPlaceHolder1$txtTimeSelector_txtMinute').value = schDateTime.getMinutes().toString().length == 1 ? '0' + schDateTime.getMinutes() : schDateTime.getMinutes();
        document.getElementById('ctl00$ContentPlaceHolder1$txtTimeSelector_txtSecond').value = schDateTime.getSeconds().toString().length == 1 ? '0' + schDateTime.getSeconds() : schDateTime.getSeconds();
        if (schTime.indexOf('AM') > 0)
            document.getElementById('ctl00$ContentPlaceHolder1$txtTimeSelector_txtAmPm').value = 'AM';
        else if (schTime.indexOf('PM') > 0)
            document.getElementById('ctl00$ContentPlaceHolder1$txtTimeSelector_txtAmPm').value = 'PM';
    }
    //$('#ctl00$ContentPlaceHolder1$txtTimeSelector_txtAmPm').val(schDateTime.getformat
}

function PopulateSelectedGroup(groupId, groupName, scheduleTime, action) {
    try {
        var sGroup = $('#hidGroupID').val();
        if (sGroup == undefined) sGroup = "";
        if (sGroup != groupId && (groupName != '' && groupName != '0')) {
            $('#hidGroupID').val(groupId);
            $('#hidGroupName').val(groupName);
            $('#txtGroup').val(groupName);
            GetSelectedGroupDetail(groupId);
            $('#txtStartDate').val(ScheduleDateFormat(scheduleTime));
            $('#rdo' + action).prop('checked', 'true');
            //console.log("action: " + action + ", scheduleTime: " + scheduleTime, ", groupId: " + groupId + ", groupName: " + groupName);
        }
        
        action === "" || action === "null" ? isExistingGroupSelected = false : isExistingGroupSelected = true;

    } catch (e) {
        closeModal();
    }
}


function ShowProgress(ctrl) {
    $('#' + ctrl).after("<span class='errorserver'>&nbsp;<img src='../images/progress.gif' title='Checking...'></span>");
}

function InvokeService(serviceID, serviceName, serviceMode, systemName, ctrl) {

    var windowServiceStatus = ''; var windowServiceStatusColumn = 8;

    if (!confirm("Are you sure to " + serviceMode + " the service " + serviceName + " in " + systemName + "?")) return false;

    windowServiceStatus = $(ctrl).closest('tr').find('td:eq(' + windowServiceStatusColumn + ')').text();

    $(ctrl).closest('tr').find('td:eq(' + windowServiceStatusColumn + ')').html('<span class="label label-warning">Processing...</span>');

    openModal();
    $.ajax({
        type: "POST",
        url: "../forms/Generic.aspx/GetWindowssService",
        data: "{'serviceName':'" + serviceName + "','serviceMode':'" + serviceMode + "','systemName':'" + systemName + "','serviceId':'" + serviceID + "','requestSource':'Desktop'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function onSuccessWinServiceOperation(response) {
            //checkServiceStatus(serviceName, serviceMode, systemName);
            if (response.d == 'cancelled') {
                //GetServiceStatus(serviceName, serviceMode, systemName, ctrl);
                alert("Service operation has beed cancelled");
                $(ctrl).closest('tr').find('td:eq(' + windowServiceStatusColumn + ')').text(windowServiceStatus);
                closeModal();
            }
            else if (response.d.toLowerCase() == 'running') {
                //GetServiceStatus(serviceName, serviceMode, systemName, ctrl);
                //$(ctrl).closest('tr').find('td:eq(' + windowServiceStatusColumn + ')').text(windowServiceStatus);
                closeModal();
            }
            else if (response.d == 'timedout') {
                $(ctrl).closest('tr').find('td:eq(' + windowServiceStatusColumn + ')').text(windowServiceStatus);
                closeModal();
                //GetServiceStatus(serviceName, serviceMode, systemName, ctrl);
            }
            else if (response.d == 'processing') {
                currentServiceStatus = setInterval(function () {
                    GetServiceStatus(
                        serviceName,
                        serviceMode,
                        systemName,
                        ctrl);
                }, 3000);
            }
            UpdateMonitorList();
        },
        failure: function (response) {
            closeModal();
            alert(response.d);
        }
    });
}

function GetServiceStatus(serviceName, serviceMode, systemName, ctrl) {
    var sAction; var windowServiceStatusColumn = 8;
    if (serviceMode == '1') serviceMode = "Running";
    else if (serviceMode == '2') serviceMode = "Stopped";
    else if (serviceMode == '3') serviceMode = "Running";
    $.ajax({
        type: "POST",
        url: "../forms/Generic.aspx/GetWindowsServiceStatus",
        data: "{'serviceName':'" + serviceName + "','systemName':'" + systemName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            sAction = response.d;
            if (serviceMode == sAction) {
                if (currentServiceStatus != undefined)
                    clearInterval(currentServiceStatus);
            }
            UpdateMonitorList();
            closeModal();
        },
        failure: function (response) {
            closeModal();
            alert(response.d);
        }
    });
}

//Compare datetime between current time and schedule time
function CompareScheduleTime(cTime, sTime) {
    var currentTime = new Date(cTime);
    var scheduleTime = new Date(sTime);
    //alert(currentTime.getTime() + ' - ' + scheduleTime.getTime());
    if (scheduleTime < currentTime)
        return false;
    else
        return true;
}
//Populate monitor service details to add into the group
function GetServiceToAdd(envId, conId, type, windowServiceName, isChecked) {
    if (isChecked) {
        if (type === "e") RemoveServiceFromList(envId, type);
        if (type === 'e' && windowServiceName === '') {
            alert('Windows service name has not configured for one or more cosmo services. Please setup the windows service name');
            return false;
        } else if (type === 'c' && windowServiceName === '') {
            alert('Windows service name has not configured for this service. Please setup the windows service name');
            return false;
        }
        if (type === 'e' && !IsEnvironmentExists(envId, type)) {
            alert('You cannot add service from other environment');
            return false;
        } else if (type === 'c' && IsConfigExists(conId, type)) {
            alert('Service: ' + ServiceHostPort + ' is already added in the group');
            return false;
        }
        if (type === 'c' && !IsEnvironmentExists(envId, type)) {
            alert('You cannot add service from other environment');
            return false;
        }

        $.ajax({
            type: "post",
            url: "../forms/Generic.aspx/getMonitorStatusWithServiceName",
            data: type === "e" ? "{'id':'" + envId + "', 'type':'" + type + "'}" : "{'id':'" + conId + "', 'type':'" + type + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: PopulateFromAddToGroup,
            failure: function(response) {
                alert(response.d);
            }
        });
    } else {
        if (type==="e")
            RemoveServiceFromList(envId, type);
        else
            RemoveServiceFromList(conId, type);
        /*
        if (type === "c")
            showAlert(windowServiceName + ' has removed from schedule list');
        else if(type==="e")
            showAlert('Services are removed from schedule list');
            */
    }
    //CheckServices(envId, type, isChecked);
    return true;
}

var IsConfigExists = function (id, type) {
    var itemExists = false;
    var table = $("#PrioritywiseBreached tbody");
    table.find('tr').each(function (key, val) {
        var $tds = $(this).find('td'),
                        wsID = $tds.eq(0).text(),
                        eID = $tds.eq(1).text(),
                        cID = $tds.eq(2).text();
        ServiceHostPort = $tds.eq(8).text() + ' for instance ' + $tds.eq(5).text();
        if (cID === id.toString() && type === 'c') {
            itemExists = true;
            return false;
        }
    });
    return itemExists;
}

var IsEnvironmentExists = function (id, type) {
    var itemExists = true;
    var table = $("#PrioritywiseBreached tbody");
    table.find('tr').each(function (key, val) {
        var $tds = $(this).find('td'),
                        wsID = $tds.eq(0).text(),
                        eID = $tds.eq(1).text(),
                        cID = $tds.eq(2).text();
        if (eID !== id.toString() && (type === 'e' || type === 'c')) {
            itemExists = false;
            return false;
        }
    });
    return itemExists;
}

var RemoveServiceFromList = function (id, type) {
    var itemExists = false;
    var table = $("#PrioritywiseBreached tbody");
    table.find('tr').each(function (key, val) {
        var $tds = $(this).find('td'),
                        wsID = $tds.eq(0).text(),
                        eID = $tds.eq(1).text(),
                        cID = $tds.eq(2).text();
        ServiceHostPort = $tds.eq(8).text() + ' for instance ' + $tds.eq(5).text();
        if ((cID === id.toString() && type === 'c') || (eID === id.toString() && type === 'e')) {
            $(this).remove();
        }
    });
}

function ClearAllScheduleServices() {
    $("#PrioritywiseBreached tbody > tr").remove();
}

function ClearAllServices() {
    var table = $("#MainWrap tbody");
    $('td input:checkbox', table).prop('checked', false);
    $("#MainWrap tbody > tr").removeClass();
    $("#PrioritywiseBreached tbody > tr").remove();
    $('#hidGroupID').val('');
    $('#hidGroupName').val('');
    //$('#txtGroup').val('');
    isExistingGroupSelected = false;
}

var GetSerialNumber = function () {
    var iSrNo = 0;
    var table = $("#PrioritywiseBreached tbody");
    //            table.find('tr').each(function (key, val) {
    //                var $tds = $(this).find('td'),
    //                        iSrNo = $tds.eq(3).text();
    //            });
    if (table.find('tr').length > 0) {
        if (table.find('tr:last td:eq(3)').text() != '' || table.find('tr:last td:eq(3)').text() != undefined)
            iSrNo = parseInt(table.find('tr:last td:eq(3)').text());
    }

    return iSrNo;
}
function PopulateFromAddToGroup(response) {
    var data = response.d;
    var statusText = "label";
    if (data != null && data.length > 0) {
        $.each(data, function (i, item) {
            if (item.WindowsServiceName != '') {
                if (IsEnvironmentExists(item.EnvID, 'e')) {
                    if (!IsConfigExists(item.ConfigID, 'c')) {
                        if (item.Incident_Issue === "Running")
                            statusText = "<span class='label label-success'>" + item.Incident_Issue + "</span>";
                        else
                            statusText = "<span class='label label-danger'>" + item.Incident_Issue + "</span>";
                        var $tr = $('<tr>').append(
                                    $('<td style="display: none">').text(item.WindowsServiceID),
                                    $('<td style="display: none">').text(item.EnvID),
                                    $('<td style="display: none">').text(item.ConfigID),
                                    //$('<td class="alignLeft" style="display:none">').text(item.Group_Name),
                                    $('<td>').text(GetSerialNumber() + 1),
                                    $('<td >').text(item.ConfigServiceType),
                                    $('<td >').text(item.ConfigServiceDescription),
                                    //$('<td class="alignLeft">').text(item.ConfigLocation),
                                    //$('<td class="alignCenter">').html('<img src="' + item.Incident_Solution + '" title="' + item.MonitorStatus + '">'),
                                    $('<td >').text(item.WindowsServiceName),
                                    $('<td class="text-center">').html(statusText)
                                    //$('<td class="alignLeft">').text(''),
                                    //$('<td class="text-center" style="margin-left:10px">').html('<a href="#" onclick="DeleteServiceFromList(\'' + item.EnvID + '\',\'' + item.ConfigID + '\',\'c\',\'' + item.WindowsServiceName + '\',false)"><img src="../images/remove.jpg" class="btnDelete" id="btnDeleteService" title="Remove service from schedule list" /></a>')
                                ).appendTo('#PrioritywiseBreached');
//                        $('.btnDelete').on('click', function () {
//                            //$(this).parent().parent().remove();
//                            //GetServiceToAdd(item.ConfigID, 'c', item.WindowsServiceName, false);
//                            var cid = $(this).closest('tr').find('td:eq(3)').text();
//                        });
                        //showAlert(item.WindowsServiceName + ' has added to the schedule list');
                        showAlertFadeOut();
                    }
                    else {
                        alert('Service: ' + item.WindowsServiceName + ' for instance ' + item.ConfigHostIP + ':' + item.ConfigPort + ' is already added in the group');
                        return false;
                    }
                }
                else {
                    alert('You cannot add service from other environment');
                    return false;
                }
            }
            else {
                alert('Windows service name has not set for this service configuration. Please setup the windows service name');
                return false;
            }
        });

        IsServiceAddedInTempTable = true;
    }
}

function PopulateGroup() {
    $.ajax({
        type: "POST",
        url: "../forms/Generic.aspx/GetAllGroups",
        data: "{'grpId':'0'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: PopulatGroupToSelect,
        failure: function (response) {
            alert(response.d);
        }
    });
}
function PopulatGroupToSelect(response) {
    var grpData = $.parseJSON(response.d);
    var selectGroup = $('#<%=drpGroup.ClientID %>');
    selectGroup.empty();
    $.each(grpData, function (i, item) {
        selectGroup.append(new Option(item.Group_Name, item.Group_ID));
    });
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
    $('#label-alert').fadeOut(1000).html('');
}

function DeleteServiceFromList(eId, cid, type, name, checked) {
    GetServiceToAdd(cid, type, name, checked);
    var table = $("#MainWrap tbody");
    table.find('tr').each(function (key, val) {
        var $tds = $(this).find('td'),
                        wsID = $tds.eq(0).text(),
                        eID = $tds.eq(1).text(),
                        cID = $tds.eq(2).text();
        ServiceHostPort = $tds.eq(8).text() + ' for instance ' + $tds.eq(5).text();
        if (cID === id.toString() && type === 'c') {
            itemExists = true;
            return false;
        }
    });
}

$(function () {
    $('td:first-child input').click(function () {
        $(this).closest('tr').toggleClass("active", this.checked);
    });
});

function SubmitStatus() {
    var tableRowCount = $("#PrioritywiseBreached tbody tr").length;

    if (tableRowCount <= 0) {
        showAlert("Please select at least one service to schedule.", "error");
        return false;
    }
    var sGroupName = $('#txtGroup').val();
    if (sGroupName.trim() === "") {
        showAlert("Invalid Group Name.", "error");
        $('#txtGroup').removeClass('InceidentText').addClass('InceidentTextError');
        $('#txtGroup').focus();
        return false;
    }

    var serverTime = new Date($("#lblNoteServerDateTime").text());
    console.log(serverTime);
    serverTime.setMinutes(serverTime.getMinutes() + 1);

    //Set option for action
    $('#rdoStart').prop("disabled", false);
    $('#rdoStop').prop("disabled", false);
    $('#rdoRestart').prop("disabled", false);
    if (!isExistingGroupSelected) {
        $('#rdoStart').prop('checked', false);
        $('#rdoStop').prop('checked', false);
        $('#rdoRestart').prop('checked', false);

        var table = $("#PrioritywiseBreached tbody");
        if (table.find('tr').length === 1) {

            var $tds = table.find('tr').find('td'),
                wsID = $tds.eq(0).text(),
                eID = $tds.eq(1).text(),
                cID = $tds.eq(2).text(),
                wsStatus = $tds.eq(7).text();

            if (wsStatus.toLowerCase().indexOf("exist") >= 0) {
                $('#rdoStart').prop("disabled", true);
                $('#rdoStop').prop("disabled", true);
                $('#rdoRestart').prop("disabled", true);
            } else if (wsStatus.toLowerCase().indexOf("stop") >= 0) {
                //$('#rdoStop').prop("disabled", true);
                //$('#rdoRestart').prop("disabled", true);
                $('#rdoStart').prop('checked', true);
            } else if (wsStatus.toLowerCase() === "running" || wsStatus.toLowerCase() === "starting") {
                $('#rdoStop').prop('checked', true);
                //$('#rdoStart').prop("disabled", true);
                //if (wsStatus.toLowerCase() === "starting")
                //    $('#rdoRestart').prop("disabled", true);
            }
        }

        $('#txtStartDate').val(ScheduleDateFormat(serverTime));
    }

    $('#WindownServiceScheduleModal').modal({ show: true });

    if (tableRowCount <= 0)
        $('#btnSubmitSchedule').prop('disabled', true);
    else {
        $('#btnSubmitSchedule').prop('disabled', false);
    }

    showAlertFadeOut();
    return true;
}

function BindControls() {
    GetAllGroups();
    var option = ['alpha', 'allpha2', 'alpha3', 'bravo', 'charlie', 'delta', 'epsilon', 'gamma', 'zulu'];

}

var GetAllGroups = function() {
 
    $.ajax({
        type: "POST",
        url: "../forms/Generic.aspx/GetAllGroups",
        data: "{'grpId':'0'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccessGetGroups,
        failure: function (response) {
            alert(response.d);
        }
    });

}

function OnSuccessGetGroups(response) {
    var random = Math.floor((Math.random() * 10) + 1);
    availableGroups = [], availableGroupsId = [];
    $('#txtGroup').typeahead('destroy');
    $('#txtGroup').val('');
    var groups = JSON.parse(response.d);
    if (groups != null) {
        $.each(groups, function(key, group) {
            availableGroups.push(group.Group_Name);
            availableGroupsId.push(group.Group_ID);
        });

        $('#txtGroup').typeahead({
            limit: 10,
            name: 'Environments_' + random,
            display: 'team',
            local: availableGroups,
            updater: function(item) { console.log(item); }
        });
        $('.tt-query').css('background-color', '#fff');
        //GetAvailableGroups();
        /*
        $('#txtGroup').on('typeahead:selected', function(e, datum) {
            console.log(datum.value + ' - ' + GetGroupId(datum.value));
        });
        $('#txtGroup').on('typeahead:autocompleted', function (e, datum) {
            console.log(datum.value + ' - ' + GetGroupId(datum.value));
        });
        */
        $('#txtGroup').on(
        {
            "typeahead:selected": function(e, datum) {
                console.log(datum.value + ' - ' + GetGroupId(datum.value));
                GetSelectedGroupDetail(GetGroupId(datum.value));
            },
            "typeahead:autocompleted": function(e, datum) {
                console.log(datum.value + ' - ' + GetGroupId(datum.value));
                GetSelectedGroupDetail(GetGroupId(datum.value));
            }

        });

        /*
        $('#txtGroup').on("keyup", function (e) {
            console.log("Cliecked");
            var keynum = '';
            if (window.event) { // IE                    
                keynum = e.keyCode;
            } else if (e.which) { // Netscape/Firefox/Opera                   
                keynum = e.which;
            }
            if (keynum === 40) {

                $(".tt-dropdown-menu").css("display", "block");
                $(".tt-dataset-Environments_" + random).css("display", "block");
            }
        });
        */
    }
    $('#txtGroup').select();
    $('#txtGroup').focus();
}

function GetSelectedGroupDetail(groupId) {
    try {
        openModal();
        ClearAllServices();
        $.ajax({
            type: "POST",
            url: "../forms/Generic.aspx/GetGroupDetails",
            data: "{'grpId':'" + groupId + "', 'envId':'0'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessGetGroupDetail,
            failure: function (response) {
                closeModal();
                alert(response.d);
            }
        });
    } catch (e) {
        closeModal();
    } 
    
}

function OnSuccessGetGroupDetail(response) {
    try {
        var details = response.d;
        if (details.length <= 0) throw "Group detail doesn't exists!";
        if (details[0].Comments.indexOf("Error:") >= 0) throw details[0].Comments;
        if (details.length <= 0) throw "Service is configured for this group";
        $('#row_' + details[0].Env_ID).show();

        $.each(details, function (key, detail) {
            var checkId = "#chk_con_" + detail.Env_ID + "_" + detail.Config_ID + "_" + detail.WinService_ID;
            $(checkId).trigger("click");
        });
        closeModal();
    } catch (e) {
        showAlert(e,"error");
        closeModal();
    }
}

function setCurrentTime(timeString, serverTimeString) {

    var dTime = new Date(timeString);
    dTime.setSeconds(dTime.getSeconds() + 1);

    var serverTime = new Date(serverTimeString);
    serverTime.setSeconds(serverTime.getSeconds() + 1);

    //$('#lblCurrentTime').html(dTime.format('M/dd/yyyy h:mm:ss tt'));
    $('#lblCurrentTime').html(DateFormat(dTime));
    $('#lblNoteServerDateTime').html(DateFormat(serverTime));

    var localTime = new Date();
    localTime.setSeconds(localTime.getSeconds() + 1);
    //$('#lblNoteClientDateTime').html(DateFormat(dTime));
    $('#lblNoteClientDateTime').html(DateFormat(new Date));

    $('#lblNoteClientTimezone').text(getTimeZone);
}


Number.prototype.padLeft = function (base, chr) {
    var len = (String(base || 10).length - String(this).length) + 1;
    return len > 0 ? new Array(len).join(chr || '0') + this : this;
}

var ScheduleDateFormat = function (d) {
    var sec = 0;
    if (firstTime) {
        d.setSeconds(d.getSeconds() + 0.99);
        firstTime = false;
    }
    var dFormat = [
            (d.getMonth() + 1).padLeft(),
            d.getDate().padLeft(),
            d.getFullYear()
        ].join('/') +
        ' ' +
        [
            d.getHours().padLeft(),
            d.getMinutes().padLeft()
        ].join(':');

    return dFormat;
}

var DateFormat = function (d) {
    var sec = 0;
    if (firstTime) {
        d.setSeconds(d.getSeconds() + 0.99);
        firstTime = false;
    }
    var dFormat = [(d.getMonth() + 1).padLeft(),
                    d.getDate().padLeft(),
                    d.getFullYear()].join('/') +
                    ' ' +
                  [d.getHours().padLeft(),
                    d.getMinutes().padLeft(),
                    d.getSeconds().padLeft()].join(':');
    return dFormat;
}
var firstTime = true;
//$('#lblCurrentTime').html(DateFormat(new Date));
$('#lblNoteClientDateTime').html(DateFormat(new Date));
setInterval(function () { setCurrentTime($('#lblCurrentTime').text(), $('#lblNoteServerDateTime').text()) }, 1000);

//GetOpenSchedule("0", "0", "O");

function getTimeZone() {
    return /\((.*)\)/.exec(new Date().toString())[1];
}

$(function () {
    var date = new Date();
    date.setDate(date.getDate());
    $('#datetimepicker1').datetimepicker({
        defaultDate: date,
        format: 'MM/DD/YYYY HH:mm'
        

    });
    $('#datetimepicker1').data("DateTimePicker").minDate(date);

});

function OpenCognosPortal1(envId, envName, page) {
    try {
        openModal();
        ClearAllServices();
        $.ajax({
            type: "POST",
            url: "../forms/Generic.aspx/LogCognosPortalResponse",
            data: "{'envId':'" + envId + "', 'urlId':'0'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessPortalDetail,
            failure: function (response) {
                closeModal();
                alert(response.d);
            }
        });
    } catch (e) {
        closeModal();
    }
}

var OnSuccessPortalDetail = function (response) {
    console.log(response.d);
}