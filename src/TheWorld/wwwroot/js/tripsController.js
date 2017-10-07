(function () {

    "use strict";
    angular.module("app-trips")     // Calling this with only one param means get the existing module
        .controller("tripsController", tripsController);

    function tripsController($http) {

        var vm = this;

        //vm.trips = [{
        //    name: "US Trip",
        //    created: new Date()
        //},
        //{
        //    name: "World Trip",
        //    created: new Date()
        //}]

        vm.trips = [];

        vm.newTrip = {};

        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips") // returns a promise
            .then(function (response) {
                // Success
                angular.copy(response.data, vm.trips);
            }, function (error) {
                // Failure
                vm.errorMessage = "Failed to load data: " + error;
            })
            .finally(function () {
                vm.isBusy = false;
            });

        vm.addNewTrip = function () {
            vm.trips.push(
                {
                    name: vm.newTrip.name,
                    created: new Date()
                });
            vm.newTrip = {};
        };
    };
})();