<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditEmailConfig.aspx.cs" Inherits="Cosmo.Web.forms.EditEmailConfig" %>
<%@ Register TagPrefix="message" Namespace="Cosmo.Web.controls" Assembly="Cosmo.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cosmo - Edit Environment Email Configuration</title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />

    <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
    <script src="../style/js/moment.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../script/environment/email_operation.js" type="text/javascript"></script>
    <script src="../script/spinning.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" class="form-horizontal"  runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" ></asp:ScriptManager>
        <div class="panel panel-primary" style="height: 400px;">
        <!-- Default panel contents -->
		    <div class="panel-heading">Configure Environment Email</div>
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
                        <label for="inputEmail3" class="col-sm-3 control-label" id="Label1" style="margin-left: -15px">Environment:</label>
                        <div class="col-sm-5">
                            <label runat="server" id="lblEnvironmentName" for="inputEmail3" class="control-label" ></label>
                            <asp:HiddenField ID="hidIsDataUpdated" runat="server" />
                        </div>
                    </div>
                    
                    <div class="row inline">
                        <div class="col-sm-offset-1 col-sm-6">
                            <div class="form-group">
                                <label style="width: 200px" for="EnvConfig">Email Address</label>
                                <asp:RadioButton runat="server" ID="rdoEmail" GroupName="rdoMessageType" Checked="True"  /> Email
                                &nbsp;&nbsp;    
                                <asp:RadioButton runat="server" ID="rdoText" GroupName="rdoMessageType" /> Text
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email Address" Width="97%"></asp:TextBox>
                                <asp:HiddenField ID="hidEnvironment" runat="server" />
                            </div>                                                       
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <label for="EnvConfig">Type</label>
                                    <asp:DropDownList runat="server" ID="drpEmailType" CssClass="form-control" Width="90%">
                                        <asp:ListItem Text="To" Value="To"></asp:ListItem>
                                        <asp:ListItem Text="Cc" Value="Cc"></asp:ListItem>
                                        <asp:ListItem Text="Bcc" Value="Bcc"></asp:ListItem>
                                    </asp:DropDownList>
                            </div>                                                       
                        </div>
                        <div class="col-sm-3">
                            <div class="form-group">
                                <label for="EnvConfig">&nbsp;</label><br/>
                                <asp:Button ID="btnAddEmail" runat="server" CssClass="btn btn-primary" Text="Add"
                                    OnClick="btnAddEmail_Click" />
                                <asp:Button ID="btnCancelUpdate" runat="server" CssClass="btn btn-primary"
                                    Text="Cancel" OnClientClick="return clearEditDetails()" />
                                <asp:HiddenField ID="hidUserEmailID" runat="server" />
                                <asp:HiddenField ID="HiddenField1" runat="server" />
                                <asp:HiddenField ID="hidSchedulerChanged" runat="server" />
                                <asp:HiddenField ID="hidCreateMode" runat="server" />
                            </div>                                                       
                        </div>
                    </div>
                    <div class="row inline">
                        
                    </div>
                    <hr class="h-line" />
                    <div class="form-group">
                        <div class="col-sm-10 col-sm-offset-1" style="height: 300px">
                            <asp:Repeater ID="rptEmaiList" runat="server" OnItemCommand="rptEmaiList_ItemCommand"
                                OnItemDataBound="rptEmaiList_ItemDataBound">
                                <HeaderTemplate>
                                    <table class="table table-bordered table-hover" style="margin-left: -18px" id="TableEmail">
                                        <thead>
                                            <tr>
                                                <th class="col-sm-1">#</th>
                                                <th>Email Address</th>
                                                <th class="col-sm-1">Mode</th>
                                                <th class="col-sm-1">Type</th>
                                                <th class="col-sm-1">Edit</th>
                                                <th class="col-sm-1">Delete</th>
                                                <th class="col-sm-1" style="display:none">userEmailId</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td ><asp:Label ID="d_lblSrNo" runat="server" /><asp:HiddenField ID="hidEmail" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"UserListID")%>' EnableViewState="false" /></td>
                                        <td ><asp:Label ID="d_lblEmailAddress" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"EmailAddress")%>' /></td>
                                        <td ><asp:Label ID="d_lblMessageType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"MessageType")%>' /></td>
                                        <td class="text-center"><asp:Label ID="d_lblEmailType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"UserListType")%>' /></td>
                                        <td class="text-center"><asp:HyperLink ID="lnkEnvEdit" runat="server"><span class="rowedit"></span></asp:HyperLink></td>
                                        <td class="text-center"><asp:LinkButton ID="lnkEnvDel" runat="server" Text="ASA"><span class="rowdelete"></span></asp:LinkButton></td>
                                        <td  style="display:none"><asp:Label ID="lblUserEmailID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"UserListID")%>' /></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>


                    <hr class="h-line" />
                    <div class="form-group text-center">
                      <div > <%--class="col-sm-offset-2 col-sm-10"--%>

                        <button type="button" class="btn btn-primary" id="btnCancel" onclick="fnCancelEmail(this)" >Close</button>
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
    </form>
</body>
</html>
