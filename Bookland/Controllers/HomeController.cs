using Bookland.Constants;
using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Helpers.Abstract;
using Bookland.Models;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository productRepo;
        private ICategoryRepository categoryRepo;
        private IProductHelpers productHelpers;

        public HomeController(IProductRepository productRepo, ICategoryRepository categoryRepo, IProductHelpers productHelpers)
        {
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
            this.productHelpers = productHelpers;
        }

        public ViewResult Index(int? category)
        {
            // Retrieve user-defined order option; if it hasn't been set up yet, just set as name ascending by default
            HttpCookie productOrder = Request.Cookies["ProductOrder"];
            string order = productOrder != null ? productOrder.Value : ProductOrderOptions.NameAsc;

            // Retrieve Category tree (a Category and its descendant Categories) for Category-filtered display
            TreeNode<Category> categoryTree = category.HasValue ? categoryRepo.GetCategoryTree(category.Value) : null;

            // Generate category filter heading by prepending parent category names
            string categoryHeading = string.Empty;
            if (categoryTree != null) 
            {
                StringBuilder categoryHeadingBuilder = new StringBuilder(categoryTree.Data.CategoryName);
                Category currentParentCategory = categoryRepo.GetParentCategory(categoryTree.Data.CategoryID);
                while (currentParentCategory != null && !currentParentCategory.CategoryName.Equals("ROOT"))
                {
                    categoryHeadingBuilder.Insert(0, " > ");
                    categoryHeadingBuilder.Insert(0, currentParentCategory.CategoryName);

                    currentParentCategory = categoryRepo.GetParentCategory(currentParentCategory.CategoryID);
                }

                categoryHeading = categoryHeadingBuilder.ToString();
            }

            return View(new ProductsViewModel
            {
                Products = productHelpers.ProductsByOrder(productRepo, order, categoryTree, 
                                                            p => p.ProductStatus.ProductStatusAvailable).ToList<Product>(),
                NumColumns = 3,
                Heading = categoryHeading,
                OrderOptions = productHelpers.ProductOrderOptionsSelectList(order)
            });
        }

        public ActionResult ProductDetails(int? productID)
        {
            if (!productID.HasValue)
            {
                TempData["message"] = "Product ID not specified";
                return RedirectToAction("Index");
            }

            Product product = productRepo.GetProduct(productID.Value);

            if (product != null && !product.ProductStatus.ProductStatusName.Equals(ProductStatusOptions.NotVisible))
            {
                return View(product);
            }

            return HttpNotFound(string.Format("No product exists with the an ID of {0}.", productID.Value));
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
