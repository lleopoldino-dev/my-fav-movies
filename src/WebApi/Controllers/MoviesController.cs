using Business.Infrastructure;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly IMovieService _movieService;

    public MoviesController(IMoviesRepository moviesRepository, IMovieService movieService)
    {
        _moviesRepository = moviesRepository;
        _movieService = movieService;
    }

    [HttpPost("")]
    [Authorize]
    [ProducesResponseType<Movie>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateMovie(CreateMovieModel createMovieModel, CancellationToken cancellationToken)
    {
        var movie = new Movie { Id = Guid.NewGuid(), Title = createMovieModel.Title, Category = createMovieModel.Category, ReleaseDate = createMovieModel.ReleaseDate };
        var validationResult = await _movieService.ValidateMovie(movie, cancellationToken);
        if (validationResult.Errors.Any()) 
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "Errors", validationResult.Errors.ToArray() } });
        }

        var createdMovie = await CreateMovieAsync(movie, cancellationToken);

        if (createdMovie == null)
        {
            return TypedResults.Problem(detail: "Failed creating movie");
        }

        return TypedResults.Created($"movies/{createdMovie.Id}", createdMovie);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> DeleteMovie(Guid id, CancellationToken cancellationToken)
    {
        var movie = await _moviesRepository.GetAsync(id, cancellationToken);

        if (movie == null)
        {
            return TypedResults.NotFound();
        }

        var result = await _moviesRepository.DeleteAsync(movie, cancellationToken);

        if (!result)
        {
            return TypedResults.Problem(detail: "Failed creating movie");
        }

        return TypedResults.NoContent();
    }

    [HttpPut("")]
    [Authorize]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<Movie>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateMovie(UpdateMovieModel updateMovieModel, CancellationToken cancellationToken)
    {
        var findedMovie = await _moviesRepository.GetAsync(updateMovieModel.MovieId, cancellationToken);
        if (findedMovie != null)
        {
            var updatedMovie = new Movie { Id = updateMovieModel.MovieId, Title = updateMovieModel.Title, Category = updateMovieModel.Category, ReleaseDate = updateMovieModel.ReleaseDate };
            if (await _moviesRepository.UpdateAsync(updatedMovie, cancellationToken))
            {
                return TypedResults.NoContent();
            }

            return TypedResults.Problem(detail: "Failed updating movie");
        }

        var movie = new Movie { Id = Guid.NewGuid(), Title = updateMovieModel.Title, Category = updateMovieModel.Category, ReleaseDate = updateMovieModel.ReleaseDate };
        var validationResult = await _movieService.ValidateMovie(movie, cancellationToken);
        if (validationResult.Errors.Any())
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "Errors", validationResult.Errors.ToArray() } });
        }

        var createdUser = await CreateMovieAsync(movie, cancellationToken);

        if (createdUser == null)
        {
            return TypedResults.Problem(detail: "Failed creating movie");
        }

        return TypedResults.Created($"/{createdUser.Id}", createdUser);
    }

    [AllowAnonymous]
    [HttpGet("")]
    [ProducesResponseType<List<Movie>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _moviesRepository.ListAllAsync(cancellationToken);

        if (result == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Movie>(StatusCodes.Status200OK)]
    public async Task<IResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _moviesRepository.GetAsync(id, cancellationToken);

        if (result == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    private async Task<Movie?> CreateMovieAsync(Movie movie, CancellationToken cancellationToken)
    {
        var result = await _moviesRepository.CreateAsync(movie, cancellationToken);

        return result;
    }
}
