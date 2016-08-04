using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class OwnerRegisterViewModel
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ContactName { get; set; }
        public string StallName { get; set; }
        public string VendPrefix { get; set; }
    }
}