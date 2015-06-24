using Bookland.Constants;
using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Helpers
{
    /// <summary>
    /// A collection of helper properties and methods that caters to displaying options and performing common functions for Products.
    /// </summary>
    public class ProductHelpers : Abstract.IProductHelpers
    {
        /// <summary>
        /// Create an enumeration of items for drop-down list of product ordering.
        /// </summary>
        /// <param name="selected">The pre-selected/displayed item for the drop-down list.</param>
        /// <returns>An enumeration of drop-down list order options.</returns>
        public IEnumerable<SelectListItem> ProductOrderOptionsSelectList(string selected)
        {
            if (selected == null)
            {
                throw new System.ArgumentNullException("selected", "selected cannot be null.");
            }

            var list = new List<SelectListItem>() {
                new SelectListItem { Text = "Name: A - Z", Value = ProductOrderOptions.NameAsc, Selected = (selected == ProductOrderOptions.NameAsc) },
                new SelectListItem { Text = "Name: Z - A", Value = ProductOrderOptions.NameDesc, Selected = (selected == ProductOrderOptions.NameDesc) },
                new SelectListItem { Text = "ID: 0 - 9", Value = ProductOrderOptions.IdAsc, Selected = (selected == ProductOrderOptions.IdAsc) },
                new SelectListItem { Text = "ID: 9 - 0", Value = ProductOrderOptions.IdDesc, Selected = (selected == ProductOrderOptions.IdDesc) },
                new SelectListItem { Text = "Price: 0 - 9", Value = ProductOrderOptions.PriceAsc, Selected = (selected == ProductOrderOptions.PriceAsc) },
                new SelectListItem { Text = "Price: 9 - 0", Value = ProductOrderOptions.PriceDesc, Selected = (selected == ProductOrderOptions.PriceDesc) },
                new SelectListItem { Text = "Date added: Old - New", Value = ProductOrderOptions.DateAddedAsc, Selected = (selected == ProductOrderOptions.DateAddedAsc) },
                new SelectListItem { Text = "Date added: New - Old", Value = ProductOrderOptions.DateAddedDesc, Selected = (selected == ProductOrderOptions.DateAddedDesc) }
            };

            return list;
        }

        /// <summary>
        /// Retrieve products, in the order specified.
        /// </summary>
        /// <param name="productRepo">An existing instance of the Product repository.</param>
        /// <param name="order">The order identifier (e.g. 'name_desc' = order by product names descending).</param>
        /// <param name="categoryTree">A category tree, where the root node is the category to be filtered, along with its descendants.</param>
        /// <returns>An enumeration of products in the desired order.</returns>
        public IEnumerable<Product> ProductsByOrder(IProductRepository productRepo, string order, TreeNode<Category> categoryTree = null)
        {
            IEnumerable<Product> products;
            switch (order)
            {
                case ProductOrderOptions.NameAsc:
                    products = productRepo.GetProducts(p => p.Name, categoryFilter: categoryTree);
                    break;
                case ProductOrderOptions.NameDesc:
                    products = productRepo.GetProducts(p => p.Name, true, categoryTree);
                    break;
                case ProductOrderOptions.IdAsc:
                    products = productRepo.GetProducts(p => p.ProductID, categoryFilter: categoryTree);
                    break;
                case ProductOrderOptions.IdDesc:
                    products = productRepo.GetProducts(p => p.ProductID, true, categoryTree);
                    break;
                case ProductOrderOptions.PriceAsc:
                    products = productRepo.GetProducts(p => p.Price, categoryFilter: categoryTree);
                    break;
                case ProductOrderOptions.PriceDesc:
                    products = productRepo.GetProducts(p => p.Price, true, categoryTree);
                    break;
                case ProductOrderOptions.DateAddedAsc:
                    products = productRepo.GetProducts(p => p.DateAdded, categoryFilter: categoryTree);
                    break;
                case ProductOrderOptions.DateAddedDesc:
                    products = productRepo.GetProducts(p => p.DateAdded, true, categoryTree);
                    break;
                case null:
                    throw new System.ArgumentNullException("order", "order cannot be null.");
                default:
                    products = productRepo.GetProducts(p => p.Name, categoryFilter: categoryTree);
                    break;
            }

            return products;
        }

        /// <summary>
        /// Adds image information to a product.
        /// </summary>
        /// <param name="product">The Product object that will accept the uploaded image.</param>
        /// <param name="productImage">The uploaded image file.</param>
        /// <returns>The Product object with image data included.</returns>
        public Product SetProductImage(Product product, HttpPostedFileBase productImage)
        {
            if (productImage != null && productImage.InputStream != null && productImage.ContentType != null)
            {
                product.ImageMimeType = productImage.ContentType;

                product.ImageData = new byte[productImage.ContentLength];
                productImage.InputStream.Read(product.ImageData, 0, productImage.ContentLength);
            }
            else
            {
                if (productImage == null)
                {
                    throw new System.ArgumentNullException("productImage", "productImage cannot be null.");
                }
                else if (productImage.InputStream == null)
                {
                    throw new System.ArgumentException("productImage's InputStream property cannot be null.", "productImage.InputStream");
                }
                else if (productImage.ContentType == null)
                {
                    throw new System.ArgumentException("productImage's ContentType property cannot be null.", "productImage.ContentType");
                }
            }

            return product;
        }

        public IEnumerable<SelectListItem> ProductStatusOptions(IEnumerable<ProductStatus> productStatuses, int? selectedStatus)
        {
            var productStatusSelectList = new List<SelectListItem>();

            if (!selectedStatus.HasValue)
                selectedStatus = 1;

            foreach (ProductStatus status in productStatuses)
            {
                productStatusSelectList.Add(new SelectListItem
                {
                    Text = status.ProductStatusName,
                    Value = status.ProductStatusID.ToString(),
                    Selected = selectedStatus.Value == status.ProductStatusID
                });
            }

            return productStatusSelectList;
        }
    }
}