using Bookland.Models;
using System;
using System.Collections.Generic;

namespace Bookland.DAL.Abstract
{
    public interface ICartRepository : IDisposable
    {
        /// <summary>
        /// Retrieve a cart, based on the cart's ID.
        /// </summary>
        /// <param name="cartID">The ID of the requested cart.</param>
        /// <returns>A Cart object of the requested cart.</returns>
        Cart GetCart(int cartID);

        /// <summary>
        /// Retrieve a cart, based on the user profile who is associated with the cart.
        /// </summary>
        /// <param name="userName">The user name of the cart's owner.</param>
        /// <returns>A Cart object of the requested cart.</returns>
        Cart GetCart(string userName);

        /// <summary>
        /// Add a cart to the DB, to be associated to a particular user.
        /// </summary>
        /// <param name="userName">The user name of the cart's intended owner.</param>
        void CreateCart(string userName);

        /// <summary>
        /// Retrieve all cart items for a specified cart.
        /// </summary>
        /// <param name="cartID">The ID of the cart that contains the requested cart items.</param>
        /// <returns>An enumeration of all cart items for the requested cart.</returns>
        IEnumerable<CartItem> GetCartItems(int cartID);

        /// <summary>
        /// Retrieve all cart items for a specified cart.
        /// </summary>
        /// <param name="userName">The user name of the user associated with the cart that contains the requested cart items.</param>
        /// <returns>An enumeration of all cart items for the requested cart.</returns>
        IEnumerable<CartItem> GetCartItems(string userName);

        /// <summary>
        /// Add a cart item to a user's cart.
        /// </summary>
        /// <param name="userName">The user name of the user associated with the cart.</param>
        /// <param name="cartItem">The cart item to be added to the user's cart.</param>
        void AddItemToCart(string userName, CartItem cartItem);

        /// <summary>
        /// Update a cart item's quantity amount.
        /// </summary>
        /// <param name="userName">The user name of the user associated with the cart.</param>
        /// <param name="productID">The ID of the product associated with the cart item.</param>
        /// <param name="quantity">The updated quantity amount.</param>
        void UpdateItemQuantity(string userName, int productID, int quantity);

        /// <summary>
        /// Remove a cart item from a user's cart.
        /// </summary>
        /// <param name="userName">The user name of the user associated with the cart.</param>
        /// <param name="productID">The ID of the product associated with the cart item.</param>
        void RemoveItemFromCart(string userName, int productID);

        /// <summary>
        /// Clear a cart's contents (i.e. all of its cart items).
        /// </summary>
        /// <param name="userName">The user name of the user associated with the cart.</param>
        void ClearCart(string userName);

        /// <summary>
        /// Commit any changes to the DB.
        /// </summary>
        void Commit();
    }
}