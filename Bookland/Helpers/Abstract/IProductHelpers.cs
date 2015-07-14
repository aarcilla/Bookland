using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Helpers.Abstract
{
    public interface IProductHelpers
    {
        IEnumerable<SelectListItem> ProductOrderOptionsSelectList(string selected);

        IEnumerable<Product> ProductsByOrder(IProductRepository productRepo, string order, TreeNode<Category> categoryTree = null, Expression<Func<Product, bool>> where = null);

        Product SetProductImage(Product product, HttpPostedFileBase productImage);

        IEnumerable<SelectListItem> ProductStatusOptions(IEnumerable<ProductStatus> productStatuses, int? selectedStatus);
    }
}
