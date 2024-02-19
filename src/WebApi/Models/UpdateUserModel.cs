using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class UpdateUserModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
