using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class CreateMovieModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }
}
