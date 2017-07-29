var iRowCount = 1;
var isDomChange = false;
$(document).ready(function() {
    $('#btnAddEmail').click(function() {
        var className = '';
        var emailExists = false;
        var emailId = $('#txtEmail').val();
        var table = $("#TableEmail tbody");
        if (isValidEmailAddress(emailId)) {
            if ($('#btnAddEmail').val() === "Add") {
                table.find("tr").each(function(key, value) {
                    var $tds = $(this).find('td'), emaiCellID = $tds.eq(6).text(), emailCellAddress = $tds.eq(1).text();
                    //$('#hidUserEmailID').val(
                    if (emailCellAddress == emailId) {
                        alert('Oops. email address :' + emailId + ' is already exists. Try chhose another email address.');
                        emailExists = true;
                        $('#txtEmail').focus().select();
                        return false;
                    }
                });
            } else if ($('#btnAddEmail').val() == "Update") {
                table.find("tr").each(function(key, value) {
                    var $tds = $(this).find('td'), emaiCellID = $tds.eq(6).text(), emailCellAddress = $tds.eq(1).text();
                    //$('#hidUserEmailID').val(
                    if (emailCellAddress == emailId && $('#hidUserEmailID').val() !== emaiCellID) {
                        alert('Oops. email address :' + emailId + ' is already exists. Try chhose another email address.');
                        emailExists = true;
                        $('#txtEmail').focus().select();
                        return false;
                    }
                });
            }
            if (emailExists) return false;

            isDomChange = false;
            window.parent.$("#hidSchedulerChanged").val("updated");
            openModal();
            return true;
        } else {
            $('#txtEmail').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
            $('#txtEmail').focus();
            return false;
        }

    });

    $('input').blur(function () {
        validateEmail();
    });

    if (!isDomChange) {
        $("input").on("propertychange change paste input", function() {
            isDomChange = true;
        });
        $("textarea").on("propertychange change paste input", function() {
            isDomChange = true;
        });
    }


    $("input:radio[name=rdoMessageType]").click(function() {
        if ($(this).val().toLowerCase() === "rdoemail") {
            $('#drpEmailType').prop("disabled", false);
        } else {
            $('#drpEmailType').val('To');
            $('#drpEmailType').prop("disabled", true);
        }
    });

});

function validateEmail() {
    if ($('#txtEmail').val() !== '') {
        if (isValidEmailAddress($('#txtEmail').val())) {
            $('#txtEmail').closest('.form-group').removeClass().addClass('form-group');
        } else {
            $('#txtEmail').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
        }
    }
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};

function AlterUserEmail(emailUserId, emailAddress, type, mode) {
    $('#hidUserEmailID').val(emailUserId);
    $('#txtEmail').val(emailAddress);
    $('#drpEmailType').val(type);
    $('#btnAddEmail').val('Update');

    if (mode === "E" || mode.toLowerCase() === "email") {
        $('input:radio[name=rdoMessageType][value=rdoEmail]').prop('checked', true);
        $('#drpEmailType').prop("disabled", false);
    } else if (mode === "T" || mode.toLowerCase() === "text") {
        $('input:radio[name=rdoMessageType][value=rdoText]').prop('checked', true);
        $('#drpEmailType').val('To');
        $('#drpEmailType').prop("disabled", true);
    }
}

function EnableEmailType(mode) {
    if (mode === "email")
        $('#drpEmailType').prop("disabled", false);
    else {
        $('#drpEmailType').val('To');
        $('#drpEmailType').prop("disabled", true);
    }
}

function clearEditDetails() {
    $('#hidUserEmailID').val('');
    $('#txtEmail').val('');
    $('#drpEmailType').val('To');
    $('#btnAddEmail').val('Add');
    $('#drpEmailType').prop("disabled", false);
    isDomChange = false;
    return false;
}

function DeleteEmailConfig(cfgID, name) {
    if (confirm("Are you sure, you want to delete email " + name + "?")) {
        // Save data to the current session's store
        openModal();
        if (cfgID == 0) {
            $.ajax({
                type: "POST",
                url: "../forms/Generic.aspx/DeleteEnvEmail",
                data: "{'type':'email', 'emailAddress':'" + name + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    alert(response.d);
                    sessionStorage.setItem("hidIsDataUpdated", "updated");
                    closeModal();
                },
                failure: function (response) {
                    closeModal();
                    alert(response.d);
                }
            });
        } else {
            var url;
            if ($('#hidCreateMode').val() == "new") {
                cfgID = name;
                url = "Generic.aspx/DeleteeMailFromSession";
                sessionStorage.setItem("hidIsDataUpdated", "updated");
            } else {
                url = "Generic.aspx/DeleteEnvConfig";
                sessionStorage.setItem("hidIsDataUpdated", "");
            }

            $.ajax({
                type: "POST",
                url: url,
                data: "{'type':'email', 'configID':'" + cfgID + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    OnSuccess(response, name);
                    closeModal();
                },
                failure: function (response) {
                    closeModal();
                    alert(response.d);
                }
            });
        }

    } else {
        closeModal();
        return false;
    }
}

function OnSuccess(response, deletedEmail) {
    if (response.d != null) {
        alert(response.d);
    } else {
        $("hidIsDataUpdated").val("updated");
        var table = $("#TableEmail tbody");
        if (table.length <= 0)
            $("hidIsDataUpdated").val('noemail');
        else if (table.length > 0) {
            table.find("tr").each(function(key, value) {
                var $tds = $(this).find('td'), emailAddress = $tds.eq(1).text();
                if (emailAddress == deletedEmail) {
                    $(this).fadeOut(400, function() {
                        $(this).remove();
                        if (window.parent.location.pathname.toLowerCase().indexOf('servicedetails') <= -1) {
                            window.parent.UpdateEnvironmentEmailList();
                        }
                    });
                    return false;
                }
            });
        }
    }

}

function EmailUpdated(val) {
    window.parent.$("#hidSchedulerChanged").val(val);
    if (window.parent.location.pathname.toLowerCase().indexOf('servicedetails') <= -1) {
        window.parent.UpdateEnvironmentEmailList();
    }
}

function EmailIdExistsStatus(val) {
    sessionStorage.setItem("hidIsDataUpdated", val);
}

function fnCancelEmail(val) {
    if (isDomChange) {
        if (!confirm('Discard the changes?')) {
            return false;
        }
    }
    if (this.parent.modalWindow != null) {
        var tRows = 0;
        if ($("#TableEmail tbody").length > 0) {
            tRows = $("#TableEmail tbody")[0].rows.length;
        }
        if ((document.getElementById('hidIsDataUpdated').value == "updated" || sessionStorage.getItem("hidIsDataUpdated") == "updated") && tRows > 0) {
            //this.parent.location.reload(false);
            window.parent.$("#btnEditSchedule").attr('disabled', false);
            window.parent.$("#btnEditSchedule").removeClass().addClass('btn btn-primary');
            window.parent.$("#btnEditSchedule").title = "";
        }
        else if (sessionStorage.getItem("hidIsDataUpdated") == "noemail" || tRows <= 0) {
            window.parent.$("#btnEditSchedule").attr('disabled', true);
            window.parent.$("#btnEditSchedule").removeClass().addClass('btn btn-default');
            window.parent.$("#btnEditSchedule").title = "Add mail address for notofication and start schedule";
        }
        else if ($("#hidIsDataUpdated").val() == "noemail" || tRows <= 0) {
            window.parent.$("#btnEditSchedule").attr('disabled', true);
            window.parent.$("#btnEditSchedule").removeClass().addClass('btn btn-default');
            window.parent.$("#btnEditSchedule").title = "Add mail address for notofication and start schedule";
        }
        if (window.parent.location.href.indexOf('ConfigEmails.aspx') > 0) {
            //this.parent.location.href = this.parent.location.href;
            //if (document.getElementById("hidIsDataUpdated").value == "updated" || sessionStorage.getItem("hidIsDataUpdated") == "updated")
            this.parent.location.reload();
        }
        this.parent.modalWindow.close();
    }
    else {
        this.close();
        return false;
    }
    try {


        var isOpen = window.parent.$("#emailDialog").dialog("isOpen");
        if (isOpen) {
            //$("#dialog").dialog("destroy");
            //window.parent.closeIframe();
            //window.parent.$("#hidSchedulerChanged").val("value assigned");
            //alert(window.parent.$("#hidSchedulerChanged").val());
            window.parent.$('#emailDialog').dialog('close');
        }

    }
    catch (ex) { }
}

function UpdateData(Val) {
    //		        if (this.parent.modalWindow != null) {
    //		            this.parent.modalWindow.save();
    //		        }
    this.parent.location.reload();
    //this.parent.location.href = this.parent.location.href;
}