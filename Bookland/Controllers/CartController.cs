using Bookland.DAL.Abstract;
using Bookland.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository productRepo;
        private ICartRepository cartRepo;

        public CartController(IProductRepository productRepo, ICartRepository cartRepo)
        {
            this.productRepo = productRepo;
            this.cartRepo = cartRepo;
        }

        public ViewResult Index(Cart cart)
        {
            IEnumerable<CartItem> cartItems = User.Identity.IsAuthenticated ? cartRepo.GetCartItems(User.Identity.Name) : cart.CartItems;

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectResult CartItemAdd(Cart cart, int productID, int quantity = 1, string returnUrl = "/")
        {
            Product product = productRepo.GetProduct(productID);

            // If product couldn't be retrieved, prepare notification message and immediately redirect
            if (product == null)
            {
                TempData["message"] = "Product cannot be found. Please contact us for help and/or further details.";
                return Redirect(returnUrl);
            }

            CartItem cartItem = new CartItem
            {
                Product = product,
                Quantity = quantity
            };


            if (User.Identity.IsAuthenticated)
            {
                cartRepo.AddItemToCart(User.Identity.Name, cartItem);
                cartRepo.Commit();
            }
            else
            {
                CartItem itemExists = cart.CartItems.FirstOrDefault(i => i.Product.ProductID == productID);

                if (itemExists == null)
                {
                    cart.CartItems.Add(cartItem);
                }
                else
                {
                    itemExists.Quantity += cartItem.Quantity;
                }
            }

            TempData["message"] = string.Format("'{0}' added to your cart.", product.Name);

            return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult CartItemRemove(Cart cart, int productID)
        {
            if (User.Identity.IsAuthenticated)
            {
                cartRepo.RemoveItemFromCart(User.Identity.Name, productID);
                cartRepo.Commit();
            }
            else
            {
                CartItem itemExists = cart.CartItems.FirstOrDefault(i => i.Product.ProductID == productID);

                if (itemExists != null)
                {
                    cart.CartItems.Remove(itemExists);
                }
            }

            Product removedProduct = productRepo.GetProduct(productID);
            TempData["message"] = string.Format("'{0}' removed from your cart.", removedProduct.Name);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult ClearCart(Cart cart)
        {
            if (User.Identity.IsAuthenticated)
            {
                cartRepo.ClearCart(User.Identity.Name);
                cartRepo.Commit();
            }
            else
            {
                cart.CartItems.Clear();
            }

            TempData["message"] = "All items have been removed from your cart.";

            return RedirectToAction("Index", "Cart");
        }

        /// <summary>
        /// Action method enabling the user to set the quantity of a cart item in their cart.
        /// </summary>
        /// <param name="cart">Session cart (for guest user), retrieved from model binding.</param>
        /// <param name="productID">The ID of the product within the cart.</param>
        /// <param name="quantity">The desired quantity for the cart item.</param>
        /// <param name="returnUrl">The URL path to redirect to after carrying out the intended action, which is typically the user's original location.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectResult CartItemQuantity(Cart cart, int productID, int quantity, string returnUrl = "/Cart")
        {
            if (User.Identity.IsAuthenticated)
            {
                cartRepo.UpdateItemQuantity(User.Identity.Name, productID, quantity);
                cartRepo.Commit();
            }
            else
            {
                CartItem item = cart.CartItems.FirstOrDefault(i => i.Product.ProductID == productID);

                if (item != null)
                {
                    item.Quantity = quantity;
                }
            }

            return Redirect(returnUrl);
        }

        /// <summary>
        /// Retrieve total price and item count of the user's cart, for display on the main layout view's cart anchor.
        /// </summary>
        /// <param name="cart">Session cart (for guest user), retrieved from model binding.</param>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult CartLink(Cart cart)
        {
            IEnumerable<CartItem> cartItems = User.Identity.IsAuthenticated ? cartRepo.GetCartItems(User.Identity.Name) : cart.CartItems;

            int cartItemCount = 0;
            decimal cartTotalCost = 0.0M;
            if (cartItems != null)
            {
                foreach (CartItem item in cartItems)
                {
                    cartItemCount += item.Quantity;
                    cartTotalCost += (item.Product.Price * item.Quantity);
                }
            }

            return PartialView(new CartLinkViewModel
            {
                ItemCount = cartItemCount,
                TotalCost = cartTotalCost
            });
        }
    }
}
