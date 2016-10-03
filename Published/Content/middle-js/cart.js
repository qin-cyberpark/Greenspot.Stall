var Greenspot = Greenspot || {};
(function () {

    Greenspot.CartStallItem = function (stallId, stallName) {
        var self = this;
        self.i = stallId;
        self.n = stallName;
        self.qty = 0;
        self.itms = [];

        self.add = function (nwItem, qty) {
            qty = qty ? qty : 1;
            self.qty += qty;

            var itemAdded = false;
            angular.forEach(self.itms, function (item, idxItem) {
                //existing item
                if (item.i == nwItem.id) {
                    item.q += qty;
                    itemAdded = true;
                    return false;
                }
            });

            if (!itemAdded) {
                //new item
                self.itms.push({ i: nwItem.id, n: nwItem.name, v: nwItem.variant, q: qty, p: nwItem.price, slctd: true });
            }
        };

        self.remove = function (itemId) {
            var removedQty = 0;
            angular.forEach(self.itms, function (item, idxItem) {
                if (item.i == itemId) {
                    var delIdx = self.itms.indexOf(item);
                    if (delIdx > -1) {
                        self.itms.splice(delIdx, 1);
                    }
                    removedQty = item.q;
                    self.qty -= item.q;

                    return false;
                }
            })
            return removedQty;
        }

        self.isChecked = function () {
            var hasSelected = false;
            var hasUnselected = false;
            angular.forEach(self.itms, function (item, idxItem) {
                hasSelected = hasSelected || item.slctd;
                hasUnselected = hasUnselected || !item.slctd;
            });
            return hasSelected && !hasUnselected;
        };

        self.isIndeterminate = function () {
            var hasSelected = false;
            var hasUnselected = false;
            angular.forEach(self.itms, function (item, idxItem) {
                hasSelected = hasSelected || item.slctd;
                hasUnselected = hasUnselected || !item.slctd;
            });
            return hasSelected && hasUnselected;
        }

        self.toggle = function (item) {
            item.slctd = !item.slctd;
        };

        self.toggleAll = function () {
            var checked = self.isIndeterminate() || !self.isChecked();
            angular.forEach(self.itms, function (item, idxItem) {
                item.slctd = checked;
            });
        }

        self.plusOne = function (itemId) {
            angular.forEach(self.itms, function (item, idxItem) {
                if (item.i == itemId && item.q < 99) {
                    item.q++;
                    self.qty++;
                }
            })
        }

        self.minusOne = function (itemId) {
            angular.forEach(self.itms, function (item, idxItem) {
                if (item.i == itemId && item.q > 0) {
                    item.q--;
                    self.qty--;
                }
            })
        }
    }

    Greenspot.Cart = function () {
        var self = this;
        self.stls = [];
        self.qty = 0;

        //remove stall
        self.removeStall = function (stallId) {
            for (var i = 0; i < self.stls.length; i++) {
                if (self.stls[i].i == stallId) {
                    self.qty -= self.stls[i].qty;
                    self.stls.splice(i, 1);
                    break;
                }
            }
            self.save();
        };

        //add item
        self.add = function (nwItem, qty) {
            console.log(nwItem);
            qty = qty ? parseInt(qty) : 1;
            self.qty += qty;

            var isExist = false;
            angular.forEach(self.stls, function (stall, idxStall) {
                if (stall.i == nwItem.stallId) {
                    //existing stl
                    isExist = true;
                    stall.add(nwItem, qty);
                    return false;
                }
            });

            if (!isExist) {
                var stall = new Greenspot.CartStallItem(nwItem.stallId, nwItem.stallName);
                stall.add(nwItem, qty);
                self.stls.push(stall);
            }

            //new item
            self.save();
        };

        //remove item
        self.remove = function (stall, itemId) {
            self.qty -= stall.remove(itemId);
            //remove empty stall
            if (stall.itms.length == 0) {
                var delIdx = self.stls.indexOf(stall);
                if (delIdx > -1) {
                    self.stls.splice(delIdx, 1);
                }
            }

            self.save();
        };

        //clear
        self.empty = function () {
            self.stls = [];
            self.qty = 0;
            Cookies.remove("cart");
        };

        //item qty+1
        self.plusOne = function (stall, itemId) {
            stall.plusOne(itemId);
            self.qty++;

            self.save();
        };

        //item qty-1
        self.minusOne = function (stall, itemId) {
            stall.minusOne(itemId);
            self.qty--;

            self.save();
        };

        //load stored cart
        self.load = function () {
            //var str = Cookies.get('cart');
            var str = window.localStorage["jdl_cart"];

            if (!str) {
                return;
            }

            var c = angular.fromJson(str);
            var timeSpan = Date.now() - c.savedTime;
            if (timeSpan > 604800000) {
                return;
            }

            self.stls = [];

            //all stall
            angular.forEach(c.stls, function (stall, idxStall) {
                var stallItem = new Greenspot.CartStallItem(stall.i, stall.n);
                stallItem.qty = stall.qty;
                stallItem.amt = stall.amt;
                stallItem.itms = stall.itms;
                self.stls.push(stallItem);
            });

            self.qty = c.qty;

        };

        //store cart
        self.save = function () {
            //Cookies.set("cart", self, { expires: 14 });
            self.savedTime = Date.now();
            window.localStorage.setItem("jdl_cart", JSON.stringify(self));
        }

        //is all items selected
        self.isChecked = function () {
            var hasChecked = false;
            var hasUnchecked = false;
            angular.forEach(self.stls, function (stall, idxStall) {
                hasChecked = hasChecked || stall.isChecked();
                hasUnchecked = hasUnchecked || !stall.isChecked();
            })
            return hasChecked && !hasUnchecked;
        }

        //select or unselect all items
        self.toggleAll = function () {
            var checked = !self.isChecked();
            angular.forEach(self.stls, function (stall, idxStall) {
                angular.forEach(stall.itms, function (item, idxItem) {
                    item.slctd = checked;
                })
            });
        }

        //get total selected items amount
        self.totalAmount = function (selectedOnly) {
            selectedOnly = selectedOnly || false;
            var amount = 0;
            angular.forEach(self.stls, function (stall, idxStall) {

                //new order
                angular.forEach(stall.itms, function (item, idxItem) {
                    if (!selectedOnly || item.slctd) {
                        amount += (item.p * item.q);
                    }
                })
            });
            return amount;
        }

        //convert selected items to order
        self.getOrders = function () {
            var orders = new Greenspot.CartOrderCollection();
            angular.forEach(self.stls, function (stall, idxStall) {
                //new order
                var order = new Greenspot.CartOrder(stall.i, stall.n);

                angular.forEach(stall.itms, function (item, idxItem) {
                    if (item.slctd) {
                        order.AddItem(item);
                    }
                })

                if (order.qty > 0) {
                    orders.AddOrder(order);
                }
            });

            return orders;
        }

        //remove selected items
        self.removeSelected = function () {
            var orders = new Greenspot.CartOrderCollection();
            angular.forEach(self.stls, function (stall, idxStall) {

                if (stall.isChecked()) {
                    self.removeStall(stall.i);
                } else {
                    angular.forEach(stall.itms, function (item, idxItem) {
                        if (item && item.slctd) {
                            self.remove(stall, item.i);
                        }
                    })
                }
            });

            self.save();
        }
    };

    Greenspot.CartOrder = function (stallId, stallName) {
        var self = this;
        self.i = stallId;
        self.n = stallName;
        self.qty = 0;
        self.amt = 0;
        self.itms = [];

        self.AddItem = function (item) {
            self.itms.push(item);
            self.qty += item.q;
            self.amt += item.p * item.q;
        }
    }

    Greenspot.CartOrderCollection = function () {
        var self = this;
        self.orders = [];
        self.amt = 0;
        self.qty = 0;
        self.AddOrder = function (order) {
            self.orders.push(order);
            self.qty += order.qty;
            self.amt += order.amt;
        }
    }
})();