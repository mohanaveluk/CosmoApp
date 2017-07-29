
var registerApp = angular.module('RegisterApp', ['ngMessages']);
registerApp.controller('RegisterController', ['$scope', '$http', '$window', function ($scope, $http, $window) {

    $scope.userID = $('#hidUserId').val();
    $scope.FirstName = '';
    $scope.LastName = '';
    $scope.email = '';
    $scope.message = '';
    $scope.password = '';
    $scope.userList = {};
    $scope.currentPassword = '';
    $scope.newPassword = '';
    $scope.confirmPassword = '';

    //--http://jsfiddle.net/jralston/czLkf4xu/
    $scope.userStatus = [
        { status: 'active', option: 'Active', selected: 'true' },
        { status: 'inactive', option: 'InActive', selected: 'false' }
    ];
    $scope.selectedStatus = $scope.userStatus[0];

    $scope.setDefault = function (status) {
        angular.forEach($scope.userStatus, function (u) {
            u.selected = false; //set them all to false
        });
        $scope.selectedStatus = status;
        status.selected = true;
        //console.log(status);
    };
    $scope.setUserStatusByEdit = function (mode) {
        angular.forEach($scope.userStatus, function (k, u) {
            if (u.status == mode) {
                $scope.selectedStatus = $scope.userStatus[k];
                $scope.setDefault(u);
                return;
            }
        });
    };

    $scope.ClearForms = function () {
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

    $scope.ClearModels = function () {
        $scope.registerForm.$setPristine();
        //$scope.registerForm.$setValidity();
        $scope.registerForm.$setUntouched();
        //$scope.message = '';
        $scope.password = '';

        $scope.currentPassword = '';
        $scope.newPassword = '';
        $scope.confirmPassword = '';
        $('#txtCurrentPassword').focus();


    };

    //Get User
    $scope.getUsers = function (userID) {
        var dataObj = {
            uid: userID,
            mode: 'edit'
        };
        $http.post('../forms/Generic.aspx/GetUser', dataObj)
        .success(function (data, status, headers, config) {
            if (data != null) {
                $scope.userList = data.d;
                $scope.edituser(userID);
            }
        })
        .error(function (data, status, headers, config) {
            alert('failure message :' + JSON.stringify({ data: data }));
        });
    };


    $scope.editRowAsyncAsJSON = function() {
        if (document.getElementById('txtNewPassword').value == '' || document.getElementById('txtCurrentPassword').value == '') {
            $scope.registerForm.$pristine = false;
            return false;
        }

        var dataObj = {
            id: $scope.userID,
            email: $scope.email,
            currentPassword: $scope.currentPassword,
            newPassword: $scope.newPassword
        };

        if (!ValidatePasswordRule($scope.password, $scope.FirstName, $scope.LastName, $scope.email)) {
            $scope.registerForm.txtPassword.$invalid = true;
            return false;
        }
        openModal();
        try {

            $http.post('../forms/Generic.aspx/EditProfile', dataObj)
                .success(function(data, status, headers, config) {
                    if (data.d == '-1') {
                        $scope.message = 'User does not exists';
                    } else if (data.d == '0') {
                        $scope.message = 'Current password is wrong';
                    } else if (data.d == '1') {
                        $scope.ClearModels();
                        $scope.message = 'User profile has been updated successfully';
                        //$window.location.href = "ProfileSuccess.aspx";
                        $('.panel-body').html("<p>&nbsp;</p><div class='alert alert-success'>User profile has been updated successfully. Please <a href='../login/Default.aspx'>logout</a> and login again to continue.</div>");
                    }
                    closeModal();
                })
                .error(function (data, status, headers, config) {
                    closeModal();
                    alert('failure message :' + JSON.stringify({ data: data }));
                });
        } catch (e) {
            closeModal();
            $scope.message = e;
        }

    };

    $scope.addRowAsyncAsJSON = function () {
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

        $http.post('../forms/Generic.aspx/InsertUpdate', dataObj)
        .success(function (data, status, headers, config) {
            if (data.d == '-1') {
                $scope.message = 'This email id is already exists. please choose another email id';
            }
            else if (data.d == '1') {
                $scope.message = 'User has been registered successfully';
                $scope.ClearModels();
            }
            else if (data.d == '2') {
                $scope.message = 'User details has been updated successfully';
                $scope.ClearModels();
            }
        })
        .error(function (data, status, headers, config) {
            alert('failure message :' + JSON.stringify({ data: data }));
        });

    };

    //User Roles
    $scope.Roles = [
    { name: 'adminService', label: 'Administrator', value: '1', selected: 'false', disabled: 'false' },
    { name: 'monitorService', label: 'Monitor Service', value: '2', selected: 'false', disabled: 'false' },
    { name: 'addService', label: 'Add Service', value: '3', selected: 'false', disabled: 'false' },
    { name: 'restartService', label: 'Restart Services', value: '4', selected: 'false', disabled: 'false' },
    { name: 'reportService', label: 'Reports Only', value: '5', selected: 'false', disabled: 'false' },
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
            }
            else if ($scope.selectedRoles[iRole] != '1' && role.value == '1') {

                return true;
            }
            else
                return false;
        }
    };

    $scope.toggleRole = function toggleRole(roleSelected, mode) {
        angular.forEach($scope.Roles, function (role) {
            if (roleSelected.value == role.value) {
                role.selected = mode;
                //console.log('role.selected :' + role.selected);
                $scope.DisableRole(role);
            }
        });
    };

    $scope.ClearAllRoles = function ClearAllRoles() {
        angular.forEach($scope.Roles, function (role) {
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
                angular.forEach($scope.userList[eUser].UserRoles, function (role, key) {
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
            $http.post('../forms/Generic.aspx/DeleteUser', dataObj)
            .success(function (data, status, headers, config) {
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
            .error(function (data, status, headers, config) {
                alert('failure message :' + JSON.stringify({ data: data }));
            });
        }
        return false;
    };

    $scope.getUsers($scope.userID);

} ]);
var compareTo = function () {
    return {
        require: "ngModel",
        scope: { otherModelValue: "=compareTo" },
        link: function (scope, elem, attr, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                //console.log(modelValue === scope.otherModelValue);
                return modelValue === scope.otherModelValue;
            };

            scope.$watch('otherModelValue', function () {
                ngModel.$validate();
            });
        }
    };
};

registerApp.directive("compareTo", compareTo);


function GetUser(id) {
    //angular.element(document.getElementById('editContainer')).scope().getUsers(id);
        
    alert('asas');
}
