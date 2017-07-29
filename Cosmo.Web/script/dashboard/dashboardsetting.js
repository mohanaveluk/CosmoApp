
var winHeight = 450;
function SetSettingWindowHeight(isIncrease) {
    var windowName = 'iframeDashboardSetting';
    var saveStatus = $('#hidIsDataUpdated').val();

    if (isIncrease) {
        this.parent.UpdateMonitorList();
        if (saveStatus !== "") {
            window.parent.$('#' + windowName)[0].height = parseInt(winHeight + 45);
            return;
        }
    }
    window.parent.$('#' + windowName)[0].height = winHeight;
}

function ValidateSelection(ctrl) {
    var listSelected = $('#lstEnvironment option:selected').val();
    var length = $('#lstEnvironment > option').length;
    if (listSelected != undefined) {
        $('#lstEnvironment').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');

        if (ctrl.id === "btnUp") {
            if ($('#lstEnvironment option:selected').index() === 0) return false;
            window.__doPostBack("btnUp", "true");
        } else {
            if ($('#lstEnvironment option:selected').index() + 1 === length) return false;
            window.__doPostBack("btnDown", "true");
        }

    }
    $('#lstEnvironment').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
    return false;
}

function fnGetValue(val) {
    if (this.parent.modalWindow != null) {
        //if (document.getElementById("hidIsDataUpdated").value === "updated")
            //this.parent.location.reload(false);
        this.parent.modalWindow.close();
    } else {
        this.close();
        return false;
    }
    return true;
}
