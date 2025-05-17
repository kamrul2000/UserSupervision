namespace UserSupervision.Models.ViewModel
{
    public class BillPayViewModel
    {
        public string InvoiceNumber { get; set; }
        public int SubscriptionId { get; set; }
        public decimal BillAmount { get; set; } 
        public decimal AmountPaid { get; set; }
        public string MoneyReceiptOrCheckNumber { get; set; }
    }

}
