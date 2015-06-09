using Bookland.Constants;
using Bookland.DAL.Abstract;
using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace Bookland.Helpers
{
    public class AccountHelpers : Abstract.IAccountHelpers
    {
        /// <summary>
        /// Create an enumeration of items for drop-down list of user profile ordering.
        /// </summary>
        /// <param name="selected">The pre-selected/displayed item for the drop-down list.</param>
        /// <returns>An enumeration of drop-down list order options.</returns>
        public IEnumerable<SelectListItem> UserProfileOrderOptionsSelectList(string selected)
        {
            if (selected == null)
            {
                throw new System.ArgumentNullException("selected", "selected cannot be null.");
            }

            var list = new List<SelectListItem>() {
                new SelectListItem { Text = "ID: 0 - 9", Value = UserProfileOrderOptions.IdAsc, Selected = (selected == UserProfileOrderOptions.IdAsc) },
                new SelectListItem { Text = "ID: 9 - 0", Value = UserProfileOrderOptions.IdDesc, Selected = (selected == UserProfileOrderOptions.IdDesc) },
                new SelectListItem { Text = "User name: A - Z", Value = UserProfileOrderOptions.UserNameAsc, Selected = (selected == UserProfileOrderOptions.UserNameAsc) },
                new SelectListItem { Text = "User name: Z - A", Value = UserProfileOrderOptions.UserNameDesc, Selected = (selected == UserProfileOrderOptions.UserNameDesc) },
                new SelectListItem { Text = "Last name: A - Z", Value = UserProfileOrderOptions.LastNameAsc, Selected = (selected == UserProfileOrderOptions.LastNameAsc) },
                new SelectListItem { Text = "Last name: Z - A", Value = UserProfileOrderOptions.LastNameDesc, Selected = (selected == UserProfileOrderOptions.LastNameDesc) }
            };

            return list;
        }

        /// <summary>
        /// Retrieve user profiles, in the order specified.
        /// </summary>
        /// <param name="userProfileRepo">An existing instance of the User Profile repository.</param>
        /// <param name="order">The order identifier (e.g. 'id_desc' order by user ID descending).</param>
        /// <returns>An enumeration of user profiles in the desired order.</returns>
        public IEnumerable<UserProfile> UserProfilesByOrder(IUserProfileRepository userProfileRepo, string order)
        {
            IEnumerable<UserProfile> userProfiles;
            switch (order)
            {
                case UserProfileOrderOptions.IdAsc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserID);
                    break;
                case UserProfileOrderOptions.IdDesc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserID, true);
                    break;
                case UserProfileOrderOptions.UserNameAsc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserName);
                    break;
                case UserProfileOrderOptions.UserNameDesc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserName, true);
                    break;
                case UserProfileOrderOptions.LastNameAsc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.LastName);
                    break;
                case UserProfileOrderOptions.LastNameDesc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.LastName, true);
                    break;
                case null:
                    throw new System.ArgumentNullException("order", "order cannot be null.");
                default:
                    throw new System.ArgumentException("order value is invalid.", "order");
            }

            return userProfiles;
        }


        public string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}