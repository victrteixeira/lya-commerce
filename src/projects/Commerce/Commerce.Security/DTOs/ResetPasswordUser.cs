using System.ComponentModel.DataAnnotations;

namespace Commerce.Security.DTOs;

public class ResetPasswordUser
{
    [Required(ErrorMessage = $"{nameof(Password)} is required.")]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(PasswordConfirm)} is required.")]
    public string PasswordConfirm { get; set; } = null!;
}