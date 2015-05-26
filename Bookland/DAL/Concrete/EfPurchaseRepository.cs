using Bookland.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bookland.Models;

namespace Bookland.DAL.Concrete
{
    public class EfPurchaseRepository : IPurchaseRepository
    {
        private BookshopContext context;

        public EfPurchaseRepository(BookshopContext context)
        {
            this.context = context;
        }

        public Purchase GetPurchase(int purchaseID)
        {
            return context.Purchases.FirstOrDefault(p => p.PurchaseID == purchaseID);
        }

        public IEnumerable<Purchase> GetPurchases(string userName)
        {
            return context.Purchases.Where(p => p.UserProfile.UserName == userName);
        }

        public IEnumerable<Purchase> GetPurchasesByTransaction(Guid transactionID)
        {
            return context.Purchases.Where(p => p.TransactionID == transactionID);
        }

        public void CreatePurchase(Purchase purchase)
        {
            context.Purchases.Add(purchase);
        }

        public void UpdatePurchase(Purchase purchase)
        {
            Purchase dbPurchase = GetPurchase(purchase.PurchaseID);

            dbPurchase.PurchasePrice = purchase.PurchasePrice;
            dbPurchase.PurchaseQuantity = purchase.PurchaseQuantity;
            dbPurchase.PurchaseStatus = purchase.PurchaseStatus;
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