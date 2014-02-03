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
    public static class ProductHelpers
    {
        public const string NameAsc = "name_asc";
        public const string NameDesc = "name_desc";
        public const string IdAsc = "id_asc";
        public const string IdDesc = "id_desc";
        public const string PriceAsc = "price_asc";
        public const string PriceDesc = "price_desc";
        public const string DateAddedAsc = "dateAdded_asc";
        public const string DateAddedDesc = "dateAdded_desc";

        /// <summary>
        /// Create an enumeration of items for drop-down list of product ordering.
        /// </summary>
        /// <param name="selected">The pre-selected/displayed item for the drop-down list.</param>
        /// <returns>An enumeration of drop-down list order options.</returns>
        public static IEnumerable<SelectListItem> ProductOrderOptions(string selected)
        {
            if (selected == null)
            {
                throw new System.ArgumentNullException("selected", "selected cannot be null.");
            }

            var list = new List<SelectListItem>() {
                new SelectListItem { Text = "Name: A - Z", Value = NameAsc, Selected = (selected == NameAsc) },
                new SelectListItem { Text = "Name: Z - A", Value = NameDesc, Selected = (selected == NameDesc) },
                new SelectListItem { Text = "ID: 0 - 9", Value = IdAsc, Selected = (selected == IdAsc) },
                new SelectListItem { Text = "ID: 9 - 0", Value = IdDesc, Selected = (selected == IdDesc) },
                new SelectListItem { Text = "Price: 0 - 9", Value = PriceAsc, Selected = (selected == PriceAsc) },
                new SelectListItem { Text = "Price: 9 - 0", Value = PriceDesc, Selected = (selected == PriceDesc) },
                new SelectListItem { Text = "Date added: Old - New", Value = DateAddedAsc, Selected = (selected == DateAddedAsc) },
                new SelectListItem { Text = "Date added: New - Old", Value = DateAddedDesc, Selected = (selected == DateAddedDesc) }
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
        public static IEnumerable<Product> ProductsByOrder(IProductRepository productRepo, string order, TreeNode<Category> categoryTree = null)
        {
            IEnumerable<Product> products;
            switch (order)
            {
                case NameAsc:
                    products = productRepo.GetProducts(p => p.Name, categoryFilter: categoryTree);
                    break;
                case NameDesc:
                    products = productRepo.GetProducts(p => p.Name, true, categoryTree);
                    break;
                case IdAsc:
                    products = productRepo.GetProducts(p => p.ProductID, categoryFilter: categoryTree);
                    break;
                case IdDesc:
                    products = productRepo.GetProducts(p => p.ProductID, true, categoryTree);
                    break;
                case PriceAsc:
                    products = productRepo.GetProducts(p => p.Price, categoryFilter: categoryTree);
                    break;
                case PriceDesc:
                    products = productRepo.GetProducts(p => p.Price, true, categoryTree);
                    break;
                case DateAddedAsc:
                    products = productRepo.GetProducts(p => p.DateAdded, categoryFilter: categoryTree);
                    break;
                case DateAddedDesc:
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
        public static Product SetProductImage(Product product, HttpPostedFileBase productImage)
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
    }
}