using Business;
using Business.Infrastructure;
using Business.Models;
using Business.Services.MovieServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : MainController
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly IMovieService _movieService;

    public MoviesController(IDateTime dateTime, IMoviesRepository moviesRepository, IMovieService movieService)
        : base(dateTime)
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
        return await HandleMovieCreationAsync(new Movie { 
                                                Id = Guid.NewGuid(), 
                                                Title = createMovieModel.Title, 
                                                Category = createMovieModel.Category, 
                                                ReleaseDate = createMovieModel.ReleaseDate }, cancellationToken);
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
            return NotFoundResult();
        }

        var result = await _moviesRepository.DeleteAsync(movie, cancellationToken);

        if (!result)
        {
            return ProblemResult("Failed creating movie");
        }

        return NoContentResult();
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
        if (findedMovie == null)
        {
            return await HandleMovieCreationAsync(new Movie { 
                                                    Id = Guid.NewGuid(), 
                                                    Title = updateMovieModel.Title, 
                                                    Category = updateMovieModel.Category, 
                                                    ReleaseDate = updateMovieModel.ReleaseDate 
                                                  }, cancellationToken);
        }

        return await HandleMovieUpdateAsync(new Movie { 
                                            Id = updateMovieModel.MovieId, 
                                            Title = updateMovieModel.Title, 
                                            Category = updateMovieModel.Category, 
                                            ReleaseDate = updateMovieModel.ReleaseDate }, cancellationToken);
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
            return NotFoundResult();
        }

        return OkResult(result);
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
            return NotFoundResult();
        }

        return OkResult(result);
    }

    private async Task<IResult> HandleMovieUpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        if (await _moviesRepository.UpdateAsync(movie, cancellationToken))
        {
            return NoContentResult();
        }

        return ProblemResult("Failed updating movie");
    }

    private async Task<IResult> HandleMovieCreationAsync(Movie movie, CancellationToken cancellationToken)
    {
        var validationResult = await _movieService.ValidateMovie(movie, cancellationToken);
        if (validationResult.Errors.Count != 0)
        {
            return ValidationProblemResult(new Dictionary<string, string[]> { { "Errors", validationResult.Errors.ToArray() } });
        }

        var createdUser = await _moviesRepository.CreateAsync(movie, cancellationToken);

        if (createdUser == null)
        {
            return ProblemResult("Failed creating movie");
        }

        return CreatedResult($"movies/{createdUser.Id}", createdUser);
    }
}
