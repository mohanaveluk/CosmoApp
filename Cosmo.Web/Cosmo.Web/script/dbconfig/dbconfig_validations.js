	    $(document).ready(function () {
	        //$(document).tooltip({
	        //    show: { effect: "slideDown", delay: 50 }
	        //});
	        $("#drpDatabaseType").change(function () {
	            $("#drpAuthenticationType").val("2").change();
	            if ($("#drpDatabaseType option:selected").val() === "1") {
	                $("#divAuthentication").addClass("hide");
	                $("#divPort").removeClass("hide");
	            } else if ($("#drpDatabaseType option:selected").val() === "2") {
	                $("#divAuthentication").removeClass("hide");
	                $("#divPort").addClass("hide");
	            }
	        });


	        $('#drpAuthenticationType').change(function () {
	            if ($('#drpAuthenticationType option:selected').val() == '2') {
	                $('#txtUserID').val('sa');
	                $('#txtUserID').prop('disabled', false);
	                $('#txtPasswrd').prop('disabled', false);
	            }
	            else {
	                $('#txtUserID').val('');
	                $('#txtPasswrd').val('');
	                $('#txtUserID').prop('disabled', true);
	                $('#txtPasswrd').prop('disabled', true);
	            }
	        });
	        $("#frmSubmit").click(function () {
	            ctrlFocus = '';
	            var Errors = false;
	            $('.errors').remove();
	            if ($('#txtServerName').val() === '') {
	                $('#txtServerName').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                setCtrlFocus('txtServerName');
	                Errors = true;
	            }

	            /*if ($('#drpAuthenticationType option:selected').val() == '2') {
	            $('#drpAuthenticationType').after("<span class='errors'>&nbsp;<img src='../images/info4.jpg' title='Please select service type'></span>");
	            setCtrlFocus('drpAuthenticationType');
	            Errors = true;
	            }*/

	            if ($('#txtDatabaseName').val() === '') {
	                $('#txtDatabaseName').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                setCtrlFocus('txtDatabaseName');
	                Errors = true;
	            }
                
	            if ($('#drpAuthenticationType option:selected').val() === '2') {
	                if ($('#txtUserID').val() === '') {
	                    $('#txtUserID').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                    setCtrlFocus('txtUserID');
	                    Errors = true;
	                }

	                if ($('#txtPasswrd').val() === '') {
	                    $('#txtPasswrd').closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                    setCtrlFocus('txtPasswrd');
	                    Errors = true;
	                }
	            }
                
	            $("input").keyup(function () {
	                if ($(this).val() === '')
	                    $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                else
	                    $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
	            });

	            $("select, input").change(function () {
	                if ($(this).val() === '')
	                    $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                else
	                    $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
	            });

	            $("select, input").blur(function () {
	                if ($(this).val() === '')
	                    $(this).closest('.form-group').removeClass('form-group').addClass('form-group has-error');
	                else
	                    $(this).closest('.form-group').removeClass('form-group has-error').addClass('form-group ');
	            });

	            if (Errors === true) {
	                $('#ContentPlaceHolder1_udpMessage').html('<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>Warning!</strong> Please fill the highlighted inputs</div>');
	                return false;
	            } else {
	                //$("#frmSubmit").after("<div class='errors_db'><span><img src='../images/progress.gif' title='Please wait while processing...'/> Processing...</span></div>");
	                $(this).val('Validating...');
	                openModal();
	                return true;
	            }
	        });

	        $("#txtPort").keydown(function (e) {
	            // Allow: backspace, delete, tab, escape, enter and ( . for 190)
	            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
	            // Allow: Ctrl+A, Command+A
                        (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
	            // Allow: home, end, left, right, down, up
                        (e.keyCode >= 35 && e.keyCode <= 40)) {
	                // let it happen, don't do anything
	                return;
	            }
	            // Ensure that it is a number and stop the keypress
	            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
	                e.preventDefault();
	            }
	        });
	    });

	    function setCtrlFocus(name) {
	        if (name !== '') {
	            $('#' + name).focus();
	        }
	    }

	    function openModal() {
	        document.getElementById('modal-process').style.display = 'block';
	        document.getElementById('fade-process').style.display = 'block';
	    }

	    function closeModal() {
	        document.getElementById('modal-process').style.display = 'none';
	        document.getElementById('fade-process').style.display = 'none';
	    }