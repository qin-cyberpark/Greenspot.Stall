(function () {
    'use strict';

    var module = angular.module('greenspotStall');
    module.controller('DialogController', ['$scope', '$mdDialog',
         function ($scope, $mdDialog) {
             $scope.hide = function () {
                 $mdDialog.hide();
             };
             $scope.cancel = function () {
                 $mdDialog.cancel();
             };
             $scope.answer = function (answer) {
                 $mdDialog.hide(answer);
             };
         }]);
})();