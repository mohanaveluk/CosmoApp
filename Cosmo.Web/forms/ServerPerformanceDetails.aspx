<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerPerformanceDetails.aspx.cs" Inherits="Cosmo.Web.forms.ServerPerformanceDetails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../style/common.css" rel="stylesheet" type="text/css" />
    <link href="../style/top-navigation.css" rel="stylesheet" type="text/css" />
    <link href="../style/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    
    <script src="../script/jquery-1.11.1.min.js" type="text/javascript"></script>
    <script src="../style/js/moment.min.js" type="text/javascript"></script>
    <script src="../style/js/bootstrap.min.js" type="text/javascript"></script>

    <script src="../script/angular.min.js" type="text/javascript"></script>
    <script src="../script/angular-messages.min.js" type="text/javascript"></script>
    <script src="../script/spinning.js" type="text/javascript"></script>
    <script src="../script/environment/appServerPerformance.js" type="text/javascript"></script>
    

    <link href="../style/chartist.css" rel="stylesheet" type="text/css" />
    <script src="../script/chartist.js" type="text/javascript"></script>
    <script src="../script/chartist-plugin-legend.js" type="text/javascript"></script>
    <link href="../style/chartist_legend.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap" ng-cloak>
        <div id="divErrorMessage">{{errorMessage}}</div>
        <form id="Form1" runat="server">
            <asp:HiddenField runat="server" ID="hidHostName"/>
                    <div class="form-group"><h5>Host Name / IP: {{hostname}}</h5></div>
                    <table class="table table-bordered" style="width: 98.6%;max-height: 200px">
                        <tr style="max-height: 200px">
                            <td class="col-sm-3" style="vertical-align: top" >
                                <span class="label label-default">Storage - Current</span>
                                <div  style="margin-top: 5px"></div>
                                <table class="table table-hover">
                                    <tr ng-repeat="driveInfo in ServerCurrentInfo.Performance.Drives">
                                        <td style="position: relative"><img src="../images/harddrive.png" style="width: 25px" alt=""/>
                                            <span style="font-size: .8em">{{driveInfo.Label}} {{driveInfo.Name}} </span>
                                            <div class="progress" style="height: 15px">
                                                <div class="progress-bar progress-bar active" role="progressbar" aria-valuenow="{{driveInfo.UsedSpaceInGb}}" aria-valuemin="0" aria-valuemax="{{driveInfo.TotalSpaceInGb}}" style="width:{{driveInfo.UsedInPercentage}}%">
                                                </div>
                                            </div>
                                            <div class="pspan">{{driveInfo.FreeSpaceInText + " free of " + driveInfo.TotalSpaceInText}}</div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="col-sm-9"  style="vertical-align: top">
                                <span class="label label-default">Storage - Last 30 days</span>
                                <div id="storageLast30Days" class="ct-chart ct-double-Pentago"></div>
                                <div id="legStorage"></div>
                            </td>

                        </tr>
                        <%--<tr>
                            <td class="col-sm-3"><div class="table-bordered text-center">{{ResponseTimeLastPing}}</div></td>
                            <td class="col-sm-3"><div class="table-bordered text-center">{{ResponseTimeLastPing}}</div></td>
                            <td class="col-sm-3"><div class="table-bordered text-center">{{ResponseTimeLastHour}}</div></td>
                            <td class="col-sm-3">
                                <div class="table-bordered text-center" style="max-height: 175px">
                                   <div class="ct-chart ct-perfect-fourth">Last 24 hours</div>
                                </div>
                            </td>
                        </tr>--%>
                    </table>
                    <table class="table table-bordered" style="width: 98.6%">
                        <tr>
                            <td class="col-sm-6" style="vertical-align: top">
                                <span class="label label-default">CPU - Last 24 hours</span>
                                <div id="CpuLast24Hours" class="ct-chart ct-double-octave"></div>
                                <div id="legCpu"></div>
                            </td>
                            <td class="col-sm-6" style="vertical-align: top">
                                <span class="label label-default">Memory - Last 24 hours</span>
                                <div id="MemoryLast24Hours" class="ct-chart ct-double-octave"></div>
                                <div id="legMemory"></div>
                            </td>
                        </tr>
                    </table>
        </form>
    </div>
    <!--Begin Page Progress-->
    <div id="fade-process">
    </div>
    <div id="modal-process">
        <img id="loader" src="../images/ajax-loader.gif" alt="Processing..." />
    </div>

</body>
</html>
