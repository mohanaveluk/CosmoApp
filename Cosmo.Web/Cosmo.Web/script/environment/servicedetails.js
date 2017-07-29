var ctrlFocus = '';
var Errorserver = false;
var isDomChange = false;

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $('#txtEnvironment').focus();
});

$(document).ready(function () {
    BindControls();
    editEmailNotify();

    $('#modScheduler').on('show.bs.modal', function (event) {

    });

    if (!$('#txtHostIP').prop('disabled')) {

        $('#txtHostIP').blur(function () {
            if (!$('#txtHostIP').val() == '') {
                $('#txtHostIP').next().remove();
                //$('#txtHostIP').after("<span class='errorserver'>&nbsp;<img src='../images/progress.gif' title='Checking...'></span>");
                $('#imgHostStatus').attr('src', "../images/progress.gif").attr('title','Checking...');
                var cmUrl = getServiceURL();
                $('#txtURL').val(cmUrl); $('#hidServiceURL').val(cmUrl);
                PingServer();
            }
            else {
                //$('#txtHostIP').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                //$('#txtHostIP').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Not a valid Host name or IP Address'></span>");
                $('#imgHostStatus').attr('src', "").removeAttr('title');
                $('#txtURL').val(''); $('#hidServiceURL').val('');
            }
        });

    }

    //code Added on 12312014
    if (!$('#txtPort').prop('disabled')) {

        $('#txtPort').blur(function () {
            if (!$('#txtPort').val() == '') {
                var cmUrl = getServiceURL(); // "http://" + $('#txtHostIP').val() + ":" + $('#txtPort').val() + "/p2pd/servlet";
                $('#txtURL').val(cmUrl); $('#hidServiceURL').val(cmUrl);
            }
            else {
                $('#txtURL').val(''); $('#hidServiceURL').val('');
            }
        });
    }
    if (!$('#drpServiceType').prop('disabled')) {
        $("#drpServiceType").change(function() {
            cmUrl = getServiceURL(); //"http://" + $('#txtHostIP').val() + ":" + $('#txtPort').val() + "/p2pd/servlet";
            $('#txtURL').val(cmUrl);
            $('#hidServiceURL').val(cmUrl);

        });
    }
    //end on 12312014
    //$(document).tooltip({ position: { my: "center bottom-20", at: "center top", using: function (position, feedback) { $(this).css(position); $("<div>").addClass("arrow").addClass(feedback.vertical).addClass(feedback.horizontal).appendTo(this); } } });
    $("#btnCreate").click(function () {
        ctrlFocus = '';
        var Errors = false;
        $('.errors').remove();
        if ($('#txtEnvironment').val() == '') {
            $('#txtEnvironment').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
            setCtrlFocus('txtEnvironment');
            Errors = true;
        } else {
            $('#txtEnvironment').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
        }

        if (!$('#txtLocation').prop('disabled')) {
            if ($('#txtLocation').val() == '') {
                $('#txtLocation').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                setCtrlFocus('txtLocation');
                Errors = true;
            } else {
                $('#txtLocation').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
            }
        }

        if (!$('#txtHostIP').prop('disabled')) {
            if ($('#txtHostIP').val() == '') {
                $('#txtHostIP').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                setCtrlFocus('txtHostIP');
                Errors = true;
            }
            else {
                $('#txtHostIP').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                if (Errorserver) {
                    setCtrlFocus('txtHostIP');
                    //Errors = true;
                }
            }

        }

        if (!$('#txtPort').prop('disabled')) {
            if ($('#txtPort').val() == '') {
                $('#txtPort').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                setCtrlFocus('txtPort');
                Errors = true;
            } else {
                $('#txtPort').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
            }
        }
        if (!$('#drpServiceType').prop('disabled')) {
            if ($('#drpServiceType option:selected').val() == '') {
                $('#drpServiceType').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                setCtrlFocus('drpServiceType');
                Errors = true;
            } else {
                $('#drpServiceType').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
            }
        }

        if (!$('#txtURL').prop('disabled')) {
            if ($('#txtURL').val() == '') {
                Errors = true;
            }
        }
        if ($('#drpMailFrequency option:selected').val() == '') {
            $('#drpMailFrequency').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
            setCtrlFocus('drpMailFrequency');
            Errors = true;
        }
        else
            $('#drpMailFrequency').closest('.form-group').removeClass('form-group has-error').addClass('form-group');

        $("input").keyup(function () {
            Validate($(this));
        });

        $("select, input").change(function () {
            Validate($(this));
        });

        $("select, input").blur(function () {
            Validate($(this));
        });

        if (Errors == true)
            return false;
        else {
            openModal();
            isDomChange = false;
            return true;
        }
    });

    $('#txtEnvironment').on('input blur change', function () {
        $('#hidEnvIDEmail').val('');
        CanSchedule($('#txtEnvironment').val(), 'name');
    });

    if (!isDomChange) {
        $("input").on("propertychange change paste input", function () {
            isDomChange = true;
        });
        $("textarea").on("propertychange change paste input", function () {
            isDomChange = true;
        });

    }

    $("#txtPort").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
                        (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
                        (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $("#btnEditSchedule").click(function () {
        if ($("#btnEditSchedule").attr('class').indexOf('btn-primary') >= 0) {
            EditSchedule();
        }
    });

    $("#btnEmailNotify").click(function () { $("#emailDialog").dialog("open"); });

    if ($('#hidIsCancelRequest').val() === 'true') {
        sessionStorage.setItem("hidIsDataUpdated", "");
        InvokeCancelRequest();
    }
});

function Validate(elem) {
    if (elem.context.name == 'txtEnvironment' || elem.context.name === 'txtHost' || elem.context.name === 'txtPort' || elem.context.name === 'txtLocation') {
        if ($(elem).val() == '')
            $(elem).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
        else
            $(elem).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
    }
}

function setCtrlFocus(name) {
    if (!name == '') {
        $('#' + name).focus();
    }
}

function BindControls() {


//    $('#txtEnvironment').autocomplete({
//        source: Environments,
//        minLength: 0,
//        scroll: true
//    }).focus(function () {
//        $(this).autocomplete("search", "");
//    });

    $('#txtEnvironment').typeahead({
        name: 'Environments',
        local: availableEnvironments, //['alpha', 'allpha2', 'alpha3', 'bravo', 'charlie', 'delta', 'epsilon', 'gamma', 'zulu']
        limit: 10
    });

    $('.tt-query').css('background-color', '#fff'); 



}


function fnCancel(val) {
    if (isDomChange) {
        if (confirm("You have made some changes in the Service configuration.\n Do you want to discard the changes?")) {
            InvokeCancelRequest();
        }
        else
            return false;

    }
    else {
        InvokeCancelRequest();
    }


}

function InvokeCancelRequest() {
    sessionStorage.setItem("hidIsDataUpdated", "");
    console.log($('#hidIsDataUpdated').val());
    if (this.parent.modalWindow != null) {
        if ($('#hidIsDataUpdated').val() === "updated") {
            //this.parent.location.reload(false);
            //this.parent.location.href = this.parent.location.href;
            //this.parent.UpdategetEnvironmentList();
        }
        this.parent.modalWindow.close();
    }
    else {
        this.close();
        return false;
    }
}

function UpdateData(Val) {
    //		        if (this.parent.modalWindow != null) {
    //		            this.parent.modalWindow.save();
    //		        }
    //this.parent.location.reload();
    this.parent.location.href = this.parent.location.href;
}

function PingServer() {
    $.ajax({
        type: "POST",
        url: "Generic.aspx/GetPingServer",
        data: "{'host':'" + $('#txtHostIP').val() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Onsuccess,
        failure: function (response) {
            alert("Failure :" + response.d);
        }
    });
}
function Onsuccess(response) {
    //alert("Success: " + response.d);
    $('#txtHostIP').closest('.form-group').removeClass('form-group').addClass('form-group has-feedback');
    if (response.d.indexOf("Success") >= 0) {
        //$('#txtHostIP').after("<span class='errorserver'>&nbsp;<img src='../images/tick16.jpg' title='" + response.d + "'></span>");
        //$('#txtHostIP').closest('.form-group').append("<span class='glyphicon glyphicon-ok form-control-feedback'' aria-hidden='true'></span>");
        $('#imgHostStatus').attr('src', "../images/15-Tick-icon.png").attr('title', response.d);
        Errorserver = false;
    }
    else {
        //$('#txtHostIP').after("<span class='errorserver'>&nbsp;<img src='../images/fail16.png' title='" + response.d + " or ping might be disabled in your networl'></span>");
        //$('#txtHostIP').closest('.form-group').append("<span class='glyphicon glyphicon-remove form-control-feedback'' aria-hidden='true'></span>");
        //$('#txtHostIP').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
        $('#imgHostStatus').attr('src', "../images/false_icon.png").attr('title', response.d + ' or ping might be disabled in your network');
        //setCtrlFocus('txtHostIP');
        Errorserver = true;
    }
}

var openServiceSchedulerModal = function (source) {
    alert("OpenDialog");

};

function closeIframe() {
    $('#dialog').dialog('close');
    return false;
}

function getServiceURL() {
    var cmUrl = '';
    if (!$('#txtPort').val() == '') {
        var sPort = '';
        if (!$('#txtPort').val() == '')
            sPort = ":" + $('#txtPort').val();

        if ($('#drpServiceType option:selected').val() == '1') {
            cmUrl = "http://" + $('#txtHostIP').val() + sPort + "/p2pd/servlet";
        }
        else if ($('#drpServiceType option:selected').val() == '2') {
            cmUrl = "http://" + $('#txtHostIP').val() + sPort + "/p2pd/servlet/gc";
        }
    }
    return cmUrl;
}

var GetEnvEmail = function (environmentId, type) {
    //alert(GroupArr.length);
    //debugger;
    if (sessionStorage.getItem("hidIsDataUpdated") == "updated") return false;
    for (var i = 0; i < EnvironmentEmails.length; i++) {
        if (type == 'id') {
            if (EnvironmentEmails[i][0] == environmentId) {
                return EnvironmentEmails[i][2];
            }
        }
        else if (type == 'name') {
            if (EnvironmentEmails[i][1] == environmentId) {
                if ($('#hidEnvIDEmail').val() == '')
                    $('#hidEnvIDEmail').val(EnvironmentEmails[i][0]);
                return EnvironmentEmails[i][2];
            }
        }
    }
    return '';
}

function CanSchedule(environmentId, type) {
    var envEmail = GetEnvEmail(environmentId, type);
    if (envEmail.length <= 0 || !envEmail) {
        EnableSchedule(false);
    }
    else {
        EnableSchedule(true);
    }
}
function EnableSchedule(mode) {
    $("#btnEditSchedule").attr('disabled', !mode);
    if (mode) {
        $("#btnEditSchedule").removeClass('btn btn-default').addClass('btn btn-primary');
        $("#btnEditSchedule").removeAttr('title');
    }
    else {
        $("#btnEditSchedule").removeClass('btn btn-primary').addClass('btn btn-default');
        $("#btnEditSchedule").attr('title', 'Add email notify to start schedule-s');
    }
}
function clearSessionStore() {
    sessionStorage.setItem("hidIsDataUpdated", "");
}

var winHeight = 600;
function setWindowHeightAlert(isIncrease) {
    this.parent.UpdategetEnvironmentList();
    if (isIncrease) {
        if (window.innerHeight <= winHeight) {
            window.parent.$('#iframeEditConfiguration')[0].height = parseInt(winHeight + 45);
            return;
        }
        else if (window.innerHeight <= parseInt(winHeight + 45)) {
            return;
        }

    }
    window.parent.$('#iframeEditConfiguration')[0].height = winHeight;

}

function EditSchedule() {
    var scheduleString = '';
    var jEnvId = $('#hidEnvIDEmail').val();
    var jConfigId = $('#hidEnvID').val();
    var jServiceType = $('#hidServiceType').val();
    if (jEnvId === '' && jConfigId === '') {
        scheduleString = '../forms/Schedule.aspx?ename=' + $('#txtEnvironment').val() + '&sp=' + $('#txtHostIP').val() + ':' + $('#txtPort').val() + '&ed=new';
    }
    else if (jServiceType === "en") {
        scheduleString = '../forms/Schedule.aspx?e=' + jEnvId + '&c=0&t=ed&ed=new';
    }
    else if (jServiceType === "ed" && jEnvId !== '' && jConfigId !== '') {
        scheduleString = '../forms/Schedule.aspx?e=' + jEnvId + '&c=' + jConfigId + '&t=ed&ed=new';
    }
    else {
        scheduleString = '../forms/Schedule.aspx?ename=' + $('#txtEnvironment').val() + '&sp=' + $('#txtHostIP').val() + ':' + $('#txtPort').val() + '&ed=new';
    }

    //openSchedulerModal(scheduleString);

    $("#dialog").dialog({
        autoOpen: false,
        modal: true,
        width: '900',
        height: '560',
        resizable: false,
        closeOnEscape: false,
        dialogClass: "no-close",
        draggable: false,
        open: function (ev, ui) {
            $('#myIframe').attr('src', scheduleString);
            $('#myIframe').attr("style", "display:block");
        },
        beforeClose: function (event, ui) {
            if ($("#hidSchedulerChanged").val() === "true") {
                if (confirm("Confirm to discard the Changes for scheduler")) {
                    $('#myIframe').attr("style", "display:none");
                    return true;
                } else {
                    return false;
                }
            }
            else if ($("#hidSchedulerChanged").val() == 'dataPassed') {
                isDomChange = true;
            }

        }
    });
    $(".ui-dialog-titlebar").hide();
    $("#dialog").dialog("open");
}

function editEmailNotify() {
    $("#emailDialog").dialog({
        autoOpen: false,
        modal: true,
        width: '800',
        height: '600',
        draggable: false,
        resizable: false,
        closeOnEscape: false,
        dialogClass: "no-close",
        open: function (ev, ui) {
            sessionStorage.setItem("hidIsDataUpdated", "");
            $('#emailFrame').attr('src', '../forms/EditEmailConfig.aspx?s=' + $('#hidEnvIDEmail').val() + '&ename=' + $('#txtEnvironment').val() + '&&ed=new');
            $('#emailFrame').attr("style", "display:block");
        },
        beforeClose: function (event, ui) {
            //if()
            //alert($(event.target)[0].id + ' ' + $("#hidSchedulerChanged").val());
            if ($("#hidSchedulerChanged").val() === "true") {
                if (confirm("Confirm to discard the Changes for email notify")) {
                    $('#myIframe').attr("style", "display:none");
                    return true;
                } else
                    return false;
            }
            else if ($("#hidSchedulerChanged").val() === 'dataPassed') {
                isDomChange = true;
            }

        }
    });
    $(".ui-dialog-titlebar").hide();
}