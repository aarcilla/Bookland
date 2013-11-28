using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Retrieve the Cart object from the current session
            Cart cart = (Cart)controllerContext.HttpContext.Session[sessionKey];

            // Create a new Cart if there wasn't one in the session data
            if (cart == null)
            {
                cart = new Cart { CartID = 0, CartItems = new List<CartItem>() };
                controllerContext.HttpContext.Session[sessionKey] = cart;
            }

            return cart;
        }
    }
}