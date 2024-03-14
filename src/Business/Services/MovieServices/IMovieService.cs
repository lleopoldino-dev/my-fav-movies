using Business.Models;

namespace Business.Services.MovieServices;

public interface IMovieService
{
    Task<IServiceResult> ValidateMovieAsync(Movie movie, CancellationToken cancellationToken);
    Task<IServiceResult> CreateMovieAsync(Movie movie, CancellationToken cancellationToken);
}
