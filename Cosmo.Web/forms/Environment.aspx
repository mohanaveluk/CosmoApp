<%@ Page Title="Cosmo - Environment" Language="C#" MasterPageFile="~/Navigation.Master" AutoEventWireup="true" CodeBehind="Environment.aspx.cs" Inherits="Cosmo.Web.forms.Environment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid body-panel" ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap" ng-cloak>
		<h3 class="top-space">ENVIRONMENT
                      <a href="#" ng-click="EditEnvironmentConfiguration(null,'n');" onclick="return false;">
                          <img src="../images/add-circle-32.png" width="28" alt="Create" title="Setup new environment or service" data-toggle="tooltip" data-placement="top"/>
                      </a>
        </h3>
		<div style="height:8px"></div>
        <div class="panel panel-primary" id="main">
		  <!-- Default panel contents -->
		  <div class="panel-heading">Service Status</div>
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
                        <a href="#" style="margin-left:10px;font-size: 1.1em" ng-click="setExpandCollapse()" id="linkExpand" onclick="return false;"><%--<img src="../images/collapseall.jpg" alt=""/>--%> 
                        <span class='glyphicon glyphicon-collapse-down'></span>
                        Collapse All</a>
                      </div>
                  </div>
                  <div class="col-sm-1">
                  </div>
                  <div class="col-sm-10 text-right">
                    <!--Cosmo service status - start-->
                    <div>
                        <p class="text-right legend">
                            <img alt="" src="../images/clock_dark.png" /> All scheduled &nbsp;
                            <img alt="" src="../images/clock_blue.png" /> Partially scheduled&nbsp;
                            <img alt="" src="../images/clock_white.png" /> Yet to be scheduled&nbsp;
                            <img alt="" src="../images/clock_blank.png" /> No service available&nbsp;
                            <a href="#" id="printPage" title="Print"  data-toggle="tooltip" data-placement="top"><img alt="Print" src="../images/print.png" /> Print</a>&nbsp;&nbsp;
                        </p>
                        
                    </div>
                    <!--Cosmo service status - end-->
                  </div>
              </div>


            <div id="divErrorMessage" class="" ng-show="errorMessage != ''">
                <span class="glyphicon glyphicon-info-sign"></span>
                {{errorMessage}}
            </div>
		    <table class="table-environemnt"  ng-repeat="environment in environmentList | orderBy:sortType:sortReverse">
			    <thead>
				    <tr class="bg-warning">
					    <th ng-click="toggle_it(environment.EnvID)" id="arrowImg_{{environment.EnvID}}" class="borderless-td caret-extension" width="20px"><span class="glyphicon glyphicon-triangle-bottom"></span></th>
					    <th class="borderless-td col-md-3"  >{{environment.EnvName}}</th>
                        <th class="borderless-td col-md-2"  >Monitor:  <img ng-src="../images/{{environment.EnvIsMonitor && 'tick16.jpg' || 'close5.jpg'}}"/></th>
                        <th class="borderless-td col-md-2">Notify:  <img ng-src="../images/{{environment.EnvIsNotify && 'tick16.jpg' || 'close5.jpg'}}"/></th>
                        <th class="borderless-td col-md-2"  >Consolidate:  <img ng-src="../images/{{environment.EnvIsServiceConsolidated && 'tick16.jpg' || 'close5.jpg'}}"/></th>
                        <th class="borderless-td col-md-3"  >Mail Frequency: <span class="text-danger">{{environment.EnvMailFrequency}} min </span></th>
                        
                        <th class="borderless-th text-right"><a href="#" ng-click="EditEnvironmentConfiguration(environment,'e')"><span style="margin-right:10px"  class="rowedit" title="Schedule" data-toggle="tooltip" data-placement="bottom"></span></a></th>
                        <th class="borderless-th text-center"><a href="#" ng-click="DeleteEnvironment(environment)"><span  style="" class="rowdelete" title="Schedule" data-toggle="tooltip" data-placement="bottom"></span></a></th>
                        <th class="borderless-th text-center" style="width: 4px; margin-left: 0px"><a href="#" ng-click="EditSchedule(environment)"><span style="margin-right: -2px"  ng-class="[getSchedularClass(environment.SchedularStatus)]" title="{{environment.SchedularTitle}}" data-toggle="tooltip" data-placement="bottom"></span></a></th>
                        <th class="borderless-td" style="width: 1px"></th>
                        <%--<th class="col-md-1">
                            <div class="row" style="margin-left:-20px;">
                                <div class="col-md-4 text-center">
                                    <a href="#"><span class="rowedit" title="Schedule" data-toggle="tooltip" data-placement="bottom"></span></a>
                                </div>    
                                <div class="col-md-4 text-center">
                                    <a href="#"><span class="rowdelete" title="Schedule" data-toggle="tooltip" data-placement="bottom"></span></a>
                                </div>    
                                <div class="col-md-4 text-center">
                                    <a href="#"><span ng-class="[getSchedularClass(environment.SchedularStatus)]" title="{{environment.SchedularTitle}}" data-toggle="tooltip" data-placement="bottom"></span></a>
                                </div>    
                            </div>
                        </th>--%>
                        

				    </tr> 
			    </thead>
			    <tbody>
				    <tr>
					    <td class="borderless-td" id="row_{{environment.EnvID}}" colspan="10">
						    <table class="table table-bordered inner-border  table-hover">
							    <thead >
								    <tr >
									    <th class="text-center">#</th>
									    <th class="col-md-1">
									        <a href="#" ng-click="sortType = 'EnvDetServiceTypeDesc'; sortReverse = !sortReverse;" onclick="return false;">
									        Service Type
                                            <span ng-show="sortType == 'EnvDetServiceTypeDesc' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetServiceTypeDesc' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-sm-2">
									        <a href="#" ng-click="sortType = 'EnvDetHostIP'; sortReverse = !sortReverse;" onclick="return false;">
									        Host / IP
                                            <span ng-show="sortType == 'EnvDetHostIP' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetHostIP' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'EnvDetPort'; sortReverse = !sortReverse;" onclick="return false;">
									        Port
                                            <span ng-show="sortType == 'EnvDetPort' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetPort' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-2">
									        <a href="#" ng-click="sortType = 'EnvDetDescription'; sortReverse = !sortReverse;" onclick="return false;">
									        Description
                                            <span ng-show="sortType == 'EnvDetDescription' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetDescription' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1">
									        <a href="#" ng-click="sortType = 'EnvDetLocation'; sortReverse = !sortReverse;" onclick="return false;">
									        Location
                                            <span ng-show="sortType == 'EnvDetLocation' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetLocation' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'EnvDetIsMonitor'; sortReverse = !sortReverse;" onclick="return false;">
									        Monitor
                                            <span ng-show="sortType == 'EnvDetIsMonitor' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetIsMonitor' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'EnvDetIsNotify'; sortReverse = !sortReverse;" onclick="return false;">
									        Notify
                                            <span ng-show="sortType == 'EnvDetIsNotify' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetIsNotify' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'EnvDetIsServiceConsolidated'; sortReverse = !sortReverse;" onclick="return false;" >
									        Consolidate
                                            <span ng-show="sortType == 'EnvDetIsServiceConsolidated' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetIsServiceConsolidated' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
									    <th class="col-md-1 text-center">
									        <a href="#" ng-click="sortType = 'EnvDetMailFrequency'; sortReverse = !sortReverse;" onclick="return false;">
									        Mail Frequency<br />(in min)
                                            <span ng-show="sortType == 'EnvDetMailFrequency' && !sortReverse" class="fa fa-caret-down"></span>
                                            <span ng-show="sortType == 'EnvDetMailFrequency' && sortReverse" class="fa fa-caret-up"></span>
                                            </a>
									    </th>
                                        <th style="width:50px"></th>
                                        <th style="width:50px"></th>
                                        <th style="width:50px"></th>
								    </tr>
							    </thead>
							    <tbody>
								    <tr ng-repeat="entity in environment.EnvDetailsList | orderBy:sortType:sortReverse">
									    <td class="text-center">{{$index + 1}}</td>
									    <td  nowrap="nowrap">{{entity.EnvDetServiceTypeDesc}}</td>
									    <td>{{entity.EnvDetHostIP | lowercase}}</td>
									    <td class="text-center">{{entity.EnvDetPort}}</td>
									    <td nowrap="nowrap">{{entity.EnvDetDescription}}</td>
									    <td>{{entity.EnvDetLocation}}</td>
									    <td class="text-center"><img ng-src="../images/{{entity.EnvDetIsMonitor && '15-Tick-icon.png' || 'false_icon.png'}}"/></td>
									    <td class="text-center"><img ng-src="../images/{{entity.EnvDetIsNotify && '15-Tick-icon.png' || 'false_icon.png'}}"/></td>
									    <td class="text-center"><img ng-src="../images/{{entity.EnvDetIsServiceConsolidated && '15-Tick-icon.png' || 'false_icon.png'}}"/></td>
									    <td class="text-center">{{entity.EnvDetMailFrequency}}</td>
                                        <td class="text-center"><a href="#" ng-click="EditEnvironmentConfiguration(entity,'c')"><span class="rowedit" title="edit" data-toggle="tooltip" data-placement="bottom"></span></a></td>
                                        <td class="text-center"><a href="#" ng-click="DeleteEnvironmentConfiguration(entity)"><span class="rowdelete" title="delete" data-toggle="tooltip" data-placement="bottom"></span></a></td>
                                        <td class="text-center"><a href="#" ng-click="EditScheduleCSM(entity,environment)"><span ng-class="[getSchedularClass(entity.SchedulerSummary)]" title="{{entity.SchedulerSummary}}" data-toggle="tooltip" data-placement="bottom"></span></a></td>
								    </tr>
							    </tbody>
						    </table>							
					    </td>
				    </tr>
			    </tbody>
		    </table>
		  </div>
		</div>        
        <p>&nbsp;</p>
    </div>
    
    <script src="../script/environment/environment_operation.js" type="text/javascript"></script>
    <script src="../script/environment/appEnvironment.js" type="text/javascript"></script>	
    <script src="../script/simple_modal.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

        });
     </script>

        
</asp:Content>
