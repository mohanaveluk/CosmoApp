var ScheduleService = function(name, status, serviceScheduled, envId) {
    var statusText = '';
    if (status === "0") {
        statusText = "No service available";
        alert("No service available to schedule for " + name + ".");
        return false;
    } else {
        if (status === "1") {
            statusText = "All service(s) were scheduled already in " + name + "." + "\n";
            statusText += "By Editing scheduler will replace the current schedule of all services." + "\n\n";
            statusText += "Do you want to continue?";
        } else if (status === "2") {
            statusText = "No service(s) are scheduled yet in " + name + "." + "\n";
            statusText += "By adding scheduler will impact all the avaialble services. " + " \n\n";
            statusText += "Do you want to continue?";
        } else if (status === "3") {
            if (parseInt(serviceScheduled) === 1)
                statusText = "1 service has been scheduled already in " + "." + name + "\n";
            else if (parseInt(serviceScheduled) >= 1) {
                statusText = serviceScheduled + " service(s) were already been scheduled in " + name + "." + "\n";
                statusText += "By Editing scheduler will replace all the current schedule of other services too." + "\n\n";
                statusText += "Do you want to continue?";
            }
        }
        if (confirm(statusText)) {
            openSchedulerModal("Schedule.aspx?e=" + envId + "&c=0&t=ed", "500");
        } else
            return false;
    }
    return true;
};

function DeleteConfig1(cfgID, name) {
    if (confirm("Are you sure, you want to delete configuration detail of " + name + "?")) {
        $.ajax({
            type: "POST",
            url: "../forms/Generic.aspx/DeleteEnvConfig",
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
    if (response.d != null)
        alert(response.d);
    else {
        location.reload();
        //this.location.href = this.location.href;
    }
}