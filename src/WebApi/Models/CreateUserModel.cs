using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public record CreateUserModel([property: Required(ErrorMessage = "Name is required")] string Name, 
                              [property: Required(ErrorMessage = "Email is required")][property: EmailAddress(ErrorMessage = "Invalid email address")] string Email, 
                              [property: Required(ErrorMessage = "Password is required")] string Password);
