
var winHeight = 390;
function SetNotifyWindowHeight(isIncrease) {
    var windowName = 'iframeNotify';
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
