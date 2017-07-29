var loginApp = angular.module('LoginApp', ['ngMessages']);
loginApp.controller('LoginController', [
    '$scope', '$http', '$window', function($scope, $http, $window) {

        $scope.UserName = '';
        $scope.userPassword = '';
        $scope.erroricon = '';
        $scope.loginError = '';
        $scope.LicenseStatus = '';
        $scope.chkRememberMe = false;

        $scope.CheckLogin = function checkLogin() {
            var dataObj = {
                userId: $scope.UserName,
                password: $scope.userPassword,
                rememberMe: $scope.chkRememberMe
            };
            
            if (dataObj.userId == undefined || dataObj.password == undefined) {
                return false;
            }

            
            if ($scope.LicenseStatus === "Failure") {
                showProductExpiry();
                return false;
            }
            
            $http.post('../forms/Generic.aspx/ValidateLoginUser', dataObj)
                .success(function(data, status, headers, config) {
                    $scope.loginError = data.d;
                    if (data.d.indexOf('.aspx') >= 0) {
                        setRememberMe($scope.UserName);
                        $scope.loginError = 'User has been logged successfully';
                        $window.location.href = '../forms/' + data.d;
                    } else if (data.d !== '') {
                        $scope.loginError = data.d;
                    } else if (data.d === '') {
                        $scope.loginError = "System error. Please contact Administrator";
                    }
                })
                .error(function(data, status, headers, config) {
                    alert('failure message :' + JSON.stringify({ data: data }));
                });
            return true;
        };

        addEventListener('load', loadIt, false);
        function loadIt() {
            $scope.LicenseStatus = document.getElementById("ContentPlaceHolder1_hidLicenseStatus").value;
            console.log($scope.LicenseStatus);
        }

        $scope.showProductExpiry = function() {
            $('#myModal').modal();
        };

        function setRememberMe(userId) {
            var isRemember = $("#chkRememberMe").is(":checked");

            if (isRemember) {
                $.cookie('userId', userId, { expires: 30 });
                $.cookie('remember', true, { expires: 30 });
            } else {
                //$.removeCookie('userId', { path: '/' });
                ///$.removeCookie('remember', { path: '/' });
                $.cookie('userId', null);
                $.cookie('remember', null);
            }
        }

    }
]);

var getRemember = function() {
    var remember = $.cookie('remember');
    var userId = $.cookie('userId');

    if (userId === "null" || userId === undefined) userId = "";
    if (remember === "null" || remember === undefined) remember = false;

    $('#txtUserName').val(userId);
    $('#chkRememberMe').prop('checked', remember);

    var scope = angular.element(document.getElementById("MainWrap")).scope();
    scope.$apply(function() {
        scope.UserName = userId;
        scope.chkRememberMe = remember;
    });
};

