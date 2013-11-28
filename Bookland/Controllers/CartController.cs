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
            CartItem cartItem = new CartItem
            {
                Product = productRepo.GetProduct(productID),
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

            return RedirectToAction("Index", "Cart");
        }

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

        [ChildActionOnly]
        public PartialViewResult MergeSessionAndDbCartsWhenAuthenticated()
        {
            var session = HttpContext.Session;
            if (User.Identity.IsAuthenticated && session["Cart"] != null)
            {
                Cart sessionCart = (Cart)HttpContext.Session["Cart"];

                if (cartRepo.GetCart(User.Identity.Name) == null)
                {
                    cartRepo.CreateCart(User.Identity.Name);
                    cartRepo.Commit();
                }

                foreach (CartItem item in sessionCart.CartItems)
                {
                    cartRepo.AddItemToCart(User.Identity.Name, item);
                }

                cartRepo.Commit();

                HttpContext.Session.Clear();
            }

            return PartialView();
        }
    }
}
