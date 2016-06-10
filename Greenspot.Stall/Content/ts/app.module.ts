'use strict';

((): void => {
    angular.module('greenspotStall',
        ['ngRoute', 'ngMaterial', 'ngMessages'])
        .config(
        function ($routeProvider,
            $locationProvider,
            $httpProvider,
            $mdThemingProvider) {
            $mdThemingProvider.theme('default')
                .primaryPalette('teal')
                .accentPalette('lime');
            greenspotStall.Routes.configure($routeProvider, $locationProvider);
            //expenseApp.Adal.configure($httpProvider, settings, adalProvider);
        });
})();