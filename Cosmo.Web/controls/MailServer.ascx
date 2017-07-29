<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailServer.ascx.cs" Inherits="Cosmo.Web.controls.MailServer" %>
<%@ Register TagPrefix="message" TagName="GenericMessage" Src="~/controls/GenericMessage.ascx" %>

    <asp:ScriptManager ID="ScriptManager2" runat="server" ></asp:ScriptManager>
        <!-- Begin page content -->
        <div class="container-fluid body-panel-setup">
		    <div style="height:8px"></div>
		    <div class="panel panel-primary">
		      <!-- Default panel contents -->
		      <div class="panel-heading" id="divPanelHeading">Mail Server Configuration</div>
		      <div class="panel-body" >

		          <div class="form-horizontal">
                    <div id="alertMessage">
                        <asp:UpdatePanel ID="udpMessage" runat="server" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <message:GenericMessage ID="genericMessage" runat="server" Visible="false" CurMessageType="Confirmation" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-3 control-label">Server Name</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtServerName"  runat="server" CssClass="form-control contr" ClientIDMode="Static" placeholder="Server name"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-3 control-label">Port</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtPort"  runat="server" CssClass="form-control contr" ClientIDMode="Static" placeholder="Server port"></asp:TextBox>
                        </div>
                    </div>
                  
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-3 control-label">Username</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtUserID"  runat="server" CssClass="form-control contr" ClientIDMode="Static" placeholder="Username"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-3 control-label">Password</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtPasswrd"  runat="server" CssClass="form-control contr" TextMode="Password" ClientIDMode="Static" placeholder="Password"></asp:TextBox>
                        </div>
                    </div>
                  
                    <div class="form-group">
                        <div class="col-sm-offset-3 col-sm-8">
                            <div class="checkbox">
                            <label style="margin-bottom:-4px">
                                <input type="checkbox" id="chkReolace" checked="True" runat="server"/>
                                <asp:HiddenField runat="server" ID="hidLoggedIn"/>
                            </label> SSL Enabled?
                            </div>
                        </div>
                    </div>  
                                    
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-3 control-label">Source Mail Id</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtFromMailId"  runat="server" CssClass="form-control contr"  ClientIDMode="Static" placeholder="Source Mail Id"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-md-12 text-center" >
                        <asp:Button ID="frmSubmit" runat="server" CssClass="btn btn-primary" 
                                    Text="Submit"  ClientIDMode="Static" onclick="btnCreate_Click" />
                        <asp:Button ID="btnSkip" runat="server" CssClass="btn btn-primary" 
                                    Text="Skip"  ClientIDMode="Static" onclick="btnSkip_Click" />
                    </div>                  

                </div>
		      </div>
		    </div>
        </div> 
        <!-- End of page content -->    
    
    
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
//            $(document).tooltip({
//                show: { effect: "slideDown", delay: 50 }
//            });

            $("#<%=frmSubmit.ClientID %>").click(function () {
                ctrlFocus = '';
                var Errors = false;
                $('.errors').remove();
                $('.help-block').remove();

                var loginstatus = $("#<%=hidLoggedIn.ClientID %>").val();

                if ($('#<%=txtServerName.ClientID %>').val() === '') {
                    //$('#<%=txtServerName.ClientID %>').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please enter Mail server name'></span>");
                    $('#txtServerName').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    setCtrlFocus('<%=txtServerName.ClientID %>');
                    Errors = true;
                }

                /*if ($('#drpAuthenticationType option:selected').val() == '2') {
                $('#drpAuthenticationType').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select service type'></span>");
                setCtrlFocus('drpAuthenticationType');
                Errors = true;
                }*/

                if ($('#<%=txtPort.ClientID %>').val() === '') {
                    //$('#<%=txtPort.ClientID %>').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please enter Mail server port'></span>");
                    $('#txtPort').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    setCtrlFocus('<%=txtPort.ClientID %>');
                    Errors = true;
                }

                if ($('#<%=txtUserID.ClientID %>').val() === '') {
                    //$('#<%=txtUserID.ClientID %>').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please enter username'></span>");
                    $('#txtUserID').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    setCtrlFocus('<%=txtUserID.ClientID %>');
                    Errors = true;
                }

                if ($('#<%=txtPasswrd.ClientID %>').val() === '') {
                    //$('#<%=txtPasswrd.ClientID %>').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please enter password'></span>");
                    $('#txtPasswrd').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    setCtrlFocus('<%=txtPasswrd.ClientID %>');
                    Errors = true;
                }

                if ($('#<%=txtFromMailId.ClientID %>').val() === '') {
                    //$('#<%=txtFromMailId.ClientID %>').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please enter password'></span>");
                    $('#txtFromMailId').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    setCtrlFocus('<%=txtFromMailId.ClientID %>');
                    Errors = true;
                }
                else if (!isValidEmailAddress($('#txtFromMailId').val())) {
                    //$('#<%=txtFromMailId.ClientID %>').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Invalid email address'></span>");
                    $('#txtFromMailId').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    $('#<%=txtFromMailId.ClientID %>').after("<span class='help-block'>Invalid email address</span>");
                    setCtrlFocus('<%=txtFromMailId.ClientID %>');
                    Errors = true;
                }

                $("input").keyup(function () {
                    if ($(this).val() === '')
                        $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    else
                        $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');

                    $('.help-block').remove();
                    if (!isValidEmailAddress($('#txtFromMailId').val())) {
                        $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                        $('#<%=txtFromMailId.ClientID %>').after("<span class='help-block'>Invalid email address</span>");
                    }
                });

                $("select, input").change(function () {
                    if ($(this).val() === '')
                        $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    else
                        $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                });

                $("select, input").blur(function () {
                    if ($(this).val() === '')
                        $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                    else
                        $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                });


                if (Errors === true) {
                    $('#<%=udpMessage.ClientID %>').html('<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>Warning!</strong> Please fill the highlighted inputs</div>');
                    return false;
                } else {
                    $('#<%=udpMessage.ClientID %>').html("");
                    if (loginstatus === "true") {
                        if (!confirm("Changes in mail server configuration causes to login again. Are you sure?")) {
                            return false;
                        }
                    }
                    //$("#<%=frmSubmit.ClientID %>").after("<div class='errors_db'><span><img src='../images/progress.gif' title='Please wait while processing...'/> Processing...</span></div>");
                    $(this).val('Processing...');
                    openModal();
                    return true;
                }
            });

            $("#<%=txtPort.ClientID %>").keydown(function (e) {
                // Allow: backspace, delete, tab, escape, enter and .
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

            $('#txtServerName').focus();
        });

        function setCtrlFocus(name) {
            if (name !== '') {
                $('#' + name).focus();
            }
        }

        function isValidEmailAddress(emailAddress) {
            var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
            return pattern.test(emailAddress);
        };

	</script>
	
    <style type="text/css">
        .errors_db {
            text-align: center;
            height: 40px;
            vertical-align: middle;
            padding-top: 10px;
        }
         .errors_db span
         {
             padding-top: 10px;
             font-family: Helvetica, Arial, sans-serif, Georgia, serif;
             font-size: .95em;
         }
    </style>



	
