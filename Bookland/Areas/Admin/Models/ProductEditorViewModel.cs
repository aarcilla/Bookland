using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Models
{
    public class ProductEditorViewModel
    {
        public Product Product { get; set; }
        public string Action { get; set; }
        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
        public IEnumerable<SelectListItem> ProductStatusOptions { get; set; }
    }
}