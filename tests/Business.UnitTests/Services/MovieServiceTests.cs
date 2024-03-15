using Business.Infrastructure;
using Business.Models;
using Business.Services;
using Business.Services.MovieServices;
using Moq;

namespace Business.UnitTests.Services;

public class MovieServiceTests
{
    [Fact]
    public async Task CreateMovie_InvalidExistingMovie_Returns_Error()
    {
        // Arrange
        var existingMovie = new Movie { Title = "Existing Movie" };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IMoviesRepository>();
        mockRepository.Setup(repo => repo.GetByTitleAsync(existingMovie.Title, cancellationToken))
            .ReturnsAsync(existingMovie);

        var movieService = new MovieService(mockRepository.Object);

        // Act
        var validationResult = await movieService.CreateMovieAsync(existingMovie, cancellationToken) as ServiceValidationResult;

        // Assert
        Assert.Contains("A movie with same title already exists", validationResult.Errors);
    }

    [Fact]
    public async Task CreateMovie_FailedCreatingMovie_Returns_Error()
    {
        // Arrange
        var existingMovie = new Movie { Title = "Existing Movie" };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IMoviesRepository>();
        
        mockRepository.Setup(repo => repo.GetByTitleAsync(existingMovie.Title, cancellationToken))
            .ReturnsAsync(null as Movie);

        mockRepository.Setup(repo => repo.CreateAsync(existingMovie, cancellationToken))
            .ReturnsAsync(null as Movie);

        var movieService = new MovieService(mockRepository.Object);

        // Act
        var serviceResult = await movieService.CreateMovieAsync(existingMovie, cancellationToken) as ServiceResult<Movie>;

        // Assert
        Assert.Contains("Failed creating movie", serviceResult.Errors);
    }

    [Fact]
    public async Task CreateMovie_NewMovie_Returns_ValidResult()
    {
        // Arrange
        var newMovie = new Movie { Title = "New Movie" };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IMoviesRepository>();
        mockRepository.Setup(repo => repo.GetByTitleAsync(newMovie.Title, cancellationToken))
            .ReturnsAsync(null as Movie);

        mockRepository.Setup(repo => repo.CreateAsync(newMovie, cancellationToken))
            .ReturnsAsync(newMovie);

        var movieService = new MovieService(mockRepository.Object);

        // Act
        var serviceResult = await movieService.CreateMovieAsync(newMovie, cancellationToken) as ServiceResult<Movie>;

        // Assert
        Assert.Empty(serviceResult.Errors);
    }
}
