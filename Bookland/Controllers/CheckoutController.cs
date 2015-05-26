using Bookland.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private ICartRepository cartRepo;
        private IPurchaseRepository purchaseRepo;

        public CheckoutController(ICartRepository cartRepo, IPurchaseRepository purchaseRepo)
        {
            this.cartRepo = cartRepo;
            this.purchaseRepo = purchaseRepo;
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
