using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Models
{
    public class ProductsViewModel
    {
        public List<Product> Products { get; set; }
        public int NumColumns { get; set; }
        public string Heading { get; set; }
        public IEnumerable<SelectListItem> OrderOptions { get; set; }
    }
}