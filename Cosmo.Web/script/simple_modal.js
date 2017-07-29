var modalWindow = {
    parent: "body",
    windowId: null,
    content: null,
    width: null,
    height: null,
    close: function () {
        $(".modal-window").remove();
        $(".modal-overlay").remove();
    },
    save: function () {
        SubmitOpenWindow();
    },
    open: function () {
        
        var modal = "";
        modal += "<div class=\"modal-overlay\"></div>";
        modal += "<div id=\"" + this.windowId + "\" class=\"modal-window\" style=\"width:" + this.width + "px; height:" + this.height + "px; margin-top:-" + (((this.height) / 2) +0) + "px; margin-left:-" + (((this.width) / 3) + 0) + "px;\">";
        modal += this.content;
        modal += "</div>";
        //alert(this.width + " - " + screen.width + " - " + this.height + " - " + ((screen.width - this.width) / 2));
        $(this.parent).append(modal);

        //$(".modal-window").append("<a class=\"close-window\" title=\"\"></a>");
        $(".close-window").click(function () { modalWindow.close(); });
        //$(".modal-overlay").click(function(){modalWindow.close();});
    }
};

var modalWindowSchedule = {
    parent: "body",
    windowId: null,
    content: null,
    width: null,
    height: null,
    close: function () {
        $(".modal-window").remove();
        $(".modal-overlay").remove();
    },
    save: function () {
        SubmitOpenWindow();
    },
    open: function () {

        var modal1 = "";
        modal1 += "<div class=\"modal-overlay\"></div>";
        modal1 += "<div id=\"" + this.windowId + "\" class=\"modal-window\" style=\"width:" + this.width + "px; height:" + this.height + "px; margin-top:-" + ((this.height) / 1) + "px; margin-left:-" + ((this.width) / 3) + "px;\">";
        modal1 += this.content;
        modal1 += "</div>";
        //alert(this.width + " - " + screen.width + " - " + this.height + " - " + ((screen.width - this.width) / 2));
        $(this.parent).append(modal1);

        $(".modal-window").append("<a class=\"close-window\" title=\"Close\"></a>");
        $(".close-window").click(function () { modalWindowSchedule.close(); });
        //$(".modal-overlay").click(function(){modalWindow.close();});
    }
};

var modalWindowHistory = {
    parent: "body",
    windowId: null,
    content: null,
    width: null,
    height: null,
    close: function () {
        $(".modal-window").remove();
        $(".modal-overlay").remove();
    },
    save: function () {
        SubmitOpenWindow();
    },
    open: function () {

        var modal1 = "";
        modal1 += "<div class=\"modal-overlay\"></div>";
        modal1 += "<div id=\"" + this.windowId + "\" class=\"modal-window\" style=\"width:" + this.width + "px; height:" + this.height + "px; margin-top:-" + ((this.height) / 2.3) + "px; margin-left:-" + ((this.width) / 2.5) + "px;\">";
        modal1 += this.content;
        modal1 += "</div>";
        //alert(this.width + " - " + screen.width + " - " + this.height + " - " + ((screen.width - this.width) / 2));
        $(this.parent).append(modal1);
       
        $(".modal-window").append("<a class=\"close-window\" title=\"Close\"></a>");
        $(".close-window").click(function () { modalWindowHistory.close(); });
        //$(".modal-overlay").click(function(){modalWindow.close();});
    }
};

function fnGetValue(val)
{
	val.parent.modalWindow.close();
}

function OpenerWindowSubmit(parentPageName, hiddenFieldName, hiddenFieldValue) {
    try {
        if (opener.window.location.href.indexOf(parentPageName) > 0) {
            if (hiddenFieldName.toString().length > 0) {
                if (opener.document.getElementById(hiddenFieldName) != null)
                    opener.document.getElementById(hiddenFieldName).value = hiddenFieldValue;
            }

            opener.document.forms[0].submit();
            //opener.document.location.reload(true);
        }
    }
    catch (ex) {
    }
}

Date.prototype.format = function (format) //author: meizz
{
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }

    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
      (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
          RegExp.$1.length == 1 ? o[k] :
            ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

function DateTimeFormat(input) {
    var todayDate = new Date(input);
    //var format ="AM";
    var hour = todayDate.getHours();
    var min = todayDate.getMinutes();
    var ss = todayDate.getSeconds();
    //if(hour>11){format="PM";}
    //if (hour   > 12) { hour = hour - 12; }
    //if (hour   == 0) { hour = 12; }  
    if (min < 10) { min = "0" + min; }
    return (todayDate.getFullYear() + "/" + (todayDate.getMonth() + 1) + "/" + todayDate.getDate() + " " + hour + ":" + min + ":" + (ss +1));

}

// Opens a new window with the given URL

function OpenNewWindow(url) {
    if (url == 'alertMessage') {
        alert("This will open Project Overview for selected Project ID in a new window.");
    }
    else {
        if (screen) {
            y = (screen.availHeight - 0 ) / 2; // y = (screen.availHeight - 700) / 2;
            x = (screen.availWidth - 0) / 2; // x = (screen.availWidth - 1010) / 2;
            //alert(screen.availHeight + ' ' + y + ' - ' + screen.availWidth + ' ' + x);
        }
        var winName = "_blank";
        window.open(url, winName, 'width=1300px,height=850px,screenX=' + x + ',screenY=' + y + ',status=yes,resizable=yes,scrollbars=yes,top=3,left=5');
    }
}

function OpenNewWindowService(url) {
    if (url == 'alertMessage') {
        alert("This will open Project Overview for selected Project ID in a new window.");
    }
    else {
        if (screen) {
            y = (screen.availHeight - 0) / 1; // y = (screen.availHeight - 700) / 2;
            x = (screen.availWidth -0) / 1.3; // x = (screen.availWidth - 1010) / 2;
            //alert(screen.availHeight + ' ' + y + ' - ' + screen.availWidth + ' ' + x);
        }
        var winName = "_blank";
        window.open(url, winName, 'width='+ x + ',height=' + y + ',screenX=' + x + ',screenY=' + y + ',status=yes,resizable=yes,scrollbars=yes,top=3,left=5');
    }
}

var openWindowToSetTimeInterval = function (source) {
    modalWindow.windowId = "myModal";
    modalWindow.width = 800;
    modalWindow.height = 450;
    modalWindow.content = "<iframe id='iframeDashboardSetting' width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin:1px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'></iframe>";
    modalWindow.open();
};

var openWindowToSetDailyReport = function (source) {
    modalWindow.windowId = "dailyReportModal";
    modalWindow.width = 800;
    modalWindow.height = 500;
    modalWindow.content = "<iframe id='iframeDashboardSetting' width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin:1px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'></iframe>";
    modalWindow.open();
};

var OpenHistory = function (source) {
    modalWindowHistory.windowId = "myModal";
    modalWindowHistory.width = 900;
    modalWindowHistory.height = 220;
    modalWindowHistory.content = "<iframe width='" + modalWindowHistory.width + "'; height='" + modalWindowHistory.height + "' style='background-color:#f7f7f7; margin:1px;' frameborder='0' scrolling='yes' allowtransparency='true' src='" + source + "'>&lt/iframe>";
    modalWindowHistory.open();
};

var openNotifyAlert = function (source) {
    modalWindow.windowId = "myModal";
    modalWindow.width = 800;
    modalWindow.height = 390;
    modalWindow.content = "<iframe id='iframeNotify' width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin-top:0px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'>&lt/iframe>";
    modalWindow.open();
};

var openMyModal = function (source) {
    modalWindow.windowId = "myModal";
    modalWindow.width = 800;
    modalWindow.height = 400;
    modalWindow.content = "<iframe width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin-top:0px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'>&lt/iframe>";
    modalWindow.open();
};

function showDialog(source) {
    var sFeatures = "dialogHeight:400px;dialogWidth:900px;";
    window.showModalDialog(source, "", sFeatures)
}

var openEmailConfig = function (source) {
    modalWindow.windowId = "myModal";
    modalWindow.width = 800;
    modalWindow.height = 630;
    modalWindow.content = "<iframe width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin:1px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'>&lt/iframe>";
    modalWindow.open();
};

var openSchedulerModal = function (source) {
    modalWindow.windowId = "ModelSchedule";
    modalWindow.width = 900;
    modalWindow.height = 550;
    modalWindow.content = "<iframe id='iframeSchedule' width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin:1px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'>&lt/iframe>";
    modalWindow.open();
};

var openEditEnvironmentModal = function (source) {
    modalWindow.windowId = "myModal";
    modalWindow.width = 1000;
    modalWindow.height = 600;
    modalWindow.content = "<iframe id='iframeEditConfiguration' width='" + modalWindow.width + "'; height='" + modalWindow.height + "' style='background-color:f7f7f7; margin:1px;' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'>&lt/iframe>";
    modalWindow.open();
};
