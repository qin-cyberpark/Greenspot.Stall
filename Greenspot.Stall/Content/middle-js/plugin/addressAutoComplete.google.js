(function () {
    'use strict';
    var module = angular.module('greenspotStall');

    //directive
    module.directive('googleplace', function () {
        return {
            scope: {
                addrObj: '=addressObject',
                placeOnSelect: '&'
            },
            link: function (scope, element, attrs, model) {

                var options = {
                    types: ['address'],
                    componentRestrictions: { country: 'nz' }
                };
                scope.googlePlaceAutocomplete = new google.maps.places.Autocomplete(element[0], options);
                google.maps.event.addListener(scope.googlePlaceAutocomplete, 'place_changed', function () {
                    scope.$apply(function () {
                        scope.addrObj = scope.googlePlaceAutocomplete.getPlace();
                    });
                    scope.placeOnSelect();
                });
            }
        };
    });
})();