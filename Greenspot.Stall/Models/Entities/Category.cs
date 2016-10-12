using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Category
    {
        public int Index { get; set; }
        public string Identifier { get; set; }
        public string English { get; set; }
        public string Chinese { get; set; }
    }
}