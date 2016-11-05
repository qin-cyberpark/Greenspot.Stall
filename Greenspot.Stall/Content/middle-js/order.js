var Greenspot = Greenspot || {};
(function () {
    Greenspot.Order = function (data, $http) {
        var self = this;
        self.$http = $http;
        self.i = data.i;
        self.n = data.n;
        self.qty = data.qty;
        self.amt = data.amt;
        self.itms = data.itms;
        self.deliveryOptionCollections = [];
        self.pickUpOptionCollections = [];
        self.selectedPickupAddressOptions = [];
        self.dateOptionCollections = [];
        self.selectedDateOptions = [];
        self.selectedDeliveryOrPickupOption = null;

        /* set delivery address */
        self.setDeliveryAddress = function (address) {
            self.deliveryAddress = address;
            self.loadDeliveryOptions();
        }



        /* get delivery option*/
        self.loadDeliveryOptions = function () {
            //clear
            self.deliveryOptionCollections = [];

            if (!self.deliveryAddress) {
                return;
            }

            //get delivery options
            var url = '/api/stall/' + self.i + '/GetDeliveryOptions?country=' + self.deliveryAddress.CountryCode;
            url += "&city=" + self.deliveryAddress.City;
            url += "&suburb=" + self.deliveryAddress.Suburb;
            url += "&area=" + self.deliveryAddress.Area;
            //var url = '/api/stall/' + self.i + '/GetDeliveryOptions?area=' + self.deliveryAddress.Area;
            url += "&orderAmount=" + self.amt;

            self.$http.get(url).success(function (result) {
                if (result.Succeeded) {
                    self.deliveryOptionCollections = result.Data;
                    if (self.hasDeliveryOption()) {
                        self.deliveryOrPickupChanged(false);
                    } else if (self.hasPickUpOption()) {
                        self.deliveryOrPickupChanged(true);
                    }
                }
                else {
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            });
        }


        /*get pickup option*/
        self.loadPickUpOptions = function () {

            //clear
            self.pickUpOptionCollections = [];

            //get pick up option
            var url = '/api/stall/' + self.i + '/GetPickUpOptions';

            $http.get(url).success(function (result) {
                if (result.Succeeded) {
                    self.pickUpOptionCollections = result.Data;
                    if (self.hasPickUpOption()) {
                        self.deliveryOrPickupChanged(true);
                    }
                }
                else {
                    //$window.alert(result.Message);
                    console.log(result.Message);
                }
            }).error(function (error) {
                console.log(error);
            });
        }

        self.hasDeliveryOption = function () {
            return self.deliveryOptionCollections && self.deliveryOptionCollections.length > 0;
        }

        self.hasPickUpOption = function () {
            return self.pickUpOptionCollections && self.pickUpOptionCollections.length > 0;
        }

        //pick up address changed
        self.pickupAddressChanged = function () {
            self.dateOptionCollections = self.selectedPickupAddressOptions.Groups;
            self.selectedDateOptions = self.dateOptionCollections[0];
            self.dateOptionCollectionChanged();
        }

        //* swtich between delivery and pickup
        self.deliveryOrPickupChanged = function (isPickup) {
            self.isPickup = isPickup;
            if (!isPickup) {
                //delivery
                self.dateOptionCollections = self.deliveryOptionCollections;
                self.selectedDateOptions = self.dateOptionCollections[0];
                self.dateOptionCollectionChanged();
            } else {
                //pickup
                self.isPickup = true;
                self.selectedPickupAddressOptions = self.pickUpOptionCollections[0];
                self.pickupAddressChanged();
            }
        }

        /* select date options collection*/
        self.dateOptionCollectionChanged = function () {
            self.selectedDeliveryOrPickupOption = self.selectedDateOptions.Options[0];
        }

        /* total amount*/
        self.totalAmount = function () {
            return self.amt + (self.selectedDeliveryOrPickupOption ? self.selectedDeliveryOrPickupOption.Fee : 0);
        }

        self.loadPickUpOptions();
    }
})();