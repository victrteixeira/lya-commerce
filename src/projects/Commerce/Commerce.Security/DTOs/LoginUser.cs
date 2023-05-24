using System.ComponentModel.DataAnnotations;

namespace Commerce.Security.DTOs;

public class LoginUser
{
    [Required(ErrorMessage = $"{nameof(Email)} must be provided.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(Password)} must be provided.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;
}