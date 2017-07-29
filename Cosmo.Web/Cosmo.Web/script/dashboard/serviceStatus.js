var cosmoServiceName = "Cosmo Monitor Service";
var systemName = '';
//var intIntervalId = setInterval11(function () {
//    console.log("calling service");
//}, 2000);


function getCosmoServiceStatus(serviceName, systemName) {
    var sAction;
    //console.log('calling GetWindowsServiceStatus: ' + systemName);
    $.ajax({
        type: "POST",
        url: "Generic.aspx/GetWindowsServiceStatus",
        data: "{'serviceName':'" + serviceName + "','systemName':'" + systemName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //console.log(response.d);
            DisplayServiceStatus(response.d);
        }
    });
}

function ServiceFunction(serviceName, action, systemName) {
    $("#divServiceStatus").html("Cosmo : <img src='../images/progress.gif' ID='imgCosmoStatus' alt='Running' />");
    $.ajax({
        type: "POST",
        url: "Generic.aspx/WindowServiceFuction",
        data: "{'serviceName':'" + serviceName + "','serviceAction':'" + action + "' ,'systemName':'" + systemName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(response) {
            DisplayServiceStatus(response.d);
        },
        error: function(response) {
            DisplayServiceStatus(response.d);
        }

    });
};
function cosmoServiceFunction(action) {
    ServiceFunction(cosmoServiceName, action, systemName);
}

function DisplayServiceStatus(status) {
    if (status === 'Running') {
        $("#divServiceStatus").html("Cosmo : <img src='../images/green_icon.jpg' ID='imgCosmoStatus' alt='Running' />"); //title='Cosmo service is running'
        $("#mnuStart a").removeAttr("onclick").addClass("disabled");
        $("#mnuStop a").attr("onclick", "cosmoServiceFunction('stop');return false;").removeClass("disabled");
        $("#mnuRestart a").attr("onclick", "cosmoServiceFunction('restart');return false;").removeClass("disabled");
    }
    else if (status === 'Stopped') {
        $("#divServiceStatus").html("Cosmo : <img src='../images/red1_icon.jpg' ID='imgCosmoStatus' alt='Stopped' />"); //title='Cosmo service is stopped'
        $("#mnuStart a").attr("onclick", "cosmoServiceFunction('start');return false;").removeClass("disabled");
        $("#mnuStop a").removeAttr("onclick").addClass("disabled");
        $("#mnuRestart a").addClass("disabled");
    }
    else if (status === 'Not Exists!') {
        $("#divServiceStatus").html("Cosmo : <img src='../images/gray_icon.png' ID='imgCosmoStatus' alt='Not Exists!' />"); //title='Cosmo service is not exists'
        $("#mnuStart a").removeAttr("onclick").addClass("disabled");
        $("#mnuStop a").removeAttr("onclick").addClass("disabled");
        $("#mnuRestart a").removeAttr("onclick").addClass("disabled");
    } else {
        $("#divServiceStatus").html("Cosmo : <img src='../images/red_icon.jpg' ID='imgCosmoStatus' alt='Error' />"); //title='Cosmo service is not exists'
        $("#mnuStart a").removeAttr("onclick").addClass("disabled");
        $("#mnuStop a").removeAttr("onclick").addClass("disabled");
        $("#mnuRestart a").removeAttr("onclick").addClass("disabled");
    }

}