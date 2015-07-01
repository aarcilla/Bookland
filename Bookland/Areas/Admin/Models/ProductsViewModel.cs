using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Models
{
    public class ProductsViewModel
    {
        public List<Product> Products { get; set; }
        public IEnumerable<SelectListItem> CategoryFilterOptions { get; set; }
        public IEnumerable<SelectListItem> OrderOptions { get; set; }
    }
}