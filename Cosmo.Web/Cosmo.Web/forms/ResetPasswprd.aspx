<%@ Page Title="Cosmo - Edit Profile" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="ResetPasswprd.aspx.cs" Inherits="Cosmo.Web.forms.ResetPasswprd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/registeruser/appEditUser.js" type="text/javascript"></script>
    <script src="../script/registeruser/password.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="RegisterApp" ng-controller="RegisterController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">EDIT PROFILE</h3>
        <div style="height: 8px">
        </div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">Reset Password</div>
            <div class="panel-body">
                <form class="form-horizontal" method="post" role="form" id="registerForm" name="registerForm" novalidate >
                  
                    <div class="form-group" ng-class="{ 'has-error': registerForm.txtFirstName.$invalid && !registerForm.txtFirstName.$pristine}">
                    <input class="form-control" type="hidden" id="hidUserId" name="hidUserId" ng-model="userId" value="<%=Convert.ToString(HttpContext.Current.Session["_LOGGED_USERD_ID"])%>"/>      
                            
                    <label for="inputName3" class="col-sm-2 control-label">First Name</label>
                    <div class="col-xs-4">
                        <input disabled class="form-control" id="txtFirstName" name="txtFirstName" placeholder="First name" ng-model="FirstName" minlength="2" maxlength="100" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtFirstName.$dirty && registerForm.txtFirstName.$error" ng-show="registerForm.txtFirstName.$touched">
                        <div ng-messages-include="Error/messages.htm"></div>
                    </div>
                    </div>

                    <div class="form-group" ng-class="{ 'has-error': registerForm.txtLastName.$invalid && !registerForm.txtLastName.$pristine}">
                    <label for="inputName3" class="col-sm-2 control-label">Last Name</label>
                    <div class="col-xs-4">
                        <input disabled class="form-control" id="txtLastName" name="txtLastName" placeholder="Last name" ng-model="LastName" minlength="2" maxlength="100" required/>
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtLastName.$dirty && registerForm.txtLastName.$error" ng-show="registerForm.txtLastName.$touched">
                        <div ng-messages-include="Error/messages.htm"></div>
                    </div>
                    </div>

                    <div class="form-group" ng-class="{ 'has-error': registerForm.txtEmail.$invalid && !registerForm.txtEmail.$pristine }">
                    <label for="inputName3" class="col-sm-2 control-label">Email Address</label>
                    <div class="col-sm-4">
                        <input disabled class="form-control" id="txtEmail" name="txtEmail" placeholder="Email Address" type="email" ng-model="email" maxlength="100" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtEmail.$dirty && registerForm.txtEmail.$error" ng-show="registerForm.txtEmail.$touched">
                        <div ng-messages-include="Error/messages.htm"></div>
                    </div>
                    </div>                    

                    <div class="form-group" ng-class="{ 'has-error': registerForm.txtCurrentPassword.$invalid && !registerForm.txtCurrentPassword.$pristine }">
                    <label for="inputName3" class="col-sm-2 control-label">Current Password</label>
                    <div class="col-sm-4">
                        <input class="form-control" type="password" id="txtCurrentPassword" name="txtCurrentPassword" placeholder="Current Password" ng-model="currentPassword" minlength="8" maxlength="20" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtCurrentPassword.$dirty && registerForm.txtCurrentPassword.$error" ng-show="registerForm.txtCurrentPassword.$touched">
                        <div ng-messages-include="Error/MessagesPassword.htm"></div>
                    </div>
                    </div>                    

                    <div class="form-group" ng-class="{ 'has-error': registerForm.txtNewPassword.$invalid && !registerForm.txtNewPassword.$pristine }">
                    <label for="inputName3" class="col-sm-2 control-label">New Password</label>
                    <div class="col-sm-4">
                        <input class="form-control" type="password" id="txtNewPassword" name="txtNewPassword" placeholder="New Password" ng-model="newPassword" minlength="8" maxlength="20" required  pattern="(?=^.{8,}$)(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9])(?=.*[!@#$%&*]).*"/>
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtNewPassword.$dirty && registerForm.txtNewPassword.$error" ng-show="registerForm.txtNewPassword.$touched" id="divPasswordError">
                        <div ng-messages-include="Error/MessagesPassword.htm"></div>
                    </div>
                    </div>                    

                    <div class="form-group" ng-class="{ 'has-error': registerForm.txtConfirmPassword.$invalid && !registerForm.txtConfirmPassword.$pristine }">
                    <label for="inputName3" class="col-sm-2 control-label">Confirm Password</label>
                    <div class="col-sm-4">
                        <input class="form-control" type="password" id="txtConfirmPassword" name="txtConfirmPassword" placeholder="Confirm Password" ng-model="confirmPassword" minlength="8" maxlength="20" compare-to="newPassword" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtConfirmPassword.$dirty && registerForm.txtConfirmPassword.$error" ng-show="registerForm.txtConfirmPassword.$touched">
                        <div ng-messages-include="Error/MessagesPassword.htm"></div>
                    </div>
                    </div>                    

                    <div class="form-group" ng-class="{ 'has-error': registerForm.hidRoles.$invalid && !registerForm.txtPassword.$pristine}">
                    <label for="inputName3" class="col-sm-2 control-label">Role</label>
                    <div class="col-sm-4" disabled>
                        <div class="checkbox" ng-repeat="role in Roles">
                            <label class="check-disabled-false">
                            <input type="checkbox" disabled id="{{role.name}}" name="chkRole" value="{{role.value}}" ng-model="role.selected" /><!--ng-required="!someSelected(selectedRoles)"-->
                            {{role.label }}
                            </label>
                        </div>
                        <input type="hidden" name="hidRoles" id="hidRoles" ng-model="selectedRolesString" required /> <!--value="{{selectedRoles}}" -->
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.hidRoles.$error" ng-show="registerForm.hidRoles.$error && registerForm.txtPassword.$touched">
                        <div ng-message="required">Please select at least one Role</div>
                    </div>
                    </div>                    
                          
                    <div class="form-group">
                        <label for="inputName3" class="col-sm-2 control-label">Status</label>
                        <div class="col-sm-4" disabled>
                            <%--<label class="radio-inline">
                                <input type="radio" name="rdoUserStatus" ng-model="user.status" id="rdoStatusActive" /> Active
                            </label>
                            <label class="radio-inline">
                                <input type="radio" name="rdoUserStatus" ng-model="user.status" id="rdoStatusInActive" /> Inactive
                            </label>--%>
                            <label class="radio-inline" ng-repeat="user in userStatus">
                                <input type="radio" disabled name="rdoUserStatus" ng-model="selectedStatus" ng-click="setDefault(user)"  ng-value="user" id="rdoStatusInActive" ng-selected="user.selected" /> {{user.option}} 
                            </label>
                            <span style="display:none">{{selectedStatus}}</span>
                        </div>
                    </div>
                    <hr class="h-line" />
                    <div class="form-group">
                        <div class="col-sm-offset-4 col-sm-8 ">
                            <button type="submit" class="btn btn-primary" ng-disabled="registerForm.$invalid" ng-click="editRowAsyncAsJSON()">Submit</button>
                            <button type="button" ng-click="ClearModels()" class="btn btn-default">Clear</button>
                            <span class="label label-success" style="font-size: 1.2em">{{message}}</span>
                        </div>
                    </div>
                          
                    <!--<div class="help-block" role="alert"> sdsd</div>-->
                </form>
            </div>
        </div>
        <p>&nbsp;</p>
    </div>
    <div id="divPasswordGuideHead" class="head hide" style="padding: 0px;"><b>Guidelines</b></div>
    <div id="divPasswordGuideContent" class="content hide">
        1. At least eight characters length<br/>
        2. One or more of each of the following:
        <ul>
            <li>lower-case letter</li>
            <li>upper-case letter</li>
            <li>number</li>
            <li>special character (!,@,#,$,%,&,*)</li>
        </ul>
        3. Should not contain the following
        <ul>
            <li>First Name</li>
            <li>Last Name</li>
            <li>Email id</li>
        </ul>
    </div>

    
    <script type="text/javascript" language="javascript">
        
            $(document).ready(function () {
                //$('#txtPassword').popover({ trigger: 'focus', title: 'Twitter Bootstrap Popover', content: "It's so simple to create a tooltop for my website!" });
                $('#txtNewPassword').popover({
                    html: true,
                    container: 'body',
                    trigger: 'focus',
                    title: function () {
                        return $('#divPasswordGuideHead').html();
                    },
                    content: function () {
                        return $('#divPasswordGuideContent').html();
                    }
                });
                $('#txtCurrentPassword').focus();
            });
        
    </script>    
</asp:Content>
