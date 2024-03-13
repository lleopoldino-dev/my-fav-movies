using Business;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IDateTime _datetime;
    private readonly IJwtHelper _jwtHelper;

    public UsersController(IUserService userService, IDateTime dateTime, IJwtHelper jwtHelper)
    {
        _userService = userService;
        _datetime = dateTime;
        _jwtHelper = jwtHelper;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Login(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userService.LoginUserAsync(email, password, cancellationToken);
        if (user == null)
        {
            return TypedResults.Problem(detail: "User not found or password is incorrect");
        }

        return TypedResults.Ok(_jwtHelper.GetAccessToken(user, _datetime.UtcNow.AddMinutes(5)));
    }

    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    public async Task<IResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.FindById(id, cancellationToken);

        if (user == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(user);
    }

    [HttpPost("")]
    [AllowAnonymous]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateUser(CreateUserModel createUserModel, CancellationToken cancellationToken)
    {
        var user = new User { Id = Guid.NewGuid(), Name = createUserModel.Name, Email = createUserModel.Email, PasswordHash = createUserModel.Password };
        var validation = await _userService.ValidateUserAsync(user, cancellationToken);
        if (validation.Errors.Count != 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "Errors", validation.Errors.ToArray() } });
        }

        var createdUser = await _userService.CreateAsync(user, user.PasswordHash, cancellationToken);

        if (createdUser == null)
        {
            return TypedResults.Problem(detail: "Failed creating user");
        }

        return TypedResults.Created($"/{createdUser.Id}", createdUser);
    }

    [HttpPut("")]
    [Authorize]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateUser(UpdateUserModel updateUserModel, CancellationToken cancellationToken)
    {
        var findedUser = await _userService.FindById(updateUserModel.UserId, cancellationToken);
        if (findedUser != null)
        {
            var updatedUser = new User { Id = updateUserModel.UserId, Name = updateUserModel.Name, Email = updateUserModel.Email, PasswordHash = updateUserModel.Password };
            if (await _userService.UpdateAsync(updatedUser, cancellationToken))
            {
                return TypedResults.NoContent();
            }

            return TypedResults.Problem(detail: "Failed updating user");
        }

        var user = new User { Id = Guid.NewGuid(), Name = updateUserModel.Name, Email = updateUserModel.Email, PasswordHash = updateUserModel.Password };
        var validation = await _userService.ValidateUserAsync(user, cancellationToken);
        if (validation.Errors.Count != 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "Errors", validation.Errors.ToArray() } });
        }

        var createdUser = await _userService.CreateAsync(user, user.PasswordHash, cancellationToken);

        if (createdUser == null)
        {
            return TypedResults.Problem(detail: "Failed creating user");
        }

        return TypedResults.Created($"/{createdUser.Id}", createdUser);
    }
}
