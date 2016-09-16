using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class StallViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public IList<StallProductViewModel> Products { get; set; }
    }

    public class StallProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int StallId { get; set; }
        public string StallName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string Quantity { get; set; } = "1";
    }
}