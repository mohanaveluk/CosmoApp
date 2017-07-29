<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UrlConfigurationDetails.aspx.cs" Inherits="Cosmo.Web.forms.UrlConfigurationDetails" %>
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
    <script src="../script/environment/urldetails.js" type="text/javascript"></script>
    <script src="../script/simple_modal.js" type="text/javascript"></script>
    <script src="../script/jquery-ui.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>
        <div class="panel panel-primary" style="height: 400px;">
        <!-- Default panel contents -->
		    <div class="panel-heading">URL Configuration </div>
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
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig">URL Type</label>
                                <div class="col-sm-8">
                                    <asp:DropDownList ID="drpUrlType" runat="server" CssClass="form-control" placeholder="Url Type" autofocus>
                                        <asp:ListItem Value="Cognos Portal URL" Text="Cognos Portal URL" Selected="True"></asp:ListItem>
                                        <%--<asp:ListItem Value="Others" Text="Others"></asp:ListItem>--%>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig">Environment</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtEnvironment" runat="server" CssClass="form-control" placeholder="Environment Name" autocomplete="off" autofocus></asp:TextBox>
                                    <asp:HiddenField ID="hidEnvironment" runat="server" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="lblLocation">URL Address</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Cognos Connection Gateway URI"></asp:TextBox>
                                </div>
                            </div>                                                       

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="Label1">Display Name</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" placeholder="Display Name"></asp:TextBox>
                                </div>
                            </div>                                                       

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="Label2">Match Content</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtMatchContent" runat="server" CssClass="form-control" placeholder="Match Content"></asp:TextBox>
                                </div>
                            </div>                                                       

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="Label3">Polling Interval</label>
                                <div class="col-sm-2">
                                    <asp:TextBox ID="txtPollingInterval" runat="server" CssClass="form-control" placeholder="0" maxlength="3" ></asp:TextBox>
                                </div> 
                                <label class="col-sm-1 control-label"> <span>Minute(s)</span></label>
                            </div>                                                       

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="Label4">User Name</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="Username"></asp:TextBox>
                                </div>
                            </div>                                                       


                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="Label5">Password</label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Password" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>                                                       

                            <div class="form-group">
                                <label class="col-sm-2 control-label" for="EnvConfig" id="Label6">Monitor</label>
                                <div class="checkbox col-sm-1">
                                  <label>
                                    <input type="checkbox" id="chkActive" runat="server" value="status" checked="True" aria-label="..." />
                                  </label>
                                </div>
                            </div>                                                       

                        </div>
                    </div>

                    

                    <hr class="h-line" />
                    <div class="form-group text-center">
                      <div > <%--class="col-sm-offset-2 col-sm-10"--%>
                        <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-primary" 
                                Text="Save" OnClick="btnCreate_Click" ClientIDMode="Static" />
                        <button type="button" class="btn btn-primary" id="btnCancel" onclick="fnCancel(this)" >Close</button>
                        <button type="button" class="btn btn-primary" id="btnTestUrl" >Test</button>
                        <asp:HiddenField ID="hidEnvID" runat="server" />
                        <asp:HiddenField ID="hidUrlID" runat="server" />
                        <asp:HiddenField ID="hidIsDataUpdated" runat="server" />
                        <asp:HiddenField ID="hidType" runat="server" />
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

    
    <!--Daily Status report - start-->
    <div class="modal fade bs-example-modal-lg" id="modalTestUrl" tabindex="-1" aria-labelledby="myLargeModalLabel" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">
                        URL Test Status</h4>
                </div>
                <div class="modal-body">
                    <div id="testMessage"></div>
                </div>
                <div class="modal-footer ">
                    <button type="button" class="btn btn-default" data-dismiss="modal" id="btnSubscriptionClose">
                        Close</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!--Daily Status report - end-->

  <script language="javascript" type="text/javascript">
      $(function () {
          //$('#txtEnvironment').focus();
          var sessionUserId = '<%= Session["_LOGGED_USERD_ID"] %>';
          console.log("User Id: " + sessionUserId);
          if (sessionUserId === null || sessionUserId === undefined || sessionUserId === "") {
              window.top.location.href = "../login/Default.aspx";
          }

          $("#txtPollingInterval").keydown(function (e) {
              // Allow: backspace, delete, tab, escape, enter and ( . for 190)
              if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
              // Allow: Ctrl+A, Command+A
                        (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
              // Allow: home, end, left, right, down, up
                        (e.keyCode >= 35 && e.keyCode <= 40)) {
                  // let it happen, don't do anything
                  return;
              }
              // Ensure that it is a number and stop the keypress
              if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                  e.preventDefault();
              }
          });

          $("#txtMatchContent1").keydown(function (e) {
              if (e.keyCode === 32)
                  e.preventDefault();
          });
      });
  </script>
</body>
</html>
