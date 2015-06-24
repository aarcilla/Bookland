using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Helpers.Abstract
{
    public interface IProductHelpers
    {
        IEnumerable<SelectListItem> ProductOrderOptionsSelectList(string selected);

        IEnumerable<Product> ProductsByOrder(IProductRepository productRepo, string order, TreeNode<Category> categoryTree = null);

        Product SetProductImage(Product product, HttpPostedFileBase productImage);

        IEnumerable<SelectListItem> ProductStatusOptions(IEnumerable<ProductStatus> productStatuses, int? selectedStatus);
    }
}
