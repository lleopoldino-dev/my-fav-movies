using Business;
using Business.Models;
using Business.Services;
using Business.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : MainController
{
    private readonly IUserService _userService;
    private readonly IJwtHelper _jwtHelper;

    public UsersController(IDateTime dateTime, IUserService userService, IJwtHelper jwtHelper)
        : base(dateTime)
    {
        _userService = userService;
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
            return ProblemResult("User not found or password is incorrect");
        }

        return OkResult(_jwtHelper.GetAccessToken(user, _datetime.UtcNow.AddMinutes(5)));
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
            return NotFoundResult();
        }

        return OkResult(user);
    }

    [HttpPost("")]
    [AllowAnonymous]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateUser(CreateUserModel createUserModel, CancellationToken cancellationToken)
    {
        return await HandleUserCreationAsync(new User { 
                                                Id = Guid.NewGuid(), 
                                                Name = createUserModel.Name, 
                                                Email = createUserModel.Email, 
                                                PasswordHash = createUserModel.Password 
                                            }, cancellationToken);
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

        if (findedUser == null)
        {
            return await HandleUserCreationAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = updateUserModel.Name,
                Email = updateUserModel.Email,
                PasswordHash = updateUserModel.Password
            }, cancellationToken);
        }

        return await HandleUserUpdateAsync(new User { 
                                            Id = updateUserModel.UserId, 
                                            Name = updateUserModel.Name, 
                                            Email = updateUserModel.Email, 
                                            PasswordHash = updateUserModel.Password 
                                          }, cancellationToken);
    }

    private async Task<IResult> HandleUserUpdateAsync(User user, CancellationToken cancellationToken)
    {
        if (await _userService.UpdateAsync(user, cancellationToken))
        {
            return NoContentResult();
        }

        return ProblemResult("Failed updating user");
    }

    private async Task<IResult> HandleUserCreationAsync(User user, CancellationToken cancellationToken) 
    {
        var result = await _userService.CreateAsync(user, user.PasswordHash, cancellationToken);

        if (result.HasErrors())
        {
            return ErrorProblemResult<User>(result);
        }

        return CreatedResult($"users/{((ServiceResult<User>)result).Entity!.Id}", ((ServiceResult<User>)result).Entity!);
    }
}
