(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('CustomerController', CustomerController);

    CustomerController.$inject = ['$http', '$location','$window'];
    function CustomerController($http, $location, $window) {
        var vm = this;

        /* redirect */
        vm.gotoUrl = function (url) {
            window.location.href = url;
        }
        
        
        /* init */
        vm.init = function () {
            vm.cart.loadFromCookie();
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

                //current stall
                //if (c.currentStall != null) {
                //    var objStall = new StallCart(c.currentStall.i, c.currentStall.n);
                //    objStall.qty = c.currentStall.qty;
                //    objStall.amt = c.currentStall.amt;
                //    objStall.itms = c.currentStall.itms;
                //    this.currentStall = objStall;
                //}

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
            vm.loadDeliveryAddress(vm.loadDeliverySchedule);
        }

        /* address management */
        vm.init_address = function () {
            vm.loadDeliveryAddress();
        }

        /*order*/
        vm.order = {
        }

        /* load address */
        vm.loadDeliveryAddress = function (callback) {
            //load address
            $http.get('/customer/DeliveryAddresses').success(function (result) {
                if (result.Succeeded) {
                    vm.deliveryAddresses = result.Data;
                    vm.order.deliveryAddress = vm.deliveryAddresses[0];

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

        /* get delivery schedule*/
        vm.loadDeliverySchedule = function () {
            //load address
            var url = '/api/stall/DeliverySchedule/' + vm.order.stall.i + "?country=" + vm.order.deliveryAddress.CountryId;
            url += "&city=" + vm.order.deliveryAddress.City;
            url += "&area=" + vm.order.deliveryAddress.Area;

            $http.get(url).success(function (result) {
                if (result.Succeeded) {
                    vm.deliverySchedule = result.Data;
                    vm.order.deliveryOption = vm.deliverySchedule[0];
                    vm.SelectDeliveryOption();
                }
                else {
                    $window.alert(result.Message);
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            });
        }

        /* get delivery fee */
        vm.getDeliveryFee = function () {
            vm.order.deliveryFee = null;
            if (!vm.order.deliveryOption.IsPickUp) {
                //get delivery fee
                var url = '/api/stall/CalcDeliveryFee/' + vm.order.stall.i + "?country=" + vm.order.deliveryAddress.CountryId;
                url += "&city=" + vm.order.deliveryAddress.City;
                url += "&suburb=" + vm.order.deliveryAddress.Suburb;
                url += "&amount=" + vm.order.stall.amt;

                $http.get(url).success(function (result) {
                    if (result.Succeeded) {
                        vm.order.deliveryFee = result.Data;
                        vm.order.amount = vm.order.stall.amt + vm.order.deliveryFee
                    }
                    else {
                        $window.alert(result.Message);
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            } else {
                vm.order.deliveryFee = 0;
                vm.order.amount = vm.order.stall.amt + vm.order.deliveryFee
            }
        }
        
        vm.SelectDeliveryAddress = function () {
            vm.loadDeliverySchedule();
        }

        vm.SelectDeliveryOption = function () {
            vm.getDeliveryFee();
        }
        

        /*new address*/
        vm.newAddress = {};

        //select address
        vm.addressSelected = function () {
            vm.newAddress.Address = vm.newAddress.AddressObject.formatted_address;
        }

        vm.addressChanged = function () {
            vm.newAddress.AddressObject = null;
        }

        //
        vm.addAddress = function () {
            //console.log(vm.newAddress.AddressObject);
            if (!vm.newAddress.AddressObject) {
                alert("请确认地址");
                return;
            }

            //load items
            $http.post('/customer/addAddress', vm.newAddress).success(function (result) {
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
            console.log(vm.order);
            $http.post('/customer/Pay', vm.order).success(function (result) {
                if (result.Succeeded) {
                    //redirect to vent page
                    console.log(result);
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
            console.log(vm.order);
            $http.post('/customer/fakePay', vm.order).success(function (result) {
                if (result.Succeeded) {
                    //redirect to vent page
                    console.log(result);
                    window.location.href = result.Data;
                }
                else {
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            });
        }
    }

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