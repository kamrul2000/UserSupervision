namespace UserSupervision.Models.ViewModel
{
    public class ReportViewModel
    {
        public int SLNo { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CompanyName { get; set; }
        public string PaymentStatus { get; set; }
    }
}
