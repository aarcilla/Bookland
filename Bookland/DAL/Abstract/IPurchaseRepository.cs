using Bookland.Models;
using System;
using System.Collections.Generic;

namespace Bookland.DAL.Abstract
{
    public interface IPurchaseRepository : IDisposable
    {
        /// <summary>
        /// Retrieve a Purchase, based on the Purchase's ID.
        /// </summary>
        /// <param name="purchaseID">The ID of the requested Purchase.</param>
        /// <returns>The requested Purchase object.</returns>
        Purchase GetPurchase(int purchaseID);

        /// <summary>
        /// Retrieve all Purchases of a specified user.
        /// </summary>
        /// <param name="userName">The user name of the user whose purchases are requested.</param>
        /// <returns>An enumeration of all Purchases for the specified user.</returns>
        IEnumerable<Purchase> GetPurchases(string userName);

        /// <summary>
        /// Retrieve all Purchases for a particular transaction/invoice.
        /// </summary>
        /// <param name="transactionID">The ID of the requested transaction.</param>
        /// <returns>An enumeration of all Purchases for the specified transaction.</returns>
        IEnumerable<Purchase> GetPurchasesByTransaction(Guid transactionID);

        /// <summary>
        /// Add a Purchase to the DB.
        /// </summary>
        /// <param name="purchase">The Purchase to add.</param>
        void CreatePurchase(Purchase purchase);

        /// <summary>
        /// Update a specific Purchase's values to the DB.
        /// </summary>
        /// <param name="purchase">The Purchase to update.</param>
        void UpdatePurchase(Purchase purchase);

        /// <summary>
        /// Delete Purchases under the same transaction/invoice.
        /// </summary>
        /// <param name="transactionID">The ID of the requested transaction.</param>
        void DeletePurchasesByTransaction(Guid transactionID);

        /// <summary>
        /// Commit any changes to the DB.
        /// </summary>
        void Commit();
    }
}
