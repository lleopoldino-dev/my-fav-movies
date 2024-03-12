﻿using Business;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using WebApi.Controllers;
using WebApi.Models;

namespace WebApi.UnitTests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;

    public UsersControllerTests()
    {
        _userServiceMock = new();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password";
        var cancellationToken = CancellationToken.None;
        var user = new User { Email = email };

        var dateTime = new Mock<IDateTime>();
        dateTime.Setup(dt => dt.UtcNow).Returns(DateTime.UtcNow.AddMinutes(10));

        _userServiceMock.Setup(service => service.LoginUserAsync(email, password, cancellationToken))
            .ReturnsAsync(user);

        var controller = new UsersController(_userServiceMock.Object, dateTime.Object);

        // Act
        var result = await controller.Login(email, password, cancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsProblemDetails()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password";
        var cancellationToken = CancellationToken.None;

        _userServiceMock.Setup(service => service.LoginUserAsync(email, password, cancellationToken))
            .ReturnsAsync((User)null);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.Login(email, password, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task GetUser_ExistingUser_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var user = new User { Id = userId };

        _userServiceMock.Setup(service => service.FindById(userId, cancellationToken))
            .ReturnsAsync(user);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.GetUser(userId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<User>>(result);
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public async Task GetUser_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _userServiceMock.Setup(service => service.FindById(userId, cancellationToken))
            .ReturnsAsync((User)null);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.GetUser(userId, cancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task CreateUser_ValidModel_ReturnsCreatedUser()
    {
        // Arrange
        var createUserModel = new CreateUserModel { Name = "User", Email = "user@example.com", Password = "password" };
        var cancellationToken = CancellationToken.None;
        var createdUser = new User { Id = Guid.NewGuid(), Name = createUserModel.Name, Email = createUserModel.Email, PasswordHash = createUserModel.Password };

        var controller = SetupControllerWithCreateUserAsync(createdUser, new Business.Services.ValidationResult(), cancellationToken);

        // Act
        var result = await controller.CreateUser(createUserModel, cancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<User>>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal($"/{createdUser.Id}", createdResult.Location);
        Assert.Equal(createdUser, createdResult.Value);
    }

    [Fact]
    public async Task CreateUser_FailedCreation_ReturnsProblemDetails()
    {
        // Arrange
        var createUserModel = new CreateUserModel { Name = "User", Email = "user@example.com", Password = "password" };
        var cancellationToken = CancellationToken.None;

        var controller = SetupControllerWithCreateUserAsync(null, new Business.Services.ValidationResult() { Errors = new List<string> { "Failed" } }, cancellationToken);

        // Act
        var result = await controller.CreateUser(createUserModel, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ValidationProblem>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    private UsersController SetupControllerWithCreateUserAsync(User? createdUser, Business.Services.ValidationResult validationResult, CancellationToken cancellationToken)
    {
        _userServiceMock.Setup(service => service.ValidateUserAsync(It.IsAny<User>(), cancellationToken))
            .ReturnsAsync(validationResult);
        _userServiceMock.Setup(service => service.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(createdUser);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());
        return controller;
    }

     [Fact]
    public async Task UpdateUser_ExistingUser_SuccessfullyUpdated_ReturnsNoContent()
    {
        // Arrange
        var updateUserModel = new UpdateUserModel
        {
            UserId = Guid.NewGuid(),
            Name = "User",
            Email = "user@example.com",
            Password = "password"
        };
        var cancellationToken = CancellationToken.None;

        _userServiceMock.Setup(service => service.FindById(updateUserModel.UserId, cancellationToken))
            .ReturnsAsync(new User { Id = updateUserModel.UserId });

        _userServiceMock.Setup(service => service.UpdateAsync(It.IsAny<User>(), cancellationToken))
            .ReturnsAsync(true);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.UpdateUser(updateUserModel, cancellationToken);

        // Assert
        var noContentResult = Assert.IsType<NoContent>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ExistingUser_FailedToUpdate_ReturnsProblemDetails()
    {
        // Arrange
        var updateUserModel = new UpdateUserModel
        {
            UserId = Guid.NewGuid(),
            Name = "User",
            Email = "user@example.com",
            Password = "password"
        };
        var cancellationToken = CancellationToken.None;

        _userServiceMock.Setup(service => service.FindById(updateUserModel.UserId, cancellationToken))
            .ReturnsAsync(new User { Id = updateUserModel.UserId });

        _userServiceMock.Setup(service => service.UpdateAsync(It.IsAny<User>(), cancellationToken))
            .ReturnsAsync(false);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.UpdateUser(updateUserModel, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_SuccessfullyCreated_ReturnsCreated()
    {
        // Arrange
        var updateUserModel = new UpdateUserModel
        {
            UserId = Guid.NewGuid(),
            Name = "User",
            Email = "user@example.com",
            Password = "password"
        };
        var cancellationToken = CancellationToken.None;
        var createdUser = new User { Id = updateUserModel.UserId };

        _userServiceMock.Setup(service => service.ValidateUserAsync(It.IsAny<User>(), cancellationToken))
            .ReturnsAsync(new Business.Services.ValidationResult());

        _userServiceMock.Setup(service => service.FindById(updateUserModel.UserId, cancellationToken))
            .ReturnsAsync((User)null);

        _userServiceMock.Setup(service => service.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(createdUser);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.UpdateUser(updateUserModel, cancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<User>>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal($"/{createdUser.Id}", createdResult.Location);
        Assert.Equal(createdUser, createdResult.Value);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_FailedToCreate_ReturnsValidationProblem()
    {
        // Arrange
        var updateUserModel = new UpdateUserModel
        {
            UserId = Guid.NewGuid(),
            Name = "User",
            Email = "user@example.com",
            Password = "password"
        };
        var cancellationToken = CancellationToken.None;

        _userServiceMock.Setup(service => service.FindById(updateUserModel.UserId, cancellationToken))
            .ReturnsAsync((User)null);

        _userServiceMock.Setup(service => service.ValidateUserAsync(It.IsAny<User>(), cancellationToken))
            .ReturnsAsync(new Business.Services.ValidationResult() { Errors = new List<string> { "User with same email already exists" } });

        _userServiceMock.Setup(service => service.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), cancellationToken))
            .ReturnsAsync((User)null);

        var controller = new UsersController(_userServiceMock.Object, Mock.Of<IDateTime>());

        // Act
        var result = await controller.UpdateUser(updateUserModel, cancellationToken);

        // Assert
        var problemResult = Assert.IsType<ValidationProblem>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }
}
