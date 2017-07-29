<%@ Page Title="Cosmo - Dashboard" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Cosmo.Web.forms.Dashboard" %>
<%@ Import Namespace="System.Net" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Begin page content -->
    <div class="container-fluid body-panel" ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap" ng-cloak>
        <div style="height: 58px">
            <div class="col-sm-3">
                <h3 class="top-space" style="margin-left: -15px">DASHBOARD</h3>        
            </div>
            <div class="col-sm-4 pull-right text-right">
                    <!--Cosmo service status - start-->
                    <div class="db_cosmo_server">
                        <ul>
                            <li>
                                <a href=""><span id="divServiceStatus" class="img_service"></span></a>
                                <ul class="fallback">
                                    <li id="mnuStart"><a href="#" onclick="cosmoServiceFunction('start')" >Start</a></li>
                                    <li id="mnuStop"><a href="#" onclick="cosmoServiceFunction('stop')" >Stop</a></li>
                                    <li id="mnuRestart"><a href="#" onclick="cosmoServiceFunction('restart')" >Restart</a></li>
                                </ul>                  
                            </li>
                        </ul>
                    </div>
                    <!--Cosmo service status - end-->
                
            </div>
        </div>
		<div style="height:8px"></div>
        <form runat="server">
		<div class="panel panel-primary" id="main">
		  <!-- Default panel contents -->
		  <div class="panel-heading">
              <div class="pull-left">
                  Service Status
              </div>
              <div class="pull-right">
                <asp:HiddenField runat="server" ID="hidCurrentTime" ClientIDMode="Static"/>
                <span id="time_div1" ng-cloak>Last Updated Time: {{lastRefreshedTime | date:'MM/dd/yyyy h:mm:ss a'}}</span>&nbsp;  &nbsp;
                <a href="#" id="lnkRefresh" title="Refresh" ng-click="getCurrentMonitorList()" onclick="return false" data-toggle="tooltip" data-placement="bottom" class="">
                <span class="glyphicon glyphicon-refresh refresh-icon gly-rotate-180"></span></a>

              </div>
              <div class="clearfix"></div>
          </div>

		  <div class="panel-body" ><!--style="background-color:#e3eef8"-->
              <div class="row">
                  <div class="col-md-2">
                      <div style="height: 10px" ng-show="errorMessage === ''">
                        <a href="#" style="margin-left:10px;font-size: 1.1em" ng-click="setExpandCollapse()" id="linkExpand" onclick="return false;"><%--<img src="../images/collapseall.jpg" alt=""/>--%> 
                        <span class='glyphicon glyphicon-collapse-down'></span>
                        Collapse All</a>
                      </div>
                  </div>
                  <div class="col-md-10 text-right">
                    <p class="text-right legend">
                        <img alt="" src="../images/green_icon.jpg" title="Service running" data-toggle="tooltip" data-placement="bottom"/> Active / Ready &nbsp;&nbsp;
                        <img alt="" src="../images/orange_icon.jpg" title="service is in standby mode" data-toggle="tooltip" data-placement="bottom"/> Standby&nbsp;&nbsp;
                        <img alt="" src="../images/blue_icon.jpg" title="Dispatcher is not ready/running" data-toggle="tooltip" data-placement="bottom"/> Not Ready&nbsp;&nbsp;
                        <img alt="" src="../images/red1_icon.jpg" title="Stopped/down" data-toggle="tooltip" data-placement="bottom"/> Stopped&nbsp;&nbsp;
                        <img alt="" src="../images/red_icon.jpg" title="One or more services are stopped / down" data-toggle="tooltip" data-placement="bottom"/> One or more services are stopped&nbsp;&nbsp;  
                        <img alt="" src="../images/gray_icon.png" title="One or more services are not available" data-toggle="tooltip" data-placement="bottom"/> One or more services are not available&nbsp;&nbsp;&nbsp;    
                        <a href="#" id="printPage" title="Print"  data-toggle="tooltip" data-placement="top" onclick=""><img alt="Print" src="../images/print.png" /> Print</a>&nbsp;&nbsp;
                    </p>
                  </div>
              </div>
            <div class="alert alert-danger" ng-show="errorMessage != ''" style="display: none" id="divErrorMessage">
                <span class="glyphicon glyphicon-info-sign"></span>
                {{errorMessage}}
            </div>
		    <table class="table table-environemnt"  ng-repeat="monitorEntity in buildDetails | orderBy:sortType:sortReverse">
			    <thead>
				    <tr>
					    <th ng-click="toggle_it(monitorEntity.EnvID)" id="arrowImg_{{monitorEntity.EnvID}}" class="borderless-td caret-extension" width="20px"><span class="glyphicon glyphicon-triangle-bottom"></span></th>
					    <th class="borderless-td" style="width:300px;">
					        <a href="#" data-toggle="modal" data-target="#modalUrlPerformanceDetails" data-eid="{{monitorEntity.EnvID}}">
                            {{monitorEntity.EnvName}}
                            </a>
					    </th>
                        <th class="borderless-td">Status: <img ng-src="{{monitorEntity.OverallStatus.ImagePath}}" alt="" title="{{monitorEntity.OverallStatus.Description}}" /></th>
				    </tr>
			    </thead>
			    <tbody>
				    <tr>
					    <td class="borderless-td" id="row_{{monitorEntity.EnvID}}" colspan="3">
						    <table class="table table-bordered inner-border  table-hover">
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
									    <th class="col-md-2">
									        <a href="#" ng-click="sortType = 'ConfigHostIP'; sortReverse = !sortReverse;" onclick="return false;">
									        Host / IP
                                            <span ng-show="sortType == 'ConfigHostIP' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'ConfigHostIP' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1">
									        <a href="#" ng-click="sortType = 'ConfigPort'; sortReverse = !sortReverse;" onclick="return false;">
									        Port
                                            <span ng-show="sortType == 'ConfigPort' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'ConfigPort' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1" nowrap="nowrap">
									        <a href="#" ng-click="sortType = 'BuildVersion'; sortReverse = !sortReverse;" onclick="return false;">
                                            Version / Build
                                            <span ng-show="sortType == 'BuildVersion' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'BuildVersion' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-2">
									        <a href="#" ng-click="sortType = 'ConfigServiceDescription'; sortReverse = !sortReverse;" onclick="return false;">
									        Description
                                            <span ng-show="sortType == 'ConfigServiceDescription' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'ConfigServiceDescription' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1">
									        <a href="#" ng-click="sortType = 'ConfigLocation'; sortReverse = !sortReverse;" onclick="return false;">
									        Location
                                            <span ng-show="sortType == 'ConfigLocation' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'ConfigLocation' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1" nowrap="nowrap">
									        <a href="#" ng-click="sortType = 'LastMoniterTime'; sortReverse = !sortReverse;" onclick="return false;">
									        Last Monitor Time
                                            <span ng-show="sortType == 'LastMoniterTime' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'LastMoniterTime' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'MonitorStatusIcon'; sortReverse = !sortReverse;" onclick="return false;">
									        Status
                                            <span ng-show="sortType == 'MonitorStatusIcon' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'MonitorStatusIcon' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1">
									        <a href="#" ng-click="sortType = 'MonitorUpTime'; sortReverse = !sortReverse;" onclick="return false;" >
									        Uptime
                                            <span ng-show="sortType == 'MonitorUpTime' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'MonitorUpTime' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'MonitorNotifyIcon'; sortReverse = !sortReverse;" onclick="return false;">
									        Alert
                                            <span ng-show="sortType == 'MonitorNotifyIcon' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'MonitorNotifyIcon' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
								    </tr>
							    </thead>
							    <tbody>
								    <tr ng-repeat="monitor in monitorEntity.monitorList | orderBy:sortType:sortReverse">
									    <td class="text-center">{{$index + 1}}</td>
									    <td  nowrap="nowrap">{{monitor.ConfigServiceType}} <span class="label label-warning" ng-show="monitor.ConfigIsPrimary === false">{{monitor.ConfigPort}}</span></td>
									    <td>
									        <a href="#" data-toggle="modal" data-target="#modalServerPerformance" data-host="{{monitor.ConfigHostIP | lowercase}}">
                                            {{monitor.ConfigHostIP | lowercase}}
                                            </a>
									    </td>
									    <td>{{monitor.ConfigPort}}</td>
									    <td>{{monitor.BuildVersion}}</td>
									    <td>{{monitor.ConfigServiceDescription}}</td>
									    <td>{{monitor.ConfigLocation}}</td>
									    <td nowrap="nowrap">{{monitor.LastMoniterTime | date:'MM/dd/yyyy HH:mm:ss'}}</td>
									    <td class="text-center" style="vertical-align: middle;"><img ng-src="../images/{{monitor.MonitorStatusIcon}}" title="{{monitor.MonitorStatus}}" data-toggle="tooltip" data-placement="bottom"/></td>
									    <td>{{monitor.MonitorUpTime}}</td>
									    <td class="text-center">
									        <a href="#" ng-click="Notify(monitor);" onclick="return false" data-toggle="tooltip" data-placement="bottom">
                                                <img ng-src="{{monitor.MonitorNotifyIcon}}" title="{{monitor.MonitorNotifyTooltip}}" data-toggle="tooltip" data-placement="bottom"/>
                                            </a>
									    </td>
								    </tr>
							    </tbody>
						    </table>							
					    </td>
				    </tr>
			    </tbody>
		    </table>
		  </div>
		</div>
        </form>
        <p>&nbsp;</p>

        <div class="row statusbar" id="dashBoardStatus">
            <div class="col-md-6">
               <%-- <a href="#" id="lnkRefresh" title="Refresh" ng-click="getCurrentMonitorList()" onclick="return false" data-toggle="tooltip" data-placement="bottom">
                <span class="glyphicon glyphicon-refresh"></span></a>
                <span id="time_div1">Last updated time: {{lastRefreshedTime | date:'MM/dd/yyyy h:mm:ss a'}}</span>--%>
            </div>
            <div class="col-md-6 text-right">
                {{ReloadInTime}}
                <a href="#" onclick="openWindowToSetTimeInterval('DashboardSetting.aspx')" id="A1" data-toggle="tooltip" data-placement="bottom" title="Dashboard Setting" ng-click="getCurrentMonitorList()" onclick="return false">
                <span class="glyphicon glyphicon-asterisk"></span></a>
            </div>
        </div>
    </div>

    <script src="../script/dashboard/serviceStatus.js" type="text/javascript"></script>
    <script src="../script/dashboard/dash_operation.js" type="text/javascript"></script>
    <script src="../script/dashboard/appMonitor.js" type="text/javascript"></script>	
    <script src="../script/simple_modal.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
        var time = "Monitor status will update in ";
        time += reloadTime + " seconds";

        $(document).ready(function () {

            $('#linkReLoad').click(function() {
                location.reload(true);
            });

//            $('#lnkRefresh').mouseover(function() {
//                $('#lnkRefresh').html('<span class="glyphicon glyphicon-refresh refresh-icon gly-rotate-180"></span>');
//            });
//            $('#lnkRefresh').mouseout(function () {
//                $('#lnkRefresh').html('<span class="glyphicon glyphicon-refresh refresh-icon"></span>');
            //            });
            //$(function () {
            //    systemName = '<%= Dns.GetHostName() %>;';
            //});
        });

    </script>
</asp:Content>

