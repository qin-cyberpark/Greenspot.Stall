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
                vm.orders = orderJson.orders;
                vm.loadDeliveryAddress();
                //vm.loadPickUpOptions();
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
                            vm.deliveryAddress = vm.deliveryAddresses[0];
                            vm.selectDeliveryAddress();
                        }
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            }

            /* get delivery option*/
            vm.loadDeliveryOptions = function (order) {
                //refresh
                order.deliveryOptions = [];
                order.deliveryOption = null;

                //delivery
                var url = '/api/stall/GetDeliveryOptions/' + order.i + "?country=" + order.deliveryAddress.CountryId;
                url += "&city=" + order.deliveryAddress.City;
                url += "&suburb=" + order.deliveryAddress.Suburb;
                url += "&area=" + order.deliveryAddress.Area;

                $http.get(url).success(function (result) {
                    if (result.Succeeded) {
                        order.deliveryOptionCollections = result.Data;
                        if (order.deliveryOptionCollections.length > 0) {
                            order.selectedDeliveryOptionCollection = order.deliveryOptionCollections[0];
                            vm.selectDeliveryDate();
                        }
                    }
                    else {
                        $window.alert(result.Message);
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            }

            /*get pickup option*/
            vm.loadPickUpOptions = function () {

                //refresh
                vm.pickUpOptions = [];
                vm.order.deliveryOption = null;

                //delivery
                var url = '/api/stall/GetPickUpOptions/' + vm.order.stall.i;

                $http.get(url).success(function (result) {
                    if (result.Succeeded) {
                        vm.pickUpOptionCollections = result.Data;
                        if (vm.deliveryOptionCollections && vm.deliveryOptionCollections.length > 0) {
                            vm.selectedDeliveryOptionCollection = vm.pickUpOptionCollections[0];
                            vm.selectDeliveryDate();
                        }
                    }
                    else {
                        $window.alert(result.Message);
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            }

            //select delivery address
            vm.selectDeliveryAddress = function () {
                $.each(vm.orders, function (orderIdx, order) {
                    order.deliveryOption = null;
                    order.deliveryAddress = vm.deliveryAddress;
                    if (vm.orders.deliveryAddress != 'pickup') {
                        order.isPickUp = false;
                        vm.loadDeliveryOptions(order);
                    } else {
                        order.isPickUp = true;
                        if (vm.pickUpOptionCollections && vm.pickUpOptionCollections.length > 0) {
                            vm.selectedDeliveryOptionCollection = vm.pickUpOptionCollections[0];
                            //vm.selectDeliveryDate();
                        }
                    }
                })
            }

            //select delivery date
            vm.selectDeliveryDate = function () {
                vm.order.deliveryOption = null;
                if (vm.selectedDeliveryOptionCollection.ApplicableOpts || vm.selectedDeliveryOptionCollection.ApplicableOpts.length > 0) {
                    vm.order.deliveryOption = vm.selectedDeliveryOptionCollection.ApplicableOpts[0];
                    vm.selectDeliveryOption();
                }
            }

            vm.selectDeliveryOption = function () {
                vm.order.amount = vm.order.stall.amt + vm.order.deliveryOption.Fee;
            }

            ///**********************************
            //address management
            //***********************************
            /* address management */
            vm.init_address = function () {
                vm.loadDeliveryAddress();
            }

            /*new address*/
            vm.editAddress = {};

            //select address
            vm.addressSelected = function () {
                //update address
                $scope.$apply(function () {
                    vm.editAddress.Address = vm.editAddress.AddressObject.FullAddress;
                });
            }

            //select area
            vm.selectArea = function (ev) {
                //got potential address

                //show dialog
                $mdDialog.show({
                    controller: 'DialogController',
                    controllerAs: 'ctrl',
                    templateUrl: '/SelectArea.tmpl.html',
                    parent: angular.element(document.body),
                    targetEvent: ev
                })
                .then(function (answer) {
                    console.log(answer);
                    vm.editAddress.Area = answer.en;
                    vm.editAddress.SelectedAreaName = answer.cn;
                });
            }

            //
            vm.addAddress = function (ev) {

                if (!vm.editAddress.AddressObject) {
                    alert("请选择地址");
                    return false;
                }

                //get area of address
                if (!vm.editAddress.Area) {
                    var url = '/api/address/Suburb2Area?country=NZ&city=' + vm.editAddress.AddressObject.CityTown;
                    url += "&suburb=" + vm.editAddress.AddressObject.Suburb;
                    $http.get(url).success(function (result) {
                        if (result.Succeeded) {
                            vm.editAddress.Area = result.Data;
                            //add address
                            vm.submitAddress();
                        }
                        else {
                            //select area
                            vm.selectArea(ev);
                        }
                    }).error(function (error) {
                        //select area
                        vm.selectArea(ev);
                    });
                } else {
                    //add address
                    vm.submitAddress();
                }
            }

            //submit address
            vm.submitAddress = function () {
                $http.post('/customer/addAddress', vm.editAddress).success(function (result) {
                    if (result.Succeeded) {
                        vm.loadDeliveryAddress();
                        vm.gotoUrl('/customer/checkout');
                    }
                    else {
                        alert(result.Message);
                    }
                }).error(function (error) {
                    alert(error);
                });
            }

            /* pay */
            vm.pay = function () {
                //load items
                if (vm.order.deliveryAddress == 'pickup') {
                    vm.order.deliveryAddress = null;
                }
                $http.post('/customer/Pay', vm.order).success(function (result) {
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
                $http.post('/customer/fakePay', vm.order).success(function (result) {
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