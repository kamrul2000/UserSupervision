using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RegisterViewModel
{
    [Required]
    public string FullName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

   
    public string Mobile { get; set; }


    public string Address { get; set; }

   
}
