using Bookland.Models;
using System.Collections.Generic;

namespace Bookland.Areas.Admin.Models
{
    public class PurchasesViewModel
    {
        public IEnumerable<Purchase> Purchases { get; set; }
        public string UserName { get; set; }
    }
}