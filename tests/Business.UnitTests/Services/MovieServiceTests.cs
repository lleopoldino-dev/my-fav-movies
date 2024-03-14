using Business.Infrastructure;
using Business.Models;
using Business.Services;
using Business.Services.MovieServices;
using Moq;

namespace Business.UnitTests.Services;

public class MovieServiceTests
{
    [Fact]
    public async Task ValidateMovie_ExistingMovie_Returns_Error()
    {
        // Arrange
        var existingMovie = new Movie { Title = "Existing Movie" };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IMoviesRepository>();
        mockRepository.Setup(repo => repo.GetByTitleAsync(existingMovie.Title, cancellationToken))
            .ReturnsAsync(existingMovie);

        var movieService = new MovieService(mockRepository.Object);

        // Act
        var validationResult = await movieService.ValidateMovieAsync(existingMovie, cancellationToken) as ServiceValidationResult;

        // Assert
        Assert.Contains("A movie with same title already exists", validationResult.ValidationErrors);
    }

    [Fact]
    public async Task ValidateMovie_NewMovie_Returns_ValidResult()
    {
        // Arrange
        var newMovie = new Movie { Title = "New Movie" };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IMoviesRepository>();
        mockRepository.Setup(repo => repo.GetByTitleAsync(newMovie.Title, cancellationToken))
            .ReturnsAsync(null as Movie);

        var movieService = new MovieService(mockRepository.Object);

        // Act
        var validationResult = await movieService.ValidateMovieAsync(newMovie, cancellationToken) as ServiceValidationResult;

        // Assert
        Assert.Empty(validationResult.ValidationErrors);
    }
}
