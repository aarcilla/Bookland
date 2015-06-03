namespace Bookland.Models
{
    public class CheckoutViewModel
    {
        public Address DeliveryAddress { get; set; }
        public PaymentModel Payment { get; set; }
        public Cart UserCart { get; set; }
    }
}