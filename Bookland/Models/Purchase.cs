using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookland.Models
{
    public enum PurchaseStatus
    {
        NotPaid, Pending, Paid, Returned, Cancelled
    }

    public class Purchase
    {
        [Display(Name = "Purchase ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseID { get; set; }

        [Required]
        [Display(Name = "Transaction ID")]
        public Guid TransactionID { get; set; }

        [Required]
        [Display(Name = "Purchase date")]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Display(Name = "Price paid")]
        [DataType(DataType.Currency)]
        public int PurchasePrice { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int PurchaseQuantity { get; set; }

        [Required]
        [Display(Name = "Status")]
        public PurchaseStatus PurchaseStatus { get; set; }

        public virtual Product Product { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}