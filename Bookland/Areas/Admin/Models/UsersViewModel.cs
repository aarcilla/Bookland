using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Models
{
    public class UsersViewModel
    {
        public IEnumerable<UserProfileWithRole> UserProfiles { get; set; }
        public SelectList RoleFilterOptions { get; set; }
        public IEnumerable<SelectListItem> OrderOptions { get; set; }
    }
}