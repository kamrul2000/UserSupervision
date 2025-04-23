namespace UserSupervision.Models
{
    public class UserBillStatus
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }

       
    }
}
