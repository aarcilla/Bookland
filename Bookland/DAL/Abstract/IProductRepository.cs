using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bookland.DAL.Abstract
{
    public interface IProductRepository : IDisposable
    {
        /// <summary>
        /// Retrieve all stored products, optionally based on specified order, category filter and/or specified criteria.
        /// </summary>
        /// <param name="order">An optional expression of a function specifying the property to order the products by.</param>
        /// <param name="descending">An optional Boolean specifying whether the order is descending (e.g. Z-A) or ascending (e.g. A-Z).</param>
        /// <param name="categoryFilter">Optionally filter products by category (with its ID).</param>
        /// <param name="where">An optional expression of a function specifying the Product-associated criteria that must be met.</param>
        /// <returns>An enumeration of stored products, in its desired order.</returns>
        IEnumerable<Product> GetProducts<T>(Expression<Func<Product, T>> order = null, bool descending = false,
                                                TreeNode<Category> categoryFilter = null, Expression<Func<Product, bool>> where = null);

        /// <summary>
        /// Retrieve a specific product, based on ID.
        /// </summary>
        /// <param name="productID">The ID of the requested product.</param>
        /// <returns>A Product object of the requested product.</returns>
        Product GetProduct(int productID);

        /// <summary>
        /// Retrieve a specific product, based on its name.
        /// </summary>
        /// <param name="productName">The name of the requested product.</param>
        /// <returns>A Product object of the requested category.</returns>
        Product GetProduct(string productName);

        /// <summary>
        /// Add a product to the DB.
        /// </summary>
        /// <param name="product">The product to add.</param>
        void CreateProduct(Product product);

        /// <summary>
        /// Update a specific product's values to the DB.
        /// </summary>
        /// <param name="product">The product to update.</param>
        void UpdateProduct(Product product);
        
        /// <summary>
        /// Delete the specified product from the DB.
        /// </summary>
        /// <param name="productID">The product to be deleted.</param>
        void DeleteProduct(int productID);

        /// <summary>
        /// Commit any changes to the DB.
        /// </summary>
        void Commit();
    }
}