<%@ Page Title="Database Configuration" Language="C#" MasterPageFile="~/Setup.Master" AutoEventWireup="true" CodeBehind="DBConfig.aspx.cs" Inherits="Cosmo.Web.setup.DBConfig" %>
<%@ Register TagPrefix="message" Src="~/controls/GenericMessage.ascx" TagName="GenericMessage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
<%--    <link rel="stylesheet" href="http://cdnjs.cloudflare.com/ajax/libs/jquery.bootstrapvalidator/0.5.3/css/bootstrapValidator.min.css"/>
    <script type="text/javascript" src="http://cdnjs.cloudflare.com/ajax/libs/jquery.bootstrapvalidator/0.5.3/js/bootstrapValidator.min.js"> </script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server" ></asp:ScriptManager>
    <div class="text-right text-danger" ><label runat="server" ID="lblExpiry"></label></div>
       
        <!-- Begin page content -->
        <div class="container-fluid body-panel-setup">
		    <div style="height:8px"></div>
		    <div class="panel panel-primary">
		      <!-- Default panel contents -->
		      <div class="panel-heading">Database Configuration</div>
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
                    <label for="inputEmail3" class="col-sm-3 control-label">Type</label>
                    <div class="col-sm-8">
                        <asp:DropDownList ID="drpDatabaseType" runat="server" CssClass="form-control" ClientIDMode="Static">
                            <asp:ListItem Value="1" Text="Oracle"></asp:ListItem>
                            <asp:ListItem Value="2" Text="SQL Server" Selected="True"></asp:ListItem>
                        </asp:DropDownList>  
                    </div>
                  </div>
                  <div class="form-group">
                    <label for="inputEmail3" class="col-sm-3 control-label">Server/Host</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtServerName"  runat="server" CssClass="form-control contr" ClientIDMode="Static" placeholder="Server/Host name"></asp:TextBox>
                    </div>
                  </div>

                  <div class="form-group hide" id="divPort">
                    <label for="inputEmail3" class="col-sm-3 control-label">Port</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtPort" runat="server" CssClass="form-control contr" ClientIDMode="Static" placeholder="Server port" MaxLength="6"></asp:TextBox>
                    </div>
                  </div>

                  <div class="form-group" id="divAuthentication">
                    <label for="inputEmail3" class="col-sm-3 control-label">Authentication</label>
                    <div class="col-sm-8">
                        <asp:DropDownList ID="drpAuthenticationType" runat="server" CssClass="form-control" ClientIDMode="Static">
                            <asp:ListItem Value="1" Text="Windows Authentication"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Sql Server Authentication" Selected></asp:ListItem>
                        </asp:DropDownList>  
                    </div>
                  </div>
                  
                  <div class="form-group">
                    <label for="inputEmail3" class="col-sm-3 control-label" CssClass="form-control" >Database/SID</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtDatabaseName"  CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="Database/Service Identifier"></asp:TextBox>
                    </div>
                  </div>
                  
                  <div class="form-group">
                    <label for="inputEmail3" class="col-sm-3 control-label" CssClass="form-control" >Username</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtUserID" CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="Username"></asp:TextBox>
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="inputEmail3" class="col-sm-3 control-label" CssClass="form-control" >Password</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtPasswrd" CssClass="form-control" TextMode="Password" runat="server" ClientIDMode="Static" placeholder="Password"></asp:TextBox>
                    </div>
                  </div>

                    <div class="form-group">
                        <div class="col-sm-offset-3 col-sm-8">
                            <div class="checkbox">
                            <label style="margin-bottom:-4px">
                                <input type="checkbox" ID="chkReolace" checked="True" runat="server"/>
                            </label> Load / Replace Repository?
                            </div>
                        </div>
                    </div>
                    

                    <div class="col-md-12 text-center" >
                        <asp:Button ID="frmSubmit" runat="server" CssClass="btn btn-primary" 
                                    Text="Connect"  ClientIDMode="Static" onclick="btnCreate_Click" />
                    </div>

                </div>
		      </div>
		    </div>
        </div> 
        <!-- End of page content -->
        
    <script src="../script/dbconfig/dbconfig_validations.js" type="text/javascript"></script>
</asp:Content>
