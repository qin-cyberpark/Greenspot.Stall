(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('CustomerController',
        ['$scope', '$http', '$location', '$window', '$mdDialog',
        function ($scope, $http, $location, $window, $mdDialog) {
            var vm = this;

            /* redirect */
            vm.gotoUrl = function (url) {
                window.location.href = url;
            }

            /* init */
            vm.init = function () {
                vm.cart.loadFromCookie();
            }

            vm.removeStallCookie = function (stallId) {
                vm.cart.loadFromCookie();
                vm.cart.removeStall(stallId);
            }

            /*cart*/
            vm.cart = new Greenspot.Cart();

            ///**********************************
            //check out
            //***********************************
            /* checkout */
            vm.checkOut = function () {
                //load items
                $http.post('/customer/CheckStock', vm.cart.getOrders()).success(function (result) {
                    if (result.Succeeded) {
                        //redirect to vent page
                        window.location.href = "/customer/Checkout";
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            }


            /* checkout init */
            vm.init_checkout = function (orderJson) {
                vm.orders = [];
                $.each(orderJson.orders, function (orderIdx, order) {
                    vm.orders.push(new Greenspot.Order(order, $http));
                });

                vm.loadDeliveryAddress();
            }

            ///**********************************
            //delivery options
            //***********************************
            /* load address */
            vm.loadDeliveryAddress = function () {
                //load address
                $http.get('/customer/DeliveryAddresses').success(function (result) {
                    if (result.Succeeded) {
                        vm.deliveryAddresses = result.Data;
                        if (vm.deliveryAddresses.length > 0) {
                            vm.selectedDeliveryAddress = vm.deliveryAddresses[0];
                            vm.deliveryAddressChanged();
                        }
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            }

            //select delivery address
            vm.deliveryAddressChanged = function () {
                $.each(vm.orders, function (orderIdx, order) {
                    order.setDeliveryAddress(vm.selectedDeliveryAddress);
                });
            }

            //
            vm.allDeliverySelected = function () {
                var len = vm.orders.length;
                for (var i = 0; i < len; i++) {
                    if (!vm.orders[i].selectedOption) {
                        return false;
                    }
                }
                return true;
            }


            /* pay */
            vm.pay = function () {
                $.each(vm.orders, function (orderIdx, order) {
                    order.deliveryOptionCollections = null;
                    order.pickUpOptionCollections = null;
                    order.optionCollections = null;
                    order.selectedOptionCollection = null;
                });

                //load items
                $http.post('/customer/Pay', vm.orders).success(function (result) {
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
            }

            vm.fakePay = function () {
                //load items
                $http.post('/customer/fakePay', vm.orders).success(function (result) {
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
            }
        }]);

    module.controller('DialogController', ['$scope', '$mdDialog',
         function ($scope, $mdDialog) {
             $scope.hide = function () {
                 $mdDialog.hide();
             };
             $scope.cancel = function () {
                 $mdDialog.cancel();
             };
             $scope.answer = function (answer) {
                 $mdDialog.hide(answer);
             };
         }
    ]);
})();