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
            $.each(self.itms, function (idxItem, item) {
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
            $.each(self.itms, function (idxItem, item) {
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
            $.each(self.itms, function (idxItem, item) {
                hasSelected = hasSelected || item.slctd;
                hasUnselected = hasUnselected || !item.slctd;
            })
            return hasSelected && !hasUnselected;
        };

        self.isIndeterminate = function () {
            var hasSelected = false;
            var hasUnselected = false;
            $.each(self.itms, function (idxItem, item) {
                hasSelected = hasSelected || item.slctd;
                hasUnselected = hasUnselected || !item.slctd;
            })
            return hasSelected && hasUnselected;
        }

        self.toggle = function (item) {
            item.slctd = !item.slctd;
        };

        self.toggleAll = function () {
            var checked = self.isIndeterminate() || !self.isChecked();
            $.each(self.itms, function (idxItem, item) {
                item.slctd = checked;
            })
        }

        self.plusOne = function (itemId) {
            $.each(self.itms, function (idxItem, item) {
                if (item.i == itemId && item.q < 99) {
                    item.q++;
                    self.qty++;
                }
            })
        }

        self.minusOne = function (itemId) {
            $.each(self.itms, function (idxItem, item) {
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
            self.writeToCookie();
        };

        //add item
        self.add = function (nwItem, qty) {
            qty = qty ? parseInt(qty) : 1;
            self.qty += qty;

            var isExist = false;
            $.each(self.stls, function (idxStall, stall) {
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
            self.writeToCookie();
        };


        //clear
        self.empty = function () {
            self.stls = [];
            self.qty = 0;
            Cookies.remove("cart");
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

            self.writeToCookie();
        };

        //this
        self.plusOne = function (stall, itemId) {
            stall.plusOne(itemId);
            self.qty++;

            self.writeToCookie();
        };

        //
        self.minusOne = function (stall, itemId) {
            stall.minusOne(itemId);
            self.qty--;

            self.writeToCookie();
        };


        self.loadFromCookie = function () {
            var str = Cookies.get('cart');

            if (!str) {
                return;
            }
            var c = $.parseJSON(str);
            self.stls = [];

            //all stall
            $.each(c.stls, function (idxStall, stall) {
                var stallItem = new Greenspot.CartStallItem(stall.i, stall.n);
                stallItem.qty = stall.qty;
                stallItem.amt = stall.amt;
                stallItem.itms = stall.itms;
                self.stls.push(stallItem);
            });

            self.qty = c.qty;

        };

        self.writeToCookie = function () {
            Cookies.set("cart", self, { expires: 14 });
        };

        self.isChecked = function () {
            var hasChecked = false;
            var hasUnchecked = false;
            $.each(self.stls, function (idxStall, stall) {
                hasChecked = hasChecked || stall.isChecked();
                hasUnchecked = hasUnchecked || !stall.isChecked();
            })
            return hasChecked && !hasUnchecked;
        };

        self.toggleAll = function () {
            var checked = !self.isChecked();
            $.each(self.stls, function (idxStall, stall) {
                $.each(stall.itms, function (idxItem, item) {
                    item.slctd = checked;
                })
            });
        }

        self.totalAmount = function () {
            var orders = self.getOrders();
            return orders.amt;
        }

        self.getOrders = function () {
            var orders = new Greenspot.SimpleOrderCollection();
            $.each(self.stls, function (idxStall, stall) {
                //new order
                var order = new Greenspot.SimpleOrder(stall.i, stall.n);

                $.each(stall.itms, function (idxItem, item) {
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
    };

    Greenspot.SimpleOrder = function (stallId, stallName) {
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

    Greenspot.SimpleOrderCollection = function () {
        var self = this;
        self.orders = [];
        self.amt = 0;

        self.AddOrder = function (order) {
            self.orders.push(order);
            self.qty += order.qty;
            self.amt += order.amt;
        }
    }
})();