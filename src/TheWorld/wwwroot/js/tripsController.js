(function () {

    "use strict";
    angular.module("app-trips")     // Calling this with only one param means get the existing module
        .controller("tripsController", tripsController);

    function tripsController() {

        var vm = this;

        vm.trips = [{
            name: "US Trip",
            created: new Date()
        },
        {
            name: "World Trip",
            created: new Date()
        }]

        vm.newTrip = {};

        vm.addNewTrip = function () {
            alert(vm.newTrip.name);
        };
    }
})();