<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashboardSetting.aspx.cs" Inherits="Cosmo.Web.forms.DashboardSetting" %>
<%@ Register TagPrefix="message" Src="~/Controls/GenericMessage.ascx" TagName="GenericMessage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />

    <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap.js" type="text/javascript"></script>
    <script src="../script/dashboard/dashboardsetting.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('[data-toggle="tooltip"]').tooltip();

                $("#txtNewRefreshTime").keydown(function (e) {
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

                $('#btnCreate').click(function () {
                    openModal();
                    if ($('#txtNewRefreshTime').val() === '') {
                        $('#txtNewRefreshTime').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
                        $('#txtNewRefreshTime').attr('title', 'Please enter time in minutes');
                        closeModal();
                        return false;
                    } else {
                        $('#txtNewRefreshTime').closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
                        return true;
                    }
                });
            });
        </script>

</head>
<body>
    <form id="form1" class="form-horizontal" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>
    
    <div class="panel panel-primary" style="height: 400px;">
    <!-- Default panel contents -->
		<div class="panel-heading">Personalize Setting </div>
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
                    <label for="inputEmail3" class="col-sm-5 control-label" id="Label1">Current Refresh interval time :</label>
                    <div class="col-sm-4">
                        <label runat="server" id="lblCurrentRefreshTime" for="inputEmail3" class="control-label" ></label>
                        <asp:HiddenField ID="hidIsDataUpdated" runat="server" />
                    </div>
                </div>

                <div class="form-group">
                    <label for="inputEmail3" class="col-sm-5 control-label" id="updatedText">Last updated time :</label>
                    <div class="col-sm-4">
                        <label for="inputEmail3" class="control-label" runat="server" id="lblLastUpdatedTime"></label>
                    </div>
                </div>

                <div class="form-group">
                    <label for="inputEmail3" class="col-sm-5 control-label" id="lblLocation">New Refresh interval time (in min) :</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtNewRefreshTime" runat="server" MaxLength="3" class="form-control" data-toggle="tooltip" data-placement="bottom" autofocus></asp:TextBox>
                    </div>
                </div>
                <hr class="h-line" />
                <div class="form-group">
                    <label for="inputEmail3" class="col-sm-5 control-label" id="Label2">Display Order :</label>
                    <div class="col-sm-4">
                        <asp:ListBox ID="lstEnvironment" runat="server" class="form-control" Rows="6" data-toggle="tooltip" data-placement="bottom" SelectionMode="Single" ClientIDMode="Static">
                            
                        </asp:ListBox>
                    </div>
                    <div class="col-sm-1" style="vertical-align: middle">
                        <div style="height:18px"></div>
                        <button type="button" class="btn btn-primary" id="btnUp" data-toggle="tooltip" data-placement="up" title="Move Up" runat="server" OnServerClick="btnUp_Click" onclick="return ValidateSelection(this);">
                            <span class="glyphicon glyphicon-arrow-up"></span>
                        </button>
                        <div style="height:8px"></div>
                        <button type="button" class="btn btn-primary" id="btnDown" data-toggle="tooltip" data-placement="bottom" title="Move Down" runat="server" OnServerClick="btnDown_Click" onclick="return ValidateSelection(this);">
                            <span class="glyphicon glyphicon-arrow-down"></span>
                        </button>
                    </div>
                </div>
                <p>&nbsp;</p>
                <hr class="h-line" />
                <div class="form-group text-center">
                  <div > <%--class="col-sm-offset-2 col-sm-10"--%>
                    <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-primary" 
                            Text="Save" OnClick="btnCreate_Click" />
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

    </form>

    <script src="../script/spinning.js" type="text/javascript"></script>
    </body>
</html>
