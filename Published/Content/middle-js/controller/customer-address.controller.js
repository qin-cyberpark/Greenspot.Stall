(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('CustomerAddressController',
        ['$scope', '$http', '$location', '$window', '$mdDialog',
        function ($scope, $http, $location, $window, $mdDialog) {
            var vm = this;
            ///**********************************
            //address management
            //***********************************
            /* redirect */
            vm.gotoUrl = function (url) {
                window.location.href = url;
            }


            /* address management */
            vm.init_address = function () {
                vm.loadDeliveryAddress();
            }

            /*new address*/
            vm.editAddress = {};

            /* load address */
            vm.loadDeliveryAddress = function () {
                //load address
                $http.get('/customer/DeliveryAddresses').success(function (result) {
                    if (result.Succeeded) {
                        vm.deliveryAddresses = result.Data;
                        if (vm.deliveryAddresses.length > 0) {
                            vm.deliveryAddress = vm.deliveryAddresses[0];
                        }
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error);
                });
            }

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
        }]);
})();