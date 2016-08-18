(function () {
    'use strict';
    angular.module('greenspotStall', ['ngMaterial', 'ngMessages'])
       .config(['$locationProvider', '$mdThemingProvider', '$mdIconProvider',
               function config($locationProvider, $mdThemingProvider, $mdIconProvider) {
                   //theme
                   $mdThemingProvider.theme('default')
                       .primaryPalette('cyan', { 'default': '700' })
                       .accentPalette('orange', { 'default': 'A200' })
                       .backgroundPalette('cyan', { 'default': '50' });

                   //
                   $mdIconProvider
                      .defaultIconSet('/static/img/icon/core-icons.svg', 24);

                   //location
                   //$locationProvider.html5Mode(true);
               }]);
})();