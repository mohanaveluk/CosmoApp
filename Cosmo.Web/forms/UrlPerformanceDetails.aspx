<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UrlPerformanceDetails.aspx.cs" Inherits="Cosmo.Web.forms.UrlPerformanceDetails" %>

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

<%--    <script src="../script/fusion/fusioncharts.js" type="text/javascript"></script>--%>
    <script src="../script/angular.min.js" type="text/javascript"></script>
    <script src="../script/angular-messages.min.js" type="text/javascript"></script>
    <script src="../script/spinning.js" type="text/javascript"></script>
    <script src="../script/environment/appUrlPerformance.js" type="text/javascript"></script>
<%--    <script src="../script/fusion/angular-fusioncharts.min.js" type="text/javascript"></script>--%>
        
    <%--<script src="../script/Chart.js" type="text/javascript"></script>--%>
<%--    <script src="../script/canvasjs.min.js" type="text/javascript"></script>--%>
    <link href="../style/chartist.css" rel="stylesheet" type="text/css" />
    <script src="../script/chartist.js" type="text/javascript"></script>

</head>
<body>
    <div ng-app="DashboardApp" ng-controller="DashboardController" id="MainWrap" ng-cloak>
        <div id="divErrorMessage">{{errorMessage}}</div>
        <form runat="server">
        <asp:HiddenField runat="server" ID="hidEnvId"/>

                    <table class="table table-borderless">
                        <tr>
                            <td class="col-sm-4"><h5>Environment: {{EnvName}}</h5></td>
                            <td class="col-sm-8" colspan="2"><h5>URL: {{Adress}}</h5></td>
                            
                        </tr>
                    </table>
                    <table class="table table-borderless">
                        <tr>
                            <td class="col-sm-3" style="vertical-align: top;height: 180px;"><div class="table-bordered text-center jumbotron">{{ResponseTimeLastPing}} <br/><span class="bot-text">Last Ping (in secs)<br/>at {{LastPingDateTime | date:'MM/dd/yyyy HH:mm:ss'}}</span></div></td>
                            <td class="col-sm-3" style="vertical-align: top;height: 180px;"><div class="table-bordered text-center jumbotron">{{ResponseTimeLastHour}} <br/><span class="bot-text">Average Ping (in secs)<br /> Last 1 hour<br/></span></div></td>
                            <td class="col-sm-6" style="vertical-align: top;">
                                <div class="table-bordered" >
                                   <%--<div id="chartContainer" style="height: 185px; width: 100%;padding-top: -20px"></div>--%>
                                   <span class="label label-default">Last 24 hours</span>
                                   <div class="ct-chart ct-perfect-fourth" style="max-height: 175px;overflow-y: hidden"></div>
                                <div id="legResponseTime"></div>
                                </div>
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
    <!--End of Page Progress-->    
<script>
    
    //PopulateDataToChart();
</script>
</body>
</html>
