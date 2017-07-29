<%@ Page Title="Cosmo - Incident Tracking" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="Incident.aspx.cs" Inherits="Cosmo.Web.forms.Incident" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="DashboardApp" ng-controller="DashboardController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">INCIDENT TRACKING</h3>
        <div style="height: 8px"></div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">Incident Details</div>
            <div class="panel-body">
                <div class="row">
                  <div class="col-md-2">
                      <div style="height: 10px" ng-show="errorMessage === ''">
                        <a href="#" style="margin-left:10px;font-size: 1.1em" ng-click="setExpandCollapse()" id="linkExpand" onclick="return false;"><%--<img src="../images/collapseall.jpg" alt=""/>--%> 
                        <span class='glyphicon glyphicon-collapse-down'></span>
                        Collapse All</a>
                      </div>
                  </div>
                    
                </div>
                <p class="text-right legend">
                    <img alt="" src="../images/green_icon.jpg" title="Service running"/> Active / Ready &nbsp;&nbsp;
                    <img alt="" src="../images/orange_icon.jpg" title="service is in standby mode"/> Standby&nbsp;&nbsp;
                    <img alt="" src="../images/blue_icon.jpg" title="Dispatcher is not ready/running"/> Not Ready&nbsp;&nbsp;
                    <img alt="" src="../images/red1_icon.jpg" title="Stopped/down"/> Stopped&nbsp;&nbsp;
                    <img alt="" src="../images/red_icon.jpg" title="One or more services are stopped / down"/> One or more services are stopped&nbsp;&nbsp;
                    <img alt="" src="../images/gray_icon.png" title="One or more services are not available" data-toggle="tooltip" data-placement="bottom"/> One or more services are not available&nbsp;&nbsp;&nbsp;
                    <a href="#" id="printPage" title="Print"  data-toggle="tooltip" data-placement="top" onclick=""><img alt="Print" src="../images/print.png" /> Print</a>&nbsp;&nbsp;
                </p>
                <div class="alert alert-danger" ng-show="errorMessage != ''" style="display: none" id="divErrorMessage">
                    <span class="glyphicon glyphicon-info-sign"></span>
                    {{errorMessage}}
                </div>
                <form id="aspnetform" action="">
                    <div class="scrollPanelHeight">
		        <table class="table-environemnt"  ng-repeat="monitorEntity in buildDetails | orderBy:sortType:sortReverse">
			        <thead>
				        <tr>
					        <th ng-click="toggle_it(monitorEntity.EnvID)" id="arrowImg_{{monitorEntity.EnvID}}" class="borderless-td caret-extension" width="20px"><span class="glyphicon glyphicon-triangle-bottom"></span></th>
					        <th style="" class="borderless-td"  >{{monitorEntity.EnvName}}</th>
				        </tr>
			        </thead>
			        <tbody>
				        <tr>
					        <td class="borderless-td" id="row_{{monitorEntity.EnvID}}" colspan="2">
						        <table class="table table-bordered inner-border table-hover" id="envDetails_{{monitorEntity.EnvID}}">
							        <thead >
								        <tr >
									        <th class="text-center">#</th>
									        <th class="col-md-1">
									            <a href="#" ng-click="sortType = 'ConfigServiceType'; sortReverse = !sortReverse;" onclick="return false;">
									            Service Type
                                                <span ng-show="sortType == 'ConfigServiceType' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'ConfigServiceType' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-2" nowrap="nowrap">
									            <a href="#" ng-click="sortType = 'ConfigServiceDescription'; sortReverse = !sortReverse;" onclick="return false;">
                                                Description
                                                <span ng-show="sortType == 'ConfigServiceDescription' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'ConfigServiceDescription' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-2">
									            <a href="#" ng-click="sortType = 'MonitorCreatedDateTime'; sortReverse = !sortReverse;" onclick="return false;">
									            Last Monitor Time
                                                <span ng-show="sortType == 'MonitorCreatedDateTime' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'MonitorCreatedDateTime' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-1 text-center">
									            <a href="#" ng-click="sortType = 'MonitorStatusIcon'; sortReverse = !sortReverse;" onclick="return false;">
									            Status
                                                <span ng-show="sortType == 'MonitorStatusIcon' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'MonitorStatusIcon' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-3 text-center">Issue</th>
									        <th class="col-md-3 text-center">Resolution</th>
								            <th class="text-center"><div class="checkbox"><label><input type='checkbox' id='chkIncidentAll_{{monitorEntity.EnvID}}' ng-click="ngSetIncidetDetails('chkIncidentAll_'+monitorEntity.EnvID, monitorEntity.EnvID, true)"/></label></div></th>
                                            <th></th>
								        </tr>
							        </thead>
							        <tbody>
								        <tr ng-repeat="monitor in monitorEntity.monitorList | orderBy:sortType:sortReverse">
									        <td class="text-center">{{$index + 1}}</td>
									        <td  nowrap="nowrap">{{monitor.ConfigServiceType}}</td>
									        <td>{{monitor.ConfigServiceDescription}}</td>

									        <td>{{monitor.MonitorCreatedDateTime | date:'MM/dd/yyyy HH:mm:ss'}}</td>
									        <td class="text-center"><img ng-src="../images/{{monitor.MonitorStatusIcon}}" title="{{monitor.MonitorStatus}}" data-toggle="tooltip" data-placement="bottom"/></td>
                                            <td><div class="form-group" style="margin-bottom: 2px;"><input class='form-control' type='text' id='txtIssue_{{monitor.MonID}}' /><input  type='hidden' id='hidtxtIssue_{{monitor.MonID}}' /></div></td>
                                            <td><div class="form-group" style="margin-bottom: 2px;"><input class='form-control' type='text' id='txSolution_{{monitor.MonID}}' /><input  type='hidden' id='hidtxSolution_{{monitor.MonID}}' /></div></td>
								            <td class="text-center">
								                <div class="checkbox"><label><input type='checkbox' id='chkReplicate_{{monitor.MonID}}' value='{{monitor.MonID}}' ng-click="ngSetIncidetDetails(this.value, monitor.MonID, false)"/><input type='hidden' id='hidchkReplicate_{{monitor.MonID}}' value='{{monitor.MonID}}'/></label></div></td>
                                            <td><button type="submit" class="btn btn-primary" ng-click="SubmitIncident(monitor.MonID,monitor.EnvID,monitor.ConfigID, $event)" onclick="false">submit</button></td>
								        </tr>
							        </tbody>
						        </table>
					        </td>
				        </tr>
			        </tbody>
		        </table>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <script src="../script/incident/incident_operation.js" type="text/javascript"></script>
    <script src="../script/incident/appIncident.js" type="text/javascript"></script>	
    <script src="../script/simple_modal.js" type="text/javascript"></script>
    
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            if (sessionStorage.getItem("RecUpdated") != null && sessionStorage.getItem("RecUpdated") == 'success') {
                $('#lblMessage').text('Resolution detail updated...');
                sessionStorage.setItem("RecUpdated", "");
            }
            else {
                $('#lblMessage').text('');
            }

            //var firstInput = $('form').find('input[type=text],input[type=password],input[type=radio],input[type=checkbox],textarea,select').filter(':visible:first');
        });

     </script>

</asp:Content>
