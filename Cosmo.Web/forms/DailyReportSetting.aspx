<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DailyReportSetting.aspx.cs" Inherits="Cosmo.Web.forms.DailyReportSetting" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />

    <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../style/js/moment.min.js" type="text/javascript"></script>
    <script src="../script/angular.min.js" type="text/javascript"></script>
    <script src="../script/angular-messages.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.linq.min.js" type="text/javascript"></script>
    <script src="../script/report/dailyReport.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" class="form-horizontal">
        <div class="panel panel-primary" style="height: 400px;" ng-app="dragdrop"  ng-controller="ctrlDualList" id="MainWrap">
            <!-- Default panel contents -->
            <div class="panel-heading">
                Daily Report Subscription
            </div>
            <div class="panel-body">
                <div class="alert alert-danger" ng-show="errorMessage != ''" style="display: none" id="divErrorMessage">
                    <span class="glyphicon glyphicon-info-sign"></span>
                    {{errorMessage}}
                </div>
                <div style="background-color: #fff;">
                    <div class="col-sm-10 col-sm-offset-1">
                        <div class="form-group">
                            <div class="alert alert-warning" id="div-alert" style="height: 50px;display: none">
                            <span class="glyphicon glyphicon-info-sign"></span>
                            <label class="control-label" id="label-alert" style="margin-top: -20px"></label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-4 control-label" id="Label1">Report Name :</label>
                        <div class="col-sm-4">  
                            <select id="drpReportName" ng-model="reportName" class="form-control">
                                <option value="Daily">Daily Status Report</option>
                            </select>
                            <asp:HiddenField ID="hidIsDataUpdated"  />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputEmail3" class="col-sm-4 control-label" id="Label2">Start at:</label>
                        <div class="col-sm-3">
                            <select id="drpReportTime" ng-model="reportTime" class="form-control">
                                <option value="0">12.00 AM</option>
                                <option value="1">1.00 AM</option>
                                <option value="2">2.00 AM</option>
                                <option value="3">3.00 AM</option>
                                <option value="4">4.00 AM</option>
                                <option value="5">5.00 AM</option>
                                <option value="6">6.00 AM</option>
                                <option value="7">7.00 AM</option>
                                <option value="8">8.00 AM</option>
                                <option value="9">9.00 AM</option>
                                <option value="10">10.00 AM</option>
                                <option value="11">11.00 AM</option>
                                <option value="12">12.00 PM</option>
                                <option value="13">1.00 PM</option>
                                <option value="14">2.00 PM</option>
                                <option value="15">3.00 PM</option>
                                <option value="16">4.00 PM</option>
                                <option value="17">5.00 PM</option>
                                <option value="18">6.00 PM</option>
                                <option value="19">7.00 PM</option>
                                <option value="20">8.00 PM</option>
                                <option value="21">9.00 PM</option>
                                <option value="22">10.00 PM</option>
                                <option value="23">11.00 PM</option>
                            </select>
                            <asp:HiddenField ID="HiddenField1"/>
                        </div>
                        <div class="col-sm-2" >
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" id="chkDisableReport" value="Disable" ng-model="reportDisable" /> Disable
                                </label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-4  col-sm-offset-1">
                          <div class="list-group scrollUserBox" id="list1">
                            <a href="#" class="list-group-item active">Available Users
                              <input title="Toggle all" ng-click="toggleA()" ng-model="toggle" value="{{toggle}}" type="checkbox" class="pull-right"></a>
                                <span ng-repeat="user in listA">
                                  <a href="#" class="list-group-item">{{user.userEmailAddress}}
                                    <input ng-click="selectA(user.id)" name="selectedA[]" value="{{user.id}}" ng-checked="selectedA.indexOf(user.id) > -1" type="checkbox" class="pull-right">
                                  </a>
                                </span>
                          </div>
                        </div>
                        <div class="col-sm-2">
                          <div class="btn-group scrollUserNavigationBox">
                            <button title="Send to list 1" class="btn btn-default" ng-click="bToA()" onclick="return false"><i class="glyphicon glyphicon-chevron-left"></i></button>
                            <button title="Send to list 2" class="btn btn-default" ng-click="aToB()" onclick="return false"><i class="glyphicon glyphicon-chevron-right"></i></button>
                
                          </div>
                        </div>
                        <div class="col-sm-4">
                          <div class="list-group scrollUserBox" id="list2">
                            <a href="#" class="list-group-item active">Selected Users
                              <input title="Toggle all" ng-click="toggleB()" ng-model="toggle" value="{{toggle}}" type="checkbox" class="pull-right">
                            </a>
                            <span ng-repeat="user in listB">
                              <a href="#" class="list-group-item">{{user.userEmailAddress}}
                                <input ng-click="selectB(user.id)" name="selectedB[]" value="{{user.id}}" ng-checked="selectedB.indexOf(user.id) > -1" type="checkbox" class="pull-right">
                              </a>
                            </span>
                          </div>
                        </div>
                    </div>
                    <div class="form-group text-center">
                        <div > <%--class="col-sm-offset-2 col-sm-10"--%>
                        <button type="button" class="btn btn-primary" id="btnCreate" ng-click="UpdateEmailSubscription()" >Save</button>
                        <button type="button" class="btn btn-default" id="btnCancel" >Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <p>&nbsp;</p>
        <!--Begin Page Progress-->
        <div id="fade-process">
        </div>
        <div id="modal-process">
            <img id="loader" src="../images/ajax-loader.gif" alt="Processing..." />
        </div>
        <!--End of Page Progress-->
    </form>
    <script src="../script/spinning.js" type="text/javascript"></script>
</body>
</html>
