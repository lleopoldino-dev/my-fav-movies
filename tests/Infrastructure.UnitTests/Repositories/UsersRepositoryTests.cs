using Business;
using Business.Models;
using Infrastructure.Adapters;
using Infrastructure.Repositories;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

public class UsersRepositoryTests
{
    [Fact]
    public async Task CreateAsync_Returns_Null_When_Fails()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };

        // Act
        var result = await usersRepository.CreateAsync(user, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Returns_User_When_Created()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };

        // Act
        var result = await usersRepository.CreateAsync(user, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_False_When_User_Not_Found()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };

        // Act
        var result = await usersRepository.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_True_When_User_Found_And_Updated()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        user.Name = "New name";

        // Act
        var result = await usersRepository.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_True_When_Movie_Found_And_Not_Updated()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        user.Name = "New name";

        // Act
        var result = await usersRepository.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAsync_Returns_Null_When_Not_Found()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var userId = Guid.NewGuid();

        // Act
        var result = await usersRepository.GetAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_Returns_User_When_Returns_Result()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var userId = user.Id;

        // Act
        var result = await usersRepository.GetAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_Returns_Null_When_Not_Found()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var userEmail = "noexistingmail@mail.com";
        var userPassword = "pass";

        // Act
        var result = await usersRepository.GetByEmailAndPasswordAsync(userEmail, userPassword, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_Returns_User_When_Founded()
    {
        // Arrange
        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow);

        var user = new User { Id = Guid.NewGuid(), Name = "UserName", Email = "user@example.com", PasswordHash = "password", CreatedDate = DateTime.UtcNow };
        var mockDbConnectionAdapter = new Mock<IDbConnectionAdapter>();
        mockDbConnectionAdapter
            .Setup(adapter => adapter.ExecuteReaderSingleAsync(It.IsAny<string>(), It.IsAny<Func<Dictionary<string, object>, User?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var usersRepository = new UsersRepository(mockDbConnectionAdapter.Object, dateTime.Object);
        var userEmail = user.Email;
        var userPassword = user.PasswordHash;

        // Act
        var result = await usersRepository.GetByEmailAndPasswordAsync(userEmail, userPassword, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}
