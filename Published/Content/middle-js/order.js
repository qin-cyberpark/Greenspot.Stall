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
        self.optionCollections = [];
        self.selectedOptionCollection = null;
        self.selectedOption = null;

        /* set delivery address */
        self.setDeliveryAddress = function (address) {
            self.deliveryAddress = address;
            self.optionCollections = [];
            self.selectedOptionCollection = null;
            self.selectedOption = null;
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
            var url = '/api/stall/GetDeliveryOptions/' + self.i + "?country=" + self.deliveryAddress.CountryId;
            url += "&city=" + self.deliveryAddress.City;
            url += "&suburb=" + self.deliveryAddress.Suburb;
            url += "&area=" + self.deliveryAddress.Area;

            self.$http.get(url).success(function (result) {
                if (result.Succeeded) {
                    self.deliveryOptionCollections = result.Data;
                    if (self.hasDeliveryOption()) {
                        self.setIsPickup(false);
                    } else if (self.hasPickUpOption()) {
                        self.setIsPickup(true);
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
            var url = '/api/stall/GetPickUpOptions/' + self.i;

            $http.get(url).success(function (result) {
                if (result.Succeeded) {
                    self.pickUpOptionCollections = result.Data;
                    if (self.hasPickUpOption()) {
                        self.setIsPickup(true);
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
            return self.deliveryOptionCollections && self.pickUpOptionCollections.length > 0;
        }


        /* select delivery option collection*/
        self.optionCollectionChanged = function () {
            self.selectedOption = self.selectedOptionCollection.ApplicableOpts[0];
            //self.optionChanged();
        }

        /* select delivery option*/
        self.optionChanged = function () {
            //console.log(self.selectedOption);
        }

        /* total amount*/
        self.totalAmount = function () {
            return self.amt + (self.selectedOption ? self.selectedOption.Fee : 0);
        }

        self.setIsPickup = function (isPickup) {
            self.isPickup = isPickup;
            self.optionCollections = isPickup ? self.pickUpOptionCollections : self.deliveryOptionCollections;
            self.selectedOptionCollection = self.optionCollections[0];
            self.optionCollectionChanged();
        }

        self.loadPickUpOptions();
    }
})();