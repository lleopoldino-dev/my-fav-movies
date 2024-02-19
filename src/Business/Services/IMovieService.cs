using Business.Models;

namespace Business.Services;

public interface IMovieService
{
    Task<ValidationResult> ValidateMovie(Movie movie, CancellationToken cancellationToken);
}
