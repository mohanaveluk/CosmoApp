<%@ Page Title="Cosmo - Mail server Configuration" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="Smtp.aspx.cs" Inherits="Cosmo.Web.forms.Smtp" %>
<%@ Register TagPrefix="mail" TagName="MialServerConfig_1" Src="~/controls/MailServer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" >
        <h3 class="top-space">MAIL SERVER CONFIGURATION</h3>
        <div style="height: 8px">
        </div>
        <form id="form1" runat="server">
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">
                Mail server Details</div>
            <div class="panel-body">
                <div class="text-right text-danger" ><label runat="server" ID="Label1"></label></div>
                <mail:MialServerConfig_1 ID="MialServerConfig_1" runat="server"></mail:MialServerConfig_1>
            </div>
        </div>
        </form>
        <p>&nbsp;</p>
    </div>
    
     <script type="text/javascript" language="javascript">
         $(document).ready(function () {
             $('#divPanelHeading').attr("style", "display:none");
             $('.body-panel-setup').attr("style", "margin-left:60px");
         });
     </script>

</asp:Content>
