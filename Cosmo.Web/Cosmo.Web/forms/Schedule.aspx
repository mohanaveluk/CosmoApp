<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs" Inherits="Cosmo.Web.forms.Schedule" %>
<%@ Register TagPrefix="message" TagName="GenericMessage" Src="~/controls/GenericMessage.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />
    <link href="../style/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />

    <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
    <script src="../style/js/moment.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap-datetimepicker.min.js" type="text/javascript"></script>
    <script src="../script/spinning.js" type="text/javascript"></script>
    <script src="../script/environment/schedule.js" type="text/javascript"></script>
    <script type="text/javascript" src="../script/jquery-ui.js"></script>
</head>
<body>
    <form id="form1" class="form-horizontal" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>
        <div class="panel panel-primary" style="height: 400px;">
        <!-- Default panel contents -->
		    <div class="panel-heading">Schedule </div>
		    <div class="panel-body" >
                <div style="background-color:#fff;">
                    <div id="DalertMessageiv1">
                        <asp:UpdatePanel ID="udpMessage" runat="server" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <message:GenericMessage ID="genericMessage" runat="server" Visible="false" CurMessageType="Confirmation" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                        
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-4 control-label" id="Label1">Environment / Service:</label>
                        <div class="col-sm-4">
                            <label runat="server" id="txtEnvironmentName" for="inputEmail3" class="control-label" ></label>
                            <asp:HiddenField ID="hidIsDataUpdated" runat="server" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-4 control-label" id="Label2">Host / IP Address:</label>
                        <div class="col-sm-8">
                            <label runat="server" id="txtPort" for="inputEmail3" class="control-label" style="text-align: left"></label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-offset-1 col-sm-2">
                            <label runat="server" id="Label4" for="inputEmail3" class="control-label" >Frequency</label>
                            <div id="divFrequency" class="radio">
                                <asp:RadioButtonList ID="rdoFrequency" runat="server" onchange="getFrequency(this);populateSchedulerSummary()"  >
                                    <asp:ListItem Text="Seconds" value="1" Selected="True" autofocus></asp:ListItem>
                                    <asp:ListItem Text="Minutes" value="2"></asp:ListItem>
                                    <asp:ListItem Text="Hours" value="3"></asp:ListItem>
                                    <asp:ListItem Text="Daily" value="4"></asp:ListItem>
                                    <asp:ListItem Text="Weekly" value="5"></asp:ListItem>
                                    <asp:ListItem Text="Monthly" value="6"></asp:ListItem>
                                    <%--<asp:ListItem Text="Yearly" value="7"></asp:ListItem>--%> 
                                </asp:RadioButtonList>
                                
                            </div>
                        </div>
                        <div class="col-sm-8 col-sm-offset-1">
                            <label runat="server" id="Label5" for="inputEmail3" class="control-label" >Recurrence</label>

                            <div class="form-group">
                                <label for="inputEmail3" class="col-sm-3 control-label" id="lblLocation">Every:</label>
                                <div class="col-sm-3">
                                    <asp:DropDownList runat="server" ID="drpInterval"  class="form-control" onchange="setDurationText(this.id);populateSchedulerSummary()"></asp:DropDownList>
                                    <asp:HiddenField runat="server" ID="hidInterval" /><asp:HiddenField runat="server" ID="hidIntervalEdit" />
                                </div>
                                <div class="col-sm-2" style="padding-top: 3px">
                                    <span id="lblDuration" >Seconds</span>
                                </div>
                            </div>

                            <div class="form-group" id="divWeekly" style="display:none">
                                <label for="inputEmail3" class="col-sm-3 control-label" id="Label6">Repeat On:</label>
                                <div class="col-sm-9">
                                    <label class="checkbox-inline">
                                    <input type="checkbox" runat="server" id="chkSunday" value="sun" onchange="populateSchedulerSummary()" /><span>S</span>
                                    </label>
                                    <label class="checkbox-inline"><input type="checkbox" runat="server" id="chkMonday" value="mon" onchange="populateSchedulerSummary()"/><span>M</span></label>
                                    <label class="checkbox-inline"><input type="checkbox" runat="server" id="chkTuesday" value="tue" onchange="populateSchedulerSummary()"/><span>T</span></label>
                                    <label class="checkbox-inline"><input type="checkbox" runat="server" id="chkWednesday" value="wed" onchange="populateSchedulerSummary()"/><span>W</span></label>
                                    <label class="checkbox-inline"><input type="checkbox" runat="server" id="chkThursday" value="thu" onchange="populateSchedulerSummary()"/><span>T</span></label>
                                    <label class="checkbox-inline"><input type="checkbox" runat="server" id="chkFriday" value="fri" onchange="populateSchedulerSummary()"/><span>F</span></label>
                                    <label class="checkbox-inline"><input type="checkbox" runat="server" id="chkSaturday" value="sat" onchange="populateSchedulerSummary()"/><span>S</span></label>
                                    
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="inputEmail3" class="col-sm-3 control-label" id="Label7">Starts On:</label>
                                <div class="col-sm-6">
                                      <div class='input-group date' id='datetimepicker1'>
                                        <asp:TextBox ID="txtStartDate" CssClass="form-control" runat="server" OnChange="populateSchedulerSummary()" onKeyPress="return false;"></asp:TextBox>                                
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                      </div>
                                </div>
                            </div>
                            
                            <div class="form-group">
                                <label for="inputEmail3" class="col-sm-3 control-label" id="Label8">Ends:</label>
                                <div class="col-sm-9">
                                    
                                    <div class="form-group">
                                        <div class="col-sm-5">
                                            <div class="radio">
                                              <label>
                                                  <input runat="server" id="rdoEndsTime_1" name="rdoEndsTime" type="radio" value="never" checked onclick="SetEndSTimecheduler();populateSchedulerSummary()"/>
                                                    Never
                                              </label>
                                            </div>
                                        </div>                                    
                                    </div>

                                    <div class="form-group">
                                        <div class="col-sm-2">
                                            <div class="radio">
                                              <label>
                                                  <input runat="server" id="rdoEndsTime_2" name="rdoEndsTime" type="radio" value="after" onclick="SetEndSTimecheduler();populateSchedulerSummary()" />
                                                    After
                                              </label>
                                            </div>
                                        </div>                                    
                                        <div class="col-sm-2">
                                            <asp:TextBox ID="txtOccurance" runat="server" MaxLength="3" class="form-control" data-toggle="tooltip" data-placement="bottom" Width="50" onchange="populateSchedulerSummary()"></asp:TextBox>
                                        </div>                                    
                                        <div class="col-sm-8" style="padding-top: 3px">
                                            <span>Occurence(s)</span>
                                            &nbsp;<span id="errmsg" class="text-danger"></span><br />
                                        </div>                                    
                                    </div>

                                    <div class="form-group">
                                        <div class="col-sm-2">
                                            <div class="radio">
                                              <label>
                                                  <input runat="server" id="rdoEndsTime_3" name="rdoEndsTime" type="radio" value="on" onclick="SetEndSTimecheduler();populateSchedulerSummary()" />
                                                    On
                                              </label>
                                            </div>
                                        </div> 
                                        <div class="col-sm-8">
                                            <div class='input-group date' id='dateEndsOn'>
                                                <asp:TextBox ID="txtEndOn" CssClass="form-control" runat="server" Onchange="populateSchedulerSummary()" onKeyPress="return false;"></asp:TextBox>                                
                                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                              </div>
                                        </div>                                   
                                    </div>


                                </div>
                            </div>

                        </div>

                    </div>
                    
                    <div class="row" style="height: 30px">
                        <div class="col-sm-offset-1 col-sm-10">
                            <div id="schedulerSummary" >Summary:</div>
                            <asp:HiddenField runat="server" ID="hidSchSummary" />
                            <asp:HiddenField ID="hidEnvID" runat="server" /><asp:HiddenField ID="HiddenField2" runat="server" />
                        </div>
                    </div>

                    <hr class="h-line" />
                    <div class="form-group text-center">
                      <div > <%--class="col-sm-offset-2 col-sm-10"--%>
                        <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-primary" 
                                Text="Save" OnClick="btnCreate_Click" ClientIDMode="Static" />
                        <%--<asp:Button ID="btnCreateClose" runat="server" CssClass="btn btn-primary" Text="Save & Close" OnClick="btnCreateClose_Click" ClientIDMode="Static" />--%>
                        <button type="button" class="btn btn-primary" id="btnCancel" onclick="fnGetValue(this)" >Close</button>
                      </div>
                    </div>

                </div>          
		    </div>
	    </div>
    <div id="divSessionAlert" style="display: none">
        alert message
    </div>
    <!--Begin Page Progress-->
    <div id="fade-process"></div>
    <div id="modal-process">
        <img id="loader" src="../images/ajax-loader.gif" alt="Processing..." />
    </div>
    <!--End of Page Progress-->

    </form>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            
            $('#btnCreate').click(function() {
                openModal();
                var errors = false;
            });

            var date = new Date();
            date.setDate(date.getDate() - 1);
            $('#datetimepicker1').datetimepicker({
                defaultDate: date

            });
            if ($('#txtStartDate').val() === "")
                $('#datetimepicker1').data("DateTimePicker").minDate(date);

            $('#dateEndsOn').datetimepicker({
                defaultDate: date
            });
            if ($('#txtEndOn').val() === "")
                $('#dateEndsOn').data("DateTimePicker").minDate(date);

            $('#txtStartDate').blur(function() {
                populateSchedulerSummary();
            });
            $('#txtEndOn').blur(function () {
                populateSchedulerSummary();
            });
        });

        $(function () {
            getFrequency();
            SetEndSTimecheduler();
            populateSchedulerSummary();
            window.parent.$("#hidSchedulerChanged").val("");
        });
        
        function fnGetValue1(val) {
            if (this.parent.modalWindow != null) {
                if (document.getElementById("hidIsDataUpdated").value == "updated")
                //this.parent.location.href = this.parent.location.href;
                    this.parent.location.reload(false);
                this.parent.modalWindow.close();
            }
            else {
                this.close();
                return false;
            }
            return true;
        }

    </script>

</body>
</html>
