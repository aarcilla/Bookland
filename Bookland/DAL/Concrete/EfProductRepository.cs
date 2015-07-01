using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bookland.DAL.Concrete
{
    public class EfProductRepository : Abstract.IProductRepository, IDisposable
    {
        private BookshopContext context;

        public EfProductRepository(BookshopContext context) 
        {
            this.context = context;
        }

        private List<Product> GetProductsByCategory(TreeNode<Category> categoryFilter)
        {
            List<Product> products = new List<Product>();

            List<Category> categoryAndDescendants = categoryFilter.ToList();

            foreach (Category cat in categoryAndDescendants)
            {
                var productsByCategory = context.Products.Where(p => p.Category.CategoryID == cat.CategoryID);

                foreach (Product prod in productsByCategory)
                {
                    products.Add(prod);
                }
            }

            return products;
        }

        public IEnumerable<Product> GetProducts(Expression<Func<Product, string>> order, bool descending = false, TreeNode<Category> categoryFilter = null)
        {
            IEnumerable<Product> products = categoryFilter != null ? GetProductsByCategory(categoryFilter) : context.Products.AsEnumerable();

            return !descending ? products.OrderBy(order.Compile()) : products.OrderByDescending(order.Compile());
        }

        public IEnumerable<Product> GetProducts(Expression<Func<Product, int>> order, bool descending = false, TreeNode<Category> categoryFilter = null)
        {
            IEnumerable<Product> products = categoryFilter != null ? GetProductsByCategory(categoryFilter) : context.Products.AsEnumerable();

            return !descending ? products.OrderBy(order.Compile()) : products.OrderByDescending(order.Compile());
        }
        public IEnumerable<Product> GetProducts(Expression<Func<Product, decimal>> order, bool descending = false, TreeNode<Category> categoryFilter = null)
        {
            IEnumerable<Product> products = categoryFilter != null ? GetProductsByCategory(categoryFilter) : context.Products.AsEnumerable();

            return !descending ? products.OrderBy(order.Compile()) : products.OrderByDescending(order.Compile());
        }
        public IEnumerable<Product> GetProducts(Expression<Func<Product, DateTime>> order, bool descending = false, TreeNode<Category> categoryFilter = null)
        {
            IEnumerable<Product> products = categoryFilter != null ? GetProductsByCategory(categoryFilter) : context.Products.AsEnumerable();

            return !descending ? products.OrderBy(order.Compile()) : products.OrderByDescending(order.Compile());
        }

        public Product GetProduct(int productID)
        {
            return context.Products.FirstOrDefault(p => p.ProductID == productID);
        }

        public Product GetProduct(string productName)
        {
            return context.Products.FirstOrDefault(p => productName.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
        }

        public void CreateProduct(Product product)
        {
            context.Products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            Product dbProduct = GetProduct(product.ProductID);

            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Year = product.Year;
            dbProduct.ReleaseDate = product.ReleaseDate;
            dbProduct.Price = product.Price;
            dbProduct.Category = product.Category;
            dbProduct.ProductStatus = product.ProductStatus;

            if (!product.IsImageInformationNullOrEmpty)
            {
                dbProduct.ImageData = product.ImageData;
                dbProduct.ImageMimeType = product.ImageMimeType;
            }
        }

        public void DeleteProduct(int productID)
        {
            Product product = GetProduct(productID);

            context.Products.Remove(product);
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        /// <summary>
        /// When called, if the DB context hasn't been already disposed of yet, dispose it now.
        /// </summary>
        /// <param name="disposing">A Boolean declaring whether to dispose of the context or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Dispose of the database context to ensure DB connection is properly closed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}