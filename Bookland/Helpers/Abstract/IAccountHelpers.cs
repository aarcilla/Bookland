using Bookland.DAL.Abstract;
using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace Bookland.Helpers.Abstract
{
    public interface IAccountHelpers
    {
        IEnumerable<SelectListItem> UserProfileOrderOptionsSelectList(string selected);

        IEnumerable<UserProfile> UserProfilesByOrder(IUserProfileRepository userProfileRepo, string order);

        string ErrorCodeToString(MembershipCreateStatus createStatus);
    }
}
