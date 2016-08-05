(function () {
    'use strict';
    var module = angular.module('greenspotStall');

    //directive
    module.directive('addressChecker', function () {
        return {
            scope: {
                addrObj: '=addressObject',
                placeOnSelect: '&'
            },

            link: function (scope, element, attrs, model) {

                var checker_options = {
                    max_results: 3,
                    search_type: "Physical",
                    populates: {
                        suburb: "Suburb",
                        city: "CityTown",
                        postcode: "Postcode"
                    },
                    theme: {
                        boxClass: "AddrAutoCompleteBox"
                    },
                    success: function (detail) {
                        scope.$apply(function () {
                            scope.addrObj = detail;
                        });
                        scope.placeOnSelect();
                    }
                };

                scope.addressChecker = new NZPost.Addressing.Checker(element[0],
                    'af322550-a9de-0132-86ce-00505692031f', checker_options);
            }
        };
    });
})();