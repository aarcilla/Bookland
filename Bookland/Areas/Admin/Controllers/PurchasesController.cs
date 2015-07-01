using Bookland.Areas.Admin.Models;
using Bookland.DAL.Abstract;
using Bookland.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator, Support, Staff")]
    public class PurchasesController : Controller
    {
        private IPurchaseRepository purchaseRepo;

        public PurchasesController(IPurchaseRepository purchaseRepo)
        {
            this.purchaseRepo = purchaseRepo;
        }

        public ActionResult Index(string userName)
        {
            IEnumerable<Purchase> userPurchases = purchaseRepo.GetPurchases(userName);

            if (userPurchases != null)
                userPurchases = userPurchases.OrderByDescending(p => p.PurchaseDate);

            return View(new PurchasesViewModel 
            {
                Purchases = userPurchases.ToList<Purchase>(),
                UserName = userName
            });
        }

    }
}
