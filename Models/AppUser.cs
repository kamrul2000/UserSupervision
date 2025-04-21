using System;

namespace UserSupervision.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string? FullName { get; set; }     // Nullable string
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Mobile { get; set; }          // Nullable int
        public string? Address { get; set; }
        public int? RoleId { get; set; }
        public int? BranchId { get; set; }
        public int SubscriptionId { get; set; }   // Not nullable (if you know it's always filled)
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
