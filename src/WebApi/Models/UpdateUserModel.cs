using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public record UpdateUserModel([property: Required] Guid UserId, 
                              [property: Required] string Name, 
                              [property: Required][property: EmailAddress(ErrorMessage = "Invalid email address")] string Email, 
                              [property: Required] string Password);
