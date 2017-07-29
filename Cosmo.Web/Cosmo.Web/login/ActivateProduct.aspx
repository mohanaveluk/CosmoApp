<%@ Page Title="Activate Product" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="ActivateProduct.aspx.cs" Inherits="Cosmo.Web.login.ActivateProduct" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="container">
      <div ng-controller='mainController'>
        <form id="Form1" class="form-horizontal" action="" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
            </asp:ScriptManager>
            <p>&nbsp;</p>
            <div class="text-right text-danger" ><label runat="server" ID="lblExpiry"></label></div>
            <div class="panel panel-primary" >
                <div class="panel-heading text-left">Activate COSMO</div>
              <div class="panel-body">
                  <br>
                  <div class="form-group" >
                    <label for="inputEmail3" class="col-sm-3 control-label">Confirmation Number</label>
                    <div class="col-sm-6" >
                      <input type="text" class="form-control" id="txtConfirmationn" placeholder="Confirmation number" runat="server" ClientIDMode="Static" />
                    </div>
                  </div>
                  <div class="form-group" >
                    <label for="inputEmail3" class="col-sm-3 control-label">Type</label>
                    <label class="checkbox-inline">
                        <input type="radio" name="rdoPackage" id="rdoWeb" value="W" checked="checked"/> Web Only
                    </label>
                    <label class="checkbox-inline">
                        <input type="radio" name="rdoPackage" id="rdoMobile" value="M" /> Web with Mobile
                    </label>
                  </div>            
                  <div class="form-group" >
                    <label for="inputEmail3" class="col-sm-3 control-label">Internal key</label>
                    <div class="col-sm-6" >
                        <input type="text" class="form-control" id="txtInternalKey" placeholder="Internal  key" runat="server" readonly="readonly" />
                        <input type="hidden" ID="hixKey" runat="server"/>
                    </div>
                        <button id="btnCopy" class="col-sm-1 btn btn-default button-text-adjust" data-clipboard-action="copy" data-clipboard-target="#<%=txtInternalKey.ClientID %>" onclick="return false;">Copy</button>
                        <%--<asp:Button runat="server" ID="btnClip" Text="Copy" class="col-sm-1 btn btn-default" onclick="btnClip_Click"/>--%>
                  </div>            
                  <div class="form-group">
                    <label for="inputEmail3" class="col-sm-3 control-label">Activation Key</label>
                    <div class="col-sm-9" style="width:800px">
                        <input type="text" class="form-control" id="txtActivationKey" placeholder="Activation key" runat="server"/>
                    </div>
                  </div> 
                  <br>
                  <div class="form-group text-left">
                    <div class="col-sm-offset-4 col-sm-8">
                      <button type="button" id="Button1" class="btn btn-primary button-text-adjust" onclick="window.history.back();">Back</button>
                      <button type="button" id="btnRequest" class="btn btn-primary button-text-adjust" onclick="SendMail()">Request Activation Key</button>
                      <button type="button" ID="btnSubmit" class="btn btn-primary button-text-adjust" runat="server" onclick="ValidateKey()">Activate</button>
                      <div style="width: 700px" id="lblResult"></div>
                    </div>
                    <div class="col-sm-offset-1 col-sm-11">
                        <br/>
                          <b>Note:</b> Request activation key from Cosmo and once you receive the key, enter in Activation Key box and submit to validate and proceed to use Cosmo Application 
                    </div>
                  </div>                  
                </div>
            </div>            
        </form>
      </div>
  </div>
  
<!-- Button trigger modal -->

<!-- Modal -->
<div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <%--<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>--%>
        <h4 class="modal-title" id="myModalLabel">Activation Status</h4>
      </div>
      <div class="modal-body">
        Cosmo product has been activated already.
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default button-text-adjust" onclick="history.back()" data-dismiss="modal">Back</button>
      </div>
    </div>
  </div>
</div>
    
    <script src="../script/clipboard/clipboard.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            if ($('#<%=txtConfirmationn.ClientID %>').val() === '') {
                $('#btnRequest').attr('disabled', 'disabled');
            }

            if ($('#<%=txtActivationKey.ClientID %>').val() === '') {
                $('#<%=btnSubmit.ClientID %>').attr('disabled', 'disabled');
            }

            $("select, input").keyup(function () {
                if ($('#<%=txtActivationKey.ClientID %>').val() !== '' && $('#<%=txtActivationKey.ClientID %>').val().length > 11) {
                    $('#<%=btnSubmit.ClientID %>').removeAttr('disabled');
                } else {
                    $('#<%=btnSubmit.ClientID %>').attr('disabled', 'disabled');
                }

                if ($('#<%=txtConfirmationn.ClientID %>').val() !== '' && $('#<%=txtConfirmationn.ClientID %>').val().length >= 5) {
                    $('#btnRequest').removeAttr('disabled');
                } else {
                    $('#btnRequest').attr('disabled', 'disabled');
                }
            });

            var clipboard = new Clipboard('#btnCopy');

            clipboard.on('success', function (e) {
                console.log(e);
            });

            clipboard.on('error', function (e) {
                console.log(e);
            });

            $("input[type='radio']").change(function () {
                var selection = $(this).val();
                console.log("Radio button selection changed. Selected: " + selection);
                window.PageMethods.rdoPackage_CheckedChanged(selection, OnSuccessInternalKey);
            });

            $('#txtConfirmationn').focus();
        });
        $(function () { $('#txtConfirmationn').focus(); });

        function SendMail() {
            var link = 'mailto:contactus@cosmo.com?subject=Activation Key for ' + $('#<%=txtConfirmationn.ClientID %>').val()
            //+ ' for ' + $('#<%=txtInternalKey.ClientID %>').val()
                + '&body=Hi Cosmo Team,' + encodeURI('\n') + encodeURI('\n')
                + 'Request for an activation key for Confirmation number: ' + $('#<%=txtConfirmationn.ClientID %>').val() + ' with an internal key of '
                + $('#<%=txtInternalKey.ClientID %>').val() + encodeURI('\n')
                + encodeURI('\n') + encodeURI('\n') + encodeURI('\n') + encodeURI('\n');
            console.log(link);
            window.open(link, 'email');
        }

        function ValidateKey() {

            var activationKey = $('#<%=txtActivationKey.ClientID %>').val();
            var confirmationNumber = $('#<%=txtConfirmationn.ClientID %>').val();

            window.PageMethods.ValidateActivationKey2(activationKey, confirmationNumber, OnSuccess);

            //            $.ajax({
            //                url: "ActivationKey.aspx/ValidateActivationKey",
            //                type: "POST",
            //                data: JSON.stringify(data),
            //                dataType: "json",
            //                contentType: "application/json; charset=utf-8",
            //                sunccess: function(response) {
            //                    console.log(response);
            //                },
            //                error: function(response) {
            //                    console.log(response);
            //                }
            //            });

        }

        

        function OnSuccess(response, userContext, methodName) {
            console.log(response);
            if (response.indexOf('Great') >= 0 || response.indexOf('Success') >= 0) {
                $('#lblResult').removeClass("alert alert-danger").addClass("alert alert-success");
                $('#lblResult').html(response);
                $('#btnRequest').attr('disabled', 'disabled');
                $('#btnSubmit').attr('disabled', 'disabled');
                ProductActivated(response);
            } else if (response.indexOf('Trial') >= 0) {
                $('#lblResult').removeClass().addClass("alert alert-info");
                $('#lblResult').html(response);
                $('#btnRequest').attr('disabled', 'disabled');
                $('#<%=btnSubmit.ClientID %>').attr('disabled', 'disabled');
            } else if (response.indexOf('Oops') >= 0) {
                $('#lblResult').removeClass("alert alert-success").addClass("alert alert-danger");
                $('#lblResult').html(response);

            }
        }

        function OnSuccessInternalKey(response, userContext, methodName) {
            console.log(response);
            $('#<%=txtInternalKey.ClientID %>').val(response);
        }

        function ProductActivated(licenseStatus) {
            //            $('#myModal').modal({
            //                show: true,
            //                keyboard: false,
            //                backdrop: 'static'
            //            });

            var activatedBox = '<div class="alert alert-info">' + licenseStatus + '</div>';
            activatedBox += '<div class="form-group"><div class="col-sm-offset-5 col-sm-2 text-center"><button type="button" id="btnLogin" class="btn btn-primary button-text-adjust" onclick="loginToCosmo()">Login to Cosmo</button></div></div>';
            $('div.panel-body').html(activatedBox);
            //$('.trial').attr("style", "visibility:hidden");

            $('#divLicenseStatus').html("<div class='alert alert-success' role='alert'>Product Activated</div>");
            //$('#lblStatus').removeClass("lic-trial").addClass("lic-activated");
            //$('#divLicenseStatus').html('');
        }

        function loginToCosmo() {
            location.href = "../index.html";
        }


        function ActivatedAlready() {
            $('#myModal').modal(
                {
                    keyboard: false
                }
            );
        }

    </script>    

</asp:Content>
