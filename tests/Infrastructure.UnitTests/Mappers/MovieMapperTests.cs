using Infrastructure.Mappers;

namespace Infrastructure.UnitTests.Mappers;

public class MovieMapperTests
{
    [Fact]
    public void MapFromDbResult_ValidData_MapsCorrectly()
    {
        // Arrange
        var data = new Dictionary<string, object>
        {
            { "id", Guid.NewGuid() },
            { "title", "Sample Movie" },
            { "category", "Action" },
            { "releasedate", DateTime.Now }
        };

        // Act
        var result = MovieMapper.MapFromDbResult(data);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(data["id"], result.Id);
        Assert.Equal(data["title"], result.Title);
        Assert.Equal(data["category"], result.Category);
        Assert.Equal(data["releasedate"], result.ReleaseDate);
    }

    [Fact]
    public void MapFromDbResult_NullData_ThrowsException()
    {
        // Arrange
        Dictionary<string, object> data = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MovieMapper.MapFromDbResult(data));
    }

    [Fact]
    public void MapFromDbResult_IncompleteData_ThrowsException()
    {
        // Arrange
        var data = new Dictionary<string, object>
        {
            { "id", Guid.NewGuid() },
            { "title", "Sample Movie" }
        };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => MovieMapper.MapFromDbResult(data));
    }
}
