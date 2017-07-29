<%@ Page Title="Page Not Found" Language="C#" MasterPageFile="~/Setup.Master" AutoEventWireup="true" CodeBehind="PN404.aspx.cs" Inherits="Cosmo.Web.error.PN404" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
        <!-- Begin page content -->
	<h3 class="top-space">&nbsp;</h3>
    <div class="container-fluid body-panel-setup">
		<div style="height:8px"></div>
		<div class="panel panel-primary">
		  <!-- Default panel contents -->
		  <div class="panel-heading">Requested page is not found</div>
		  <div class="panel-body" ><!--style="background-color:#e3eef8"-->
            <div class="alert alert-info">
                <asp:Label runat="server" ID="lblEcrrorMessage"></asp:Label>
            </div>
                <p>&nbsp;</p>
            <div runat="server" id="divRedirect" class="error_content">
                Please contact adminstrator
            </div>
		  </div>
		</div>
    </div> 

</asp:Content>
