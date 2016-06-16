using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Greenspot.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;

namespace Greenspot.Stall.Models
{
    public partial class User
    {
        [NotMapped]
        private GreenspotUser _greenspotUser;

        #region User
        [NotMapped]
        public string Email
        {
            get
            {
                return _greenspotUser.Email;
            }
            set
            {
                _greenspotUser.Email = value;
            }
        }

        [NotMapped]
        public string PhoneNumber
        {
            get
            {
                return _greenspotUser.PhoneNumber;
            }
            set
            {
                _greenspotUser.PhoneNumber = value;
            }
        }

        [NotMapped]
        public bool HasName
        {
            get
            {
                return !string.IsNullOrEmpty(FirstName);
            }
        }

        public bool Save()
        {
            var result = UserManager.Update(_greenspotUser);
            if (!result.Succeeded)
            {
                return false;
            }

            using (var db = new StallEntities())
            {
                db.Set<User>().AddOrUpdate(this);
                db.SaveChanges();
                return true;
            }
        }
        #endregion


        #region static
        [NotMapped]
        public static string CurrentUserId
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
        }

        [NotMapped]
        private static GreenspotUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<GreenspotUserManager>();
            }
        }

        [NotMapped]
        private static GreenspotUser CurrentGreenspotUser
        {
            get
            {
                return UserManager.FindByIdAsync(CurrentUserId).Result;
            }
        }

        [NotMapped]
        public static User CurrentUser
        {
            get
            {
                User user = null;
                using (var db = new StallEntities())
                {
                    user = db.Users.FirstOrDefault(x => x.Id.Equals(CurrentUserId));
                }

                if (user == null)
                {
                    user = new User()
                    {
                        Id = CurrentUserId
                    };
                }

                user._greenspotUser = CurrentGreenspotUser;

                return user;
            }
        }
        #endregion
    }
}