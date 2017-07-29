<%@ Page Title="Cosmo - Notification" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="EnvironmentEmail.aspx.cs" Inherits="Cosmo.Web.forms.EnvironmentEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap" ng-cloak>
		<h3 class="top-space">NOTIFICATION</h3>
		<div style="height:8px"></div>
        <div class="panel panel-primary">
		  <!-- Default panel contents -->
		  <div class="panel-heading">Distribution List</div>
		  <div class="panel-body" ><!--style="background-color:#e3eef8"-->
            <p>&nbsp;</p>  
            <div id="divErrorMessage" class="" ng-show="errorMessage != ''">
                <span class="glyphicon glyphicon-info-sign"></span>
                {{errorMessage}}
            </div>
		    <table class="table table-bordered inner-border table-hover" >
			    <thead>
				    <tr class="">
					    <th class=""  >#</th>
					    <th class=""  >
					        <a href="#" ng-click="sortType = 'EnvName'; sortReverse = !sortReverse;" onclick="return false;">
                            Environment
                            <span ng-show="sortType == 'EnvName' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'EnvName' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
					    </th>
                        <th class=""  >
					        <a href="#" ng-click="sortType = 'ToAddress'; sortReverse = !sortReverse;" onclick="return false;">
                            To Address
                            <span ng-show="sortType == 'ToAddress' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'ToAddress' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
                        </th>
                        <th class=""  >
					        <a href="#" ng-click="sortType = 'CcAddress'; sortReverse = !sortReverse;" onclick="return false;">
                            CC Address
                            <span ng-show="sortType == 'CcAddress' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'CcAddress' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
                        </th>
                        <th class=""  >
					        <a href="#" ng-click="sortType = 'BccAddress'; sortReverse = !sortReverse;" onclick="return false;">
                            BCC Address
                            <span ng-show="sortType == 'BccAddress' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'BccAddress' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
                            
                        </th>
                        <th class=""  ></th>
				    </tr> 
			    </thead>
			    <tbody>
				    <tr ng-repeat="environment in environmentList | orderBy:sortType:sortReverse">
					    <%--<td class="borderless-td" id="row_{{environment.EnvID}}" colspan="10"></td>--%>
    					<td class=""  >{{$index + 1}}</td>
					    <td class=""  >{{environment.EnvName}}</td>
					    <td class=""  >{{environment.ToAddress}}</td>
					    <td class=""  >{{environment.CcAddress}}</td>
					    <td class=""  >{{environment.BccAddress}}</td>
                        <td class="text-center"><a href="#" ng-click="EditEnvironmentEmailConfiguration(environment.EnvID,'c')"><span class="rowedit" title="edit" data-toggle="tooltip" data-placement="bottom"></span></a></td>
				    </tr>
			    </tbody>
		    </table>
		  </div>
		</div>        
        <p>&nbsp;</p>
    </div>
    
    <script src="../script/environment/email_operation.js" type="text/javascript"></script>
    <script src="../script/environment/appEmails.js" type="text/javascript"></script>	
    <script src="../script/simple_modal.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {

        });
     </script>
    
</asp:Content>
