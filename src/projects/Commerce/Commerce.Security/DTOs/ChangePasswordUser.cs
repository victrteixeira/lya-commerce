using System.ComponentModel.DataAnnotations;

namespace Commerce.Security.DTOs;

public class ChangePasswordUser
{
    [Required(ErrorMessage = $"{nameof(Email)} is required.")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(OldPassword)} is required.")]
    public string OldPassword { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(NewPassword)} is required.")]
    public string NewPassword { get; set; } = null!;
}