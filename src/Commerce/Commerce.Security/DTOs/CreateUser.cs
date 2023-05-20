using System.ComponentModel.DataAnnotations;

namespace Commerce.Security.DTOs;

public class CreateUser
{
    [Required(ErrorMessage = $"{nameof(FirstName)} is required.")]
    public string FirstName { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(LastName)} is required.")]
    public string LastName { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(Email)} is required.")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(Password)} is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = $"{nameof(ConfirmPassword)} is required.")]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
    
}