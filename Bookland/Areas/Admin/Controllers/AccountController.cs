using Bookland.Areas.Admin.Models;
using Bookland.DAL.Abstract;
using Bookland.Helpers;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Bookland.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator, Support, Staff")]
    public class AccountController : Controller
    {
        private IUserProfileRepository userProfileRepo;

        public AccountController(IUserProfileRepository userProfileRepo)
        {
            this.userProfileRepo = userProfileRepo;
        }

        public ViewResult Index(string role = "All", string order = null)
        {
            // If no order is specified, attempt to retrieve cookie containing last-used order option
            HttpCookie userProfileOrder = Request.Cookies["UserProfileOrder"];
            if (userProfileOrder != null && order == null)
            {
                order = userProfileOrder.Value;
            }
            else if (order == null) // I.e. cookie is null, but no order is specified (e.g. first-ever access of 'Index')
            {
                order = AccountHelpers.IdAsc;   // ID ascending is assumed as default order
            }

            IEnumerable<UserProfile> userProfiles = AccountHelpers.UserProfilesByOrder(userProfileRepo, order);

            var userProfilesWithRoles = new List<UserProfileWithRole>();
            foreach (UserProfile userProfile in userProfiles)
            {
                string[] userRole = Roles.GetRolesForUser(userProfile.UserName);
                var userProfileWithRole = new UserProfileWithRole
                {
                    UserID = userProfile.UserID,
                    UserName = userProfile.UserName,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    Email = userProfile.Email,
                    Role = userRole.Length > 0 ? userRole[0] : ""
                };

                // Only Admin user accounts are being able to see or edit all types of user accounts
                if (!User.IsInRole("Administrator"))
                {
                    // Support accounts can only see staff and customer accounts
                    if (User.IsInRole("Support") && (userProfileWithRole.Role != "Administrator" && userProfileWithRole.Role != "Support"))
                    {
                        userProfilesWithRoles.Add(userProfileWithRole);
                    }
                    else
                    {
                        // Staff and other non-customer accounts can only see customer accounts
                        if (userProfileWithRole.Role != "Administrator" && userProfileWithRole.Role != "Support" && userProfileWithRole.Role != "Staff")
                        {
                            userProfilesWithRoles.Add(userProfileWithRole);
                        }
                    }
                }
                else
                {
                    userProfilesWithRoles.Add(userProfileWithRole);
                }
            }

            if (userProfileOrder == null)
            {
                userProfileOrder = new HttpCookie("UserProfileOrder");
                Response.Cookies.Add(userProfileOrder);
            }

            // Update cookie value and expiry if it has changed, or was just created
            if (userProfileOrder.Value != order)
            {
                Response.Cookies["UserProfileOrder"].Value = order;
                Response.Cookies["UserProfileOrder"].Expires = DateTime.Now.AddMonths(1);
            }

            return View(new UsersViewModel
            {
                UserProfiles = userProfilesWithRoles.Where(u => role == "All" || u.Role.ToUpper() == role.ToUpper()),
                RoleFilterOptions = new SelectList(new string[] { "All" }.Concat(RoleOptions()), role),
                OrderOptions = AccountHelpers.UserProfileOrderOptions(order)
            });
        }

        public ViewResult Create()
        {
            return View("Editor", new UserEditorViewModel
            {
                Action = "Create",
                RoleOptions = new SelectList(RoleOptions())
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    });

                    // Assign the specified role for this user
                    if (!Roles.RoleExists(model.Role)) { Roles.CreateRole(model.Role); }
                    Roles.AddUserToRole(model.UserName, model.Role);

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

                    TempData["message"] = String.Format("'{0}' user account successfully added to the database.", model.UserName);

                    return RedirectToAction("Index", "Account");
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

            model.RoleOptions = new SelectList(RoleOptions());
            return View("Editor", model);
        }

        public ActionResult Update(string userName)
        {
            UserProfile userProfile = userProfileRepo.GetUserProfile(userName);

            if (userProfile != null)
            {
                string[] userRole = Roles.GetRolesForUser(userName);

                return View("Editor", new UserEditorViewModel
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
                    Action = "Update",
                    RoleOptions = new SelectList(RoleOptions(), userRole.Length > 0 ? userRole[0] : null)
                });
            }
            else
            {
                string message404 = String.Format("'{0}' does not exist or is invalid.", userName);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UserEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!Roles.IsUserInRole(model.UserName, model.Role))
                    {
                        string[] oldRoles = Roles.GetRolesForUser(model.UserName);
                        if (oldRoles.Length > 0)
                        {
                            Roles.RemoveUserFromRoles(model.UserName, oldRoles);
                        }

                        if (!Roles.RoleExists(model.Role))
                        {
                            Roles.CreateRole(model.Role);
                        }
                        Roles.AddUserToRole(model.UserName, model.Role);
                    }

                    userProfileRepo.UpdateUserProfile(new UserProfile
                    {
                        UserName = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
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

                    TempData["message"] = String.Format("'{0}' user account successfully updated.", model.UserName);

                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problems persist.");
                }
            }

            string[] userRole = Roles.GetRolesForUser(model.UserName);
            model.RoleOptions = new SelectList(RoleOptions(), userRole.Length > 0 ? userRole[0] : null);

            return View("Editor", model);
        }

        [Authorize(Roles = "Administrator,Support")]
        public ActionResult ChangePassword(string userName)
        {
            UserProfile userProfile = userProfileRepo.GetUserProfile(userName);
            if (userProfile != null)
            {
                return View("ChangePassword", new LocalPasswordModel { UserName = userName });
            }
            else
            {
                string message404 = String.Format("'{0}' does not exist or is invalid.", userName);
                return HttpNotFound(message404);
            }
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
                    changePasswordSucceeded = WebSecurity.ChangePassword(model.UserName, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    TempData["message"] = String.Format("{0}'s password changed successfully.", model.UserName);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            return View();
        }

        /// <summary>
        /// Generate role options for drop-down lists to be used in user account editor view, based on the current user's role clearance (e.g. Staff can only set Customer users).
        /// </summary>
        /// <returns>Enumeration of role options, based on current user's role.</returns>
        private IEnumerable<string> RoleOptions()
        {
            return User.IsInRole("Administrator") ? Roles.GetAllRoles()
                : User.IsInRole("Support") ? new string[] { "Staff", "Customer" }
                : new string[] { "Customer" };
        }

    }
}
