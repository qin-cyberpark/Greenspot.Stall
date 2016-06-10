module greenspotStall {
    'use strict';

    export class Routes {
        static configure($routeProvider, $locationProvider) {
            var viewBase: string = '/views/';

            $routeProvider
                .when('/owner/register/', {
                    controller: 'greenspotStall.OwnerRegisterController',
                    templateUrl: viewBase + 'owner/register-contact.html',
                    controllerAs: 'vm'
                })
                .when('/owner/register/vend', {
                    controller: 'greenspotStall.OwnerRegisterController',
                    templateUrl: viewBase + 'owner/register-vend.html',
                    controllerAs: 'vm'
                })
                .when('/owner/register/completed', {
                    controller: 'greenspotStall.OwnerRegisterController',
                    templateUrl: viewBase + 'owner/register-completed.html',
                    controllerAs: 'vm'
                })
                .otherwise({ redirectTo: '/' });

            // configure html5 to get links working on jsfiddle
            $locationProvider.html5Mode(true);
        }
    }
}