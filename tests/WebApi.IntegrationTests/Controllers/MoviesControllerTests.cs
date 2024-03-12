using Business.Infrastructure;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using WebApi.Controllers;
using WebApi.Models;

namespace WebApi.UnitTests.Controllers;

public class MoviesControllerTests
{
    private readonly Mock<IMovieService> _movieServiceMock;
    private readonly Mock<IMoviesRepository> _moviesRepositoryMock;

    public MoviesControllerTests()
    {
        _movieServiceMock = new();
        _moviesRepositoryMock = new();
    }

    [Fact]
    public async Task CreateMovie_ValidModel_ReturnsCreatedMovie()
    {
        // Arrange
        var createMovieModel = new CreateMovieModel { Title = "Test Movie", Category = "Test Category", ReleaseDate = new DateTime(1990, 1, 1) };
        var cancellationToken = CancellationToken.None;
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = new DateTime(1990, 1, 1) };

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        _moviesRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(movie);

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.CreateMovie(createMovieModel, cancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<Movie>>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal($"movies/{movie.Id}", createdResult.Location);
        Assert.Equal(movie, createdResult.Value);
    }

    [Fact]
    public async Task CreateMovie_InvalidModel_ReturnsValidationProblem()
    {
        // Arrange
        var createMovieModel = new CreateMovieModel { Title = "Already existing movie", Category = "Test Category", ReleaseDate = new DateTime(1990, 1, 1) };
        var cancellationToken = CancellationToken.None;

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult { Errors = { "Error message" } });

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.CreateMovie(createMovieModel, cancellationToken);

        // Assert
        var validationProblemResult = Assert.IsType<ValidationProblem>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, validationProblemResult.StatusCode);
    }

    [Fact]
    public async Task CreateMovie_FailedCreation_ReturnsProblemDetails()
    {
        // Arrange
        var createMovieModel = new CreateMovieModel { Title = "Test Movie", Category = "Test Category", ReleaseDate = new DateTime(1990, 1, 1) };
        var cancellationToken = CancellationToken.None;

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        _moviesRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync((Movie)null);

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.CreateMovie(createMovieModel, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_ExistingMovie_SuccessfullyDeleted_ReturnsNoContent()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(movieId, cancellationToken))
            .ReturnsAsync(new Movie { Id = movieId });

        _moviesRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(true);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.DeleteMovie(movieId, cancellationToken);

        // Assert
        var noContentResult = Assert.IsType<NoContent>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_NonExistingMovie_ReturnsNotFound()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(movieId, cancellationToken))
            .ReturnsAsync((Movie)null);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.DeleteMovie(movieId, cancellationToken);

        // Assert
        var notFoundResult = Assert.IsType<NotFound>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_FailedDeletion_ReturnsProblemDetails()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(movieId, cancellationToken))
            .ReturnsAsync(new Movie { Id = movieId });

        _moviesRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(false);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.DeleteMovie(movieId, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_ExistingMovie_SuccessfullyUpdated_ReturnsNoContent()
    {
        // Arrange
        var updateMovieModel = new UpdateMovieModel
        {
            MovieId = Guid.NewGuid(),
            Title = "Updated Title",
            Category = "Updated Category",
            ReleaseDate = DateTime.Now
        };
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(updateMovieModel.MovieId, cancellationToken))
            .ReturnsAsync(new Movie { Id = updateMovieModel.MovieId });

        _moviesRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(true);

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.UpdateMovie(updateMovieModel, cancellationToken);

        // Assert
        var noContentResult = Assert.IsType<NoContent>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_ExistingMovie_FailedToUpdate_ReturnsProblemDetails()
    {
        // Arrange
        var updateMovieModel = new UpdateMovieModel
        {
            MovieId = Guid.NewGuid(),
            Title = "Updated Title",
            Category = "Updated Category",
            ReleaseDate = DateTime.Now
        };
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(updateMovieModel.MovieId, cancellationToken))
            .ReturnsAsync(new Movie { Id = updateMovieModel.MovieId });

        _moviesRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(false);

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.UpdateMovie(updateMovieModel, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_NonExistingMovie_ValidModel_ReturnsCreatedMovie()
    {
        // Arrange
        var updateMovieModel = new UpdateMovieModel
        {
            MovieId = Guid.NewGuid(),
            Title = "New Title",
            Category = "New Category",
            ReleaseDate = DateTime.Now
        };
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(updateMovieModel.MovieId, cancellationToken))
            .ReturnsAsync((Movie)null);

        _moviesRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new Movie { Id = Guid.NewGuid() });

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.UpdateMovie(updateMovieModel, cancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<Movie>>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task UpdateMovie_NonExistingMovie_InvalidModel_ReturnsValidationProblem()
    {
        // Arrange
        var updateMovieModel = new UpdateMovieModel
        {
            MovieId = Guid.NewGuid(),
            Title = "New Title",
            Category = "New Category",
            ReleaseDate = DateTime.Now
        };
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(updateMovieModel.MovieId, cancellationToken))
            .ReturnsAsync((Movie)null);

        _movieServiceMock.Setup(service => service.ValidateMovie(It.IsAny<Movie>(), cancellationToken))
            .ReturnsAsync(new ValidationResult { Errors = { "Error message" } });

        var controller = new MoviesController(_moviesRepositoryMock.Object, _movieServiceMock.Object);

        // Act
        var result = await controller.UpdateMovie(updateMovieModel, cancellationToken);

        // Assert
        var validationProblemResult = Assert.IsType<ValidationProblem>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, validationProblemResult.StatusCode);
    }

    [Fact]
    public async Task GetAll_MoviesExist_ReturnsListOfMovies()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), Title = "Movie 1" },
            new Movie { Id = Guid.NewGuid(), Title = "Movie 2" },
            new Movie { Id = Guid.NewGuid(), Title = "Movie 3" }
        };

        _moviesRepositoryMock.Setup(repo => repo.ListAllAsync(cancellationToken))
            .ReturnsAsync(movies);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.GetAll(cancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<List<Movie>>>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        var returnedMovies = Assert.IsType<List<Movie>>(okResult.Value);
        Assert.Equal(movies.Count, returnedMovies.Count);
        Assert.All(movies, movie => Assert.Contains(returnedMovies, m => m.Id == movie.Id && m.Title == movie.Title));
    }

    [Fact]
    public async Task GetAll_NoMoviesExist_ReturnsNotFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.ListAllAsync(cancellationToken))
            .ReturnsAsync((List<Movie>?)null);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.GetAll(cancellationToken);

        // Assert
        var notFoundResult = Assert.IsType<NotFound>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Get_ExistingMovie_ReturnsMovie()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var movie = new Movie { Id = movieId, Title = "Test Movie" };

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(movieId, cancellationToken))
            .ReturnsAsync(movie);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.Get(movieId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<Movie>>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        var returnedMovie = Assert.IsType<Movie>(okResult.Value);
        Assert.Equal(movie.Id, returnedMovie.Id);
        Assert.Equal(movie.Title, returnedMovie.Title);
    }

    [Fact]
    public async Task Get_NonExistingMovie_ReturnsNotFound()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _moviesRepositoryMock.Setup(repo => repo.GetAsync(movieId, cancellationToken))
            .ReturnsAsync((Movie)null);

        var controller = new MoviesController(_moviesRepositoryMock.Object, Mock.Of<IMovieService>());

        // Act
        var result = await controller.Get(movieId, cancellationToken);

        // Assert
        var notFoundResult = Assert.IsType<NotFound>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}
