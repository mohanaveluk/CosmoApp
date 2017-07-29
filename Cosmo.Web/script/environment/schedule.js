

function getFrequency() {
    setWindowHeight(false); 
    var optionChecked = $('#rdoFrequency input:checked').val();
    $('#divWeekly').css('display', 'none');
    $('#drpInterval').empty();
    //seconds
    if (optionChecked == "1") {
        $('#lblDuration').html('seconds');
        for (var iNumber = 5; iNumber < 100; iNumber += 5) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }
    //Minutes
    if (optionChecked == "2") {
        $('#lblDuration').html('minute');
        for (var iNumber = 1; iNumber < 60; iNumber++) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }
    //Hours
    if (optionChecked == "3") {
        $('#lblDuration').html('hour');
        for (var iNumber = 1; iNumber < 13; iNumber++) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }
    //Daily
    if (optionChecked == "4") {
        $('#lblDuration').html('day');
        for (var iNumber = 1; iNumber < 31; iNumber++) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }
    //Weekly
    if (optionChecked == "5") {
        setWindowHeight(true);
        $('#divWeekly').css('display', 'block');
        $('#lblDuration').html('week');
        for (var iNumber = 1; iNumber < 5; iNumber++) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }
    //Monthly
    if (optionChecked == "6") {
        $('#lblDuration').html('month');
        for (var iNumber = 1; iNumber < 13; iNumber++) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }
    //Yearly
    if (optionChecked == "7") {
        $('#lblDuration').html('year');
        for (var iNumber = 1; iNumber < 10; iNumber++) {
            $('#drpInterval').append('<option>' + iNumber + '</option>');
        }
    }

    if ($('#hidIntervalEdit').val() != '') {
        $('#drpInterval').val($('#hidIntervalEdit').val());
        setDurationText('drpInterval');
    }
    //Assign selected interval in to the Hidden variable
    getSelectedInterval();
}

//Assign selected interval in to the Hidden variable
function getSelectedInterval() {
    $('#hidInterval').val($('#drpInterval').val());
}

//Assign populate stored interval dropdown
function setSelectedInterval() {
    $('#drpInterval').val($('#hidInterval').val());
}

function setDurationText(name) {
    $('#hidIntervalEdit').val('');
    var optionChecked = $('#rdoFrequency input:checked').val();
    var selectedOption = $('#' + name).val();
    $('#hidInterval').val(selectedOption); // assign interval value to the hiden variable
    //getSelectedInterval(); we can call this to assign the selected interval into the Hidden variable as well if the above code is commented

    if (optionChecked == "1") {
        if (selectedOption == "1") {
            $('#lblDuration').html('second');
        }
        else {
            $('#lblDuration').html('seconds');
        }
    }
    if (optionChecked == "2") {
        if (selectedOption == "1") {
            $('#lblDuration').html('minute');
        }
        else {
            $('#lblDuration').html('minutes');
        }
    }
    if (optionChecked == "3") {
        if (selectedOption == "1") {
            $('#lblDuration').html('hour');
        }
        else {
            $('#lblDuration').html('hours');
        }
    }
    if (optionChecked == "4") {
        if (selectedOption == "1") {
            $('#lblDuration').html('day');
        }
        else {
            $('#lblDuration').html('days');
        }
    }
    if (optionChecked == "5") {
        if (selectedOption == "1") {
            $('#lblDuration').html('week');
        }
        else {
            $('#lblDuration').html('weeks');
        }
    }
    if (optionChecked == "6") {
        if (selectedOption == "1") {
            $('#lblDuration').html('month');
        }
        else {
            $('#lblDuration').html('months');
        }
    }
    if (optionChecked == "7") {
        if (selectedOption == "1") {
            $('#lblDuration').html('year');
        }
        else {
            $('#lblDuration').html('yeas');
        }
    }
}

function populateSchedulerSummary() {
    var summaryText = '';
    var selectedWeekdays = '';
    var optionChecked = $('#rdoFrequency input:checked').val();
    var selectedDuration = $('#drpInterval').val();
    var startDate = $('#txtStartDate').val();
    //var startTime = $('#txtTimeSelector_txtHour').val() + ":" + $('#txtTimeSelector_txtMinute').val() + ":" + $('#txtTimeSelector_txtSecond').val() + " " + $('#txtTimeSelector_txtAmPm').val();
    var endDateOption = getSchedulerEndDate(); // $('input:radio[name=rdoEndsTime]:checked').val();

    if (optionChecked == "1") {
        if (selectedDuration == "1")
            summaryText = 'every second';
        else
            summaryText = 'every ' + selectedDuration + ' seconds';
    }
    else if (optionChecked == "2") {
        if (selectedDuration == "1")
            summaryText = 'every minute';
        else
            summaryText = 'every ' + selectedDuration + ' minutes';
    }
    else if (optionChecked == "3") {
        if (selectedDuration == "1")
            summaryText = 'every hour';
        else
            summaryText = 'every ' + selectedDuration + ' hours';
    }
    else if (optionChecked == "4") {
        if (selectedDuration == "1")
            summaryText = 'every day';
        else
            summaryText = 'every ' + selectedDuration + ' days';
    }
    else if (optionChecked == "5") {
        if (selectedDuration == "1")
            summaryText = 'every week';
        else
            summaryText = 'every ' + selectedDuration + ' weeks';

        selectedWeekdays = getSelectedWeekdays();
    }
    else if (optionChecked == "6") {
        if (selectedDuration == "1")
            summaryText = 'every month';
        else
            summaryText = 'every ' + selectedDuration + ' months';
    }
    else if (optionChecked == "7") {

    }
    if (endDateOption != '') {
        $('#schedulerSummary').html("Summary : Service runs " + summaryText + ' ' + selectedWeekdays + ' which starts on ' + startDate + ' ' + endDateOption);
    }
    else {
        $('#schedulerSummary').html("Summary : Service runs " + summaryText + ' ' + selectedWeekdays + ' which starts on ' + startDate);
    }
    $('#hidSchSummary').val($('#schedulerSummary').html());
    window.parent.$("#hidSchedulerChanged").val("true");

}


function getSchedulerEndDate() {
    var schedulerEndDateText = '';

    var endDateOption = $('input:radio[name=rdoEndsTime]:checked').val();
    if (endDateOption == 'never') {
        schedulerEndDateText = '';
    }
    else if (endDateOption == 'after') {
        if ($('#txtOccurance').val() != "" && $.isNumeric($('#txtOccurance').val())) {
            if ($('#txtOccurance').val() == "1")
                schedulerEndDateText = 'for once';
            else
                schedulerEndDateText = 'for ' + $('#txtOccurance').val() + ' times';
        }
    }
    else if (endDateOption == 'on') {
        schedulerEndDateText = "until " + $('#txtEndOn').val();
    }
    return schedulerEndDateText;
}

function SetEndSTimecheduler() {
    var endDateOption = $('input:radio[name=rdoEndsTime]:checked').val();
    if (endDateOption == 'never') {
        $('#txtOccurance').attr('disabled', 'disabled');
        $('#txtEndOn').attr('disabled', 'disabled');
        $('#txtOccurance').val('');
        //$('#txtEndOn').val('');
    }
    else if (endDateOption == 'after') {
        $('#txtOccurance').removeAttr('disabled');
        $('#txtEndOn').attr('disabled', 'disabled');
        if ($('#txtOccurance').val() == '')
            $('#txtOccurance').val('5');
        $('#txtOccurance').focus();
        //$('#txtEndOn').val('');
    }
    else if (endDateOption == 'on') {
        $('#txtOccurance').attr('disabled', 'disabled');
        $('#txtEndOn').removeAttr('disabled');
        $('#txtOccurance').val('');
        if ($('#txtEndOn').val() == '') {
            $('#txtEndOn').val($.datepicker.formatDate('mm/dd/yy', new Date()));
            $('#txtEndOn').focus();
        }
    }

}

function getSelectedWeekdays() {
    var selectedWeekdays = '';
    if ($('#chkSaturday').is(":checked") && $('#chkMonday').is(":checked") && $('#chkTuesday').is(":checked") && $('#chkWednesday').is(":checked") && $('#chkThursday').is(":checked") && $('#chkFriday').is(":checked") && $('#chkSaturday').is(":checked")) {
        selectedWeekdays = 'all days';
    }
    else {
        if ($('#chkSunday').is(":checked")) {
            selectedWeekdays = $('#chkSunday').val();
        }
        if ($('#chkMonday').is(":checked")) {
            if (selectedWeekdays != '')
                selectedWeekdays += ', ' + $('#chkMonday').val();
            else
                selectedWeekdays += $('#chkMonday').val();
        }
        if ($('#chkTuesday').is(":checked")) {
            if (selectedWeekdays != '')
                selectedWeekdays += ', ' + $('#chkTuesday').val();
            else
                selectedWeekdays += $('#chkTuesday').val();
        }
        if ($('#chkWednesday').is(":checked")) {
            if (selectedWeekdays != '')
                selectedWeekdays += ', ' + $('#chkWednesday').val();
            else
                selectedWeekdays += $('#chkWednesday').val();
        }
        if ($('#chkThursday').is(":checked")) {
            if (selectedWeekdays != '')
                selectedWeekdays += ', ' + $('#chkThursday').val();
            else
                selectedWeekdays += $('#chkThursday').val();
        }
        if ($('#chkFriday').is(":checked")) {
            if (selectedWeekdays != '')
                selectedWeekdays += ', ' + $('#chkFriday').val();
            else
                selectedWeekdays += $('#chkFriday').val();
        }
        if ($('#chkSaturday').is(":checked")) {
            if (selectedWeekdays != '')
                selectedWeekdays += ', ' + $('#chkSaturday').val();
            else
                selectedWeekdays += $('#chkSaturday').val();
        }
    }
    return selectedWeekdays;
}

$(document).ready(function () {
   
    //$("#txtStartDate").prop("readonly", true);
    //$("#txtEndOn").prop("readonly", true);
   
    //called when key is pressed in textbox
    $("#txtOccurance").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $("#errmsg").html("Numbers Only").show().fadeOut("slow");
            return false;
        }
    });
    $("#txtOccurance").focusout(function (e) {
        if (!$.isNumeric($("#txtOccurance").val())) {
            $("#errmsg").html("Numbers Only").show().fadeOut("slow");
            $("#txtOccurance").val('');
            return false;
        }
    });
    $("#btnCreate").click(function () {
        ctrlFocus = '';
        var Errors = false;
        $('.errors').remove();

        /*if ($('#drpEnvironment option:selected').val() == '') {
        $('#drpEnvironment').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select environment'></span>");
        setCtrlFocus('drpEnvironment');
        Errors = true;
        }

        if ($('#drpHostPort option:selected').val() == '') {
        $('#drpHostPort').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select service'></span>");
        setCtrlFocus('drpHostPort');
        Errors = true;
        }*/

        var optionChecked = $('#rdoFrequency input:checked').val();
        if (optionChecked == '') {
            $('#rdoFrequency').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please choose frequency'></span>");
            setCtrlFocus('rdoFrequency');
            Errors = true;
        }

        var endDateOption = $('input:radio[name=rdoEndsTime]:checked').val();
        if (endDateOption == '') {
            $('#rdoEndsTime').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please choose end date time option'></span>");
            setCtrlFocus('rdoEndsTime');
            Errors = true;
        }

        if (endDateOption == 'after') {
            if ($('#txtOccurance').val() == '') {
                $('#txtOccurance').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please enter the occurance '></span>");
                setCtrlFocus('txtOccurance');
                Errors = true;
            }
        }
        else if (endDateOption == 'on') {
            if ($('#txtEndOn').val() == '') {
                $('#txtEndOn').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select the end date of schedule '></span>");
                setCtrlFocus('txtEndOn');
                Errors = true;
            }
        }

        if (Errors == true)
            return false;
        else
            return true;

    });
});

function setCtrlFocus(name) {
    if (!name == '') {
        $('#' + name).focus();
    }
}

function fnGetValue(val) {
    if (this.parent.modalWindow != null) {
        if (document.getElementById("hidIsDataUpdated").value == "updated")
            this.parent.location.href = this.parent.location.href;
        //this.parent.location.reload(false);
        if (val === "dataPassed" || $('#hidIsDataUpdated').val() === "dataPassed")
            window.parent.$("#hidSchedulerChanged").val("dataPassed");
        this.parent.modalWindow.close();
    } else {
        this.close();
        return false;
    }
    try {
        if (window.parent.location.pathname.toLowerCase().indexOf('servicedetails') >= 0) {
            var isOpen = $("#dialog").dialog("isOpen");
            if (isOpen && isOpen !== 'undefined') {
                //$("#dialog").dialog("destroy");
                //window.parent.closeIframe();
                //window.parent.$("#hidSchedulerChanged").val("value assigned");
                window.parent.$('#dialog').dialog('close');
            }
        }
    } catch (ex) {
        throw ex;
    }
}

var winHeight = 560;
function setWindowHeight(isIncrease) {
    var windowName = 'iframeSchedule';

    var saveStatus = $('#hidIsDataUpdated').val();
    if (window.parent.$('#iframeSchedule').length <= 0) {
        windowName = "dialog";
        //return;
    }
    if (isIncrease) {
        if (windowName === 'iframeSchedule') {
            this.parent.UpdategetEnvironmentList();
            if (window.innerHeight <= winHeight && saveStatus !== "") {
                window.parent.$('#' + windowName)[0].height = parseInt(winHeight + 90);
                return;
            }
            if (window.innerHeight > winHeight && saveStatus !== "") {
                window.parent.$('#' + windowName)[0].height = parseInt(winHeight + 90);
                return;
            } else if (window.innerHeight <= winHeight) {
                window.parent.$('#' + windowName)[0].height = parseInt(winHeight + 45);
                return;
            }


            window.parent.$('#' + windowName)[0].height = winHeight;
        }
        else if (windowName === 'dialog') {
            if (window.innerHeight <= winHeight && saveStatus !== "") {
                window.parent.$('#' + windowName).css("height", parseInt(winHeight + 90).toString() + 'px');
                return;
            }
            if (window.innerHeight > winHeight && saveStatus !== "") {
                window.parent.$('#' + windowName).css("height", parseInt(winHeight + 90).toString() + 'px');
                return;
            } else if (window.innerHeight <= winHeight) {
                window.parent.$('#' + windowName).css("height", parseInt(winHeight + 45).toString() + 'px');
                return;
            }
        }
    } else  {
        if (saveStatus !== "")
            window.parent.$('#' + windowName)[0].height = parseInt(winHeight + 45);
        else
            window.parent.$('#' + windowName)[0].height = winHeight;
    }
}

function setWindowHeightAlert(isIncrease) {
//    if (isIncrease) {
//        if (window.innerHeight <= parseInt(winHeight + 45)) {
//            window.parent.$('#iframeSchedule')[0].height = parseInt(window.innerHeight + 45).toString();
//        }
//    }
}