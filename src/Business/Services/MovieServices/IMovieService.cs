using Business.Models;

namespace Business.Services.MovieServices;

public interface IMovieService
{
    Task<ServiceResult> ValidateMovie(Movie movie, CancellationToken cancellationToken);
}
