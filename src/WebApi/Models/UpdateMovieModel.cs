using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class UpdateMovieModel
{
    [Required]
    public Guid MovieId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }
}
