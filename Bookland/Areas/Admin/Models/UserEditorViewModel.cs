using Bookland.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Models
{
    public class UserEditorViewModel : RegisterModel
    {
        [Required]
        [Display(Name = "Account type")]
        public string Role { get; set; }

        public string Action { get; set; }
        public SelectList RoleOptions { get; set; }
    }
}