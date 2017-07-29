<%@ Page Title="Cosmo - Login" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Cosmo.Web.login.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/loginuser/appLogin.js" type="text/javascript"></script>
    <script src="../script/jquery.cookie.js" type="text/javascript"></script>

    <script>
        $(document).ready(function () {
            $('#txtUserName').focus();
            getRemember();
            showProductExpiry();
        });

        function showProductExpiry() {
            if ($('#<%=hidLicenseStatus.ClientID %>').val() === "Failure") {
                $('#myModal').modal();
            }
        }


    </script>   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <p>&nbsp;</p>
        <div class="text-right text-danger" ><label runat="server" ID="lblExpiry"></label></div>
        <!-- Begin page content -->
        <div class="container-fluid body-panel-login" ng-app="LoginApp" ng-controller="LoginController" id="MainWrap"  ng-cloak>
		    <div style="height:8px"></div>
		    <div class="panel panel-primary">
		      <!-- Default panel contents -->
		      <div class="panel-heading">Login</div>
		      <div class="panel-body" >
		          <form name="loginForm" class="form-horizontal" enctype="multipart/form-data" method="post"  ng-submit="CheckLogin()" novalidate >
                    <div style="background-color:#fff;">
                        <div ng-messages="loginForm.txtPassword.$error" role="alert">
                            <div ng-class="loginError.indexOf('success') <= 0 ? 'alert alert-danger': 'alert alert-success'" ng-if="loginError != ''">
                                <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>
                                <span class="sr-only">Error:</span>
                                {{loginError}}
                            </div>
                        </div>                  
                        </div>          


                    <div class="input-group" >
                        <span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span>
                        <input id="txtUserName" type="email" 
                        class="form-control" 
                        name="txtUserName" 
                        ng-model="UserName" 
                        placeholder="Email" 
                        minlength="5"
                        required 
                        />
                    </div>
                    <div style="background-color:#fff; height:30px">
                        <div ng-messages="loginForm.txtUserName.$dirty && loginForm.txtUserName.$error" role="alert" ng-show="loginForm.txtUserName.$touched">
                            <div ng-message="minlength" class="has-error">
                                <span class="help-block"> Your email must be min of 5 char</span>
                            </div>
                            <div ng-message="email" class="has-error"><span class="help-block">Your email address is invalid</span></div>
                        </div>  
                    </div>
                                
                    <div class="input-group">
                        <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                        <input id="txtPassword" type="password" class="form-control" name="txtPassword" ng-model="userPassword" placeholder="Password" minlength="8" required />
                    </div> 
                    <div style="background-color:#fff; height:30px">
                        <div ng-messages="loginForm.txtPassword.$dirty && loginForm.txtPassword.$error" role="alert" ng-show="loginForm.txtPassword.$touched">
                            <div ng-message="minlength" class="has-error">
                                <span class="help-block"> Your password must be min of 8 char</span>
                            </div>
                        </div>  
                    </div>

                    <div class="checkbox">
                        <label>
                            &nbsp;<input type="checkbox" id="chkRememberMe" name="chkRememberMe" ng-model="chkRememberMe"/> Remember me
                        </label>
                    </div>


                    <div class="form-group">
                        <!-- Button -->
                        <div class="col-sm-12">
                            <button type="submit" ng-disabled="loginForm.$invalid" class="btn btn-primary pull-right button-text-adjust"><i class="glyphicon glyphicon-log-in"></i>&nbsp; Log in</button>                          
                            <input type="hidden" runat="server" id="hidLicenseStatus" name="hidLicenseStatus" ng-model="LicenseStatus"  />
                        </div>
                    </div>

                    <!--<a href="#">Forgot password?</a>-->
                </form>
		      </div>
		    </div>
        </div> 
        <div class="modal fade bs-example-modal-lg" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel"
        id="myModal">
            <div class="modal-dialog modal-lg" style="width: 50%">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">
                            Product Status</h4>
                    </div>
                    <div class="modal-body">

                        <span id="lblLicStatus" runat="server"></span>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">
                            Close</button>
                    </div>
                </div>
            </div>
        </div>
    <!-- End of page content -->     
</asp:Content>
