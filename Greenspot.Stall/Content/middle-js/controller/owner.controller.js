(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller(
        'OwnerController',
        ['$http', '$location', 'CommonService',
    function ($http, $location, comSrv) {
        var vm = this;

        //step 1 - Register
        vm.Apply = function () {
            comSrv.showLoading();
            //load items
            $http.post('/owner/apply', this.OwnerInfo).success(function (result) {
                if (result.Succeeded) {
                    //redirect to vent page
                    //window.location.href = result.Data;
                    window.location.reload();
                }
                else {
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            }).finally(function () {
                comSrv.hideLoading();
            });;
        };
    }]);
})();