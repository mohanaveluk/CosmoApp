function printPageContent(printContainer) {

    var headstr = "<html><head><title>Print title page</title>";
    headstr += "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/common.css\" />";
    headstr += "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/home_menu.css\" />";
    headstr += "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/bootstrap.css\" />";
    headstr += "<script src=\"../scripts/angular.min.js\" type=\"text/javascript\"></script>";
    headstr += "</head><body>";
    headstr += "<p>&nbsp;</p>";
    headstr += "<p>&nbsp;</p>";
    
    var footstr = "</body></html>";
    var documentContainer = document.getElementById(printContainer);
    var windowObject = window.open('', "PrintWindow", "width=100,height=100,top=100,left=200,toolbars=no,scrollbars=no,status=no,resizable=yes");
    windowObject.document.writeln(headstr + documentContainer.innerHTML + footstr);
    windowObject.document.close();
    windowObject.focus();
    windowObject.print();
    windowObject.close();
}

function printPageContent1(printContainer) {
    var documentContainer = document.getElementById(printContainer);
    var windowObject = window.open('', "PrintWindow", "width=100,height=100,top=100,left=200,toolbars=no,scrollbars=no,status=no,resizable=no");
    windowObject.document.writeln(documentContainer.innerHTML);
    windowObject.document.close();
    windowObject.focus();
    windowObject.print();
    windowObject.close();
}


function CompareDates(startDate, endDate) {
    var stDate = Date.parse(startDate);
    var enDate = Date.parse(endDate);
    if (stDate > enDate) {
        alert("To date shouldn't be less than start date");
        return false;
    }
    return true;
}


$(document).ready(function () {
    $('#printPage').click(function () {
        //window.print();
        printPageContent('main');
        return false;
    });
});