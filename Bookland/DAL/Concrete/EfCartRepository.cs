using Bookland.DAL.Abstract;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookland.DAL.Concrete
{
    public class EfCartRepository : ICartRepository, IDisposable
    {
        private BookshopContext context;

        public EfCartRepository(BookshopContext context)
        {
            this.context = context;
        }

        public Cart GetCart(int cartID)
        {
            return context.Carts.Include("CartItems").FirstOrDefault(c => c.CartID == cartID);
        }

        public Cart GetCart(string userName)
        {
            return context.Carts.Include("CartItems").FirstOrDefault(c => c.UserProfile.UserName == userName);
        }

        public void CreateCart(string userName)
        {
            UserProfile userProfile = context.UserProfiles.FirstOrDefault(u => u.UserName == userName);

            if (userProfile != null)
            {
                context.Carts.Add(new Cart
                {
                    UserProfile = userProfile
                });
            }
            else
            {
                throw new ArgumentException("userName is not valid.", "userName");
            }
        }

        public IEnumerable<CartItem> GetCartItems(int cartID)
        {
            return context.CartItems.Include("Product").Where(cI => cI.Cart.CartID == cartID);
        }

        public IEnumerable<CartItem> GetCartItems(string userName)
        {
            return context.CartItems.Include("Product").Where(cI => cI.Cart.UserProfile.UserName == userName);
        }

        public void AddItemToCart(string userName, CartItem cartItem)
        {
            Cart cart = GetCart(userName);
            if (cart != null)
            {
                CartItem itemExistsForUser = cart.CartItems.FirstOrDefault(cI => cI.Product.ProductID == cartItem.Product.ProductID);

                if (itemExistsForUser == null)
                {
                    context.CartItems.Add(cartItem);
                    cart.CartItems.Add(cartItem);
                }
                else
                {
                    // N.B.: Maximum quantity for a cart item is 10
                    if ((itemExistsForUser.Quantity + cartItem.Quantity) <= 10)
                    {
                        itemExistsForUser.Quantity += cartItem.Quantity;
                    }
                    else
                    {
                        itemExistsForUser.Quantity = 10;
                    }
                }
            }
            else
            {
                throw new ArgumentException("userName is not valid.", "userName");
            }
        }

        public void UpdateItemQuantity(string userName, int productID, int quantity)
        {
            Cart cart = GetCart(userName);
            CartItem dbCartItem = cart.CartItems.FirstOrDefault(cI => cI.Product.ProductID == productID);

            dbCartItem.Quantity = quantity;
        }

        public void RemoveItemFromCart(string userName, int productID)
        {
            Cart cart = GetCart(userName);
            CartItem itemToRemove = cart.CartItems.FirstOrDefault(cI => cI.Product.ProductID == productID);

            if (cart != null && itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);
                context.CartItems.Remove(itemToRemove);
            }
        }

        public void ClearCart(string userName)
        {
            Cart cart = GetCart(userName);

            cart.CartItems.Clear();
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

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