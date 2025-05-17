namespace UserSupervision.Models
{
    public class UserWithCompanyViewModel
    {
        public User? User { get; set; }
        public decimal? Amount { get; set; }
        public string? CompanyName { get; set; }
        public string? PaymentStatus { get; set; } 

    }
}
