(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('HomeController',
        ['$scope', '$http', 'CommonService',
            function ($scope, $http, comSrv) {
                var vm = this;

                /*
                === init ===
                */
                vm.init = function () {
                    vm.firstPage = true;
                    vm.loadRecommendTakeaway();

                    /*cart*/
                    vm.cart = new Greenspot.Cart();
                    vm.cart.loadFromCookie();
                }

                /*
                === Recommend ===
                */
                vm.loadRecommendTakeaway = function () {
                    comSrv.showLoading();
                    vm.recommendTakeaway = null;
                    //load address
                    $http.get('/api/home/recommendTakeaway?area=' + vm.searchCondition.area).success(function (result) {
                        if (result.Succeeded) {
                            vm.recommendTakeaway = result.Data;
                            console.log(vm.recommendTakeaway);
                        }
                        else {
                            console.log(result.Message);
                        }
                    }).error(function (error) {
                        console.log(error)
                    }).finally(function () {
                        comSrv.hideLoading();
                    });
                }

                vm.areaChanged = function () {
                    if (vm.firstPage) {
                        vm.loadRecommendTakeaway();
                    }
                }

                /*
                === search ===
                */
                vm.searchCondition = {
                    area: "NZ-Auckland",
                    category: "TAKEAWAY",
                    keyword: ""
                }

                vm.search = function () {
                    vm.firstPage = false;
                    comSrv.showLoading();
                    vm.recommendTakeaway = null;
                    vm.matchedStalls = null;
                    vm.matchedProducts = null;

                    //search stall
                    $http.get('/api/home/SearchTakeawayStall?area=' + vm.searchCondition.area + '&keyword=' + vm.searchCondition.keyword).success(function (result) {
                        if (result.Succeeded) {
                            vm.matchedStalls = result.Data;
                            console.log(vm.matchedStalls);
                        }
                        else {
                            console.log(result.Message);
                        }
                    }).error(function (error) {
                        console.log(error)
                    }).finally(function () {
                        comSrv.hideLoading();
                    });

                    //search product
                    $http.get('/api/home/SearchTakeawayProduct?area=' + vm.searchCondition.area + '&keyword=' + vm.searchCondition.keyword).success(function (result) {
                        if (result.Succeeded) {
                            vm.matchedProducts = result.Data;
                        }
                        else {
                            console.log(result.Message);
                        }
                    }).error(function (error) {
                        console.log(error)
                    }).finally(function () {
                        comSrv.hideLoading();
                    });
                }
            }]);
})();