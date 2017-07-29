console.log(location.href);

$(function() {

    $("#drpHistory").change(function() {
        PopulateDateRange();
    });
    $('#divDateRange').hide();

    var date = new Date();

    if (location.href.indexOf("ReportStatusHistory") >= 0) {
        $('#datetimepicker1').datetimepicker({
            format: 'MM/DD/YYYY',
            defaultDate: date
        });
        $('#datetimepicker1').data("DateTimePicker").maxDate(date);

        $('#datetimepicker2').datetimepicker({
            format: 'MM/DD/YYYY',
            defaultDate: date
        });
        $('#datetimepicker2').data("DateTimePicker").maxDate(date);
    } else {
        $('#datetimepicker1').datetimepicker({
            defaultDate: date
        });
        $('#datetimepicker1').data("DateTimePicker").maxDate(date);

        $('#datetimepicker2').datetimepicker({
            defaultDate: date
        });
        $('#datetimepicker2').data("DateTimePicker").maxDate(date);
    }
});

function PopulateDateRange() {
    if ($('#drpHistory option:selected').val() == '20') {
        $('#divDateRange').show();
        $('#btnHistoryGo').attr("disabled", true);
        $('#btnHistoryGo').removeClass().addClass('btn btn-default');
    }
    else {
        $('#divDateRange').hide();
        $('#btnHistoryGo').removeAttr("disabled");
        $('#btnHistoryGo').removeClass().addClass('btn btn-primary');
    }
}

function updateDate() {
    console.log('calling UpdategetEnvironmentEmailList');
    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function () {
        scope.startDate = $('#txtFromDateTime').val();
        scope.endDate = $('#txtToDateTime').val();
    });
}

function SetTableSize(drpHistory) {
    console.log(drpHistory);
    if (drpHistory === "1" || drpHistory === "2" || drpHistory === "3") {
        $('#tblStatus').removeClass().addClass('table0_5');
    }
    else if (drpHistory === "4") {
        $('#tblStatus').removeClass().addClass('table0_75');
    } else {
        $('#tblStatus').removeClass().addClass('table1');
    }
}