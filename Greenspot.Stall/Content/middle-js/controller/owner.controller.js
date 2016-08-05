(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller(
        'OwnerController',
        ['$http', '$location',
        function ($http, $location) {
            var vm = this;

            /* redirect */
            vm.gotoUrl = function (url) {
                window.location.href = url;
            }

            //step 1 - Register
            vm.Register = function () {
                //load items
                $http.post('/owner/Register', this.OwnerInfo).success(function (result) {
                    if (result.Succeeded) {
                        //redirect to vent page
                        window.location.href = result.Data;
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            };
        }]);
})();