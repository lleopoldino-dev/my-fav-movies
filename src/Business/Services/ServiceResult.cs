using Business.Models;

namespace Business.Services;

public class ServiceResult<T> : IServiceResult where T : BaseEntity
{
    public List<string> Errors { get; set; } = new List<string>();
    public T? Entity { get; set; }

    public ServiceResult(string error)
    {
        Errors.Add(error);
    }

    public ServiceResult(T? entity)
    {
        Entity = entity;
    }

    public ServiceResult() { }
}

public class ServiceValidationResult : IServiceResult
{
    public List<string> ValidationErrors { get; set; } = new List<string>();

    public ServiceValidationResult(string error)
    {
        ValidationErrors.Add(error);
    }

    public ServiceValidationResult() { }
}
