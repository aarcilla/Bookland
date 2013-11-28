using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Helpers;
using Bookland.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository productRepo;
        private ICategoryRepository categoryRepo;

        public HomeController(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
        }

        public ViewResult Index(int? category, string order = null)
        {
            // If no order is specified, attempt to retrieve cookie containing last-used order option
            HttpCookie productOrder = Request.Cookies["ProductOrder"];
            if (productOrder != null && order == null)
            {
                order = productOrder.Value;
            }
            else if (order == null)     // I.e. cookie is null, but no order is specified (e.g. first-ever access of 'Index')
            {
                order = ProductHelpers.NameAsc;     // Name ascending is assumed as default order
            }

            if (productOrder == null)
            {
                productOrder = new HttpCookie("ProductOrder");
                Response.Cookies.Add(productOrder);
            }

            // Update cookie value and expiry if it has changed, or was just created
            if (productOrder.Value != order)
            {
                Response.Cookies["ProductOrder"].Value = order;
                Response.Cookies["ProductOrder"].Expires = DateTime.Now.AddMonths(1);
            }

            // Retrieve Category tree (a Category and its descendant Categories) for Category-filtered display
            TreeNode<Category> categoryTree = category.HasValue ? categoryRepo.GetCategoryTree(category.Value) : null;

            return View(new ProductsViewModel
            {
                Products = ProductHelpers.ProductsByOrder(productRepo, order, categoryTree),
                NumColumns = 3,
                Heading = categoryTree != null ? categoryTree.Data.CategoryName : null,
                OrderOptions = ProductHelpers.ProductOrderOptions(order)
            });
        }

        /// <summary>
        /// Retrieve images contained within the DB.
        /// </summary>
        /// <param name="productID">The ID of the requested product.</param>
        /// <returns>A File object containing the desired image, or null if product isn't found.</returns>
        public FileContentResult GetImage(int productID)
        {
            Product product = productRepo.GetProduct(productID);

            if (product != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }

            return null;
        }
    }
}
