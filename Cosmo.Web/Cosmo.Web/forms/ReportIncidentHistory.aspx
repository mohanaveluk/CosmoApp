<%@ Page Title="Cosmo - Service Incident Report" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="ReportIncidentHistory.aspx.cs" Inherits="Cosmo.Web.forms.ReportIncidentHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/build/appServiceIncidentHistory.js" type="text/javascript"></script>
    <script src="../script/printreport.js" type="text/javascript"></script>
    <style>
        .form-control
        { font-size: 14px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="ServiceBuildApp" ng-controller="ServiceBuildController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">
            SERVICE INCIDENT REPORT</h3>
        <div style="height: 8px">
        </div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">Service Incident
            - Report of {{reportType=='0' ? reportTypeText : reportTypeText + ' Environment '}} from {{startDate | date: "MM/dd/yyyy"}} to {{endDate | date: "MM/dd/yyyy"}}
            </div>
            <div class="panel-body">
                <form id="Form1" class="form-inline" runat="server" style="font-size: 16px">
                        <div class="form-group">
                            <label class="control-label">Environment</label>
                        </div>
                        <div class="form-group">
                            <select class="form-control" name="drpReportType" id="drpReportType" ng-model="reportType"  ng-change="getBuildHistory()">
                                <option ng-repeat="envi in buildEnvironment" value="{{envi.EnvID}}">{{envi.EnvName}}</option>
                                <option value="0">All Environments</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <select id="drpHistory" ng-model="reportRange" class="form-control">
	                            <option value="1">Last 24 hours</option>
	                            <option value="2">Last 48 hours</option>
	                            <option value="3">Last 1 week</option>
	                            <option value="4">Last 2 weeks</option>
	                            <option value="5">Last 1 month</option>
	                            <option value="20">Custom</option>
                            </select>   
                                                     
                        </div>
                        <div class="form-group">
                            <button type="button" class="btn btn-primary" id="btnHistoryGo" ng-click="getDates();" onclick="return false">Go</button>
                        </div>
                        &nbsp;
                        <div class="form-group" id="divDateRange">
                            History From
                            <div class='input-group date' id='datetimepicker1'>
                                <input type="text" class="form-control" id="txtFromDateTime" onKeyPress="return false;" ng-model="startDate" />
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                            </div>
                            to
                            <div class='input-group date' id='datetimepicker2'>
                                <input type="text" class="form-control" id="txtToDateTime" onKeyPress="return false;" ng-model="endDate" />
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                            </div>
                             <button type="button" class="btn btn-primary" id="btnGo" ng-click="getBuildHistory();" onclick="updateDate();return false;">Go</button>
                        </div>
                </form>
                <br/>

                <div class="form-group">
                    <div class="col-sm-7"> 
                        <div class="input-group">
                        <div class="input-group-addon"><i class="fa fa-search"></i></div>
                            <input type="text" class="form-control" placeholder="Search" ng-model="searchSchedule" />
                        </div> 
                    </div>
                    <div class="col-sm-5" style="padding-top: 5px;">
                        <a href="#" id="printPage" ><%--<span class="glyphicon glyphicon-print"></span>--%>
                        <img alt="Print" src="../images/print.png" /> Print</a>
                    </div>     
                </div>

                <br/><br/>
                

                <div id="main">
                    
                    <div class="">
                      <table class="table" id="PrioritywiseBreached">
                        <thead>
					        <tr class="repHeader">
                                <th class="reportSNCol">#</th>
                                <th class="report-environemtn-col">
                                    <a href="#" ng-click="sortType = 'EnvName'; sortReverse = !sortReverse">
                                    Environment
                                    <span ng-show="sortType == 'EnvName' && !sortReverse" class="fa fa-caret-down"></span>
                                    <span ng-show="sortType == 'EnvName' && sortReverse" class="fa fa-caret-up"></span>
                                    </a>
                                </th>
                                <th style="padding-left: 50px">Incident Details</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr ng-repeat="build in buildDetails | orderBy:sortType:sortReverse | filter:searchSchedule">
                                <td>{{$index + 1}}</td>
                                <td>{{build.EnvName}}</td>
                                <td> 
                                    <table class="table table-hover table-borderless table-striped" >
                                        <thead>
                                            <tr>
                                                <th style="width: 10px">#</th>
                                                <th class="col-sm-1" nowrap="nowrap">
                                                    <a href="#" ng-click="sortType_build = 'ConfigServiceType'; sortReverse_build = !sortReverse_build">
                                                    Service Type
                                                    <span ng-show="sortType_build == 'ConfigServiceType' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'ConfigServiceType' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="col-sm-2">
                                                    <a href="#" ng-click="sortType_build = 'ConfigServiceDescription'; sortReverse_build = !sortReverse_build">
                                                    Description
                                                    <span ng-show="sortType_build == 'ConfigServiceDescription' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'ConfigServiceDescription' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <%--<th class="reportPortCol">
                                                    <a href="#" ng-click="sortType_build = 'ConfigPort'; sortReverse_build = !sortReverse_build">
                                                    Port
                                                    <span ng-show="sortType_build == 'ConfigPort' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'ConfigPort' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>--%>
                                                <th class="col-sm-1">
                                                    <a href="#" ng-click="sortType_build = 'ConfigLocation'; sortReverse_build = !sortReverse_build">
                                                    Location
                                                    <span ng-show="sortType_build == 'ConfigLocation' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'ConfigLocation' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="col-sm-1 text-center">
                                                    <a href="#" ng-click="sortType_build = 'MonitorComments'; sortReverse_build = !sortReverse_build">
                                                    Status
                                                    <span ng-show="sortType_build == 'MonitorComments' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'MonitorComments' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="col-sm-2">
                                                    <a href="#" ng-click="sortType_build = 'MonitorCreatedDateTime'; sortReverse_build = !sortReverse_build">
                                                    Last Monitor Date/Time
                                                    <span ng-show="sortType_build == 'MonitorCreatedDateTime' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'MonitorCreatedDateTime' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="col-sm-2">
                                                    <a href="#" ng-click="sortType_build = 'Incident_Issue'; sortReverse_build = !sortReverse_build">
                                                    Issue
                                                    <span ng-show="sortType_build == 'Incident_Issue' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'Incident_Issue' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="col-sm-2">
                                                    <a href="#" ng-click="sortType_build = 'Incident_Solution'; sortReverse_build = !sortReverse_build">
                                                    Resolution
                                                    <span ng-show="sortType_build == 'Incident_Solution' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'Incident_Solution' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="col-sm-2">
                                                    <a href="#" ng-click="sortType_build = 'ResolutionCreatedDateTime'; sortReverse_build = !sortReverse_build">
                                                    Updated Date/Time
                                                    <span ng-show="sortType_build == 'ResolutionCreatedDateTime' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                    <span ng-show="sortType_build == 'ResolutionCreatedDateTime' && sortReverse_build" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th class="hide">Environment</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr ng-repeat="monitor in build.monitorList | orderBy:sortType_build:sortReverse_build | filter:searchSchedule">
                                                <td>{{$index + 1}}</td>
                                                <td nowrap="nowrap">{{monitor.ConfigServiceType}}</td>
                                                <td>{{monitor.ConfigServiceDescription}}</td>
                                                <%--<td>{{monitor.ConfigPort}}</td>--%>
                                                <td>{{monitor.ConfigLocation}}</td>
                                                <td class="text-center"><img ng-src="{{monitor.MonitorComments}}" title="{{monitor.MonitorStatus}}" alt="{{monitor.MonitorStatus}}" /></td>
                                                <td>{{monitor.MonitorCreatedDateTime | date:'MM/dd/yyyy HH:mm:ss'}}</td>
                                                <td>{{monitor.Incident_Issue}}</td>
                                                <td>{{monitor.Incident_Solution}}</td>
                                                <td class="hide">{{monitor.EnvName}}</td>
                                                <td>{{monitor.ResolutionCreatedDateTime | date:'MM/dd/yyyy HH:mm:ss'}}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>

                        </tbody>

                      </table>
                    </div>
                </div>
            </div>
        </div>
        <p>&nbsp;</p>
    </div>
    <script src="../script/simple_modal.js" type="text/javascript"></script>
    <script src="../script/report/selection.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            openModal();
        });

     </script>    
    
</asp:Content>
