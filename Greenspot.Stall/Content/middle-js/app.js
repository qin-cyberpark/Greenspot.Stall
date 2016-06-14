(function () {
    'use strict';
    angular.module('greenspotStall', ['ngRoute', 'ngMaterial', 'ngMessages'])
       .config(['$locationProvider', '$routeProvider','$mdThemingProvider',
               function config($locationProvider, $routeProvider, $mdThemingProvider) {
                   $locationProvider.html5Mode(true);
                   //route
                   var viewBase = '/views/';
                   $routeProvider
                      .when('/owner/register/', {
                          controller: 'OwnerRegisterController',
                          templateUrl: viewBase + 'owner/register-info.html',
                          controllerAs: 'vm'
                      })
                    .when('/owner/register/completed', {
                        controller: 'OwnerRegisterController',
                        templateUrl: viewBase + 'owner/register-completed.html',
                        controllerAs: 'vm'
                    })
                    .otherwise({ redirectTo: '/' });

                   $locationProvider.html5Mode(true);

                   //theme
                   $mdThemingProvider.theme('default')
                       .primaryPalette('teal')
                       .accentPalette('lime');
               }]);
})();