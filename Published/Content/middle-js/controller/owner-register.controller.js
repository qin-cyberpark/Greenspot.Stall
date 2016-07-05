(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('OwnerRegisterController', OwnerRegisterController);

    OwnerRegisterController.$inject = ['$http', '$location'];
    function OwnerRegisterController($http, $location) {
        var vm = this;
        //step 1 - submit info
        vm.SubmitInfo = function () {
            //load items
            $http.post('~/owner/Register', this.OwnerInfo).success(function (result) {
                if (result.Succeeded) {
                    //redirect to vent page
                    window.location.href=result.Data;
                }
                else {
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            });
        };
    }
})();