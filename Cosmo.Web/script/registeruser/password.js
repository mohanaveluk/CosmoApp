var ValidatePassword = function(password) {
    var pattern1 = "((?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})";
    var pattern2 = "(?=^.{8,}$)(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9])(?=.*[@#$%]).*";
    if (pattern2.test(password)) {
        return true;
    } else {
        return false;
    }
};

//$('#txtPassword').on('input blur change', function () {
//    consol
//});

var ValidatePasswordRule = function(password, fn, ln, email) {
    if (password === '') return false;
    $('#divPasswordError').html("");

    if (fn !== "" && password.indexOf(fn) >= 0) {
        $('#divPasswordError').html("Password should not contain First Name");
        $('#divPasswordError').closest('.form-group').removeClass().addClass('form-group has-error');
        return false;
    } else {
        $('#divPasswordError').closest('.form-group').removeClass().addClass('form-group');
    }

    if (ln !== "" && password.indexOf(ln) >= 0) {
        $('#divPasswordError').html("Password should not contain Last Name");
        $('#divPasswordError').closest('.form-group').removeClass().addClass('form-group has-error');
        return false;
    } else {
        $('#divPasswordError').closest('.form-group').removeClass().addClass('form-group');
    }

    if (email !== "" && password.indexOf(email) >= 0) {
        $('#divPasswordError').html("Password should not contain Email");
        $('#divPasswordError').closest('.form-group').removeClass().addClass('form-group has-error');
        return false;
    } else {
        $('#divPasswordError').closest('.form-group').removeClass().addClass('form-group');
    }
    return true;
}

$(function() {
    $('#txtPassword1').on("change", function () {
        var fn = $('#txtFirstName').val();
        var ln = $('#txtLastName').val();
        var email = $('#txtEmail').val();
        var scope = angular.element(document.getElementById("MainWrap")).scope();
        scope.$apply(function () {
            scope.ValidatePassword();
        });
    });
});