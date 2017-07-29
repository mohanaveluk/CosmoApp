<%@ Page Title="Error" Language="C#" MasterPageFile="~/Setup.Master" AutoEventWireup="true" CodeBehind="Oops.aspx.cs" Inherits="Cosmo.Web.error.Oops" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
        <!-- Begin page content -->
	<h3 class="top-space">&nbsp;</h3>
    <div class="container-fluid body-panel-setup">
		<div style="height:8px"></div>
		<div class="panel panel-primary">
		  <!-- Default panel contents -->
		  <div class="panel-heading">An error has occurred</div>
		  <div class="panel-body" ><!--style="background-color:#e3eef8"-->
            <div class="alert alert-danger">
                <asp:Label runat="server" ID="lblErrorMessage"></asp:Label>
            </div>
             <p>&nbsp;</p>
            <div runat="server" id="divRedirect" class="error_content">
                Click <a href="../setup/DBConfig.aspx">here</a> to change / modify the database configuration or try <a href="../login/Default.aspx">login</a> again
            </div>
		  </div>
		</div>
    </div> 

</asp:Content>
