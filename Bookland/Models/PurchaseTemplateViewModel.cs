using System;

namespace Bookland.Models
{
    public class PurchaseTemplateViewModel : CheckoutViewModel
    {
        public Guid TransactionID { get; set; }
    }
}