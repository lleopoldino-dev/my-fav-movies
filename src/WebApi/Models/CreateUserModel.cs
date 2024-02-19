using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class CreateUserModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email {  get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
