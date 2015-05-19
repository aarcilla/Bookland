using Bookland.DAL;
using Bookland.DAL.Abstract;
using Bookland.Filters;
using Bookland.Helpers;
using Bookland.Models;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
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
