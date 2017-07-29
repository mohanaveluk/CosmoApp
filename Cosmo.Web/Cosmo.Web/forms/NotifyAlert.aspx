<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotifyAlert.aspx.cs" Inherits="Cosmo.Web.forms.NotifyAlert" %>
<%@ Register TagPrefix="message" Src="~/Controls/GenericMessage.ascx" TagName="GenericMessage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" class="form-horizontal" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>
    
        <div class="panel panel-primary" style="height: 400px;">
        <!-- Default panel contents -->
		    <div class="panel-heading">Notification </div>
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
                        <div class="col-sm-7">
                            <label runat="server" id="lblEnvironmentName" for="inputEmail3" class="control-label" ></label>
                            <asp:HiddenField ID="hidIsDataUpdated" runat="server" />
                        </div>
                    </div>

                    <div class="form-group radio">
                        <label for="inputEmail3" class="col-sm-4 control-label" id="updatedText">Alert Mode :</label>
                        <div class="col-sm-7 ">
                            <asp:RadioButton  runat="server" GroupName="grpAcknoledge" ID="opt_Acknowledge" Text="Acknowledge" Checked="True" />
                        </div>
                        <div class="col-sm-7 ">
                            <asp:RadioButton runat="server" GroupName="grpAcknoledge" ID="opt_Acknowledge_alert" Text="" />
                        </div>
                    </div>
                    <p>&nbsp;</p>
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-4 control-label" id="lblLocation">Reason for change :</label>
                        <div class="col-sm-5">
                            <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Rows="4"  class="form-control" data-toggle="tooltip" data-placement="bottom"></asp:TextBox>
                            <asp:HiddenField ID="hidUserEmailID" runat="server" /><%--to store the userEmai id in check whether to add / update the email id in the table --%>
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                            <asp:HiddenField ID="hidMode" runat="server" />
                        </div>
                    </div>
                    
                    <%--<span id="helpBlock2" class="help-block">Please fill the mandatory input(s)</span>--%>
                    
                    <hr class="h-line" />
                    <div class="form-group text-center">
                      <div > <%--class="col-sm-offset-2 col-sm-10"--%>
                        <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-primary" 
                                Text="Save" OnClick="btnCreate_Click" ClientIDMode="Static" />
                        <button type="button" class="btn btn-primary" id="btnCancel" onclick="fnGetValue(this)" >Close</button>
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
        <!--End of Page Progress-->
        <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
        <script src="../style/js/bootstrap.min.js" type="text/javascript"></script>
        <script src="../script/spinning.js" type="text/javascript"></script>
        <script src="../script/dashboard/notifyalert.js" type="text/javascript"></script>

    </form>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {

            $('#btnCreate').click(function() {
                var sessionUserId = '<%= Session["_LOGGED_USERD_ID"] %>';
                console.log("User Id: " + sessionUserId);
                if (sessionUserId === null || sessionUserId === undefined || sessionUserId === "") {
                    window.top.location.href = "../login/Default.aspx";
                }
                var errors = false;

                if ($('#opt_Acknowledge').is(':checked') === false && $('#opt_Acknowledge_alert').is(':checked') === false) {
                    $('#opt_Acknowledge').closest('.form-group').removeClass('form-group radio').addClass('form-group radio has-error');
                    errors = true;
                } else {
                    $('#opt_Acknowledge').closest('.form-group').removeClass('form-group radio has-error').addClass('form-group radio');
                }

                if ($('#txtReason').val() === '') {
                    $('#txtReason').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    errors = true;
                } else {
                    $('#txtReason').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                }

                $("input, textarea").keyup(function () {
                    if ($(this).val() === '')
                        $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    else
                        $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                });

                $("select, input").change(function () {
                    if ($(this).val() === '')
                        $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    else
                        $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                });


                if (errors) {
                    return false;
                } else {
                    openModal();
                    return true;
                }

            });
            $('#txtReason').focus();
        });

        function fnGetValue(val) {
            if (this.parent.modalWindow != null) {
                if ($("#hidIsDataUpdated").val() === "updated") {
                    this.parent.UpdateMonitorList();
                }
                this.parent.modalWindow.close();
            } else {
                this.close();
                return false;
            }
            return true;
        }


    </script>
</body>
</html>
