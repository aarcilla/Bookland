using Bookland.Models;
using System;
using System.Collections.Generic;

namespace Bookland.DAL.Abstract
{
    public interface IProductStatusRepository : IDisposable
    {
        /// <summary>
        /// Retrieve a specific product status based on its ID.
        /// </summary>
        /// <param name="statusID">The unique ID of the desired product status.</param>
        /// <returns>The desired product status.</returns>
        ProductStatus GetProductStatus(int statusID);

        /// <summary>
        /// Retrieve a specific product based on its name.
        /// </summary>
        /// <param name="statusName">The name of the desired product status.</param>
        /// <returns>The desired product status.</returns>
        ProductStatus GetProductStatus(string statusName);

        /// <summary>
        /// Retrieve all stored product statuses.
        /// </summary>
        /// <returns>An enumeration of stored product statuses.</returns>
        IEnumerable<ProductStatus> GetProductStatuses();

        /// <summary>
        /// Add a new product status to the DB.
        /// </summary>
        /// <param name="productStatus"></param>
        void CreateProductStatus(ProductStatus productStatus);

        /// <summary>
        /// Update a specific product status's values to the DB.
        /// </summary>
        /// <param name="productStatus"></param>
        void UpdateProductStatus(ProductStatus productStatus);

        /// <summary>
        /// Commit any changes to the DB.
        /// </summary>
        void Commit();
    }
}
