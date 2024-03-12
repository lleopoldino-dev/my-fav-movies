using Infrastructure.Mappers;

namespace Infrastructure.UnitTests.Mappers;

public class UserMapperTests
{
    [Fact]
    public void MapFromDbResult_ValidData_MapsCorrectly()
    {
        // Arrange
        var data = new Dictionary<string, object>
        {
            { "id", Guid.NewGuid() },
            { "name", "Username" },
            { "email", "user@example.com" },
            { "passwordhash", "password" }
        };

        // Act
        var result = UserMapper.MapFromDbResult(data);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(data["id"], result.Id);
        Assert.Equal(data["name"], result.Name);
        Assert.Equal(data["email"], result.Email);
        Assert.Equal(data["passwordhash"], result.PasswordHash);
    }

    [Fact]
    public void MapFromDbResult_NullData_ThrowsException()
    {
        // Arrange
        Dictionary<string, object>? data = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UserMapper.MapFromDbResult(data));
    }

    [Fact]
    public void MapFromDbResult_IncompleteData_ThrowsException()
    {
        // Arrange
        var data = new Dictionary<string, object>
        {
            { "id", Guid.NewGuid() },
            { "name", "User name" }
        };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => UserMapper.MapFromDbResult(data));
    }
}
