<%@ Page Title="Cosmo - Register User" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="RegisterUser.aspx.cs" Inherits="Cosmo.Web.forms.RegisterUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/dirPagination.js" type="text/javascript"></script>
    <script src="../script/registeruser/appRegister.js" type="text/javascript"></script>
    <script src="../script/registeruser/password.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="RegisterApp" ng-controller="RegisterController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">
            REGISTER USER</h3>
        <div style="height: 8px">
        </div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">
                User Details</div>
            <div class="panel-body">
                <form class="form-horizontal" method="post" role="form" name="registerForm" novalidate>
                <div class="form-group" ng-class="{ 'has-error': registerForm.txtFirstName.$invalid && !registerForm.txtFirstName.$pristine}">
                    <input class="form-control" type="hidden" id="hidUserId" name="hidUserId" ng-model="userID" />
                    <label for="inputName3" class="col-sm-2 control-label">
                        First Name</label>
                    <div class="col-xs-4">
                        <input class="form-control" id="txtFirstName" name="txtFirstName" placeholder="First name"
                            ng-model="FirstName" minlength="2" maxlength="100" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtFirstName.$dirty && registerForm.txtFirstName.$error"
                        ng-show="registerForm.txtFirstName.$touched">
                        <div ng-messages-include="error/messages.htm">
                        </div>
                    </div>
                </div>
                <div class="form-group" ng-class="{ 'has-error': registerForm.txtLastName.$invalid && !registerForm.txtLastName.$pristine}">
                    <label for="inputName3" class="col-sm-2 control-label">
                        Last Name</label>
                    <div class="col-xs-4">
                        <input class="form-control" id="txtLastName" name="txtLastName" placeholder="Last name"
                            ng-model="LastName" minlength="2" maxlength="100" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtLastName.$dirty && registerForm.txtLastName.$error"
                        ng-show="registerForm.txtLastName.$touched">
                        <div ng-messages-include="error/messages.htm">
                        </div>
                    </div>
                </div>
                <div class="form-group" ng-class="{ 'has-error': registerForm.txtEmail.$invalid && !registerForm.txtEmail.$pristine }">
                    <label for="inputName3" class="col-sm-2 control-label">
                        Email Address</label>
                    <div class="col-sm-4">
                        <input class="form-control" id="txtEmail" name="txtEmail" placeholder="Email Address will be a Cosmo username"
                            type="email" ng-model="email" maxlength="100" required />
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtEmail.$dirty && registerForm.txtEmail.$error"
                        ng-show="registerForm.txtEmail.$touched">
                        <div ng-messages-include="error/messages.htm">
                        </div>
                    </div>
                </div>
                <div class="form-group" ng-class="{ 'has-error': registerForm.txtPassword.$invalid && !registerForm.txtPassword.$pristine }">
                    <label for="inputName3" class="col-sm-2 control-label">
                        Password</label>
                    <div class="col-sm-4">
                        <input class="form-control" type="password" id="txtPassword" name="txtPassword" placeholder="Password"
                            ng-model="password" minlength="8" maxlength="20" required  pattern="(?=^.{8,}$)(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9])(?=.*[!@#$%&*]).*" /> <%--data-toggle="popover" data-trigger="focus" data-placement="right" data-content="1. Minimum 8 Charecters" data-container="body"--%>
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.txtPassword.$dirty && registerForm.txtPassword.$error"
                        ng-show="registerForm.txtPassword.$touched" id="divPasswordError">
                        <div ng-messages-include="Error/MessagesPassword.htm">
                        </div>
                    </div>
                </div>
                <div class="form-group" ng-class="{ 'has-error': registerForm.hidRoles.$invalid && !registerForm.txtPassword.$pristine}">
                    <label for="inputName3" class="col-sm-2 control-label">
                        Role</label>
                    <div class="col-sm-4">
                        <div class="checkbox" ng-repeat="role in Roles">
                            <label class="check-disabled-false">
                                <input type="checkbox" id="{{role.name}}" name="chkRole" value="{{role.value}}" ng-click="roleToggleSelection(role)"
                                    ng-disabled="DisableRole(role)" ng-model="role.selected" /><!--ng-required="!someSelected(selectedRoles)"-->
                                {{role.label }}
                            </label>
                        </div>
                        <input type="hidden" name="hidRoles" id="hidRoles" ng-model="selectedRolesString"
                            required />
                        <!--value="{{selectedRoles}}" -->
                    </div>
                    <div class="col-xs-offset-2 help-block" ng-messages="registerForm.hidRoles.$error"
                        ng-show="registerForm.hidRoles.$error && registerForm.txtPassword.$touched">
                        <div ng-message="required">
                            Please select at least one Role</div>
                    </div>
                </div>
                <div class="form-group">
                    <label for="inputName3" class="col-sm-2 control-label">
                        Status</label>
                    <div class="col-sm-4">
                        <%--<label class="radio-inline">
                                  <input type="radio" name="rdoUserStatus" ng-model="user.status" id="rdoStatusActive" /> Active
                                </label>
                                <label class="radio-inline">
                                  <input type="radio" name="rdoUserStatus" ng-model="user.status" id="rdoStatusInactive" /> Inactive
                                </label>--%>
                        <label class="radio-inline" ng-repeat="user in userStatus">
                            <input type="radio" name="rdoUserStatus" ng-model="selectedStatus" ng-click="setDefault(user)"
                                ng-value="user" id="rdoStatusInActive" ng-selected="user.selected" />
                            {{user.option}}
                        </label>
                        <span style="display: none">{{selectedStatus}}</span>
                    </div>
                </div>
                <hr class="h-line"/>
                <div class="form-group">
                    <div class="col-sm-offset-4 col-sm-8 ">
                        <button type="submit" class="btn btn-primary" ng-disabled="registerForm.$invalid"
                            ng-click="addRowAsyncAsJSON()">
                            Submit</button>
                        <button type="button" ng-click="ClearModels()" class="btn btn-default">
                            Clear</button>
                        <span class="label label-success" style="font-size: 1.2em">{{message}}</span>
                    </div>
                </div>
                
                <!--<div class="help-block" role="alert"> sdsd</div>-->
                <div class="form-group">
                
                    <h4><b>Available Users</b></h4>
                    <table class="table table-bordered table-striped" >
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Email</th>
                            <th>First Name</th>
                            <th>Last Name</th>
                            <th>Roles</th>
                            <th>Status</th>
                            <th></th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr dir-paginate="user in userList | itemsPerPage:pageSize" current-page="currentPage">
                            <td>{{$index + 1}}</td>
                            <td>{{user.Email}}</td>
                            <td>{{user.FirstName}}</td>
                            <td>{{user.LastName}}</td>
                            <td>
                                <li ng-repeat="userRole in user.UserRoles" style="margin-left:10px">{{userRole.RoleName}}</li>
                            </td>
                            <td>{{user.IsActive == true ? 'Active' : 'Inactive'}}</td>
                            <td class="text-center" style="vertical-align: middle"><a href="#" ng-click="edituser(user.UserID)"><img src="../images/edit.png" title="edit" alt="edit"/></a></td>
                            <td class="text-center" style="vertical-align: middle"><a href="#" ng-click="deleteuser(user.UserID,user.Email)"><img src="../images/remove.jpg" alt="delete" title="delete" /></a> </td>
                        </tr>
                    </tbody>
                </table>
                    <div class="text-center">
                                <dir-pagination-controls boundary-links="true"  template-url="../Forms/Error/dirPagination.tpl.html" ></dir-pagination-controls>
                    </div>
                </div>
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

    <script src="../script/spinning.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function() {
            //$('#txtPassword').popover({ trigger: 'focus', title: 'Twitter Bootstrap Popover', content: "It's so simple to create a tooltop for my website!" });
            $('#txtPassword').popover({
                html: true,
                container: 'body',
                trigger: 'focus',
                title: function() {
                    return $('#divPasswordGuideHead').html();
                },
                content: function() {
                    return $('#divPasswordGuideContent').html();
                }
            });
            $('#txtFirstName').focus();
        });
     </script>

</asp:Content>
