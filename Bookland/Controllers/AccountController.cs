using Bookland.DAL.Abstract;
using Bookland.Filters;
using Bookland.Helpers;
using Bookland.Models;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Bookland.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private IUserProfileRepository userProfileRepo;

        public AccountController(IUserProfileRepository userProfileRepo)
        {
            this.userProfileRepo = userProfileRepo;
        }

        //
        // GET: /Account/LogIn

        [AllowAnonymous]
        public ViewResult LogIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/LogIn

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        public ViewResult LogOut(string returnUrl)
        {
            return View((object)returnUrl);
        }

        //
        // POST: /Account/LogOut

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult LogOut()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ViewResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    });

                    // Assign the 'Customer' role for this user
                    if (!Roles.RoleExists("Customer")) { Roles.CreateRole("Customer"); }
                    Roles.AddUserToRole(model.UserName, "Customer");

                    // Add associated Address record to DB
                    userProfileRepo.CreateAddress(new Address
                    {
                        StreetLine1 = model.StreetLine1,
                        StreetLine2 = model.StreetLine2,
                        City = model.City,
                        State = model.State,
                        Country = model.Country,
                        Postcode = model.Postcode
                    }, model.UserName);
                    userProfileRepo.Commit();

                    WebSecurity.Login(model.UserName, model.Password);

                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", AccountHelpers.ErrorCodeToString(e.StatusCode));
                }
                catch (DataException)
                {
                    ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problem persists.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    TempData["message"] = String.Format("{0}'s password changed successfully.", User.Identity.Name);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            return View();
        }

        public ActionResult Update()
        {
            UserProfile userProfile = userProfileRepo.GetUserProfile(User.Identity.Name);

            if (userProfile != null)
            {
                return View(new RegisterModel 
                {
                    UserName = userProfile.UserName,
                    Email = userProfile.Email,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    StreetLine1 = userProfile.Address.StreetLine1,
                    StreetLine2 = userProfile.Address.StreetLine2,
                    City = userProfile.Address.City,
                    State = userProfile.Address.State,
                    Country = userProfile.Address.Country,
                    Postcode = userProfile.Address.Postcode,
                    Password = "dummy-value",
                    ConfirmPassword = "dummy-value"
                });
            }
            else
            {
                string message404 = String.Format("'{0}' does not exist or is invalid.", User.Identity.Name);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    userProfileRepo.UpdateUserProfile(new UserProfile 
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    });
                    
                    userProfileRepo.UpdateAddress(new Address
                    {
                        StreetLine1 = model.StreetLine1,
                        StreetLine2 = model.StreetLine2,
                        City = model.City,
                        State = model.State,
                        Country = model.Country,
                        Postcode = model.Postcode
                    }, model.UserName);

                    userProfileRepo.Commit();

                    TempData["message"] = "Your user details have been successfully updated.";

                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problems persist.");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RequestPasswordReset()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RequestPasswordReset(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return View();

            UserProfile userProfile = userProfileRepo.GetUserProfile(userName);

            if (userProfile != null)
            {
                string passwordResetToken = WebSecurity.GeneratePasswordResetToken(userProfile.UserName, 60);
                string passwordResetUrl = Url.Action("ResetPassword", "Account", new { resetToken = passwordResetToken }, Request.Url.Scheme);

                string resetEmailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Content\EmailTemplates\ResetPasswordTemplate.html";
                string resetEmailTemplate = System.IO.File.ReadAllText(resetEmailTemplatePath);
                string mailBody = string.Format(resetEmailTemplate, userName, passwordResetUrl);

                bool mailSuccess = MailHelpers.SendAdminEmail(userProfile.Email, "Bookland: Password Reset", mailBody);

                if (mailSuccess)
                    TempData["message"] = "Password reset email sent. Please check your inbox and reset your password within 1 hour.";
                else
                    TempData["message"] = "Password reset email unsuccessfully sent. Please try again later.";

                return RedirectToAction("Index", "Home");
            }

            TempData["message"] = string.Format("No user exists under the name '{0}'", userName);

            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string resetToken)
        {
            if (!string.IsNullOrWhiteSpace(resetToken))
                return View(new ResetPasswordModel { ResetToken = resetToken, OldPassword = "dummy-value" });
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool resetSuccess = WebSecurity.ResetPassword(model.ResetToken, model.NewPassword);

                if (resetSuccess)
                    TempData["message"] = "Password reset successful.";
                else
                    TempData["message"] = "Password reset unsuccessful. Make sure that you view the newest Bookland email or try resending the password reset request.";

                return RedirectToAction("LogIn");
            }

            return View(model);
        }


        [AllowAnonymous]
        [ChildActionOnly]
        public PartialViewResult UserLinks()
        {
            string userName = null;
            bool admin = false;
            if (User.Identity.IsAuthenticated)
            {
                userName = WebSecurity.CurrentUserName;

                string[] userRoles = Roles.GetRolesForUser(userName);
                admin = userRoles.Contains("Administrator") || userRoles.Contains("Support") || userRoles.Contains("Staff");
            }

            return PartialView(new UserLinksViewModel
            {
                UserName = userName,
                AdminAccess = admin
            });
        }

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}
