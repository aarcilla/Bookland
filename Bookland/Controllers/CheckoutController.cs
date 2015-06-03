using Bookland.DAL.Abstract;
using Bookland.Helpers;
using Bookland.Models;
using System;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private ICartRepository cartRepo;
        private IPurchaseRepository purchaseRepo;
        private IUserProfileRepository userProfileRepo;

        public CheckoutController(ICartRepository cartRepo, IPurchaseRepository purchaseRepo,
            IUserProfileRepository userProfileRepo)
        {
            this.cartRepo = cartRepo;
            this.purchaseRepo = purchaseRepo;
            this.userProfileRepo = userProfileRepo;
        }

        public ActionResult Index()
        {
            Address userAddress = userProfileRepo.GetAddress(User.Identity.Name);
            Cart userCart = cartRepo.GetCart(User.Identity.Name);

            return View(new CheckoutViewModel 
            {
                DeliveryAddress = userAddress,
                UserCart = userCart
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckoutViewModel models)
        {
            int cardExpYear = models.Payment.CardExpiryYear;

            int cardExpMonth;
            bool isCardExpMonthValid = Int32.TryParse(models.Payment.CardExpiryMonth, out cardExpMonth);

            if (!isCardExpMonthValid || cardExpMonth < 1 || cardExpMonth > 12)
            {
                ModelState.AddModelError("", "Card expiry month is invalid");
            }
            else 
            {
                if (cardExpYear < DateTime.Now.Year || (cardExpYear == DateTime.Now.Year && cardExpMonth < DateTime.Now.Month))
                {
                    ModelState.AddModelError("", "Card is expired");
                }
            }

            if (ModelState.IsValid)
            {
                HttpRuntime.Cache.Insert("DeliveryAddress", models.DeliveryAddress, null, DateTime.Now.AddHours(3), Cache.NoSlidingExpiration);
                HttpRuntime.Cache.Insert("Payment", models.Payment, null, DateTime.Now.AddHours(3), Cache.NoSlidingExpiration);

                return RedirectToAction("Confirm");
            }

            models.UserCart = cartRepo.GetCart(User.Identity.Name);

            return View(models);
        }

        public ActionResult Confirm()
        {
            PaymentModel payment = (PaymentModel)HttpRuntime.Cache["Payment"];
            Address deliveryAddress = (Address)HttpRuntime.Cache["DeliveryAddress"];

            if (payment != null && deliveryAddress != null)
            {
                return View(new CheckoutViewModel
                {
                    Payment = payment,
                    DeliveryAddress = deliveryAddress,
                    UserCart = cartRepo.GetCart(User.Identity.Name)
                });
            }
            else
            {
                TempData["message"] = "Checkout process incomplete.";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCheckout()
        {
            // Perform payment processing

            // Add to purchase history
            Address deliveryAddress = (Address)HttpRuntime.Cache["DeliveryAddress"];
            if (deliveryAddress == null)
            {
                TempData["message"] = "Checkout process expired. Please try again.";
                return RedirectToAction("Index", "Cart");
            }

            UserProfile userProfile = userProfileRepo.GetUserProfile(User.Identity.Name);
            Cart userCart = cartRepo.GetCart(User.Identity.Name);
            Guid transactionID = Guid.NewGuid();

            try
            {
                foreach (CartItem cartItem in userCart.CartItems)
                {
                    purchaseRepo.CreatePurchase(new Purchase
                    {
                        TransactionID = transactionID,
                        PurchaseDate = DateTime.Now,
                        PurchasePrice = cartItem.Product.Price,
                        PurchaseQuantity = cartItem.Quantity,
                        PurchaseStatus = PurchaseStatus.Paid,
                        Product = cartItem.Product,
                        UserProfile = userProfile
                    });
                }

                purchaseRepo.Commit();
            }
            catch (System.Data.DataException)
            {
                TempData["message"] = "Checkout process failed: adding cart items to your purchase history unsuccessful.";
                return RedirectToAction("Index", "Cart");
            }

            // Send email invoice
            string purchaseEmailString = (new MvcHelpers()).RenderViewToString(ControllerContext, @"~/Views/Shared/EmailTemplates/_PurchaseTemplate.cshtml", new PurchaseTemplateViewModel
            {
                DeliveryAddress = deliveryAddress,
                UserCart = userCart,
                TransactionID = transactionID
            });
            
            bool emailSuccess = MailHelpers.SendAdminEmail(userProfile.Email, "Bookland: Order " + transactionID.ToString(), purchaseEmailString);
            if (!emailSuccess)
            {
                // Revert changes made in DB's Purchase table
                purchaseRepo.DeletePurchasesByTransaction(transactionID);
                purchaseRepo.Commit();

                TempData["message"] = "Checkout process failed: order email failed to send.";
                return RedirectToAction("Index", "Cart");
            }

            // Clear cache data
            HttpRuntime.Cache.Remove("Payment");
            HttpRuntime.Cache.Remove("DeliveryAddress");

            // Clear cart
            cartRepo.ClearCart(User.Identity.Name);
            cartRepo.Commit();

            TempData["message"] = "Checkout successful. Check your email inbox for an invoice.";
            return RedirectToAction("Index", "Home");
        }
    }
}
