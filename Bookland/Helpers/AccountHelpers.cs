using Bookland.DAL.Abstract;
using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace Bookland.Helpers
{
    public static class AccountHelpers
    {
        public const string IdAsc = "id_asc";
        public const string IdDesc = "id_desc";
        public const string UserNameAsc = "userName_asc";
        public const string UserNameDesc = "userName_desc";
        public const string LastNameAsc = "lastName_asc";
        public const string LastNameDesc = "lastName_desc";

        /// <summary>
        /// Create an enumeration of items for drop-down list of user profile ordering.
        /// </summary>
        /// <param name="selected">The pre-selected/displayed item for the drop-down list.</param>
        /// <returns>An enumeration of drop-down list order options.</returns>
        public static IEnumerable<SelectListItem> UserProfileOrderOptions(string selected)
        {
            if (selected == null)
            {
                throw new System.ArgumentNullException("selected", "selected cannot be null.");
            }

            var list = new List<SelectListItem>() {
                new SelectListItem { Text = "ID: 0 - 9", Value = IdAsc, Selected = (selected == IdAsc) },
                new SelectListItem { Text = "ID: 9 - 0", Value = IdDesc, Selected = (selected == IdDesc) },
                new SelectListItem { Text = "User name: A - Z", Value = UserNameAsc, Selected = (selected == UserNameAsc) },
                new SelectListItem { Text = "User name: Z - A", Value = UserNameDesc, Selected = (selected == UserNameDesc) },
                new SelectListItem { Text = "Last name: A - Z", Value = LastNameAsc, Selected = (selected == LastNameAsc) },
                new SelectListItem { Text = "Last name: Z - A", Value = LastNameDesc, Selected = (selected == LastNameDesc) }
            };

            return list;
        }

        /// <summary>
        /// Retrieve user profiles, in the order specified.
        /// </summary>
        /// <param name="userProfileRepo">An existing instance of the User Profile repository.</param>
        /// <param name="order">The order identifier (e.g. 'id_desc' order by user ID descending).</param>
        /// <returns>An enumeration of user profiles in the desired order.</returns>
        public static IEnumerable<UserProfile> UserProfilesByOrder(IUserProfileRepository userProfileRepo, string order)
        {
            IEnumerable<UserProfile> userProfiles;
            switch (order)
            {
                case IdAsc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserID);
                    break;
                case IdDesc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserID, true);
                    break;
                case UserNameAsc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserName);
                    break;
                case UserNameDesc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.UserName, true);
                    break;
                case LastNameAsc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.LastName);
                    break;
                case LastNameDesc:
                    userProfiles = userProfileRepo.GetUserProfiles(u => u.LastName, true);
                    break;
                case null:
                    throw new System.ArgumentNullException("order", "order cannot be null.");
                default:
                    throw new System.ArgumentException("order value is invalid.", "order");
            }

            return userProfiles;
        }


        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
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