using Bookland.DAL.Abstract;
using Bookland.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        private IPurchaseRepository purchaseRepo;

        public PurchasesController(IPurchaseRepository purchaseRepo)
        {
            this.purchaseRepo = purchaseRepo;
        }

        public ActionResult Index()
        {
            IEnumerable<Purchase> userPurchases = purchaseRepo.GetPurchases(User.Identity.Name);

            if (userPurchases != null)
                userPurchases = userPurchases.OrderByDescending(p => p.PurchaseDate);

            return View(userPurchases);
        }
    }
}
