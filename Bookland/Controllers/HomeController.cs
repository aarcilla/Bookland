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

        public ViewResult Index(int? category)
        {
            // Retrieve user-defined order option; if it hasn't been set up yet, just set as name ascending by default
            HttpCookie productOrder = Request.Cookies["ProductOrder"];
            string order = productOrder != null ? productOrder.Value : ProductHelpers.NameAsc;

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
        /// Sets the order that products will be displayed in, then redirects back to the user's original location.
        /// </summary>
        /// <param name="order">The specified order.</param>
        /// <param name="returnUrl">The URL path to redirect to after carrying out the intended action, which is typically the user's original location.</param>
        /// <returns></returns>
        public RedirectResult Order(string order, string returnUrl)
        {
            HttpCookie productOrder = Request.Cookies["ProductOrder"];
            if (productOrder == null)
            {
                productOrder = new HttpCookie("ProductOrder");
                Response.Cookies.Add(productOrder);
            }

            Response.Cookies["ProductOrder"].Value = order;
            Response.Cookies["ProductOrder"].Expires = DateTime.Now.AddMonths(1);

            return Redirect(returnUrl);
        }

        /// <summary>
        /// Retrieve images contained within the DB.
        /// </summary>
        /// <param name="productID">The ID of the requested product.</param>
        /// <returns>A File object containing the desired image, or null if product isn't found.</returns>
        public ActionResult GetImage(int productID)
        {
            Product product = productRepo.GetProduct(productID);

            if (product != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }
            else
            {
                string message404 = String.Format("No product exists under the ID of {0}.", productID);
                return HttpNotFound(message404);
            }
        }
    }
}
