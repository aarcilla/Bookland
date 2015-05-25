using Bookland.DAL;
using Bookland.DAL.Abstract;
using Bookland.DAL.Concrete;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Infrastructure
{
    /// <summary>
    /// If a user logs in and their cart while as a guest (i.e. before logging in) contains items, add those items to the user's DB-stored cart.
    /// This is called whenever the main layout view is loaded.
    /// </summary>
    public class MergeSessionAndDbCartsAttribute : ActionFilterAttribute, IActionFilter
    {
        ICartRepository cartRepo;

        public MergeSessionAndDbCartsAttribute(ICartRepository cartRepo)
        {
            this.cartRepo = cartRepo;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.RequestContext.HttpContext.Session;
            var user = filterContext.RequestContext.HttpContext.User.Identity;

            if (user.IsAuthenticated && session["Cart"] != null)
            {
                Cart sessionCart = (Cart)session["Cart"];

                if (cartRepo.GetCart(user.Name) == null)
                {
                    cartRepo.GetCart(user.Name);
                    cartRepo.Commit();
                }

                foreach (CartItem item in sessionCart.CartItems)
                {
                    cartRepo.AddItemToCart(user.Name, item);
                }

                cartRepo.Commit();

                filterContext.HttpContext.Session.Clear();

                base.OnActionExecuting(filterContext);
            }
        }
    }
}