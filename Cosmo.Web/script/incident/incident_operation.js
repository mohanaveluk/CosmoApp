var tableStatus = '';
var isExpand = false;
var sThisVal;
var checkedValue;
var sIssueDesc = '';
var sIssueResolution = '';
var issueCntl = '', resolutionCntl;
var selectedIncidents = new Array();


function getCurrentValues(parentCntl) {
    //Get value of issue desc from any incident if available
    sIssueDesc = sIssueResolution = '';
    $('#' + parentCntl).find("input[id^=txtIssue]").each(function () {
        if (this.value != '') {
            issueCntl = this.id;
            sIssueDesc = this.value;
            return false;
        }
    });
    //Get value of resolution desc from any incident if available
    $('#' + parentCntl).find("input[id^=txSolution]").each(function () {
        if (this.value != '') {
            resolutionCntl = this.id;
            sIssueResolution = this.value;
            return false;
        }
    });
}

function PopulateAll(thisCntl, parentCntl) {
    //if ($('#' + thisCntl.id).is(':checked')) {
    if ($('#' + thisCntl).is(':checked')) {
        getCurrentValues(parentCntl);
        if (sIssueDesc != '' && sIssueResolution != '') {
            $('#' + parentCntl).find("input[type=checkbox]").each(function () {
                if (this.id != 'chkIncidentAll') {
                    sThisVal = (this.checked ? $(this).val() : "");
                    checkedValue = this.value;
                    $('#txtIssue_' + checkedValue).val(sIssueDesc);
                    $('#txSolution_' + checkedValue).val(sIssueResolution);
                    this.checked = true;
                }
            });
        }
        else {
            alert('Please fill atleast one issue and resolution detail to populate all incidents.');
            $('#' + thisCntl).prop('checked', false);
            return false;
        }
    }
    else {
        $('#' + parentCntl).find("input[type=checkbox]").each(function () {
            if (this.id != 'chkIncidentAll') {
                sThisVal = (this.checked ? $(this).val() : "");
                checkedValue = this.value;
                this.checked = false;
                $('#txtIssue_' + checkedValue).val('');
                $('#txSolution_' + checkedValue).val('');
            }
        });
        if (sIssueDesc != '' && sIssueResolution != '') {
            $('#' + issueCntl).val(sIssueDesc);
            $('#' + resolutionCntl).val(sIssueResolution);
            issueCntl = resolutionCntl = '';

        }
    }
}

function setIncidetDetails(checkValue, parentCntl) {
    if ($('#chkReplicate_' + checkValue).is(':checked')) {
        getCurrentValues(parentCntl);
        if (sIssueDesc == '' || sIssueResolution == '') {
            alert('Please enter issue and resolution details to replicate for other incident');

            $('#chkReplicate_' + checkValue).prop('checked', false);
            return false;

            /*sIssueDesc = $('#txtIssue_' + checkValue).val();
            sIssueResolution = $('#txSolution_' + checkValue).val();
            if (sIssueDesc == '' || sIssueResolution == '') {
            alert('Please enter issue and resolution details to replicate for other incident');
            $('#chkReplicate_' + checkValue).checked = false;
            return false;
            }*/
        }
        else {
            $('#txtIssue_' + checkValue).val(sIssueDesc);
            $('#txSolution_' + checkValue).val(sIssueResolution);
        }
    }
    else {
        $('#txtIssue_' + checkValue).val('');
        $('#txSolution_' + checkValue).val('');

    }
}
function DeleteConfig(cfgID, name) {
    if (confirm("Are you sure, you want to delete configuration detail " + name + "?")) {
        openModal();
        $.ajax({
            type: "POST",
            url: "Generic.aspx/DeleteEnvConfig",
            data: "{'type':'cfg', 'configID':'" + cfgID + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess,
            failure: function (response) {
                alert(response.d);
            }
        });
    }
}

function OnSuccess(response) {
    if (response.d != null) {
        console.log(response);
        alert(response.d);
    } else {
        sessionStorage.setItem("RecUpdated", "success");
        UpdategetEnvironmentList();
        //location.reload();
        console.log("success: " + success);
    }
}
function toggle_it(itemID) {
    // Toggle visibility between none and ''
    if (document.getElementById("row_" + itemID) != null || document.getElementById("row_" + itemID) == undefined) {
        if ((document.getElementById("row_" + itemID).style.display == 'none')) {
            document.getElementById("row_" + itemID).style.display = '';
            $("#arrowImg_" + itemID).removeClass("arrow").addClass("arrowup");
            tableStatus = "open";
        }
        else {
            document.getElementById("row_" + itemID).style.display = 'none';
            $("#arrowImg_" + itemID).removeClass("arrowup").addClass("arrow");
            tableStatus = "close";
        }
    }
}
function toggle_all(itemID, mode) {
    // Toggle visibility between none and ''
    if (document.getElementById("row_" + itemID) != null || document.getElementById("row_" + itemID) == undefined) {
        if (mode) {
            document.getElementById("row_" + itemID).style.display = '';
            $("#arrowImg_" + itemID).removeClass("arrow").addClass("arrowup");
            tableStatus = "open";
        }
        else {
            document.getElementById("row_" + itemID).style.display = 'none';
            $("#arrowImg_" + itemID).removeClass("arrowup").addClass("arrow");
            tableStatus = "close";
        }
    }
}

function setExpandCollapse() {
    //alert(environments.length);
    if (environments != 'undefined') {
        for (var iEn = 0; iEn < environments.length; iEn++) {
            toggle_all(environments[iEn], isExpand);
        }
        isExpand = !isExpand;
        //if ($("#linkExpand").html().indexOf("Expand") >= 0) {
        if (tableStatus == "open") {
            $("#linkExpand").html("<img src='../images/collapseall.jpg' /> Collapse All");
        }
        else {
            $("#linkExpand").html("<img src='../images/expandall.jpg' /> Expand All");
        }
    }
}

function ScheduleService(name, status, serviceScheduled, envID) {
    var statusText = '';
    if (status == "0") {
        statusText = "No service available";
        alert("No service available to schedule for " + name + ".");
        return false;
    }
    else {
        if (status == "1") {
            statusText = "All service(s) were scheduled already in " + name + "." + "\n";
            statusText += "By Editing scheduler will replace the current schedule of all services." + "\n\n";
            statusText += "Do you want to continue?";
        }
        else if (status == "2") {
            statusText = "No service(s) are scheduled yet in " + name + "." + "\n";
            statusText += "By adding scheduler will impact all the avaialble services. " + " \n\n";
            statusText += "Do you want to continue?";
        }
        else if (status == "3") {
            if (parseInt(serviceScheduled) == 1)
                statusText = "1 service has been scheduled already in " + "." + name + "\n";
            else if (parseInt(serviceScheduled) >= 1) {
                statusText = serviceScheduled + " service(s) were already been scheduled in " + name + "." + "\n";
                statusText += "By Editing scheduler will replace all the current schedule of other services too." + "\n\n";
                statusText += "Do you want to continue?";
            }
        }
        if (confirm(statusText)) {
            openMyModal("Schedule.aspx?e=" + envID + "&c=0&t=ed");
        }
        else
            return false;

    }
}

function SubmitIncident(monID, envID, configID) {
    var confirmation;
    var issudID = "txtIssue_" + monID;
    var solutionID = "txSolution_" + monID;
    var Errors = false;

    var incidentID = '';

    var issueDesc = $('#' + issudID).val();
    var solutionDesc = $('#' + solutionID).val();
    selectedIncidents = new Array();

    $('.errors').remove();
    if (issueDesc.trim() === '') {
        //$('#' + issudID).after("<span class='errorserver'>&nbsp;<img src='../images/tick16.jpg' title='Emai is valid'></span>");
        $('#' + issudID).closest('.form-group').removeClass().addClass('form-group has-error');
        $('#' + issudID).attr("title", "Please enter the details about this incident");
        $('#' + issudID).focus();
        Errors = true;
    }
    if (solutionDesc.trim() === '') {
        //$('#' + solutionID).after("<span class='errorserver'>&nbsp;<img src='../images/tick16.jpg' title='Emai is valid'></span>");
        $('#' + solutionID).closest('.form-group').removeClass().addClass('form-group has-error');
        $('#' + solutionID).attr("title", "Please enter the solution");
        if (Errors == false) $('#' + solutionID).focus();
        Errors = true;
    }
    if (Errors === true)
        return false;
    else {
        $('input[type=checkbox]').each(function () {
            if (this.id.indexOf('chkIncidentAll') < 0) {
                if ($(this).is(':checked')) {
                    checkedValue = this.value;
                    selectedIncidents[selectedIncidents.length] = this.value;
                    if ($('#txtIssue_' + checkedValue).val() === '' && $('#txSolution_' + checkedValue).val() === '') {
                        $('#txtIssue_' + checkedValue).closest('.form-group').removeClass().addClass('form-group has-error');
                        $('#txtIssue_' + checkedValue).attr("title", "Please enter the solution");

                        $('#txSolution_' + checkedValue).closest('.form-group').removeClass().addClass('form-group has-error');
                        $('#txSolution_' + checkedValue).attr("title", "Please enter the solution");
                        Errors = true;
                    }
                }
            }
        });
        console.log("Error: " + Errors);

        if (Errors === true)
            return false;

        console.log("selectedIncidents.length: " + selectedIncidents.length);

        if (selectedIncidents.length === 0) {
            confirmation = "Are you sure, you want to submit this solution?";
            if (confirm(confirmation)) {
                incidentID = monID;
            }
        }
        else {
            confirmation = "Are you sure, you want to submit all the selected resolutions?";
            if (confirm(confirmation)) {
                incidentID = issueDesc = solutionDesc = '';
                for (var iItem = 0; iItem < selectedIncidents.length; iItem++) {
                    if ($('#txtIssue_' + selectedIncidents[iItem]).val() != '' && $('#txSolution_' + selectedIncidents[iItem]).val() !== '') {
                        incidentID += selectedIncidents[iItem] + '^';
                        issueDesc += $('#txtIssue_' + selectedIncidents[iItem]).val() + '^';
                        solutionDesc += $('#txSolution_' + selectedIncidents[iItem]).val() + '^';
                    }
                }

                if (issueDesc !== '' && solutionDesc !== '') {
                }
            }
        }

        try {
            //var parameter = "type=incident&monitorId=" + incidentID + "&environmentId=" + envID + "&serviceId=" + configID + "&serviceIssue=" + issueDesc + "&serviceSolution=" + solutionDesc;
            //var parameter = '{"type":"incident", "monitorId":"' + incidentID + '", "environmentId":"' + envID + '", "serviceId":"' + configID + '", "serviceIssue":"' + issueDesc + '", "serviceSolution":"' + solutionDesc + '"}';
            var parameter = '{monitorId:"' + incidentID + '", environmentId:"' + envID + '", serviceId:"' + configID + '", serviceIssue:"' + issueDesc + '", serviceSolution:"' + solutionDesc + '"}';
            //var parameter = '{type:"incident", monitorId:"' + incidentID + '"}';

            console.log(parameter);

            $.ajax({
                type: "POST",
                url: "Generic.aspx/IncidentTracking",
                data: parameter,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function(response) {
                    closeModal();
                    console.log(response);
                    alert(response.d);
                }
            });
        } catch (e) {
            alert("Error: " + e);
        }

        return true;
    }
}

function UpdategetEnvironmentList() {
    console.log('calling UpdategetEnvironmentList');
    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function () {
        scope.getCurrentIncidentList();
    });
}