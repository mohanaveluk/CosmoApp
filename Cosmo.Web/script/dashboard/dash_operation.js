var reloadTime = '<%=RefreshTime %>' * 60;
var nIntervId;
var isExpand = false;
var tableStatus;

function toggle_it(itemId) {
    if (document.getElementById("row_" + itemId) != null || document.getElementById("row_" + itemId) == undefined) {
        if ((document.getElementById("row_" + itemId).style.display == 'none')) {
            document.getElementById("row_" + itemId).style.display = '';
            $('#arrowImg_' + itemId + ' > span').removeClass('glyphicon glyphicon-triangle-bottom').addClass('glyphicon glyphicon-triangle-top');
            tableStatus = "open";
        }
        else {
            document.getElementById("row_" + itemId).style.display = 'none';
            $('#arrowImg_               ' + itemId + ' > span').removeClass('glyphicon glyphicon-triangle-top').addClass('glyphicon glyphicon-triangle-bottom');
            tableStatus = "close";
        }
    }
}


function onStart() {
    getCosmoServiceStatus(cosmoServiceName, systemName);

    var intIntervalId = setInterval(function () {
        getCosmoServiceStatus(cosmoServiceName, systemName);
    }, 5000);

    $('.db_cosmo_server li ul').hide().removeClass('fallback');
    $('.db_cosmo_server li').hover(
        function() {
            $('ul', this).stop().slideDown(200);
        },
        function() {
            $('ul', this).stop().slideUp(200);
        }
    );
}

function ReloadPage() {
    reloadTime--;
    if (reloadTime <= 0) {
        location.reload(true);
    }
}

function ReloadMonitor() {
    nIntervId = setInterval(function() {
        ReloadPage();
        var time = "Monitor status will update in ";
        time += reloadTime > 1 ? reloadTime + ' seconds' : reloadTime + ' second';
        window.parent.$("#lblTimeToRefresh").html(time);

    }, 1000);
}

onStart();