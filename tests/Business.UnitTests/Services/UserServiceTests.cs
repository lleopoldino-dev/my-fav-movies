using Business.Infrastructure;
using Business.Models;
using Business.Services;
using Moq;

namespace Business.UnitTests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task LoginUserAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password";
        var cancellationToken = CancellationToken.None;
        var user = new User { Email = email };

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.GetByEmailAndPasswordAsync(email, password, cancellationToken))
            .ReturnsAsync(user);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.LoginUserAsync(email, password, cancellationToken);

        // Assert
        Assert.Equal(user, result);
        mockRepository.Verify(repo => repo.GetByEmailAndPasswordAsync(email, password, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_InValidCredentials_ReturnsNull()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password";
        var cancellationToken = CancellationToken.None;
        var user = new User { Email = email };

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.GetByEmailAndPasswordAsync(email, password, cancellationToken))
            .ReturnsAsync((User)null);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.LoginUserAsync(email, password, cancellationToken);

        // Assert
        Assert.Null(result);
        mockRepository.Verify(repo => repo.GetByEmailAndPasswordAsync(email, password, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NewUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = new User { Email = "newuser@example.com" };
        var password = "password";
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.CreateAsync(user, cancellationToken))
            .ReturnsAsync(user);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.CreateAsync(user, password, cancellationToken);

        // Assert
        Assert.Equal(user, result);
        mockRepository.Verify(repo => repo.CreateAsync(user, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NewUser_Returns_Null_Failed_Creating()
    {
        // Arrange
        var user = new User { Email = "newuser@example.com" };
        var password = "password";
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.CreateAsync(user, cancellationToken))
            .ReturnsAsync((User)null);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.CreateAsync(user, password, cancellationToken);

        // Assert
        Assert.Null(result);
        mockRepository.Verify(repo => repo.CreateAsync(user, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidUser_ReturnsTrue()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.UpdateAsync(user, cancellationToken))
            .ReturnsAsync(true);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.UpdateAsync(user, cancellationToken);

        // Assert
        Assert.True(result);
        mockRepository.Verify(repo => repo.UpdateAsync(user, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidUser_ReturnsFalse()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.UpdateAsync(user, cancellationToken))
            .ReturnsAsync(false);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.UpdateAsync(user, cancellationToken);

        // Assert
        Assert.False(result);
        mockRepository.Verify(repo => repo.UpdateAsync(user, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task FindById_ValidId_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.GetAsync(userId, cancellationToken))
            .ReturnsAsync(user);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.FindById(userId, cancellationToken);

        // Assert
        Assert.Equal(user, result);
        mockRepository.Verify(repo => repo.GetAsync(userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task FindById_InvalidId_Returns_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IUsersRepository>();
        mockRepository.Setup(repo => repo.GetAsync(userId, cancellationToken))
            .ReturnsAsync((User)null);

        var userService = new UserService(mockRepository.Object);

        // Act
        var result = await userService.FindById(userId, cancellationToken);

        // Assert
        Assert.Null(result);
        mockRepository.Verify(repo => repo.GetAsync(userId, cancellationToken), Times.Once);
    }
}
