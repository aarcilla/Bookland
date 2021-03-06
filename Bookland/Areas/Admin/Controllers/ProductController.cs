﻿using Bookland.Areas.Admin.Models;
using Bookland.Constants;
using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Helpers.Abstract;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator, Support, Staff")]
    public class ProductController : Controller
    {
        private readonly string[] safeTags = { "p", "b", "i", "ul", "ol", "li" };
        private readonly string[] safeSingleTags = { "br" };

        private IProductRepository productRepo;
        private ICategoryRepository categoryRepo;
        private IProductStatusRepository productStatusRepo;
        private IProductHelpers productHelpers;
        private ICategoryHelpers categoryHelpers;

        public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo, IProductStatusRepository productStatusRepo,
            IProductHelpers productHelpers, ICategoryHelpers categoryHelpers)
        {
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
            this.productStatusRepo = productStatusRepo;
            this.productHelpers = productHelpers;
            this.categoryHelpers = categoryHelpers;
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
            
            var products = productHelpers.ProductsByOrder(productRepo, order, categoryTree).ToList<Product>();

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
                CategoryFilterOptions = categoryHelpers.ParentCategoryOptions(categoryRepo.GetCategoryTree(), categoryID),
                OrderOptions = productHelpers.ProductOrderOptionsSelectList(order)
            });
        }


        public ViewResult Create()
        {
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();
            IEnumerable<ProductStatus> productStatuses = productStatusRepo.GetProductStatuses();

            return View("Editor", new ProductEditorViewModel
            {
                Product = null,
                Action = "Create",
                CategoryOptions = categoryHelpers.ParentCategoryOptions(categoryTree, 1),
                ProductStatusOptions = productHelpers.ProductStatusOptions(productStatuses, 1)
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name, Description, Year, ReleaseDate, Price")]Product product, 
            int categoryID, int productStatusID, HttpPostedFileBase productImage)
        {
            try
            {
                product.Name = HttpUtility.HtmlEncode(product.Name);
                if (product.Name.Length > product.NameMaxLength)
                    ModelState.AddModelError("", "Name in HTML-encoded format exceeds maximum character length.");

                product.Description = EncodeAndAllowSafeHtmlTags(product.Description);
                if (product.Description.Length > product.DescriptionMaxLength)
                    ModelState.AddModelError("", "Description in HTML-encoded format exceeds maximum character length.");

                if (ModelState.IsValid)
                {
                    if (productImage != null)
                    {
                        product = productHelpers.SetProductImage(product, productImage);
                    }

                    product.Category = categoryRepo.GetCategory(categoryID);
                    product.ProductStatus = productStatusRepo.GetProductStatus(productStatusID);
                    product.DateAdded = DateTime.Now;

                    productRepo.CreateProduct(product);
                    productRepo.Commit();
                    TempData["message"] = String.Format("'{0}' product successfully added to the database.", product.Name);

                    return RedirectToAction("Index", "Product");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please contact your system admin if problem persists.");
            }
            
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();
            IEnumerable<ProductStatus> productStatuses = productStatusRepo.GetProductStatuses();

            return View("Editor", new ProductEditorViewModel
            {
                Product = product,
                Action = "Create",
                CategoryOptions = categoryHelpers.ParentCategoryOptions(categoryTree, categoryID),
                ProductStatusOptions = productHelpers.ProductStatusOptions(productStatuses, productStatusID)
            });
        }
                 

        public ActionResult Update(int productID) 
        {
            Product product = productRepo.GetProduct(productID);

            if (product != null)
            {
                TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();
                IEnumerable<ProductStatus> productStatuses = productStatusRepo.GetProductStatuses();

                product.Description = HttpUtility.HtmlDecode(product.Description);

                return View("Editor", new ProductEditorViewModel
                {
                    Product = product,
                    Action = "Update",
                    CategoryOptions = categoryHelpers.ParentCategoryOptions(categoryTree, 
                                        product.Category != null ? product.Category.CategoryID : 1),
                    ProductStatusOptions = productHelpers.ProductStatusOptions(productStatuses, 
                                            product.ProductStatus != null ? product.ProductStatus.ProductStatusID : (int?)null)
                });
            }
            else
            {
                string message404 = String.Format("No product exists under the ID of {0}.", productID);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ProductID, Name, Description, ReleaseDate, Year, Price")]Product product,
            int categoryID, int productStatusID, HttpPostedFileBase productImage)
        {
            try
            {
                product.Name = HttpUtility.HtmlEncode(product.Name);
                if (product.Name.Length > product.NameMaxLength)
                    ModelState.AddModelError("", "Name in HTML-encoded format exceeds maximum character length.");

                product.Description = EncodeAndAllowSafeHtmlTags(product.Description);
                if (product.Description.Length > product.DescriptionMaxLength)
                    ModelState.AddModelError("", "Description in HTML-encoded format exceeds maximum character length.");

                if (ModelState.IsValid)
                {
                    if (productImage != null)
                    {
                        product = productHelpers.SetProductImage(product, productImage);
                    }

                    product.Category = categoryRepo.GetCategory(categoryID);
                    product.ProductStatus = productStatusRepo.GetProductStatus(productStatusID);

                    productRepo.UpdateProduct(product);
                    productRepo.Commit();
                    TempData["message"] = String.Format("'{0}' product successfully updated.", product.Name);

                    return RedirectToAction("Index", "Product");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please contact your system admin if problems persist.");
            }

            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();
            IEnumerable<ProductStatus> productStatuses = productStatusRepo.GetProductStatuses();

            return View("Editor", new ProductEditorViewModel
            {
                Product = product,
                Action = "Update",
                CategoryOptions = categoryHelpers.ParentCategoryOptions(categoryTree, categoryID),
                ProductStatusOptions = productHelpers.ProductStatusOptions(productStatuses, productStatusID)
            });
        }

        /*
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
        */

        private string EncodeAndAllowSafeHtmlTags(string inputHtml)
        {
            StringBuilder inputHtmlEncoded = new StringBuilder(HttpUtility.HtmlEncode(inputHtml));

            foreach (string safeTag in safeTags)
            {
                inputHtmlEncoded.Replace(string.Format("&lt;{0}&gt;", safeTag), string.Format("<{0}>", safeTag));
                inputHtmlEncoded.Replace(string.Format("&lt;/{0}&gt;", safeTag), string.Format("</{0}>", safeTag));
            }

            foreach (string safeSingleTag in safeSingleTags)
            {
                inputHtmlEncoded.Replace(string.Format("&lt;{0} /&gt;", safeSingleTag), string.Format("<{0} />", safeSingleTag));
            }

            return inputHtmlEncoded.ToString();
        }
    }
}
