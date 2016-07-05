(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('MainController', MainController);

    MainController.$inject = ['$http', '$location'];
    function MainController($http, $location) {
        var vm = this;

        /* init */
        vm.init = function () {
            vm.cart.loadFromCookie();
            console.log(vm.cart);
        }

        vm.gotoUrl = function (url) {
            console.log(url);
            window.location.href = url;
        }

        /*cart*/
        vm.cart = {
            stls: [],
            qty: 0,
            amt: 0,
            add: function (nwItem) {
                console.log(nwItem);
                this.qty++;
                this.amt += nwItem.price;

                var addedStall = false;
                var itemAdded = false;

                $.each(this.stls, function (idxStall, stall) {
                    if (stall.i == nwItem.stallId) {
                        //existing stl
                        addedStall = stall;
                        $.each(stall.itms, function (idxItem, item) {
                            //existing item
                            if (item.i == nwItem.id) {
                                item.q++;
                                stall.amt += nwItem.price;
                                itemAdded = true;
                                return false;
                            }
                        });

                        if (itemAdded) {
                            return false;
                        }
                    }
                });
                if (!addedStall) {
                    var stall = { i: nwItem.stallId, n: nwItem.stallName, amt:nwItem.price, itms: [] };
                    stall.itms.push({ i: nwItem.id, n: nwItem.name, v: nwItem.variant, q: 1, p: nwItem.price });
                    this.stls.push(stall);
                } else if (!itemAdded) {
                    addedStall.amt += nwItem.price;
                    addedStall.itms.push({ i: nwItem.id, n: nwItem.name, v: nwItem.variant, q: 1, p: nwItem.price });
                }

                //new item
                this.writeToCookie();
                console.log("this is:", this);
            },
            empty: function () {
                this.stls = [];
                this.qty = 0;
                this.amt = 0;
                Cookies.remove("cart");
            },

            remove: function (itemId) {
                var delStall = null;
                var delIdx = -1;
                $.each(this.stls, function (idxStall, stall) {
                    $.each(stall.itms, function (idxItem, item) {
                        if (item.i == itemId) {
                            vm.cart.qty -= item.q;
                            vm.cart.amt -= item.p * item.q;
                            stall.amt -= item.p * item.q;
                            delIdx = stall.itms.indexOf(item);
                            if (delIdx > -1) {
                                stall.itms.splice(delIdx, 1);
                                delStall = stall;
                            }
                            return false;
                        }
                    })
                });

                //remove empty stall
                if (delStall && delStall.itms.length == 0) {
                    delIdx = this.stls.indexOf(delStall);
                    if (delIdx > -1) {
                        this.stls.splice(delIdx, 1);
                    }
                }

                this.writeToCookie();
            },
            plusOne: function (itemId) {
                $.each(this.stls, function (idxStall, stall) {
                    $.each(stall.itms, function (idxItem, item) {
                        if (item.i == itemId && item.q < 99) {
                            item.q++;
                            vm.cart.qty++;
                            vm.cart.amt += item.p;
                            stall.amt += item.p;
                        }
                    })
                })

                this.writeToCookie();
            },
            minusOne: function (itemId) {
                $.each(this.stls, function (idxStall, stall) {
                    $.each(stall.itms, function (idxItem, item) {
                        if (item.i == itemId && item.q > 0) {
                            item.q--;
                            vm.cart.qty--;
                            vm.cart.amt -= item.p;
                            stall.amt -= item.p;
                        }
                    })
                })

                this.writeToCookie();
            },
            loadFromCookie: function () {
                var str = Cookies.get('cart');
                if (!str) {
                    return;
                }
                var c = $.parseJSON(str);
                this.stls = c.stls;
                this.qty = c.qty;
                this.amt = c.amt;
            },
            writeToCookie: function () {
                Cookies.set("cart", this, { expires: 14 });
            }
        }
    }
})();