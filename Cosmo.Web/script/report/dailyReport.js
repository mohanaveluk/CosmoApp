var dragdrop = angular.module("dragdrop", ['ngMessages']);
dragdrop.controller('ctrlDualList', [
    '$scope', '$http', function($scope, $http) {

        $scope.reportName = "Daily";
        $scope.reportTime = "6";
        $scope.reportDisable = false;
        $scope.errorMessage = "";
        $scope.subscriptionId = "0";

        $scope.subscriptionDetails = {};
        $scope.UserEmails = [];
        $scope.listA = [];
        $scope.listB = [];

        $scope.GetDailyReportSubscription = function() {
            var dataObj = {
                envId: "0"
            };

            openModal();
            try {
                $http.post('../forms/Generic.aspx/GetReportSubscription', dataObj)
                    .success(function(data, status, headers, config) {
                        $scope.subscriptionDetails = data.d;
                        if ($scope.subscriptionDetails != null) {
                            angular.forEach($scope.subscriptionDetails, function(detail) {
                                var emailExists = false;
                                if (detail.SubscriptionId > 0)
                                    $scope.reportDisable = !detail.SubscriptionIsActive;
                                angular.forEach($scope.UserEmails, function(email, index) {
                                    if (email.userEmailAddress == detail.UserListEmailId) {
                                        emailExists = true;
                                        return;
                                    }
                                });

                                if (!emailExists) {
                                    $scope.UserEmails.push(
                                    {
                                        id: detail.UserListId.toString(),
                                        userEmailAddress: detail.UserListEmailId,
                                        subscriptionUserListId: detail.SubscriptionUserListId != null ? detail.SubscriptionUserListId : "",
                                        subscriptionDetailId: detail.SubscriptionDetailId != null ? detail.SubscriptionDetailId : "",
                                        selectedEmailAddress: detail.SubscriptionEmail != null ? detail.SubscriptionEmail : ""
                                    });


                                    if (detail.SubscriptionEmail != null) {
                                        $scope.listB.push(
                                            {
                                                //id: detail.SubscriptionUserListId != null ? detail.SubscriptionUserListId : "",
                                                id: detail.UserListId.toString(),
                                                userEmailAddress: detail.SubscriptionEmail
                                            }
                                        );
                                        $scope.reportName = detail.SubscriptionType != null ? detail.SubscriptionType : "Daily";
                                        $scope.reportTime = detail.SubscriptionTime != null ? detail.SubscriptionTime : "6";
                                        $scope.subscriptionId = detail.SubscriptionId != null ? detail.SubscriptionId : "0";

                                    } else if (detail.UserListEmailId != null && detail.SubscriptionEmail == null) {
                                        $scope.listA.push(
                                            {
                                                id: detail.UserListId.toString(),
                                                userEmailAddress: detail.UserListEmailId
                                            }
                                        );
                                    }
                                }
                            });
                            
                            $scope.ctrlDualList();
                        }
                        closeModal();
                    })
                    .error(function(data, status, headers, config) {
                        closeModal();
                        showAlert(data.Message, "error");
                        console.log(data.Message);
                        console.log(data.StackTrace);
                    });

            } catch (e) {
                console.log(e);
                showAlert(e, "error");
                closeModal();
            } 
        };

        var userData = [
            { id: 1, firstName: 'Mary@someemail.com', lastName: 'Goodman', approved: true, points: 34 },
            { id: 2, firstName: 'Mark@someemail.com', lastName: 'Wilson', approved: true, points: 4 },
            { id: 3, firstName: 'Alex@someemail.com', lastName: 'Davies', approved: true, points: 56 },
            { id: 4, firstName: 'Bob@someemail.com', lastName: 'Banks', approved: false, points: 14 },
            { id: 5, firstName: 'David@someemail.com', lastName: 'Stevens', approved: false, points: 100 },
            { id: 6, firstName: 'Jason@someemail.com', lastName: 'Durham', approved: false, points: 0 },
            { id: 7, firstName: 'Jeff@someemail.com', lastName: 'Marks', approved: true, points: 8 },
            { id: 8, firstName: 'Betty@someemail.com', lastName: 'Abercrombie', approved: true, points: 18 },
            { id: 9, firstName: 'Krista@someemail.com', lastName: 'Michaelson', approved: true, points: 10 },
            { id: 11, firstName: 'Devin@someemail.com', lastName: 'Sumner', approved: false, points: 3 },
            { id: 12, firstName: 'Navid@someemail.com', lastName: 'Palit', approved: true, points: 57 },
            { id: 13, firstName: 'Bhat@someemail.com', lastName: 'Phuart', approved: false, points: 314 },
            { id: 14, firstName: 'Nuper@someemail.com', lastName: 'Galzona', approved: true, points: 94 }
        ];

        $scope.ctrlDualList = function() {

            // init
            $scope.selectedA = [];
            $scope.selectedB = [];

            //$scope.listA = userData.slice(0, 8);
            //$scope.listB = userData.slice(9, 14);
            $scope.items = $scope.UserEmails; //userData;

            $scope.checkedA = false;
            $scope.checkedB = false;


        };
        
        $scope.reset = function() {
            $scope.selectedA = [];
            $scope.selectedB = [];
            $scope.toggle = 0;
        };
        
        $scope.arrayObjectIndexOf = function(myArray, searchTerm, property) {
            for (var i = 0, len = myArray.length; i < len; i++) {
                if (myArray[i][property] === searchTerm) return i;
            }
            return -1;
        };

        $scope.aToB = function() {
            for (var i in $scope.selectedA) {
                var moveId = $scope.arrayObjectIndexOf($scope.items, $scope.selectedA[i], "id");
                $scope.listB.push($scope.items[moveId]);
                var delId = $scope.arrayObjectIndexOf($scope.listA, $scope.selectedA[i], "id");
                $scope.listA.splice(delId, 1);
            }
            $scope.reset();
        };

        $scope.bToA = function() {
            for (var i in $scope.selectedB) {
                var moveId = $scope.arrayObjectIndexOf($scope.items, $scope.selectedB[i], "id");
                $scope.listA.push($scope.items[moveId]);
                var delId = $scope.arrayObjectIndexOf($scope.listB, $scope.selectedB[i], "id");
                $scope.listB.splice(delId, 1);
            }
            $scope.reset();
        };

        $scope.toggleA = function() {

            if ($scope.selectedA.length > 0) {
                $scope.selectedA = [];
            } else {
                for (var i in $scope.listA) {
                    $scope.selectedA.push($scope.listA[i].id);
                }
            }
        }

        $scope.toggleB = function() {

            if ($scope.selectedB.length > 0) {
                $scope.selectedB = [];
            } else {
                for (var i in $scope.listB) {
                    $scope.selectedB.push($scope.listB[i].id);
                }
            }
        }

        $scope.selectA = function(i) {
            $scope.selectedA.push(i);
        };

        $scope.selectB = function(i) {
            $scope.selectedB.push(i);
        };

        $scope.UpdateEmailSubscription = function() {
            var emailIds = "";
            angular.forEach($scope.listB, function(detail) {
                emailIds += detail.userEmailAddress + ",";
            });
            emailIds = emailIds !== "" ? emailIds.substr(0, emailIds.length - 1) : "";

            if(emailIds === "") showAlert("Please select at least one user to create subscription", "error");

            console.log($scope.listB);
            console.log(emailIds);
            var dataObj = {
                subscriptionId: $scope.subscriptionId,
                reportType: $scope.reportName,
                reportTime: $scope.reportTime,
                isDisable: $scope.reportDisable,
                selectedUserEmails: emailIds
            };

            if (emailIds !== "") {
                try {
                    $http.post('../forms/Generic.aspx/InsUpdReportSubscription', dataObj)
                        .success(function(data, status, headers, config) {
                            var result = data.d;
                            if (result.Status === "Success") {
                                showAlert($("#drpReportName option:selected").text() + " has successsfully updated", "success");
                                $scope.subscriptionId = result.SubscriptionId;
                            } else
                                showAlert(result.Status + ": " + result.Message, "error");
                        })
                        .error(function(data, status, headers, config) {
                            closeModal();
                            console.log(data.Message);
                            console.log(data.StackTrace);
                            showAlert(data.Message, "error");
                        });
                } catch (e) {
                    closeModal();
                    showAlert(e, "error");
                    console.log(e);
                } 
            }

            //Refresh the DailyReportSubscription
            //$scope.GetDailyReportSubscription();
        };

        $scope.GetDailyReportSubscription();
    }
]);

dragdrop.directive('lvlDraggable', ['$rootScope', 'uuid', function($rootScope, uuid) {
	    return {
	        restrict: 'A',
	        link: function(scope, el, attrs, controller) {
	        	angular.element(el).attr("draggable", "true");

	            var id = angular.element(el).attr("id");
	            if (!id) {
	                id = uuid.new()
	                angular.element(el).attr("id", id);
	            }

	            el.bind("dragstart", function(e) {
	                e.dataTransfer.setData('text', id);

	                $rootScope.$emit("LVL-DRAG-START");
	            });

	            el.bind("dragend", function(e) {
	                $rootScope.$emit("LVL-DRAG-END");
	            });
	        }
    	}
	}]);

dragdrop.directive('lvlDropTarget', ['$rootScope', 'uuid', function($rootScope, uuid) {
	    return {
	        restrict: 'A',
	        scope: {
	            onDrop: '&'
	        },
	        link: function(scope, el, attrs, controller) {
	            var id = angular.element(el).attr("id");
	            if (!id) {
	                id = uuid.new()
	                angular.element(el).attr("id", id);
	            }

	            el.bind("dragover", function(e) {
	              if (e.preventDefault) {
	                e.preventDefault(); // Necessary. Allows us to drop.
	              }

	              e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
	              return false;
	            });

	            el.bind("dragenter", function(e) {
	              // this / e.target is the current hover target.
	              angular.element(e.target).addClass('lvl-over');
	            });

	            el.bind("dragleave", function(e) {
	              angular.element(e.target).removeClass('lvl-over');  // this / e.target is previous target element.
	            });

	            el.bind("drop", function(e) {
	              if (e.preventDefault) {
	                e.preventDefault(); // Necessary. Allows us to drop.
	              }

	              if (e.stopPropagation) {
	                e.stopPropagation(); // Necessary. Allows us to drop.
	              }
	            	var data = e.dataTransfer.getData("text");
	                var dest = document.getElementById(id);
	                var src = document.getElementById(data);

	                scope.onDrop({dragEl: src, dropEl: dest});
	            });

	            $rootScope.$on("LVL-DRAG-START", function() {
	                var el = document.getElementById(id);
	                angular.element(el).addClass("lvl-target");
	            });

	            $rootScope.$on("LVL-DRAG-END", function() {
	                var el = document.getElementById(id);
	                angular.element(el).removeClass("lvl-target");
	                angular.element(el).removeClass("lvl-over");
	            });
	        }
    	}
	}]);

//ctrlDualList.$inject = ['$scope'];


$(document).ready(function() {

    function updateMonitorList() {
        console.log('calling UpdategetEnvironmentList');
        var scope = angular.element(document.getElementById("MainWrap")).scope();
        scope.$apply(function() {
            scope.ctrlDualList();
        });
    }

//    updateMonitorList();
    $("#btnCancel").click(function() {
        window.close();
    });

    //angular.element(document).ready(function() { angular.bootstrap(document, ['dragdrop']); });
});

function showAlert(text, alertMode) {
    $('#div-alert').show();

    if (alertMode === "error") {
        $('#div-alert').removeClass().addClass('alert alert-danger');
    } else if (alertMode === "success") {
        $('#div-alert').removeClass().addClass('alert alert-success');
    } else {
        $('#div-alert').removeClass().addClass('alert alert-warning');
    }
    $('#label-alert').html(text).fadeIn('fast');
}
