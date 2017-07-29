
var registerApp = angular.module('RegisterApp', ['ngMessages','angularUtils.directives.dirPagination']);
registerApp.controller('RegisterController', [
    '$scope', '$http', function($scope, $http) {

        $scope.userID = '0';
        $scope.FirstName = '';
        $scope.LastName = '';
        $scope.email = '';
        $scope.message = '';
        $scope.password = '';
        $scope.userList = {};
        $scope.currentPage = 1;
        $scope.pageSize = 5;

        //--http://jsfiddle.net/jralston/czLkf4xu/
        $scope.userStatus = [
            { status: 'active', option: 'Active', selected: 'true' },
            { status: 'inactive', option: 'Inactive', selected: 'false' }
        ];
        $scope.selectedStatus = $scope.userStatus[0];

        $scope.setDefault = function(status) {
            angular.forEach($scope.userStatus, function(u) {
                u.selected = false; //set them all to false
            });
            $scope.selectedStatus = status;
            status.selected = true;
            //console.log(status);
        };
        $scope.setUserStatusByEdit = function(mode) {
            angular.forEach($scope.userStatus, function(k, u) {
                if (u.status == mode) {
                    $scope.selectedStatus = $scope.userStatus[k];
                    $scope.setDefault(u);
                    return;
                }
            });
        };

        $scope.ClearForms = function() {
            document.getElementById('txtFirstName').value = '';
            document.getElementById('txtLastName').value = '';
            document.getElementById('txtEmail').value = '';
            document.getElementById('txtPassword').value = '';
            //document.getElementById('hidRoles').value = '';
            $scope.selectedRoles = [];
            $scope.ClearAllRoles();
            $scope.getUsers('0');
            $scope.userID = '0';

            /*
            $scope.userId = '0';
            $scope.FirstName = '';
            $scope.LastName = '';
            $scope.email = '';
            $scope.password = '';
            $scope.user = { status: 'True' };
            $scope.selectedRolesString = '';
            */
        }

        $scope.ClearModels = function() {
            $scope.registerForm.$setPristine();
            //$scope.registerForm.$setValidity();
            $scope.registerForm.$setUntouched();
            $scope.userID = '0';
            $scope.FirstName = '';
            $scope.LastName = '';
            $scope.email = '';
            $scope.message = '';
            $scope.password = '';
            $scope.userStatus[0].selected = true;
            $scope.selectedStatus = $scope.userStatus[0];
            //$scope.setDefault($scope.selectedStatus);

            $scope.selectedRoles = [];
            $scope.ClearAllRoles();
            $scope.selectedRolesString = '';
            $scope.getUsers('0');

        };

        //Get User
        $scope.getUsers = function(userID) {
            openModal();
            var dataObj = {
                uid: userID,
                mode: 'all'
            };
            $http.post('../Forms/Generic.aspx/GetUser', dataObj)
                .success(function(data, status, headers, config) {
                    if (data != null) {
                        $scope.userList = data.d;
                    }
                    closeModal();
                })
                .error(function(data, status, headers, config) {
                    closeModal();
                    alert('failure message :' + JSON.stringify({ data: data }));
                });
        };

        $scope.addRowAsyncAsJSON = function() {

            if (document.getElementById('txtEmail').value == '') {
                $scope.registerForm.$pristine = false;
                return false;
            }
            var dataObj = {
                id: $scope.userID,
                firstName: $scope.FirstName,
                lastName: $scope.LastName,
                email: $scope.email,
                password: $scope.password,
                roles: $scope.selectedRoles,
                status: $scope.selectedStatus.status
            };

            if (!ValidatePasswordRule($scope.password, $scope.FirstName, $scope.LastName, $scope.email)) {
                $scope.registerForm.txtPassword.$invalid = true;
                return false;
            }

            openModal();
            try {

                $http.post('../Forms/Generic.aspx/InsertUpdateUser', dataObj)
                    .success(function(data, status, headers, config) {
                        if (data.d === '-1') {
                            $scope.message = 'This email id is already exists. please choose another email id';
                        } else if (data.d === '1') {
                            $scope.ClearModels();
                            $scope.message = 'User has been registered successfully';
                        } else if (data.d === '2') {
                            $scope.ClearModels();
                            $scope.message = 'User details has been updated successfully';
                        }
                        closeModal();
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        alert('failure message :' + JSON.stringify({ data: data }));
                    });
            } catch (e) {
                closeModal();
                $scope.message = e;
            }

            /*var res = $http({
            method: "POST",
            url: "../Forms/Generic.aspx/InsertUpdateUser?" + transformRequest,
            data: {}
            });*/

            /*
            var res = $http.post('../Forms/Generic.aspx/InsertUpdateUser',dataObj); //JSON.stringify(dataObj)
    
    
            
    
            res.success(function (data, status, headers, config) {
            $scope.message = data;
            });
            res.error(function (data, status, headers, config) {
            alert('failure message :' + JSON.stringify({ data: data }));
            });
    
            $scope.userId = '';
            $scope.FirstName = '';
            $scope.LastName = '';
            $scope.email = '';
            */

        };
        /*$scope.someSelected = function (object) {
        return Object.keys(object).some(function (key) {
        return object[key];
        });
        }*/

        //User Roles
        $scope.Roles = [
            { name: 'adminService', label: 'Administrator', value: '1', selected: 'false', disabled: 'false' },
            { name: 'monitorService', label: 'Dashboard', value: '2', selected: 'false', disabled: 'false' },
            { name: 'addService', label: 'Setup Environment', value: '3', selected: 'false', disabled: 'false' },
            { name: 'restartService', label: 'Restart Services', value: '4', selected: 'false', disabled: 'false' },
            { name: 'reportService', label: 'Reports Only', value: '5', selected: 'false', disabled: 'false' },
            { name: 'reportService', label: 'Mobile', value: '6', selected: 'false', disabled: 'false' }
        ];

        $scope.selectedRoles = [];
        $scope.selectedRolesString = '';

        $scope.roleToggleSelection = function roleToggleSelection(role) {
            var idx = $scope.selectedRoles.indexOf(role.value);
            // is currently selected
            if (idx > -1) { ////&& role.selected == false)
                $scope.selectedRoles.splice(idx, 1);
            }
            // is newly selected //if(idx <= 0 && role.selected == true)
            else {
                $scope.selectedRoles.push(role.value);
            }
            $scope.selectedRolesString = $scope.selectedRoles.toString();
        };

        $scope.DisableRole = function DisableRole(role) {
            //console.log(role.name);
            for (var iRole = 0; iRole < $scope.selectedRoles.length; iRole++) {
                if ($scope.selectedRoles[iRole] == '1' && role.value != '1') {

                    return true;
                } else if ($scope.selectedRoles[iRole] != '1' && role.value == '1') {

                    return true;
                } else
                    return false;
            }
        };

        $scope.toggleRole = function toggleRole(roleSelected, mode) {
            angular.forEach($scope.Roles, function(role) {
                if (roleSelected.value == role.value) {
                    role.selected = mode;
                    //console.log('role.selected :' + role.selected);
                    $scope.DisableRole(role);
                }
            });
        };

        $scope.ClearAllRoles = function ClearAllRoles() {
            angular.forEach($scope.Roles, function(role) {
                role.selected = false;
                //console.log('role.selected :' + role.selected);
                $scope.DisableRole(role);
            });
        };

        //Edit user
        $scope.edituser = function edituser(id) {
            for (var eUser = 0; eUser < $scope.userList.length; eUser++) {

                if ($scope.userList[eUser].UserID == id) {
                    //$scope.ClearModels();

                    $scope.userID = id;
                    $scope.FirstName = $scope.userList[eUser].FirstName;
                    $scope.LastName = $scope.userList[eUser].LastName;
                    $scope.email = $scope.userList[eUser].Email;
                    $scope.password = $scope.userList[eUser].Password;
                    //$scope.setUserStatusByEdit($scope.userList[eUser].IsActive); //{ status: $scope.userList[eUser].IsActive.toString() };
                    if ($scope.userList[eUser].IsActive == true) {
                        $scope.userStatus[0].selected = true;
                        $scope.userStatus[1].selected = false;
                        $scope.selectedStatus = $scope.userStatus[0];
                    } else {
                        $scope.userStatus[0].selected = false;
                        $scope.userStatus[1].selected = true;
                        $scope.selectedStatus = $scope.userStatus[1];
                    }
                    //$scope.setDefault($scope.selectedStatus);
                    $scope.ClearAllRoles();
                    $scope.selectedRoles = [];
                    $scope.tempRole = {};
                    angular.forEach($scope.userList[eUser].UserRoles, function(role, key) {
                        $scope.selectedRoles.push(role.RoleID.toString());
                        $scope.tempRole = {
                            name: role.RoleName,
                            label: role.RoleName,
                            value: role.RoleID,
                            selected: 'true',
                            disabled: 'false'
                        };

                        $scope.toggleRole($scope.tempRole, true);
                    });
                    $scope.selectedRolesString = $scope.selectedRoles.toString();
                    break;
                }
            }
        };

        //Delete User
        $scope.deleteuser = function deleteuser(userID, email) {
            if (confirm("Are you sure to delete the user '" + email + "'")) {
                var dataObj = {
                    uid: userID
                };
                $http.post('../Forms/Generic.aspx/DeleteUser', dataObj)
                    .success(function(data, status, headers, config) {
                        if (data != null) {
                            if (data.d == 1)
                                $scope.message = "User deleted";
                            else if (data.d == -1)
                                $scope.message = "Failed to delete user.";
                            else
                                $scope.message = "Unknown error";

                            $scope.getUsers('0');
                        }
                    })
                    .error(function(data, status, headers, config) {
                        alert('failure message :' + JSON.stringify({ data: data }));
                    });
            }
            return false;
        };

        $scope.ValidatePassword = function() {
            if (!ValidatePasswordRule($scope.password, $scope.FirstName, $scope.LastName, $scope.email)) {
                $scope.registerForm.txtPassword.$invalid = true;
                return false;
            }
            return true;
        }

        $scope.getUsers('0');

    }
]);

registerApp.directive("ngValidate", [function() {
    return {
        require: 'ngModel',
        link: function (scope, elem, attrs, ctrl) {
//            var inputPassword = '#' + attrs.pwCheck;
//            elem.add(inputPassword).on('keyup', function () {
//                scope.$apply(function() {
//                    
//                });
            //            });
            console.log("asas");
        }
    }
}]);

            // Base64 encoding service used by AuthenticationService
            var Base64 = {

                keyStr: 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=',

                encode: function (input) {
                    var output = "";
                    var chr1, chr2, chr3 = "";
                    var enc1, enc2, enc3, enc4 = "";
                    var i = 0;

                    do {
                        chr1 = input.charCodeAt(i++);
                        chr2 = input.charCodeAt(i++);
                        chr3 = input.charCodeAt(i++);

                        enc1 = chr1 >> 2;
                        enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                        enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                        enc4 = chr3 & 63;

                        if (isNaN(chr2)) {
                            enc3 = enc4 = 64;
                        } else if (isNaN(chr3)) {
                            enc4 = 64;
                        }

                        output = output +
                    this.keyStr.charAt(enc1) +
                    this.keyStr.charAt(enc2) +
                    this.keyStr.charAt(enc3) +
                    this.keyStr.charAt(enc4);
                        chr1 = chr2 = chr3 = "";
                        enc1 = enc2 = enc3 = enc4 = "";
                    } while (i < input.length);

                    return output;
                },

                decode: function (input) {
                    var output = "";
                    var chr1, chr2, chr3 = "";
                    var enc1, enc2, enc3, enc4 = "";
                    var i = 0;

                    // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
                    var base64test = /[^A-Za-z0-9\+\/\=]/g;
                    if (base64test.exec(input)) {
                        window.alert("There were invalid base64 characters in the input text.\n" +
                    "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                    "Expect errors in decoding.");
                    }
                    input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

                    do {
                        enc1 = this.keyStr.indexOf(input.charAt(i++));
                        enc2 = this.keyStr.indexOf(input.charAt(i++));
                        enc3 = this.keyStr.indexOf(input.charAt(i++));
                        enc4 = this.keyStr.indexOf(input.charAt(i++));

                        chr1 = (enc1 << 2) | (enc2 >> 4);
                        chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                        chr3 = ((enc3 & 3) << 6) | enc4;

                        output = output + String.fromCharCode(chr1);

                        if (enc3 != 64) {
                            output = output + String.fromCharCode(chr2);
                        }
                        if (enc4 != 64) {
                            output = output + String.fromCharCode(chr3);
                        }

                        chr1 = chr2 = chr3 = "";
                        enc1 = enc2 = enc3 = enc4 = "";

                    } while (i < input.length);

                    return output;
                }
            };
 