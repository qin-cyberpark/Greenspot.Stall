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
            vm.cart = {
                stls: [],
                qty: 0,
                //currentStall: null,
                selectStall: function (stall) {
                    vm.order.stall = stall;
                    //this.currentStall = stall;
                    //this.writeToCookie();
                },

                //remove stall
                removeStall: function (stallId) {
                    for (var i = 0; i < this.stls.length; i++) {
                        if (this.stls[i].i == stallId) {
                            this.qty -= this.stls[i].qty;
                            this.stls.splice(i, 1);
                            break;
                        }
                    }
                    this.writeToCookie();
                },

                add: function (nwItem) {
                    this.qty++;

                    var isExist = false;
                    $.each(this.stls, function (idxStall, stall) {
                        if (stall.i == nwItem.stallId) {
                            //existing stl
                            isExist = true;
                            stall.add(nwItem);
                            return false;
                        }
                    });

                    if (!isExist) {
                        var stall = new StallCart(nwItem.stallId, nwItem.stallName);
                        stall.add(nwItem);
                        this.stls.push(stall);
                    }

                    //new item
                    this.writeToCookie();
                },

                empty: function () {
                    this.stls = [];
                    this.qty = 0;
                    Cookies.remove("cart");
                },

                remove: function (stall, itemId) {
                    this.qty -= stall.remove(itemId);
                    //remove empty stall
                    if (stall.itms.length == 0) {
                        var delIdx = this.stls.indexOf(stall);
                        if (delIdx > -1) {
                            this.stls.splice(delIdx, 1);
                        }
                    }

                    this.writeToCookie();
                },

                plusOne: function (stall, itemId) {
                    stall.plusOne(itemId);
                    this.qty++;

                    this.writeToCookie();
                },

                minusOne: function (stall, itemId) {
                    stall.minusOne(itemId);
                    this.qty--;

                    this.writeToCookie();
                },

                loadFromCookie: function () {
                    var str = Cookies.get('cart');

                    if (!str) {
                        return;
                    }
                    var c = $.parseJSON(str);
                    this.stls = [];
                    //this.currentStall = null;

                    //all stall
                    $.each(c.stls, function (idxStall, stall) {
                        var objStall = new StallCart(stall.i, stall.n);
                        objStall.qty = stall.qty;
                        objStall.amt = stall.amt;
                        objStall.itms = stall.itms;
                        vm.cart.stls.push(objStall);
                    });



                    this.qty = c.qty;

                    //select first
                    //if (this.currentStall == null && this.stls.length > 0) {
                    //    this.currentStall = this.stls[0];
                    //}
                    vm.order.stall = vm.cart.stls[0];

                },

                writeToCookie: function () {
                    Cookies.set("cart", this, { expires: 14 });
                }
            }

            ///**********************************
            //check out
            //***********************************
            /* checkout */
            vm.checkOut = function () {
                //load items
                $http.post('/customer/CheckStock', vm.order).success(function (result) {
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
                vm.order = orderJson;
                vm.loadDeliveryAddress(vm.loadDeliveryOptions);
                vm.loadPickUpOptions();
            }


            /*order*/
            vm.order = {
            }

            ///**********************************
            //delivery options
            //***********************************
            /* load address */
            vm.loadDeliveryAddress = function (callback) {
                //load address
                $http.get('/customer/DeliveryAddresses').success(function (result) {
                    if (result.Succeeded) {
                        vm.deliveryAddresses = result.Data;
                        if (vm.deliveryAddresses.length > 0) {
                            vm.order.deliveryAddress = vm.deliveryAddresses[0];
                        }

                        if (callback) {
                            callback();
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
            vm.loadDeliveryOptions = function () {
                //refresh
                vm.deliveryOptions = [];
                vm.order.deliveryOption = null;

                //delivery
                var url = '/api/stall/GetDeliveryOptions/' + vm.order.stall.i + "?country=" + vm.order.deliveryAddress.CountryId;
                url += "&city=" + vm.order.deliveryAddress.City;
                url += "&suburb=" + vm.order.deliveryAddress.Suburb;
                url += "&area=" + vm.order.deliveryAddress.Area;

                $http.get(url).success(function (result) {
                    if (result.Succeeded) {
                        vm.deliveryOptionCollections = result.Data;
                        if (vm.deliveryOptionCollections.length > 0) {
                            vm.selectedDeliveryOptionCollection = vm.deliveryOptionCollections[0];
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
                vm.order.deliveryOption = null;
                if (vm.order.deliveryAddress != 'pickup') {
                    vm.order.isPickUp = false;
                    vm.loadDeliveryOptions();
                } else {
                    vm.order.isPickUp = true;
                    if (vm.pickUpOptionCollections && vm.pickUpOptionCollections.length > 0) {
                        vm.selectedDeliveryOptionCollection = vm.pickUpOptionCollections[0];
                        vm.selectDeliveryDate();
                    }
                }
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


    function StallCart(stallId, stallName) {
        var self = this;

        self.i = stallId;
        self.n = stallName;
        self.qty = 0;
        self.amt = 0;
        self.itms = [];

        self.add = function (nwItem) {
            self.qty++;
            self.amt += nwItem.price;

            var itemAdded = false;
            $.each(self.itms, function (idxItem, item) {
                //existing item
                if (item.i == nwItem.id) {
                    item.q++;
                    itemAdded = true;
                    return false;
                }
            });

            if (!itemAdded) {
                //new item
                self.itms.push({ i: nwItem.id, n: nwItem.name, v: nwItem.variant, q: 1, p: nwItem.price });
            }
        };

        self.remove = function (itemId) {
            var removedQty = 0;
            $.each(self.itms, function (idxItem, item) {
                if (item.i == itemId) {
                    var delIdx = self.itms.indexOf(item);
                    if (delIdx > -1) {
                        self.itms.splice(delIdx, 1);
                    }
                    removedQty = item.q;
                    self.qty -= item.q;
                    self.amt -= item.p * item.q;

                    return false;
                }
            })
            return removedQty;
        }

        self.plusOne = function (itemId) {
            $.each(self.itms, function (idxItem, item) {
                if (item.i == itemId && item.q < 99) {
                    item.q++;
                    self.qty++;
                    self.amt += item.p;
                }
            })
        }

        self.minusOne = function (itemId) {
            $.each(self.itms, function (idxItem, item) {
                if (item.i == itemId && item.q > 0) {
                    item.q--;
                    self.qty--;
                    self.amt -= item.p;
                }
            })
        }
    }
})();