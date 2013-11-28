using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookland.Models
{
    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }

    public class CartItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Cart Cart { get; set; }
    }
}