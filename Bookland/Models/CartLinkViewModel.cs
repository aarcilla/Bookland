using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookland.Models
{
    public class CartLinkViewModel
    {
        public int ItemCount { get; set; }
        public decimal TotalCost { get; set; }
    }
}