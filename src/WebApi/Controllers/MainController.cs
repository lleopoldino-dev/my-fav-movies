using Business;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class MainController : ControllerBase
{
    protected private readonly IDateTime _datetime;

    public MainController(IDateTime datetime)
    {
        _datetime = datetime;
    }

    protected IResult ProblemResult(string detail)
    {
        return TypedResults.Problem(detail: detail);
    }

    protected IResult ValidationProblemResult(Dictionary<string, string[]> errors)
    {
        return TypedResults.ValidationProblem(errors);
    }

    protected IResult ValidationProblemResult(string[] errors)
    {
        return ValidationProblemResult(new Dictionary<string, string[]> { { "Errors", errors } });
    }

    protected IResult NotFoundResult()
    {
        return TypedResults.NotFound();
    }

    protected IResult NoContentResult()
    {
        return TypedResults.NoContent();
    }

    protected IResult CreatedResult<T>(string uri, T responseObject) where T : class
    {
        return TypedResults.Created(uri, responseObject);
    }

    protected IResult OkResult<T>(T responseObject) where T : class
    {
        return TypedResults.Ok(responseObject);
    }
}
