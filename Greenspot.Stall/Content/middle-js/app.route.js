var greenspotStall;
(function (greenspotStall) {
    'use strict';
    var Routes = (function () {
        function Routes() {
        }
        Routes.configure = function ($routeProvider, $locationProvider) {
            var viewBase = '/views/';
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
        };
        return Routes;
    }());
    greenspotStall.Routes = Routes;
})(greenspotStall || (greenspotStall = {}));
//# sourceMappingURL=app.route.js.map