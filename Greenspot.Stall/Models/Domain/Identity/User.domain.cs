using Greenspot.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    public partial class User
    {
        public static IList<User> GetByRole(StallEntities db, string roleName)
        {
            var role = db.Roles.FirstOrDefault(x => x.Name.Equals(roleName));
            if (role == null)
            {
                return new List<User>();
            }
           
            return role.Users.ToList();
        }
    }
}
