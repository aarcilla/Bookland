using Bookland.Areas.Admin.Models;
using Bookland.Constants;
using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Helpers;
using Bookland.Helpers.Abstract;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator, Support, Staff")]
    public class ProductController : Controller
    {
        private IProductRepository productRepo;
        private ICategoryRepository categoryRepo;
        private IProductHelpers productHelpers;

        public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo, IProductHelpers productHelpers)
        {
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
            this.productHelpers = productHelpers;
        }

        public ViewResult Index(int categoryID = 1, string order = null)
        {
            // If no order is specified, attempt to retrieve cookie containing last-used order option
            HttpCookie productOrder = Request.Cookies["ProductOrder"];
            if (productOrder != null && order == null)
            {
                order = productOrder.Value;
            }
            else if (order == null)     // I.e. cookie is null, but no order is specified (e.g. first-ever access of 'Index')
            {
                order = ProductOrderOptions.NameAsc;     // Name ascending is assumed as default order
            }

            // Retrieve category tree of category to be filtered
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree(categoryID);
            
            IEnumerable<Product> products = productHelpers.ProductsByOrder(productRepo, order, categoryTree);

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

            return View(new Admin.Models.ProductsViewModel
            {
                Products = products,
                CategoryFilterOptions = CategoryHelpers.ParentCategoryOptions(categoryRepo.GetCategoryTree(), categoryID),
                OrderOptions = productHelpers.ProductOrderOptionsSelectList(order)
            });
        }


        public ViewResult Create()
        {
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

            return View("Editor", new ProductEditorViewModel
            {
                Product = null,
                Action = "Create",
                CategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTree, 1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name, Description, Year, Price")]Product product, 
            int categoryID, HttpPostedFileBase productImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productImage != null)
                    {
                        product = productHelpers.SetProductImage(product, productImage);
                    }

                    product.Category = categoryRepo.GetCategory(categoryID);
                    product.DateAdded = DateTime.Now;

                    productRepo.CreateProduct(product);
                    productRepo.Commit();
                    TempData["message"] = String.Format("'{0}' product successfully added to the database.", product.Name);

                    return RedirectToAction("Index", "Product");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problem persists.");
            }
            
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

            return View("Editor", new ProductEditorViewModel
            {
                Product = product,
                Action = "Create",
                CategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTree, 1)
            });
        }
                 

        public ActionResult Update(int productID) 
        {
            Product product = productRepo.GetProduct(productID);

            if (product != null)
            {
                TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

                return View("Editor", new ProductEditorViewModel
                {
                    Product = product,
                    Action = "Update",
                    CategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTree, product.Category != null ? product.Category.CategoryID : 1)
                });
            }
            else
            {
                string message404 = String.Format("No product exists under the ID of {0}.", productID);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ProductID, Name, Description, Year, Price")]Product product,
            int categoryID, HttpPostedFileBase productImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productImage != null)
                    {
                        product = productHelpers.SetProductImage(product, productImage);
                    }

                    product.Category = categoryRepo.GetCategory(categoryID);

                    productRepo.UpdateProduct(product);
                    productRepo.Commit();
                    TempData["message"] = String.Format("'{0}' product successfully updated.", product.Name);

                    return RedirectToAction("Index", "Product");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problems persist.");
            }

            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

            return View("Editor", new ProductEditorViewModel
            {
                Product = product,
                Action = "Update",
                CategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTree, 1)
            });
        }


        public ActionResult Delete(int productID)
        {
            Product product = productRepo.GetProduct(productID);

            if (product != null)
            {
                // Return view of confirmation message (i.e. "Are you sure?")
                return View(product);
            }
            else
            {
                string message404 = String.Format("No product exists under the ID of {0}.", productID);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Delete(int productID, string name)
        {
            try
            {
                productRepo.DeleteProduct(productID);
                productRepo.Commit();
                TempData["message"] = String.Format("'{0}' product successfully deleted from the database.", name);

                return RedirectToAction("Index", "Product");
            }
            catch (DataException)
            {
                TempData["message"] = "Deletion was not successful. Please contact your system admin if problems persist.";
            }

            return RedirectToAction("Delete", new { productID = productID });
        }
    }
}
