(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('CustomerController', CustomerController);

    CustomerController.$inject = ['$http', '$location'];
    function CustomerController($http, $location) {
        var vm = this;

        /* init */
        vm.init = function () {
            vm.cart.loadFromCookie();
        }


        /* redirect */
        vm.gotoUrl =function(url) {
            window.location.href = url;
        }

        
        /*cart*/
        vm.cart = {
            stls: [],
            qty: 0,
            currentStall: null,
            selectStall: function (stall) {
                this.currentStall = stall;
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
                this.currentStall = null;

                //current stall
                if (c.currentStall != null) {
                    var objStall = new StallCart(c.currentStall.i, c.currentStall.n);
                    objStall.qty = c.currentStall.qty;
                    objStall.amt = c.currentStall.amt;
                    objStall.itms = c.currentStall.itms;
                    this.currentStall = objStall;
                }

                //all stall
                $.each(c.stls, function (idxStall, stall) {
                    var objStall = new StallCart(stall.i, stall.n);
                    objStall.qty = stall.qty;
                    objStall.amt = stall.amt;
                    objStall.itms = stall.itms;
                    vm.cart.stls.push(objStall);
                });



                this.qty = c.qty;
                console.log(this);
                console.log(this.currentStall);
                //select first
                if (this.currentStall == null && this.stls.length > 0) {
                    console.log(1);
                    this.currentStall = this.stls[0];
                }
               
            },

            writeToCookie: function () {
                console.log("this is:", this);
                Cookies.set("cart", this, { expires: 14 });
            }
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