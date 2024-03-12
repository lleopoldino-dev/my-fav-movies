using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public record CreateMovieModel([property: Required] string Title, [property: Required] string Category, [property: Required] DateTime ReleaseDate);
