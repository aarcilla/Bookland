using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookland.Models
{
    public class PaymentModel
    {
        [Required]
        [Display(Name = "Owner's name")]
        [StringLength(150)]
        public string CardOwnerName { get; set; }

        [Required]
        [Display(Name = "Card number")]
        [RegularExpression(@"^[\d]+$")]
        [StringLength(4, MinimumLength = 4)]
        public string CardNumber1 { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$")]
        [StringLength(4, MinimumLength = 4)]
        public string CardNumber2 { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$")]
        [StringLength(4, MinimumLength = 4)]
        public string CardNumber3 { get; set; }

        [Required]
        [RegularExpression(@"^[\d]+$")]
        [StringLength(4)]
        public string CardNumber4 { get; set; }

        [RegularExpression(@"^[\d]+$")]
        [StringLength(3)]
        public string CardNumber5 { get; set; }

        /// <summary>
        /// CardExpiryMonth is string because of MM format, where for instance 01 is a possible value
        /// </summary>
        [Required]
        [Display(Name = "Expiry date")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Your expiry date contains invalid characters.")]
        [StringLength(2)]
        public string CardExpiryMonth { get; set; }

        public int CardExpiryYear { get; set; }

        [Required]
        [Display(Name = "CVV")]
        public int CardCvv { get; set; }
    }
}