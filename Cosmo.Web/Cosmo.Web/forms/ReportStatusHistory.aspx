<%@ Page Title="Cosmo - Service Status Report - History" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="ReportStatusHistory.aspx.cs" Inherits="Cosmo.Web.forms.ReportStatusHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/restart/appReportServiceStatusHistory.js" type="text/javascript"></script>
    <script src="../script/printreport.js" type="text/javascript"></script>
    <style>
        .form-control
        { font-size: 14px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="ServiceStatusApp" ng-controller="ServiceBuildController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">
            SERVICE STATUS REPORT</h3>
        <div style="height: 8px">
        </div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">Service Status History - Report of {{reportType=='0' ? reportTypeText : reportTypeText
                        + ' Environment '}} from {{startDate | date: "MM/dd/yyyy"}} to {{endDate | date:
                        "MM/dd/yyyy"}}</div>
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
                            To
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
                    <div style="float: right; margin-right: 50px" class="legend">
                        <img src="../images/green_icon.jpg" class="icon-legend" alt="Active CM" />
                        Active CM &nbsp;&nbsp;
                        <img src="../images/orange_icon.jpg" class="icon-legend" alt="Standby CM" />
                        Standby CM &nbsp;&nbsp;
                        <img src="../images/yellow_green.png" class="icon-legend" alt="Standby CM" />
                        Partial Active / Standby CM &nbsp;&nbsp;
                        <img src="../images/blue_icon.png" class="icon-legend" alt="Dispatcher Ready" />
                        Dispatcher Ready &nbsp;&nbsp;
                        <img src="../images/gray_icon.png" class="icon-legend" alt="Not Available" />
                        Not Available &nbsp;&nbsp;
                        <img src="../images/pink_icon.png" class="icon-legend" alt="Ready and Down"/>
                        Partial Up / Down &nbsp;&nbsp;
                        <%--A - Active CM | S - Standby CM | R - Dispatcher Ready | N/A - Not Available | R/D - Ready and Down--%>
                    </div>
                    <div style="overflow: auto; width: 97vw">
                        <table class="table" id="PrioritywiseBreached">
                            <thead>
                                <%--<tr class="repHeader">
                            <th >#</th>
                            <th >
                                <a href="#" ng-click="sortType = 'EnvName'; sortReverse = !sortReverse | filter:searchSchedule">
                                Environment
                                <span ng-show="sortType == 'EnvName' && !sortReverse" class="fa fa-caret-down"></span>
                                <span ng-show="sortType == 'EnvName' && sortReverse" class="fa fa-caret-up"></span>
                                </a>
                            </th>
                        </tr>--%>
                            </thead>
                            <tbody>
                                <tr class="repHeader">
                                    <td>
                                        <table class="table table-borderless">
                                            <tr>
                                                <th class="reportSNCol">
                                                    #
                                                </th>
                                                <th style="padding-left: 10px">
                                                    <a href="#" ng-click="sortType = 'EnvName'; sortReverse = !sortReverse">Environment
                                                        <span ng-show="sortType == 'EnvName' && !sortReverse" class="fa fa-caret-down"></span>
                                                        <span ng-show="sortType == 'EnvName' && sortReverse" class="fa fa-caret-up"></span>
                                                    </a>
                                                </th>
                                                <th style="float: right">
                                                </th>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr ng-repeat="build in buildDetails | orderBy:sortType:sortReverse | filter:searchSchedule">
                                    <td>
                                        <table class="table table-responsive table-striped" id="tblStatus" ng-class="[SetTableSize()]">
                                            <tr>
                                                <td style="width: 50px">
                                                    {{$index + 1}}
                                                </td>
                                                <td>
                                                    {{build.EnvName}}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table class="table table-hover table-striped">
                                                        <thead>
                                                            <tr class="repHeader">
                                                                <th style="width: 10px" rowspan="2">
                                                                    #
                                                                </th>
                                                                <th ng-class="[SetColSize()]" rowspan="2" nowrap="nowrap" id="col_service_contenttype">
                                                                    <a href="#" ng-click="sortType_build = 'ConfigServiceType'; sortReverse_build = !sortReverse_build">
                                                                        Service Type <span ng-show="sortType_build == 'ConfigServiceType' && !sortReverse_build"
                                                                            class="fa fa-caret-down"></span><span ng-show="sortType_build == 'ConfigServiceType' && sortReverse_build"
                                                                                class="fa fa-caret-up"></span></a>
                                                                </th>
                                                                <th ng-class="[SetColSize()]" nowrap="nowrap" rowspan="2" id="col_service_description">
                                                                    <a href="#" ng-click="sortType_build = 'ConfigServiceDescription'; sortReverse_build = !sortReverse_build">
                                                                        Description <span ng-show="sortType_build == 'ConfigServiceDescription' && !sortReverse_build"
                                                                            class="fa fa-caret-down"></span><span ng-show="sortType_build == 'ConfigServiceDescription' && sortReverse_build"
                                                                                class="fa fa-caret-up"></span></a>
                                                                </th>
                                                                <th class="hide">
                                                                    Environment
                                                                </th>
                                                                <th ng-repeat="colCount in columnMonthHeadersCount  track by $index" colspan="{{colCount}}"
                                                                    class="col-center">
                                                                    {{columnMonthHeaders[$index]}}
                                                                </th>
                                                            </tr>
                                                            <tr>
                                                                <th ng-repeat="col in columnDateHeaders" class="col-center">
                                                                    {{col | date:'dd'}}
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <tr ng-repeat="monitor in build.monitorList | orderBy:sortType_build:sortReverse_build  | filter:searchSchedule">
                                                                <td>
                                                                    {{$index + 1}}
                                                                </td>
                                                                <td nowrap="nowrap">
                                                                    {{monitor.ConfigServiceType}}
                                                                </td>
                                                                <td nowrap="nowrap">
                                                                    {{monitor.ConfigServiceDescription}}
                                                                </td>
                                                                <td class="hide">
                                                                    {{monitor.EnvName}}
                                                                </td>
                                                                <td ng-repeat="sts in monitor.MonitorComments track by $index" class="col-center">
                                                                    <img ng-src="{{sts}}" alt="" />
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
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
