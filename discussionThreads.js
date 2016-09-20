(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('groupDetailsDiscussionThreadsController', MsgThreadController);

    MsgThreadController.$inject = ['$scope', '$baseController', '$msgThreadService'];

    function MsgThreadController(
        $scope
        , $baseController
        , $msgThreadService) {

        var vm = this; //this points to a new {}, attach properties to object
        vm.threads = {}; //ajax calls will attach here
        vm.newThread = {};
        vm.newThread.groupId = $("#groupId").val();

        //for pagination
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 20;


        //attach dependency injections to the vm
        vm.$msgThreadService = $msgThreadService;
        vm.$scope = $scope;

        //bindable functions
        vm.onMsgThreadSuccess = _onMsgThreadSuccess; //success handler
        vm.onMsgThreadError = _onMsgThreadError; //error handler
        vm.addThread = _addThread //add new thread fn, which will run 'insert' ajax call
        vm.loadThreads = _loadThreads //get all Threads
        vm.onAddThreadSuccess = _onAddThreadSuccess
        //functions for Pagination
        vm.pageChangeHandler = _pageChangeHandler;
        vm.itemsPerPageHandler = _itemsPerPageHandler;



        //-- this line to simulate inheritance
        $baseController.merge(vm, $baseController);

        //this is a wrapper for our small dependency on $scope
        vm.notify = vm.$msgThreadService.getNotifier($scope);

        //startUp function
        render();

        function render() {
            console.log("render/startUp has fired");
            _loadThreads();
        };


        function _onAddThreadSuccess(data) {
            //this receives the data and calls the special notify method that will trigger ng to refresh UI
            console.log("_onMsgThreadSuccess has fired");
            vm.notify(function () {
                vm.newThread = {};
                vm.newThread.groupId = $("#groupId").val();
                _loadThreads();
                vm.$systemEventService.broadcast('refresh');


            });
        };



        function _onMsgThreadSuccess(data) {
            //this receives the data and calls the special notify method that will trigger ng to refresh UI
            console.log("_onMsgThreadSuccess has fired");
            vm.notify(function () {
                vm.threads = data.items;
                console.log('this is vm.threads', vm.threads);
                vm.totalItems = data.totalItems;
            });
        };


        function _onMsgThreadError(jqXhr, error) {
            console.error(error);
        };



        function _addThread() {
            console.log("addThread fn has fired");
            vm.$msgThreadService.postMsgThread(vm.newThread, _onAddThreadSuccess, _onMsgThreadError); //insert ajax call

        };


        //Pagination:
        function _pageChangeHandler(num) {
            console.log("change page -> ", num);
            vm.currentPage = num;
            _loadThreads();
        }


        //Pagination:
        function _itemsPerPageHandler() {

            console.log(vm.pageSize);
            _loadThreads();
        }





        function _loadThreads() {
            console.log("_loadThreads has been called");

            var payload = {
                pageSize: vm.pageSize,
                pageNumber: vm.currentPage,
                groupId: $("#groupId").val()
            }

            vm.$msgThreadService.getThreadsByGroupId(payload, _onMsgThreadSuccess, _onMsgThreadError);
            console.log("this is payload", payload);
        };





    }

})();