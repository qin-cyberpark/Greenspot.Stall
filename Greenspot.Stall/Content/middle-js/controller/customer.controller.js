﻿(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('CustomerController',
        ['$scope', '$http', 'CommonService',
    function ($scope, $http, commSrv) {
        var vm = this;

        /* init */
        vm.init = function () {
            /*cart*/
            vm.cart = new Greenspot.Cart();
            vm.cart.load();
        }

        ///**********************************
        //stall home page
        //***********************************

        vm.init_stallHome = function (stallId) {
            vm.init();
            vm.loadStallProducts(stallId);
        }

        /* load stall products */
        vm.loadStallProducts = function (stallId) {
            commSrv.showLoading();

            //load stall products
            $http.get('/api/stall/GetStallProducts/' + stallId).success(function (result) {
                if (result.Succeeded) {
                    vm.currentStall = result.Data;
                    document.title = vm.currentStall.Name;
                }
                else {
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            }).finally(function () {
                commSrv.hideLoading();
                if (!vm.currentStall) {
                    vm.currentStall = null;
                };
            });
        }


        ///**********************************
        //product home page
        //***********************************
        /*product home page */
        vm.init_productHome = function (productId) {
            vm.init();
            vm.loadProduct(productId);
        }

        /* load product */
        vm.loadProduct = function (productId) {
            commSrv.showLoading();

            //load product
            $http.get('/api/product/' + productId).success(function (result) {
                if (result.Succeeded) {
                    vm.currentProduct = result.Data;
                    document.title = vm.currentProduct.Name;
                }
                else {
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            }).finally(function () {
                commSrv.hideLoading();
                if (!vm.currentProduct) {
                    vm.currentProduct = null;
                };
            });
        }

        ///**********************************
        //check out
        //***********************************
        /* checkout */
        vm.checkOut = function () {
            //console.log(vm.cart);
            vm.cart.save();
            window.location.href = "/customer/Checkout";
            //load items
            //$http.post('/customer/Checkout', vm.cart.getOrders()).success(function (result) {
            //    commSrv.showLoading();
            //    if (result.Succeeded) {
            //        vm.cart.save();
            //        console.log(result);
            //        //redirect to vent page
            //        window.location.href = "/customer/Checkout";
            //    }
            //    else {
            //        console.log(result.Message);
            //    }
            //}).error(function (error) {
            //    console.log(error);
            //});
        }


        /* checkout init */
        vm.init_checkout = function () {
            vm.init();
            vm.loadDeliveryAddress();
            vm.orders = [];
            var simpleOrder = vm.cart.getOrders();
            $.each(simpleOrder.orders, function (orderIdx, order) {
                vm.orders.push(new Greenspot.Order(order, $http));
            });
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
            commSrv.showLoading();

            //clear unnecessary data
            vm.error = {}
            var orders = angular.copy(vm.orders);
            $.each(orders, function (orderIdx, order) {
                delete order.deliveryOptionCollections;
                delete order.pickUpOptionCollections;
                delete order.optionCollections;
                delete order.selectedOptionCollection;
            });

            //post order
            $http.post('/customer/Pay', vm.orders).success(function (result) {
                if (result.Succeeded) {
                    //clear error
                    delete vm.error;
                    //redirect to payment page
                    window.location.href = result.Data;
                }
                else if (result.ErrorType === 'OutOfStock') {
                    vm.error.outOfStock = true
                    vm.error.outOfStockData = $.parseJSON(result.Data);
                } else {
                    vm.error.other = true;
                    vm.error.message = result.Data;
                }
            }).error(function (error) {
                vm.error.other = true;
                vm.error.message = error;
            });
        }

        ///**********************************
        //orders
        //***********************************
        //orders init
        vm.init_orders = function (isPaid) {

            vm.init();

            if (isPaid) {
                vm.cart.removeSelected();
            }
        }
    }]);
})();