
<%@ Page Title="Cosmo - Service Restart Operation" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="WinServiceOperation.aspx.cs" Inherits="Cosmo.Web.forms.WinServiceOperation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap1"  ng-cloak>
		<h3 class="top-space">SERVICE RESTART</h3>
		<div style="height:8px"></div>
		<div class="panel panel-primary" id="main">
		  <!-- Default panel contents -->
		  <div class="panel-heading">
              <div class="pull-left">
                  Service Status
              </div>
              <div class="pull-right">
                  Time: <span runat="server" id='lblCurrentTime' clientidmode="Static"></span>&nbsp;  &nbsp;
                  <a href="#" id="lnkRefresh" title="Refresh" ng-click="getCurrentMonitorList();callLoadServiceStatus()" onclick="return false"
                      data-toggle="tooltip" data-placement="bottom" class="">
                      <span class="glyphicon glyphicon-refresh refresh-icon gly-rotate-180"></span>
                  </a>
              </div>
              <div class="clearfix"></div>
          </div>
		  <form class="panel-body" ><!--style="background-color:#e3eef8"-->
              <div class="row">
                  <div class="col-sm-2">
                      <div class="form-group">
                        <input type="text" id="txtGroup" class="form-control" placeholder="Group Name"/>
                      </div>
                  </div>
                  <div class="col-sm-2 wsbuttoncontainer">
                      <div class="form-group">
                          <button class="btn btn-primary" onclick="SubmitStatus(); return false;" title="Schedule Cognos Services"
                              > <%--data-toggle="modal" data-target="#WindownServiceScheduleModal"--%>
                              Schedule</button>
                          <%--<button class="btn btn-primary" onclick="return false;" data-toggle="modal" data-target="#WindownServiceScheduleModal">MODAL</button>--%>
                          <button id="btnCancel" class="btn btn-default" onclick="ClearAllServices(); return false;" title="Schedule Cognos Services">Cancel</button>
                      </div>
                  </div>
                  <div class="col-sm-4">
                      <div class="form-group">
                          <div class="alert alert-warning" id="div-alert" style="height: 50px;display: none">
                          <span class="glyphicon glyphicon-info-sign"></span>
                          <label class="control-label" id="label-alert"></label>
                          </div>
                      </div>
                  </div>
              </div>
              <div class="row">
                  <div class="col-sm-2">
                      <div style="height: 10px" ng-show="errorMessage === ''">
                        <a href="#" style="margin-left:10px;font-size: 1.1em" ng-click="setExpandCollapse()" id="linkExpand" onclick="return false;"><%--<img src="../images/collapseall.jpg" alt=""/>--%> 
                        <span class='glyphicon glyphicon-collapse-down'></span>
                        Collapse All</a>
                      </div>
                  </div>
                  <%--<div class="col-sm-2">
                      <div style="height: 10px" ng-show="errorMessage === ''">
                        <a href="#" style="margin-left:10px;font-size: 1.1em" ng-click="setExpandCollapse()" id="clearA1" onclick="return false;">
                        <span class='glyphicon glyphicon-collapse-down'></span>
                        Clear all</a>
                      </div>        
                  </div>--%>
                    <div class="col-md-10 text-right pull-right">
                        <p class="text-right legend">
                            <img alt="" src="../images/green_icon.jpg" title="Service running"/> Active / Ready &nbsp;&nbsp;
                            <img alt="" src="../images/orange_icon.jpg" title="service is in standby mode"/> Standby&nbsp;&nbsp;
                            <img alt="" src="../images/blue_icon.jpg" title="Dispatcher is not ready/running"/> Not Ready&nbsp;&nbsp;
                            <img alt="" src="../images/red1_icon.jpg" title="Stopped/down"/> Stopped&nbsp;&nbsp;
                            <img alt="" src="../images/red_icon.jpg" title="One or more services are stopped / down"/> One or more services are stopped&nbsp;&nbsp;
                            <img alt="" src="../images/gray_icon.png" title="One or more services are not available" data-toggle="tooltip" data-placement="bottom"/> One or more services are not available&nbsp;&nbsp;&nbsp;
                            <a href="#" id="printPage" title="Print"  data-toggle="tooltip" data-placement="top" onclick=""><img alt="Print" src="../images/print.png" /> Print</a>&nbsp;&nbsp;
                        </p>
                    </div>
              </div>
            <div class="alert alert-danger" ng-show="errorMessage != ''" style="display: none" id="divErrorMessage">
                <span class="glyphicon glyphicon-info-sign"></span>
                {{errorMessage}}
            </div>
            <div class="scrollServiceListBox">
		        <table class="table-environemnt"  ng-repeat="monitorEntity in buildDetails | orderBy:sortType:sortReverse" id="MainWrap" ng-cloak>
			        <thead>
				        <tr>
					        <th ng-click="toggle_it(monitorEntity.EnvID)" id="arrowImg_{{monitorEntity.EnvID}}" class="borderless-td caret-extension" width="20px"><span class="glyphicon glyphicon-triangle-bottom"></span></th>
					        <th style="" class="borderless-td dropdown" >
					            <a href="#" class="dropdown-toggle" data-toggle="dropdown" data-hover="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                {{monitorEntity.EnvName}}
                                <span class="caret"></span></a>

                                <ul class="dropdown-menu" style="font-size: .9em">
                                <li><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'connection')" tablindex="-1">Cognos Connection</a></li>
                                <li class="dropdown-submenu-slideright"><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'system')" tablindex="-1">System</a>
                                    <ul class="dropdown-menu">
                                        <li><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'relationships')">Relationship</a></li>
                                        <li><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'settings')">Settings</a></li>
                                        <li><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'metrics')">Metrics</a></li>
                                    </ul>
                                </li>
                                <li><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'styles')" tablindex="-1">Styles</a></li>
                                <li><a href="#" ng-click="OpenCognosPortal(monitorEntity.EnvID, monitorEntity.EnvName,'capabilities')" tablindex="-1">Capabilities</a></li>
                                </ul>
					        </th>
				        </tr>
			        </thead>
			        <tbody>
				        <tr>
					        <td class="borderless-td" id="row_{{monitorEntity.EnvID}}" colspan="2">
						        <table class="table table-bordered inner-border  table-hover" id="tblService_{{monitorEntity.EnvID}}">
							        <thead >
								        <tr >
								            <th class="text-center" style="width: 20px">
								                <input type="checkbox" value="env_{{monitorEntity.EnvName}}" id="chk_env_{{monitorEntity.EnvID}}" ng-click="AddServiceToSchedule(monitorEntity,'e', $event)"/>
								            </th>
									        <th class="text-center" style="width: 20px">#</th>
									        <th class="col-md-1">
									            <a href="#" ng-click="sortType = 'ConfigServiceType'; sortReverse = !sortReverse;" onclick="return false;">
									            Service Type
                                                <span ng-show="sortType == 'ConfigServiceType' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'ConfigServiceType' && sortReverse" class="fa fa-caret-up"></span>
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
									        <th class="col-md-2">
									            <a href="#" ng-click="sortType = 'LastMoniterTime'; sortReverse = !sortReverse;" onclick="return false;">
									            Last Monitor Time
                                                <span ng-show="sortType == 'LastMoniterTime' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'LastMoniterTime' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-1 text-center">
									            <a href="#" ng-click="sortType = 'MonitorStatusIcon'; sortReverse = !sortReverse;" onclick="return false;">
									            Monitor Status
                                                <span ng-show="sortType == 'MonitorStatusIcon' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'MonitorStatusIcon' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-2 text-left">
									            <a href="#" ng-click="sortType = 'WindowsServiceName'; sortReverse = !sortReverse;" onclick="return false;">
									            Service Name
                                                <span ng-show="sortType == 'WindowsServiceName' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'WindowsServiceName' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-1 text-center">
									            <a href="#" ng-click="sortType = 'WindowsServiceStatus'; sortReverse = !sortReverse;" onclick="return false;">
									            Service Status
                                                <span ng-show="sortType == 'WindowsServiceStatus' && !sortReverse" class="fa fa-caret-down"></span>
                                                <span ng-show="sortType == 'WindowsServiceStatus' && sortReverse" class="fa fa-caret-up"></span>
                                                </a>
									        </th>
									        <th class="col-md-1 text-center">
                                            </th>
									    
								        </tr>
							        </thead>
							        <tbody>
								        <tr ng-repeat="monitor in monitorEntity.monitorList | orderBy:sortType:sortReverse">
								            <td class="text-center" style="width: 20px"><input type="checkbox" value="con_{{monitor.ConfigID}}" id="chk_con_{{monitor.EnvID + '_' + monitor.ConfigID + '_' + monitor.WindowsServiceID}}" ng-click="AddServiceToSchedule(monitor,'c', $event)"/></td>
									        <td class="text-center">{{$index + 1}}</td>
									        <td  nowrap="nowrap">{{monitor.ConfigServiceType}}</td>
									        <td title="{{monitor.ConfigHostIP + ' ' + monitor.ConfigPort | lowercase}}">{{monitor.ConfigServiceDescription}}</td>
									        <td>{{monitor.ConfigLocation}}</td>
									        <td>{{monitor.LastMoniterTime | date:'MM/dd/yyyy HH:mm:ss'}}</td>
									        <td class="text-center" style="vertical-align: middle;"><img ng-src="../images/{{monitor.MonitorStatusIcon}}" title="{{monitor.MonitorStatus}}" data-toggle="tooltip" data-placement="bottom"/></td>
                                            <td>{{monitor.WindowsServiceName}}</td>
									        <td class="text-center" style="vertical-align: middle" ><span ng-class="monitor.StatusClass"><img src='../images/progress.gif' id='imgServiceStatus' alt='Loading' ng-show="monitor.WindowsServiceStatus == '' || monitor.WindowsServiceStatus == 'Loading'" />{{monitor.WindowsServiceStatus}}</span></td>
                                            <%--<td class="text-center" style="vertical-align: middle" ><span ng-class="{'label label-success': monitor.WindowsServiceStatus === 'Success', 'label label-danger': monitor.WindowsServiceStatus === 'Not Reachable' || monitor.WindowsServiceStatus === 'Not Exists!' || monitor.WindowsServiceStatus === 'Stopped'}">{{monitor.WindowsServiceStatus}}</span></td>--%>
                                            <td nowrap="nowrap"><div class="form-inline">
                                                
                                                <button id='btnStart_{{monitor.WindowsServiceID}}' class="btn btn-default btn-sm" value='Start' ng-disabled="monitor.ServiceStrategy.Start === false" data-toggle="tooltip" data-placement="bottom" title="Start" ng-click="InvokeService(monitor, $event,'start')">
                                                    <span ng-class="monitor.ServiceStrategy.Start==true ? 'glyphicon glyphicon-play text-success' : 'glyphicon glyphicon-play text-muted'"> </span>
    <%--                                                <img src='../images/act_start.gif' title='Start' ng-show="monitor.ServiceStrategy.Start === true" alt="Start"  data-toggle="tooltip" data-placement="bottom"/>
                                                    <img src='../images/media_controls_light_play.png' title='Start' ng-show="monitor.ServiceStrategy.Start===false" alt="Start" data-toggle="tooltip" data-placement="bottom" />
    --%>                                            </button>
                                                <button id='btnStop' class="btn btn-default btn-sm" value='Stop' ng-disabled="monitor.ServiceStrategy.Stop === false" data-toggle="tooltip" data-placement="bottom" title="Stop" ng-click="InvokeService(monitor, $event,'stop')">
                                                    <span ng-class="monitor.ServiceStrategy.Stop==true ? 'glyphicon glyphicon-stop' : 'glyphicon glyphicon-stop text-muted'"> </span>
    <%--                                                <img src='../images/action_stop.gif' title='Stop' ng-show="monitor.ServiceStrategy.Stop === true" alt="Start"  data-toggle="tooltip" data-placement="bottom"/>
                                                    <img src='../images/playback_stop_disable.png' ng-show="monitor.ServiceStrategy.Stop===false" alt="Stop" />
    --%>                                            </button>
                                                <button id='btnRestart' class="btn btn-default btn-sm" value='Retart' ng-disabled="monitor.ServiceStrategy.Restart === false" data-toggle="tooltip" data-placement="bottom" title="Restart" ng-click="InvokeService(monitor, $event,'restart')">
                                                    <span ng-class="monitor.ServiceStrategy.Restart==true ? 'glyphicon glyphicon-eject gly-rotate-90 text-success' : 'glyphicon glyphicon-eject gly-rotate-90 text-muted'"> </span>
    <%--                                                <img src='../images/action_restart.gif' title='Restart' ng-show="monitor.ServiceStrategy.Start === true" alt="Start"  data-toggle="tooltip" data-placement="bottom"/>
                                                    <img src='../images/action_restart.gif' ng-show="monitor.ServiceStrategy.Start===false"  alt="Restart"/>--%>
                                                </button>

                                                </div>
                                            </td>
								        </tr>
							        </tbody>
						        </table>							
					        </td>
				        </tr>
			        </tbody>
		        </table>
            </div>
            <!--Starting to populate upcoming schedules-->
            <hr class="h-line"/>
            <h4><b>Upcoming Schedules</b>
                
            </h4>
            
            <div class="form-group">
                <div class="col-sm-3 col-sm-offset-2">
                    <label class="radio-inline">
                        <input type="radio" name="inlineRadioOptions" id="inlineRadio1" value="" ng-model="scheduleFilter">
                        All
                    </label>
                    <label class="radio-inline">
                        <input type="radio" name="inlineRadioOptions" id="inlineRadio2" value="Scheduled" ng-model="scheduleFilter">
                        Scheduled
                    </label>
                    <label class="radio-inline">
                        <input type="radio" name="inlineRadioOptions" id="inlineRadio3" value=null ng-model="scheduleFilter">
                        Open to Schedule
                    </label>           
                </div> 
                <div class="col-sm-5"> 
                    <div class="input-group">
                    <div class="input-group-addon"><i class="fa fa-search"></i></div>
                        <input type="text" class="form-control" placeholder="Search" ng-model="searchSchedule" />
                    </div> 
                </div>
            </div>
            <p>&nbsp;</p><br/>
            <div class="scrollOpenScheduleBox">
                <table class='table table-striped' id="tblGroupSchedules">
                    <thead>
                        <tr>
                            <th class="text-center">#</th>
					        <th >
						        <a href="#" ng-click="sortType = 'Group_Name'; sortReverse = !sortReverse;" onclick="return false;">
						        Group Name
                                <span ng-show="sortType == 'Group_Name' && !sortReverse" class="fa fa-caret-down"></span>
                                <span ng-show="sortType == 'Group_Name' && sortReverse" class="fa fa-caret-up"></span>
                                </a>
					        </th>
					        <th >
						        <a href="#" ng-click="sortType = 'Group_Schedule_DatatimeStr'; sortReverse = !sortReverse;" onclick="return false;">
						        Schedule Time
                                <span ng-show="sortType == 'Group_Schedule_DatatimeStr' && !sortReverse" class="fa fa-caret-down"></span>
                                <span ng-show="sortType == 'Group_Schedule_DatatimeStr' && sortReverse" class="fa fa-caret-up"></span>
                                </a>
					        </th>
					        <th >
						        <a href="#" ng-click="sortType = 'Group_Schedule_Action'; sortReverse = !sortReverse;" onclick="return false;">
						        Action
                                <span ng-show="sortType == 'Group_Schedule_Action' && !sortReverse" class="fa fa-caret-down"></span>
                                <span ng-show="sortType == 'Group_Schedule_Action' && sortReverse" class="fa fa-caret-up"></span>
                                </a>
					        </th>
					        <th >
						        <a href="#" ng-click="sortType = 'Group_Schedule_Status'; sortReverse = !sortReverse;" onclick="return false;">
						        Status
                                <span ng-show="sortType == 'Group_Schedule_Status' && !sortReverse" class="fa fa-caret-down"></span>
                                <span ng-show="sortType == 'Group_Schedule_Status' && sortReverse" class="fa fa-caret-up"></span>
                                </a>
					        </th>

                        </tr>
                    </thead>
                    <tbody>
                        <tr ng-repeat="groupSchedule in groupWithSchedule | orderBy:sortType:sortReverse | ScheduleStatus: scheduleFilter | filter:searchSchedule">
                            <td class="text-center">{{$index + 1}}</td>
                            <td><a href=""  ng-click="EditGroupSchedule(groupSchedule)">{{groupSchedule.Group_Name}}</a></td>
                            <td>{{groupSchedule.Group_Schedule_DatatimeStr | date:'MM/dd/yyyy HH:mm:ss'}}</td>
                            <td>{{groupSchedule.Group_Schedule_Action}}</td>
                            <td>{{groupSchedule.Group_Schedule_Status}}</td>
                            <td><a href="" ng-show="groupSchedule.Group_Schedule_ID > 0" ng-click="CancelGroupSchedule(groupSchedule.Group_Schedule_ID)">
                                <span class="rowdelete" title="Cancel Schedule" data-toggle="tooltip" data-placement="bottom" data-container="body" data-original-title="Cancel Schedule"></span>
                            </a></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <input type="hidden" id="hidGroupID" />
            <input type="hidden" id="hidGroupName" />
            <!--End of upcoming schedules-->
            <p>&nbsp;</p>
            <div class="legend">
                <address>
                    <strong>Important Notes:</strong><br />
                    The above time denotes server time where Cosmo App installed.<br />
                    The current server time is <span runat="server" id='lblNoteServerDateTime' clientidmode="Static"></span> & Server time zone is <span runat="server" id='lblNoteServerTimezone' clientidmode="Static"></span><br />
                    The current local time is <span runat="server" id='lblNoteClientDateTime' clientidmode="Static"></span> & Local time zone is <span runat="server" id='lblNoteClientTimezone' clientidmode="Static"></span><br />
                </address> 
            </div>
		  </form>
		</div>
        <%--<div class="row statusbar">
            <div class="col-md-6">
                <a href="#" id="lnkRefresh" title="Refresh" ng-click="getCurrentMonitorList()" onclick="return false" data-toggle="tooltip" data-placement="bottom">
                <span class="glyphicon glyphicon-refresh"></span></a>
                <span id="time_div1">Last updated time: {{lastRefreshedTime | date:'MM/dd/yyyy h:mm:ss a'}}</span>
            </div>
            <div class="col-md-6 text-right">
                {{ReloadInTime}}
                <a href="#" onclick="openWindowToSetTimeInterval('DashboardSetting.aspx')" id="A1" data-toggle="tooltip" data-placement="bottom" title="Monitor Reload Interval Setting" ng-click="getCurrentMonitorList()" onclick="return false">
                <span class="glyphicon glyphicon-asterisk"></span></a>
            </div>
        </div>--%>
    </div>

    <!--Begin of schedule-->
    <div class="modal fade  bs-example-modal-lg" id="WindownServiceScheduleModal" tabindex="-1" role="dialog"
        aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        &times;</button>
                    <h4 class="modal-title">
                        Schedule</h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal">
                        <div class="col-sm-10 col-sm-offset-1">
                            
                                <div class="alert alert-danger" id="div-schedule-error" style="display: none">
                                    <span class="glyphicon glyphicon-info-sign"></span>
                                    <label class="control-label" id="lblSchedule-error"></label>
                                </div>
                            
                            <div class="form-group">
                                <label for="inputEmail3" class="col-sm-4 control-label">
                                    Scheduled To: </label>
                                <div class="col-sm-7">
                                    <label class="radio-inline">
                                        <input type="radio" name="rdoServiceMode" id="rdoStart" value="1"/>Start
                                    </label>
                                    <label class="radio-inline">
                                        <input type="radio" name="rdoServiceMode" id="rdoStop" value="2"/>Stop
                                    </label>
                                    <label class="radio-inline">
                                        <input type="radio" name="rdoServiceMode" id="rdoRestart" value="3"/>Restart
                                    </label>
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="inputEmail3" class="col-sm-4 control-label">
                                    Schedule By: </label>
                                <div class="col-sm-4">
                                    <div class='input-group date' id='datetimepicker1' data-toggle="tooltip">
                                        <input type="text" id="txtStartDate" class="form-control" onkeypress="return false;" aria-describedby="lblDateError" data-toggle="tooltip"/>
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <button class="btn btn-primary" id="btnSubmitSchedule">Submit</button>
                                </div>
                            </div>
                            <div class="form-group text-center">
                                
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <hr class="h-line col-sm-12" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <label for="inputEmail3" class="col-sm-4 control-label">
                            Selected Services
                            </label>
                            <table class="table table-bordered" id="PrioritywiseBreached">
                                <thead>
                                    <tr>
                                        <th style="display: none">W</th>
                                        <th style="display: none">E</th>
                                        <th style="display: none">C</th>
                                        <th class="text-center" style="width: 20px">#</th>
                                        <th class="col-md-3">Service Type</th>
                                        <th class="col-md-4">Description</th>
                                        <th class="col-md-3">Service Name</th>
                                        <th class="col-md-2 text-center">Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <p>&nbsp;</p>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-dismiss="modal">
                        Close</button>
                </div>
            </div>
        </div>
    </div>
    <!--End of schedule-->

    <script src="../style/js/moment.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap-datetimepicker.min.js" type="text/javascript"></script>
    <script src="../script/restart/restart_operation.js" type="text/javascript"></script>
    <script src="../script/restart/appServiceRestart.js" type="text/javascript"></script>	
    <%--<script src="../script/typeahead.jquery.js" type="text/javascript"></script>--%>
    <script src="../script/typeahead.bundle.js" type="text/javascript"></script>
    <script src="../script/simple_modal.js" type="text/javascript"></script>
    <script src="../script/generictime.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var time = "Monitor status will update in ";
        var reloadTime = '';
        time += reloadTime + " seconds";
        $(document).ready(function () {
            openModal();
            //            $('#linkReLoad').click(function () {
//                location.reload(true);
            //            });
            $('input[type="checkbox"]').change(function() {
                console.log('checkbox status changed');
//                if ($(this).is(":checked")) {
//                    $(this).closest('tr').removeClass().addClass('');
//                } else {
//                    $(this).closest('tr').removeClass().addClass('');
//                }
            });
        });

    </script>

</asp:Content>
