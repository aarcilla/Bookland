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

        public decimal GetTotalPrice()
        {
            decimal totalPrice = 0.0M;
            foreach (CartItem cartItem in CartItems)
                totalPrice += cartItem.GetCartItemPrice();

            return totalPrice;
        }

        public int GetItemCount()
        {
            int count = 0;
            foreach (CartItem cartItem in CartItems)
                count += cartItem.Quantity;

            return count;
        }
    }

    public class CartItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Cart Cart { get; set; }

        public decimal GetCartItemPrice()
        {
            return Product.Price * Quantity;
        }
    }
}