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
                   $locationProvider.html5Mode(true);
               }]);

    angular.module('greenspotStall')
        .service('CommonService', function ($rootScope) {
            var self = this;

            /* redirect */
            $rootScope.gotoUrl = function (url) {
                self.showLoading();
                window.location.href = url;
            }


            $rootScope.loadingCircle = false;
            self.showLoading = function () {
                $rootScope.loadingCircle = true;
            }

            self.hideLoading = function () {
                $rootScope.loadingCircle = false;
            }

            self.isLoading = function () {
                return $rootScope.loadingCircle;
            }
        });

    angular.module('greenspotStall')
        .filter("trust", ['$sce', function ($sce) {
            return function (htmlCode) {
                return $sce.trustAsHtml(htmlCode);
            }
        }]);
})();