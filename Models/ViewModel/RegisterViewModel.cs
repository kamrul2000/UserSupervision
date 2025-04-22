using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSupervision.Models.ViewModel;
public class RegisterViewModel
{
    [Required]
    public string? FullName { get; set; }
    [Required, EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    public int? Mobile { get; set; }
}
