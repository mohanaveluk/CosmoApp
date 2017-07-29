<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceDetails.aspx.cs" Inherits="Cosmo.Web.forms.ServiceDetails" %>
<%@ Register TagPrefix="message" TagName="GenericMessage" Src="~/controls/GenericMessage.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />
    <link href="../style/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="../style/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
    <script src="../style/js/moment.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap-datetimepicker.min.js" type="text/javascript"></script>
    <script src="../script/spinning.js" type="text/javascript"></script>
    <script src="../script/typeahead.bundle.js" type="text/javascript"></script>
    <script src="../script/environment/servicedetails.js" type="text/javascript"></script>
    <script src="../script/simple_modal.js" type="text/javascript"></script>
    <script src="../script/jquery-ui.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>
        <div class="panel panel-primary" style="height: 400px;">
        <!-- Default panel contents -->
		    <div class="panel-heading">Environment </div>
		    <div class="panel-body" >
                <div style="background-color:#fff;">
                    <div id="DalertMessageiv1">
                        <asp:UpdatePanel ID="udpMessage" runat="server" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <message:GenericMessage ID="genericMessage" runat="server" Visible="false" CurMessageType="Confirmation" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    
                    <div class="row">
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig">Environment Name</label>
                                <asp:TextBox ID="txtEnvironment" runat="server" CssClass="form-control" placeholder="Environment Name" autocomplete="off" autofocus></asp:TextBox>
                                <asp:HiddenField ID="hidEnvironment" runat="server" />
                            </div>                                                       
                        </div>
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig" runat="server" id="lblLocation">Location</label>
                                <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" placeholder="Server Location"></asp:TextBox>
                            </div>                                                       
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig" runat="server" id="lblIPAddress">Host / IP Address</label>
                                <asp:TextBox ID="txtHostIP" runat="server" CssClass="form-control" placeholder="Host Name / IP Address" style="display: inline-block" MaxLength="95"></asp:TextBox>
                            </div>                                                       
                        </div>
                        <div class="col-sm-1">
                            <div class="ping-status"><img id="imgHostStatus" src=""  alt=""/></div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig" runat="server" id="lblPort">Port</label>
                                <asp:TextBox ID="txtPort" runat="server" CssClass="form-control" placeholder="Server Port"></asp:TextBox>
                            </div>                                                       
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig" runat="server" id="lblServerDescription">Server Description</label>
                                <asp:TextBox ID="txtServerDescripption" runat="server" CssClass="form-control" placeholder="Server Description"></asp:TextBox>
                            </div>                                                       
                        </div>
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig" runat="server" id="lblServiceType" >Service Type</label>
                                <asp:DropDownList ID="drpServiceType" runat="server" CssClass="form-control">
                                    <%--<asp:ListItem Text="Select service" Value=""></asp:ListItem>--%>       
                                    <asp:ListItem Text="Content Manager" Value="1" Selected="True"></asp:ListItem>       
                                    <asp:ListItem Text="Dispatcher" Value="2"></asp:ListItem>       
                                </asp:DropDownList>
                            </div>                                                       
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig">Service Name</label>
                                <asp:TextBox ID="txtWindowsServiceName" runat="server" CssClass="form-control" placeholder="Windows Service Name"></asp:TextBox>
                                <asp:HiddenField runat="server" ID="hidWindowsServiceID" />
                            </div>                                                       
                        </div>
                        <asp:TextBox ID="txtURL" runat="server" ReadOnly="true" style="display: none"></asp:TextBox>
                        <asp:HiddenField runat="server" ID="hidServiceURL" />
                        <div class="col-sm-offset-1 col-sm-4">
                            <div CssClass="form-group">
                                <label for="EnvConfig">Mail Frequency</label>
                                <asp:DropDownList ID="drpMailFrequency" runat="server" class="form-control">
                                </asp:DropDownList>
                            </div>                                                       
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <label for="EnvConfig">Comments</label>
                                <asp:TextBox ID="txtComments" runat="server" CssClass="form-control" placeholder="Comments" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                <asp:HiddenField ID="hidEnvID" runat="server" />
                                <asp:HiddenField ID="hidEnvIDEmail" runat="server" />
                                <asp:HiddenField ID="hidIsDataUpdated" runat="server" ClientIDMode="Static"/>
                                <asp:HiddenField ID="hidSchedulerChanged" runat="server" ClientIDMode="Static"/>
                                <asp:HiddenField ID="hidIsCancelRequest" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="hidServiceType" runat="server" />

                            </div>                                                       
                        </div>
                        <div class="col-sm-offset-1 col-sm-4">
                            <div class="form-group">
                                <br/>
                                <div class="checkbox">
                                    <label>
                                        <asp:CheckBox ID="chkIsMonitor" runat="server" Checked="true"></asp:CheckBox> Is monitered?
                                        <span class=""><img src='../images/help1.png' title='This option decides whether to monitor this server or not' alt='' /></span>                                        
                                    </label>
                                </div>
                                <div class="checkbox">
                                    <label>
                                        <asp:CheckBox ID="chkIsConsolidated" runat="server" Checked="true"></asp:CheckBox> Is consolidated?
                                        <span class=""><img src='../images/help1.png' title='This option decides whether to consolidate all the service faliures and send a mail or not' alt='' /></span>
                                    </label>
                                </div>
                                <div class="checkbox">
                                    <label>
                                        <asp:CheckBox ID="chkIsNotify" runat="server" Checked="true"></asp:CheckBox> Is notify?
                                        <span class=""><img src='../images/help1.png' title='This option decides whether to notify if any service failure or not' alt='' /></span>
                                    </label>
                                </div>
                            </div>                                                       
                        </div>
                    </div>
                    <hr class="h-line" />
                    <div class="form-group text-center">
                      <div > <%--class="col-sm-offset-2 col-sm-10"--%>
                        <button class="btn btn-primary"  onclick="return false" id="btnEmailNotify" >Notification</button>
                        <button class="btn btn-default"  onclick="return false" id="btnEditSchedule" disabled="disabled" 
                        title='Add mail address for notofication and start schedule'>Schedule</button>
                        <%--<a href="../forms/Schedule.aspx" class="btn btn-lg btn-primary" data-toggle="modal" data-target="#modalScheduler">Launch Demo Modal</a>--%>
                        <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-primary" 
                                Text="Save" OnClick="btnCreate_Click" ClientIDMode="Static" />
                        <button type="button" class="btn btn-primary" id="btnCancel" onclick="fnCancel(this)" >Close</button>
                      </div>
                    </div>

                </div>          
		    </div>
	    </div>
    <!--Begin Page Progress-->
    <div id="fade-process"></div>
    <div id="modal-process">
        <img id="loader" src="../images/ajax-loader.gif" alt="Processing..." />
    </div>
    <div id="dialog" style="left: 20px" top="20px"><iframe class='scheduleFrame' id="myIframe" src="" width="100%" height="100%"  scrolling="no" frameborder="0" style="display: none"></iframe></div>
    <div id="emailDialog"><iframe class='scheduleFrame' id="emailFrame" src="" width="100%" height="100%"  scrolling="no" frameborder="0" style="display: none"></iframe></div>

    <!--End of Page Progress-->
    

    <div class="modal fade" tabindex="-1" id="modalScheduler" role="dialog" style="height: auto">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            <h4 class="modal-title">Modal title</h4>
          </div>
          <div class="modal-body">
            <p>One fine body&hellip;</p>
          </div>
        </div><!-- /.modal-content -->
      </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->    

  </form>
  <script language="javascript" type="text/javascript">
      $(function () {
          //$('#txtEnvironment').focus();
          var sessionUserId = '<%= Session["_LOGGED_USERD_ID"] %>';
          console.log("User Id: " + sessionUserId);
          if (sessionUserId === null || sessionUserId === undefined || sessionUserId === "") {
              window.top.location.href = "../login/Default.aspx";
          }
      });
  </script>
</body>
</html>
