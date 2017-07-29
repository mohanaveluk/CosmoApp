<%@ Page Title="" Language="C#" MasterPageFile="~/Setup.Master" AutoEventWireup="true" CodeBehind="MailServerConfig.aspx.cs" Inherits="Cosmo.Web.setup.MailServerConfig" %>
<%@ Register  TagPrefix="Mail" TagName="MialServerConfig" src="~/controls/MailServer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="text-right text-danger" ><label runat="server" ID="lblExpiry"></label></div>
    <mail:MialServerConfig ID="mialServerConfig" runat="server"></mail:MialServerConfig>

</asp:Content>
