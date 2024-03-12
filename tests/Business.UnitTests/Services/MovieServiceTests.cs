using Business.Infrastructure;
using Business.Models;
using Business.Services;
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
        var validationResult = await movieService.ValidateMovie(existingMovie, cancellationToken);

        // Assert
        Assert.Contains("A movie with same title already exists", validationResult.Errors);
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
        var validationResult = await movieService.ValidateMovie(newMovie, cancellationToken);

        // Assert
        Assert.Empty(validationResult.Errors);
    }
}
