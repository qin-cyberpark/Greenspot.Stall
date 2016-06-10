module greenspotStall {
    class OwnerRegisterInfo {
        public FirstName: string;
        public LastName: string;
        public Mobile: string;
        public Email: string;
    }

    class OwnerRegisterController {
        public OwnerInfo: OwnerRegisterInfo;
        public constructor() {
            this.OwnerInfo = new OwnerRegisterInfo();
        }

        public SubmitContact() {
            console.debug(this.OwnerInfo.FirstName);
        }
    }

    angular.module('greenspotStall').controller('greenspotStall.OwnerRegisterController', OwnerRegisterController);
}