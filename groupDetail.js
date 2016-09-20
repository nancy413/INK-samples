        (function () {
            "use strict";

            angular.module(APPNAME)
                .controller('groupDetailController', GroupDetailController);

            GroupDetailController.$inject = ['$scope', '$baseController', '$groupService', "$uibModal"];

            function GroupDetailController(
                $scope
                    , $baseController
                        , $groupService
                            , $uibModal) {

                var vm = this;                

                vm.headingInfo = 'Detail Group Screen';
                vm.groupId = $("#groupId").val();
                vm.group = null;

                vm.$groupService = $groupService;
                vm.$scope = $scope;
                vm.$uibModal = $uibModal;

                vm.getGroupDetailSuccess = _getGroupDetailSuccess;
                vm.getGroupDetailError = _getGroupDetailError;

                vm.joinGroup = _joinGroup;
                vm.leaveGroup = _leaveGroup;

                vm.onJoinGroupSuccess = _onJoinGroupSuccess;
                vm.onJoinGroupError = _onJoinGroupError;

                vm.onLeaveGroupSuccess = _onLeaveGroupSuccess;
                vm.onLeaveGroupError = _onLeaveGroupError;

                vm.tabClass = _tabClass;
                vm.setSelectedTab = _setSelectedTab;

                vm.openModal = _openModal;

                $baseController.merge(vm, $baseController);
                vm.notify = vm.$groupService.getNotifier($scope);

                vm.tabs = [
                    { link: '#/activity/' + vm.groupId, label: 'Activity' },
                    { link: '#/members/' + vm.groupId, label: 'Members' },
                    { link: '#/' + vm.groupId + '/events', label: 'Events' },
                    { link: '#/discussion/' + vm.groupId, label: 'Discussion' }
                ];

                vm.selectedTab = vm.tabs[0];

                getGroupDetail();

                function getGroupDetail() {                    
                    vm.$groupService.getGroupDataById(vm.groupId, vm.getGroupDetailSuccess, vm.getGroupDetailError);
                    vm.$systemEventService.listen('refresh', getGroupDetail);
                }

                function _tabClass(tab) {
                    if (vm.selectedTab == tab) {
                        return "active";
                    } else {
                        return "";
                    }
                }

                function _setSelectedTab(tab) {
                    console.log("set selected tab", tab);
                    vm.selectedTab = tab;
                }


                function _getGroupDetailSuccess(data) {
                    vm.notify(function () {
                        vm.group = data.item;
                        console.log(vm.group);
                    });
                }


                function _getGroupDetailError(jqXhr, error) {
                    console.error(error);
                }


                function _joinGroup() {
                    console.log(vm.group);
                    var confirmJoin = confirm("Join This Group?");
                    if (confirmJoin) {
                        vm.$groupService.insertUserGroup(vm.group.id, vm.onJoinGroupSuccess, vm.onJoinGroupError);
                    }
                    else {
                        return false;
                    }
                }


                function _onJoinGroupSuccess() {
                    console.log("Join Group Ajax Success");
                    getGroupDetail();
                }


                function _onJoinGroupError(jqXhr, error) {
                    console.log("Join Group Ajax Error");
                }


                function _leaveGroup() {
                    console.log(vm.group);
                    var confirmLeave = confirm("Leave This Group?");
                    if (confirmLeave) {
                        vm.$groupService.deleteUserGroup(vm.group.id, vm.onLeaveGroupSuccess, vm.onLeaveGroupError);
                    }
                    else {
                        return false;
                    }
                }


                function _onLeaveGroupSuccess() {
                    console.log("Leave Group Ajax Success");
                    getGroupDetail();
                }


                function _onLeaveGroupError(jqXhr, error) {
                    console.log("Leave Group Ajax Error");
                }


                //OPEN MODAL
                function _openModal(data) {
                    var modalInstance = vm.$uibModal.open(
                    {
                        animation: true,
                        templateUrl: '/Scripts/sabio/app/groupDetails/templates/editModal.html',       //This tells it what html template to use. it must exist in a script tag OR external file
                        controller: 'modalController as mc',    //This controller must exist and be registered with angular for this to work
                        size: 'md',
                        resolve:
                        {  //Anything passed to resolve can be injected into the modal controller as shown below                            
                            selectedGroup: function () {
                                return data;
                            }
                        }
                    });


                    //When the modal closes it returns a promise
                    modalInstance.result.then(function (selectedItem) {
                        vm.modalSelected = selectedItem;                    //If the user closed the modal by clicking Save
                        getGroupDetail();                                   //Refreshes the groups details page.
                    }, function () {
                        console.log('Modal dismissed at: ' + new Date());   //If the user closed the modal by clicking cancel
                        getGroupDetail();
                    });
                }



            }
        })();