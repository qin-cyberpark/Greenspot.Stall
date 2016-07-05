(function () {
    'use strict';
    angular.module('greenspotStall', ['ngMaterial', 'ngMessages'])
       .config(['$locationProvider', '$mdThemingProvider', '$mdIconProvider',
               function config($locationProvider, $mdThemingProvider, $mdIconProvider) {
                   //theme
                   $mdThemingProvider.theme('default')
                       .primaryPalette('teal')
                       .accentPalette('lime');

                   //
                   $mdIconProvider
                      .defaultIconSet('/static/img/icon/core-icons.svg', 24);

                   //location
                   $locationProvider.html5Mode(true);
               }]);
})();