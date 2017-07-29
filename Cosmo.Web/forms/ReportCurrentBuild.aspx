<%@ Page Title="Cosmo - Environment Build Report - Current" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="ReportCurrentBuild.aspx.cs" Inherits="Cosmo.Web.forms.ReportCurrentBuild" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script/build/appServiceBuild.js" type="text/javascript"></script>
    <script src="../script/printreport.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="ServiceBuildApp" ng-controller="ServiceBuildController"
        id="MainWrap" ng-cloak>
        <h3 class="top-space">
            ENVIRONMENT BUILD REPORT - CURRENT</h3>
        <div style="height: 8px">
        </div>
        <div class="panel panel-primary">
            <!-- Default panel contents -->
            <div class="panel-heading">
                Version / Build - Current</div>
            <div class="panel-body">
                <div class="row">
                    <div class="expand_dashboard">
                        <div class="pringbutton">
                        <a href="#" id="printPage" ><%--<span class="glyphicon glyphicon-print"></span>--%>
                        <img alt="Print" src="../images/print.png" /> Print</a>
                        </div>
                    </div>

                    <div class="form-group">
                      <div class="input-group">
                        <div class="input-group-addon"><i class="fa fa-search"></i></div>
                        <input type="text" class="form-control" placeholder="Search" ng-model="searchSchedule" />

                      </div>      
                    </div>
                    
                </div>
                <div id="main">

                <div class="row">
                  <table class="table table-responsive" id="PrioritywiseBreached">
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
                            <th >Build Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr ng-repeat="build in buildDetails | orderBy:sortType:sortReverse | filter:searchSchedule">
                            <td>{{$index + 1}}</td>
                            <td>{{build.EnvName}}</td>
                            <td> 
                                <table class="table table-hover table-borderless table-striped" >
                                    <thead>
                                        <tr class="repHeader">
                                            <th class="reportSNCol">#</th>
                                            <th class="col-sm-2">
                                                <a href="#" ng-click="sortType_build = 'ConfigServiceType'; sortReverse_build = !sortReverse_build">
                                                Service Type
                                                <span ng-show="sortType_build == 'ConfigServiceType' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType_build == 'ConfigServiceType' && sortReverse_build" class="fa fa-caret-up"></span>
                                                </a>
                                            </th>
                                            <th class="col-sm-3">
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
                                            <th class="col-sm-2">
                                                <a href="#" ng-click="sortType_build = 'ConfigLocation'; sortReverse_build = !sortReverse_build">
                                                Location
                                                <span ng-show="sortType_build == 'ConfigLocation' && !sortReverse_build" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType_build == 'ConfigLocation' && sortReverse_build" class="fa fa-caret-up"></span>
                                                </a>
                                            </th>
                                            <th class="col-sm-2">
                                                <a href="#" ng-click="sortType_build = 'MonitorComments'; sortReverse_build = !sortReverse_build">
                                                Version / Build
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
                                            <th class="hide">
                                                Environment
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr ng-repeat="monitor in build.monitorList | orderBy:sortType_build:sortReverse_build | filter:searchSchedule">
                                            <td>{{$index + 1}}</td>
                                            <td nowrap="nowrap">{{monitor.ConfigServiceType}}</td>
                                            <td>{{monitor.ConfigServiceDescription}}</td>
                                            <%--<td>{{monitor.ConfigPort}}</td>--%>
                                            <td>{{monitor.ConfigLocation}}</td>
                                            <td>{{monitor.MonitorComments}}</td>
                                            <td>{{monitor.MonitorCreatedDateTime | date:'MM/dd/yyyy HH:mm:ss'}}</td>
                                            <td class="hide">{{monitor.EnvName}}</td>
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

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            openModal();
        });
     </script>    
</asp:Content>
