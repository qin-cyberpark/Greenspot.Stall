(function () {
    'use strict';

    angular.module('greenspotStall', ['ngMaterial', 'ngMessages'])
       .config(['$locationProvider', '$mdThemingProvider', '$mdIconProvider',
               function config($locationProvider, $mdThemingProvider, $mdIconProvider) {
                   //theme
                   $mdThemingProvider.theme('default')
                       .primaryPalette('cyan', { 'default': '700' })
                       .accentPalette('orange', { 'default': '800' })
                       .backgroundPalette('grey', { 'default': '100' });

                   //
                   $mdIconProvider
                      .defaultIconSet('/static/img/icon/core-icons.svg', 24);

                   //location
                   //$locationProvider.html5Mode(true);
               }]);

    angular.module('greenspotStall')
        .service('CommonService', function ($rootScope) {
            /* redirect */
            $rootScope.gotoUrl = function (url) {
                window.location.href = url;
            }


            $rootScope.loadingCircle = true;
            this.showLoading = function () {
                $rootScope.loadingCircle = true;
            }

            this.hideLoading = function () {
                $rootScope.loadingCircle = false;
            }
        });
})();