<%@ Page Title="Cosmo - URL Configuration" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="UrlConfiguration.aspx.cs" Inherits="Cosmo.Web.forms.UrlConfiguration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap" ng-cloak>
		<h3 class="top-space">URL Configuration
                      <a href="#" ng-click="EditEnvironmentConfiguration(null,'n');" onclick="return false;">
                          <img src="../images/add-circle-32.png" width="28" alt="Create" title="Setup new URL configuration" data-toggle="tooltip" data-placement="top"/>
                      </a>
        </h3>
		<div style="height:8px"></div>
        <div class="panel panel-primary" id="main">
		  <!-- Default panel contents -->
		  <div class="panel-heading">List of URL</div>
		  <div class="panel-body" ><!--style="background-color:#e3eef8"-->
              <%--<div class="row">
                  <div class="col-md-2">
                      <div class="form-group">
                    <button class="btn btn-primary" ng-click="EditEnvironmentConfiguration(null,'n');" onclick="return false;" title="Setup new environment or service" data-toggle="tooltip" data-placement="right">Create</button>
                      </div> 
                  </div>
              </div>--%>
              <div class="row">
                  <div class="col-sm-1" style="white-space:nowrap">
                      <div style="height: 10px" ng-show="errorMessage === ''">
                      </div>
                  </div>
                  <div class="col-sm-1">
                  </div>
                  <div class="col-sm-10 text-right">
                    <!--Cosmo service status - start-->
                    <div>
                        <p class="text-right legend">&nbsp;</p>
                    </div>
                    <!--Cosmo service status - end-->
                  </div>
              </div>


            <div id="divErrorMessage" class="" ng-show="errorMessage != ''">
                <span class="glyphicon glyphicon-info-sign"></span>
                {{errorMessage}}
            </div>
			<table class="table table-bordered inner-border  table-hover">
				<thead >
					<tr >
						<th class="text-center" style="width: 10px">#</th>
						<th class="col-md-2">
							<a href="#" ng-click="sortType = 'EnvName'; sortReverse = !sortReverse;" onclick="return false;">
							Environment
                            <span ng-show="sortType == 'EnvName' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'EnvName' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
						<th class="col-md-1" nowrap="nowrap">
							<a href="#" ng-click="sortType = 'Type'; sortReverse = !sortReverse;" onclick="return false;">
							URL Type
                            <span ng-show="sortType == 'Type' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'Type' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
						<th class="col-sm-4" nowrap="nowrap">
							<a href="#" ng-click="sortType = 'Adress'; sortReverse = !sortReverse;" onclick="return false;">
							URL Address
                            <span ng-show="sortType == 'Adress' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'Adress' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
						<th class="col-md-2" nowrap="nowrap">
							<a href="#" ng-click="sortType = 'DisplayName'; sortReverse = !sortReverse;" onclick="return false;">
							Display Name
                            <span ng-show="sortType == 'DisplayName' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'DisplayName' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
						<th class="col-md-2">
							<a href="#" ng-click="sortType = 'MatchContent'; sortReverse = !sortReverse;" onclick="return false;">
							Match Content
                            <span ng-show="sortType == 'MatchContent' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'MatchContent' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
						<th class="col-md-1" nowrap="nowrap">
							<a href="#" ng-click="sortType = 'Interval'; sortReverse = !sortReverse;" onclick="return false;">
							Interval
                            <span ng-show="sortType == 'Interval' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'Interval' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
						<th class="col-md-1" nowrap="nowrap">
							<a href="#" ng-click="sortType = 'Status'; sortReverse = !sortReverse;" onclick="return false;">
							Status
                            <span ng-show="sortType == 'Status' && !sortReverse" class="fa fa-caret-down"></span>
                            <span ng-show="sortType == 'Status' && sortReverse" class="fa fa-caret-up"></span>
                            </a>
						</th>
                        <th style="width:50px"></th>
                        <th style="width:50px"></th>
					</tr>
				</thead>
				<tbody>
					<tr ng-repeat="entity in environmentList | orderBy:sortType:sortReverse">
						<td class="text-center">{{$index + 1}}</td>
						<td  nowrap="nowrap">{{entity.EnvName}}</td>
						<td  nowrap="nowrap">{{entity.Type}}</td>
						<td>{{entity.Adress | lowercase}}</td>
						<td class="">{{entity.DisplayName}}</td>
						<td nowrap="nowrap">{{entity.MatchContent}}</td>
						<td class="text-center">{{entity.Interval}}</td>
                        <td class="text-center"><img ng-src="../images/{{entity.Status && '15-Tick-icon.png' || 'false_icon.png'}}"/></td>
                        <td class="text-center"><a href="#" ng-click="EditEnvironmentConfiguration(entity,'c')"><span class="rowedit" title="edit" data-toggle="tooltip" data-placement="bottom"></span></a></td>
                        <td class="text-center"><a href="#" ng-click="DeleteEnvironmentConfiguration(entity)"><span class="rowdelete" title="delete" data-toggle="tooltip" data-placement="bottom"></span></a></td>
					</tr>
				</tbody>
			</table>							
		    
		  </div>
		</div>        
        <p>&nbsp;</p>
    </div>
    
    <script src="../script/environment/urlconfiguration_operation.js" type="text/javascript"></script>
    <script src="../script/environment/appUrlConfiguration.js" type="text/javascript"></script>	
    <script src="../script/simple_modal.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

        });
     </script>

        
</asp:Content>
