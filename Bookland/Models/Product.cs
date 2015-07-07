using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookland.Models
{
    public class Product
    {
        private const int nameMaxLength = 150;
        private const int descriptionMaxLength = 1000;

        [NotMapped]
        public int NameMaxLength { get { return nameMaxLength; } }

        [NotMapped]
        public int DescriptionMaxLength { get { return descriptionMaxLength; } }

        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        [StringLength(nameMaxLength, ErrorMessage = "Maximum of {0} characters.")]
        public string Name { get; set; }

        [StringLength(descriptionMaxLength, ErrorMessage = "Maximum of {0} characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int Year { get; set; }
        
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Release date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }
        
        [Display(Name = "Date added")]
        public DateTime DateAdded { get; set; }

        [Display(Name = "Image")]
        public byte[] ImageData { get; set; }

        public string ImageMimeType { get; set; }

        [Display(Name = "Status")]
        public virtual ProductStatus ProductStatus { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }

        public bool IsImageInformationNullOrEmpty
        {
            get
            {
                return ((ImageData == null || ImageData.Length <= 0) || String.IsNullOrEmpty(ImageMimeType));
            }
        }
    }
}