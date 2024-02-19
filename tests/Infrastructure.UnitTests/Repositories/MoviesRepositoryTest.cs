using Business.Models;
using Infrastructure.Adapters;
using Infrastructure.Repositories;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

public class MoviesRepositoryTest
{
    [Fact]
    public async Task CreateAsync_Returns_Null_When_Fails()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };

        // Act
        var result = await moviesRepository.CreateAsync(movie, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Returns_Movie_When_Created()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };

        // Act
        var result = await moviesRepository.CreateAsync(movie, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_False_When_Movie_Not_Found()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };

        // Act
        var result = await moviesRepository.UpdateAsync(movie, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_True_When_Movie_Found_And_Updated()
    {
        // Arrange
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        movie.Title = "New title";

        // Act
        var result = await moviesRepository.UpdateAsync(movie, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_True_When_Movie_Found_And_Not_Updated()
    {
        // Arrange
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        movie.Title = "New title";

        // Act
        var result = await moviesRepository.UpdateAsync(movie, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_Succeeds()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };

        // Act
        var result = await moviesRepository.DeleteAsync(movie, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_Returns_False_When_Fails()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };

        // Act
        var result = await moviesRepository.DeleteAsync(movie, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAsync_Returns_Null_When_Returns_No_Rows()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movieId = Guid.NewGuid();

        // Act
        var result = await moviesRepository.GetAsync(movieId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_Returns_Null_When_Returns_Result()
    {
        // Arrange
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movieId = movie.Id;

        // Act
        var result = await moviesRepository.GetAsync(movieId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ListAllAsync_Returns_Empty_List_When_No_Movies_Found()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);

        // Act
        var result = await moviesRepository.ListAllAsync(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListAllAsync_Returns_List_When_Movies_Found()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>() { new Movie { Id = Guid.NewGuid(), Title = "Test Movie 1", Category = "Test Category", ReleaseDate = DateTime.UtcNow },
                                              new Movie { Id = Guid.NewGuid(), Title = "Test Movie2", Category = "Test Category", ReleaseDate = DateTime.UtcNow } });

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);

        // Act
        var result = await moviesRepository.ListAllAsync(CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetByTitleAsync_Returns_Null_When_No_Movie_Found()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);
        var movieTitle = "Non-existent Movie";

        // Act
        var result = await moviesRepository.GetByTitleAsync(movieTitle, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTitleAsync_Returns_Movie_When_Movie_Found()
    {
        // Arrange
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Category = "Test Category", ReleaseDate = DateTime.UtcNow };
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, Movie?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        var moviesRepository = new MoviesRepository(mockDbConnectionAdapter.Object);

        // Act
        var result = await moviesRepository.GetByTitleAsync(movie.Title, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}