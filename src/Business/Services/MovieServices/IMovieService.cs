using Business.Models;

namespace Business.Services.MovieServices;

public interface IMovieService
{
    Task<IServiceResult> CreateMovieAsync(Movie movie, CancellationToken cancellationToken);
}
