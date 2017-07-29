var ctrlFocus    = '';
var Errorserver = false;
var isDomChange = false;

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $('#txtEnvironment').focus();
});

$(document).ready(function () {
    BindControls();

    $("#btnCreate").click(function () {
        ctrlFocus = "";
        var errors = false;
        $('.errors').remove();

        var environment = validateContent('txtEnvironment');
        if (environment) {errors = true;}

         var address = validateContent('txtAddress');
        if (address) errors = true;

        var display = validateContent('txtDisplayName');
        if (display) errors = true;

        var match = validateContent('txtMatchContent');
        if (match) errors = true;

        var interval = validateContent('txtPollingInterval');
        if (interval) errors = true;

        var user = validateContent('txtUserName');
        if (user) errors = true;

        var password = validateContent('txtPassword');
        if (password) errors = true;


        if (errors === true) {
            setCtrlFocus(ctrlFocus);
            return false;
        }
        else {
            openModal();
            isDomChange = false;
            return true;
        }
    });

    $("#btnTestUrl").click(function () {
        var errors = false;
        ctrlFocus = "";

        var environment = validateContent('txtAddress');
        if (environment) { errors = true; }

        var match = validateContent('txtMatchContent');
        if (match) errors = true;

        var user = validateContent('txtUserName');
        if (user) errors = true;

        var password = validateContent('txtPassword');
        if (password) errors = true;

        var url = $("#").val();

        if (errors === true) {
            setCtrlFocus(ctrlFocus);
            return false;
        }
        else {
            openModal();
            var url = $("#txtAddress").val();
            var match = $("#txtMatchContent").val();
            var user = $("#txtUserName").val();
            var password = $("#txtPassword").val();
            var result = GetPingResult(url, match, user, password);

            closeModal();
            return true;
        }

    });
});

var validateContent = function(controlName) {
    var name = "#" + controlName;
    if ($(name).val() === "") {
        $(name).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
        //setCtrlFocus(controlName);
        if (ctrlFocus === "") ctrlFocus = controlName;
        return true;
    } else {
        $(name).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
    }
    return false;
};

function setCtrlFocus(name) {
    if (!name == '') {
        $('#' + name).focus();
    }
}

function BindControls() {
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
        } else
            return false;

    } else {
        InvokeCancelRequest();
    }

    return true;
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

function GetPingResult(url, match, user, password) {
    $.ajax({
        type: "POST",
        url: "../forms/Generic.aspx/LogWebsiteWithCredential",
        data: "{'url':'" + url + "', 'match':'" + match + "', 'username':'" + user + "', 'password':'" + password + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var result = response.d;
            if (result.Status.toLowerCase() === "failure" || result.Status === "") {
                $("#testMessage").html("Status: <span class='glyphicon glyphicon-remove' style='color:red'></span><br/>Error: " + result.Message); //" + result.Status +
            } else {
                $("#testMessage").html("Status: <span class='glyphicon glyphicon-ok' style='color:green'></span><br/>Response Time: " + result.ResponseTime);
            }

            $('#modalTestUrl').modal({
                keyboard: false
            });

            closeModal();
        },
        failure: function (response) {
            closeModal();
            alert(response.d);
        }
    });
}