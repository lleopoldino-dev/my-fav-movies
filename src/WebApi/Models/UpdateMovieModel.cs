using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public record UpdateMovieModel([property: Required] Guid MovieId, 
                               [property: Required] string Title, 
                               [property: Required] string Category, 
                               [property: Required] DateTime ReleaseDate);
