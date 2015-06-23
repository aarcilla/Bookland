using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookland.DAL.Concrete
{
    public class EfProductStatusRepository : Abstract.IProductStatusRepository, IDisposable
    {
        private BookshopContext context;

        public EfProductStatusRepository(BookshopContext context) 
        {
            this.context = context;
        }

        public ProductStatus GetProductStatus(int statusID)
        {
            return context.ProductStatuses.FirstOrDefault(s => s.ProductStatusID == statusID);
        }

        public ProductStatus GetProductStatus(string statusName)
        {
            return context.ProductStatuses.FirstOrDefault(s => s.ProductStatusName.Equals(statusName));
        }

        public IEnumerable<ProductStatus> GetProductStatuses()
        {
            return context.ProductStatuses.AsEnumerable();
        }

        public void CreateProductStatus(ProductStatus productStatus)
        {
            context.ProductStatuses.Add(productStatus);
        }

        public void UpdateProductStatus(ProductStatus productStatus)
        {
            ProductStatus dbProductStatus = GetProductStatus(productStatus.ProductStatusName);

            if (dbProductStatus != null)
            {
                dbProductStatus.ProductStatusName = productStatus.ProductStatusName;
            }
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