var greenspotStall;
(function (greenspotStall) {
    var OwnerRegisterInfo = (function () {
        function OwnerRegisterInfo() {
        }
        return OwnerRegisterInfo;
    }());
    var OwnerRegisterController = (function () {
        function OwnerRegisterController() {
            this.OwnerInfo = new OwnerRegisterInfo();
        }
        OwnerRegisterController.prototype.SubmitContact = function () {
            console.debug(this.OwnerInfo.FirstName);
        };
        return OwnerRegisterController;
    }());
    angular.module('greenspotStall').controller('greenspotStall.OwnerRegisterController', OwnerRegisterController);
})(greenspotStall || (greenspotStall = {}));
//# sourceMappingURL=owner-register.controller.js.map