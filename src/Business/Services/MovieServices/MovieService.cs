using Business.Infrastructure;
using Business.Models;

namespace Business.Services.MovieServices;

public class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;

    public MovieService(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    public async Task<ServiceResult> ValidateMovie(Movie movie, CancellationToken cancellationToken)
    {
        var validation = new ServiceResult();
        var movieSameTitle = await _moviesRepository.GetByTitleAsync(movie.Title, cancellationToken);

        if (movieSameTitle != null)
        {
            validation.Errors.Add("A movie with same title already exists");
        }

        return validation;
    }
}
