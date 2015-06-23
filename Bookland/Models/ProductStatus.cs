using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookland.Models
{
    public class ProductStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductStatusID { get; set; }

        [Display(Name = "Status name")]
        [StringLength(30)]
        public string ProductStatusName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}