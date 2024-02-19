using Business.Infrastructure;
using Business.Models;

namespace Business.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;

    public MovieService(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    public async Task<ValidationResult> ValidateMovie(Movie movie, CancellationToken cancellationToken)
    {
        var validation = new ValidationResult();
        var movieSameTitle = await _moviesRepository.GetByTitleAsync(movie.Title, cancellationToken);

        if (movieSameTitle != null) 
        {
            validation.Errors.Add("A movie with same title already exists");
        }

        return validation;
    }
}
