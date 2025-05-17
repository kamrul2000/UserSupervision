namespace UserSupervision.Models
{
    public class BillPay
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } 
        public string MoneyReceiptOrCheckNumber { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
