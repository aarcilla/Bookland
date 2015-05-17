using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookland.Models
{
    public abstract class AbstractUserProfile
    {
        public abstract int UserID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name must be {0} characters or less.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Email must be {0} characters or less.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "First name must be {0} characters or less.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Last name must be {0} characters or less.")]
        public string LastName { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile : AbstractUserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int UserID { get; set; }

        public virtual Address Address { get; set; }
        public virtual Cart Cart { get; set; }
    }

    /// <summary>
    /// A concrete user profile model that also encapsulates the user's role. Used for user account viewing purposes
    /// (i.e. Admin/Account/Index). Not to be used in Entity Framework Code First table generation (unlike sibling class 'UserProfile').
    /// </summary>
    public class UserProfileWithRole : AbstractUserProfile
    {
        public override int UserID { get; set; }

        public string Role { get; set; }
    }

    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressID { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Street line 1 must be {0} characters or less.")]
        public string StreetLine1 { get; set; }

        [StringLength(250, ErrorMessage = "Street line 2 must be {0} characters or less.")]
        public string StreetLine2 { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "City must be {0} characters or less.")]
        public string City { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "State must be {0} characters or less.")]
        public string State { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Country must be {0} characters or less.")]
        public string Country { get; set; }

        public int Postcode { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        [RegularExpression(@"^[\w]+$", ErrorMessage = "Your user name contains invalid characters.")]
        [StringLength(50, ErrorMessage = "Name must be {0} characters or less.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^[\w]+([\.][\w]+)*@[\w]+(\.[\w]+)+$", ErrorMessage = "Not a valid email address.")]
        [StringLength(250, ErrorMessage = "Email must be {0} characters or less.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(100, ErrorMessage = "First name must be {0} characters or less.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(100, ErrorMessage = "Last name must be {0} characters or less.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Street line 1")]
        [StringLength(250, ErrorMessage = "Street line 1 must be {0} characters or less.")]
        public string StreetLine1 { get; set; }

        [Display(Name = "Street line 2")]
        [StringLength(250, ErrorMessage = "Street line 2 must be {0} characters or less.")]
        public string StreetLine2 { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "City must be {0} characters or less.")]
        public string City { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "State must be {0} characters or less.")]
        public string State { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Country must be {0} characters or less.")]
        public string Country { get; set; }

        public int Postcode { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserID { get; set; }
    }
}