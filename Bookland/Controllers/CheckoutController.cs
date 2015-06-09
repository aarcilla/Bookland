using Bookland.DAL.Abstract;
using Bookland.Helpers.Abstract;
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
        private IMailHelpers mailHelpers;
        private IMvcHelpers mvcHelpers;

        private int CheckoutExpiryMinutes = 30;

        public CheckoutController(ICartRepository cartRepo, IPurchaseRepository purchaseRepo,
            IUserProfileRepository userProfileRepo, IMailHelpers mailHelpers, IMvcHelpers mvcHelpers)
        {
            this.cartRepo = cartRepo;
            this.purchaseRepo = purchaseRepo;
            this.userProfileRepo = userProfileRepo;
            this.mailHelpers = mailHelpers;
            this.mvcHelpers = mvcHelpers;
        }

        public ActionResult Index()
        {
            // If delivery and payment details have already been entered (for current cart), skip to Confirm page
            if (HttpRuntime.Cache["DeliveryAddress"] != null && HttpRuntime.Cache["Payment"] != null
                && HttpRuntime.Cache["CheckoutCart"] != null)
                return RedirectToAction("Confirm");

            Address userAddress = userProfileRepo.GetAddress(User.Identity.Name);
            Cart userCart = cartRepo.GetCart(User.Identity.Name);

            // Add user's cart to cache to reduce future (more expensive) calls to DB, 
            // with 30 minute expiration (also for checkout process expiration)
            HttpRuntime.Cache.Insert("CheckoutCart", userCart, null, DateTime.Now.AddMinutes(CheckoutExpiryMinutes), Cache.NoSlidingExpiration);

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
            Cart userCart = (Cart)HttpRuntime.Cache["CheckoutCart"];
            if (userCart == null)
                return CheckoutExpiredRedirect();

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
                // Add delivery address and payment details to cache to prepare for final checkout processing
                HttpRuntime.Cache.Insert("DeliveryAddress", models.DeliveryAddress, null, 
                    DateTime.Now.AddMinutes(CheckoutExpiryMinutes), Cache.NoSlidingExpiration);
                HttpRuntime.Cache.Insert("Payment", models.Payment, null, 
                    DateTime.Now.AddMinutes(CheckoutExpiryMinutes), Cache.NoSlidingExpiration);

                return RedirectToAction("Confirm");
            }
                        
            models.UserCart = userCart;

            return View(models);
        }

        public ActionResult Confirm()
        {
            PaymentModel payment = (PaymentModel)HttpRuntime.Cache["Payment"];
            Address deliveryAddress = (Address)HttpRuntime.Cache["DeliveryAddress"];
            Cart userCart = (Cart)HttpRuntime.Cache["CheckoutCart"];

            if (payment != null && deliveryAddress != null && userCart != null)
            {
                return View(new CheckoutViewModel
                {
                    Payment = payment,
                    DeliveryAddress = deliveryAddress,
                    UserCart = userCart
                });
            }

            return CheckoutExpiredRedirect();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCheckout()
        {
            // Perform payment processing

            // Add to purchase history
            Address deliveryAddress = (Address)HttpRuntime.Cache["DeliveryAddress"];
            Cart userCart = (Cart)HttpRuntime.Cache["CheckoutCart"];
            if (deliveryAddress == null || userCart == null)
                return CheckoutExpiredRedirect();

            UserProfile userProfile = userProfileRepo.GetUserProfile(User.Identity.Name);
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
            string purchaseEmailString = mvcHelpers.RenderViewToString(ControllerContext, @"~/Views/Shared/EmailTemplates/_PurchaseTemplate.cshtml", new PurchaseTemplateViewModel
            {
                DeliveryAddress = deliveryAddress,
                UserCart = userCart,
                TransactionID = transactionID
            });
            
            bool emailSuccess = mailHelpers.SendAdminEmail(userProfile.Email, "Bookland: Order " + transactionID.ToString(), purchaseEmailString);
            if (!emailSuccess)
            {
                // Revert changes made in DB's Purchase table
                purchaseRepo.DeletePurchasesByTransaction(transactionID);
                purchaseRepo.Commit();

                TempData["message"] = "Checkout process failed: order email failed to send.";
                return RedirectToAction("Index", "Cart");
            }

            CheckoutCleanup();

            // Clear cart
            cartRepo.ClearCart(User.Identity.Name);
            cartRepo.Commit();

            TempData["message"] = "Checkout successful. Check your email inbox for an invoice.";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CancelCheckout()
        {
            CheckoutCleanup();

            return RedirectToAction("Index", "Home");
        }

        #region Helper methods

        private ActionResult CheckoutExpiredRedirect()
        {
            TempData["message"] = "Checkout process incomplete or expired. Please try again.";
            return RedirectToAction("Index", "Cart");
        }

        private void CheckoutCleanup()
        {
            // Clear checkout cache data
            HttpRuntime.Cache.Remove("Payment");
            HttpRuntime.Cache.Remove("DeliveryAddress");
            HttpRuntime.Cache.Remove("CheckoutCart");
        }

        #endregion
    }
}
