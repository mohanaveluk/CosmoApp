<%@ Page Title="Cosmo - Service Restart Report" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="ReportServiceRestart.aspx.cs" Inherits="Cosmo.Web.forms.ReportServiceRestart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/restart/appReportServiceRestart.js" type="text/javascript"></script>
    <script src="../script/printreport.js" type="text/javascript"></script>
    <style>
        .form-control
        { font-size: 14px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="ServiceRestartApp" ng-controller="ServiceRestartController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">
            SERVICE RESTART REPORT</h3>
        <div style="height: 8px">
        </div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">Service Schedule - Stop/Start/Restart Schedule Report from {{startDate | date: "MM/dd/yyyy"}}
                        to {{endDate | date: "MM/dd/yyyy"}}</div>
            <div class="panel-body">
                <form id="Form1" class="form-inline" runat="server" style="font-size: 16px">
                        <div class="form-group">
                            <label class="control-label">Environment</label>
                        </div>
                        <div class="form-group">
                            <select class="form-control" name="drpReportType" id="drpReportType" ng-model="reportType"  ng-change="getServiceScheduleReport()">
                                <option ng-repeat="envi in buildEnvironment" value="{{envi.EnvID}}">{{envi.EnvName}}</option>
                                    <option value="O">Upcoming Schedule</option>
                                    <option value="S">Completed Schedule</option>
                                    <option value="C">Cancelled Schedule</option>
                                    <option value="all">All Schedule</option>
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
                             <button type="button" class="btn btn-primary" id="btnGo" ng-click="getServiceScheduleReport();" onclick="updateDate();return false">Go</button>
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
                    <table class="table table-bordered table-striped" id="PrioritywiseBreached">
                        <thead>
                            <tr class="repHeader">
                                <th>
                                    #
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_NAME'; sortReverse = !sortReverse">Group Name
                                        <span ng-show="sortType == 'GROUP_NAME' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'GROUP_NAME' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'ENV_NAME'; sortReverse = !sortReverse">Environment
                                        Name <span ng-show="sortType == 'ENV_NAME' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'ENV_NAME' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'CONFIG_SERVICE_TYPE'; sortReverse = !sortReverse">
                                        Service Type <span ng-show="sortType == 'CONFIG_SERVICE_TYPE' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'CONFIG_SERVICE_TYPE' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'CONFIG_SERVICE_NAME'; sortReverse = !sortReverse">Description
                                        <span ng-show="sortType == 'CONFIG_SERVICE_NAME' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'CONFIG_SERVICE_NAME' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_SCH_ACTION'; sortReverse = !sortReverse">Request <span ng-show="sortType == 'GROUP_SCH_ACTION' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'GROUP_SCH_ACTION' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'Yes'; sortReverse = !sortReverse">On-Demand? <span
                                        ng-show="sortType == 'Yes' && !sortReverse" class="fa fa-caret-down"></span><span
                                            ng-show="sortType == 'Yes' && sortReverse" class="fa fa-caret-up"></span>
                                    </a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_SCH_CREATED_DATETIME'; sortReverse = !sortReverse">Created Date/Time
                                        <span ng-show="sortType == 'GROUP_SCH_CREATED_DATETIME' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'GROUP_SCH_CREATED_DATETIME' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_SCH_TIME'; sortReverse = !sortReverse">Scheduled Date/Time
                                        <span ng-show="sortType == 'GROUP_SCH_TIME' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'GROUP_SCH_TIME' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'ServiceStartedTime'; sortReverse = !sortReverse">Start Date/Time
                                        <span ng-show="sortType == 'ServiceStartedTime' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'ServiceStartedTime' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'ServiceCompletionTime'; sortReverse = !sortReverse">Completion Date/Time
                                        <span ng-show="sortType == 'ServiceCompletionTime' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'ServiceCompletionTime' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_SCH_CREATED_BY_USERNAME'; sortReverse = !sortReverse">
                                        Scheduled By <span ng-show="sortType == 'GROUP_SCH_CREATED_BY_USERNAME' && !sortReverse"
                                            class="fa fa-caret-down"></span><span ng-show="sortType == 'GROUP_SCH_CREATED_BY_USERNAME' && sortReverse"
                                                class="fa fa-caret-up"></span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_SCH_TYPE'; sortReverse = !sortReverse">Scheduled
                                        Type <span ng-show="sortType == 'GROUP_SCH_TYPE' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'GROUP_SCH_TYPE' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'GROUP_SCH_STATUS'; sortReverse = !sortReverse">
                                        Source <span ng-show="sortType == 'GROUP_SCH_STATUS' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'GROUP_SCH_STATUS' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                                <th>
                                    <a href="#" ng-click="sortType = 'RequestSource'; sortReverse = !sortReverse">
                                        Status <span ng-show="sortType == 'RequestSource' && !sortReverse" class="fa fa-caret-down">
                                        </span><span ng-show="sortType == 'RequestSource' && sortReverse" class="fa fa-caret-up">
                                        </span></a>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr ng-repeat="schedule in schedules | orderBy:sortType:sortReverse | filter:searchSchedule">
                                <td>
                                    {{$index + 1}}
                                </td>
                                <td>
                                    {{schedule.GROUP_NAME}}
                                </td>
                                <td>
                                    {{schedule.ENV_NAME }}
                                </td>
                                <td>
                                    {{schedule.CONFIG_SERVICE_TYPE }}
                                </td>
                                <td>
                                    {{schedule.CONFIG_SERVICE_NAME }}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_ACTION }}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_ONDEMAND }}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_CREATED_DATETIME | date: 'MM/dd/yyyy HH:mm:ss'}}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_TIME | date: 'MM/dd/yyyy HH:mm:ss'}}
                                </td>
                                <td>
                                    {{schedule.ServiceStartedTime | date: 'MM/dd/yyyy HH:mm:ss'}}
                                </td>
                                <td>
                                    {{schedule.ServiceCompletionTime | date: 'MM/dd/yyyy HH:mm:ss'}}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_CREATED_BY_USERNAME }}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_TYPE }}
                                </td>
                                <td>
                                    {{schedule.RequestSource }}
                                </td>
                                <td>
                                    {{schedule.GROUP_SCH_RESULT }}
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div>
                      <div class="form-group">
                          <div class="alert alert-warning" id="div-alert" style="height: 50px;display: none">
                          <span class="glyphicon glyphicon-info-sign"></span>
                          <label class="control-label" id="label-alert"></label>
                          </div>
                      </div>
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
