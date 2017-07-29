<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GenericMessage.ascx.cs" Inherits="Cosmo.Web.controls.GenericMessage" %>
<table id="LbTable" cellspacing="0" cellpadding="2" width="100%" align="center" border="0" class="alert alert-warning">
    <tr>
        <td valign="middle" align="center">
            <asp:Table ID="tblMessage" runat="server" Visible="False" CellPadding="0" CssClass="message">
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Image ID="imgMessage" runat="server" ImageUrl="~/images/alert.gif" Visible="False"
                            Width="32px" Height="32px" />&nbsp;&nbsp;
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" HorizontalAlign="Center" VerticalAlign="Middle" runat="server">
                        <asp:Label ID="lblStatus" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
    </tr>
</table>
<%--<div class="alert alert-danger" role="alert">
    <asp:Image ID="imgMessage" runat="server" ImageUrl="~/images/alert.gif" Visible="False"
                            Width="32px" Height="32px" />&nbsp;&nbsp;
    <asp:Label ID="lblStatus" runat="server" />
</div>--%>