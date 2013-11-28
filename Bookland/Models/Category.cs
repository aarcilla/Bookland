using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookland.Models
{
    public class Category
    {
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Category name")]
        [StringLength(50, ErrorMessage = "Name must be {0} characters or less.")]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "Description must be {0} characters or less.")]
        [DataType(DataType.MultilineText)]
        public string CategoryDescription { get; set; }

        [Display(Name = "Level")]
        public int CategoryLevel { get; set; }

        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}