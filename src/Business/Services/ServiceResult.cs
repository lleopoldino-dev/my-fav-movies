using Business.Models;

namespace Business.Services;

public class ServiceResult<T> : BaseServiceResult, IServiceResult where T : BaseEntity
{
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

public class ServiceValidationResult : BaseServiceResult, IServiceResult
{
    public ServiceValidationResult(string error)
    {
        Errors.Add(error);
    }

    public ServiceValidationResult() { }
}

public abstract class BaseServiceResult
{
    public List<string> Errors { get; set; } = new List<string>();

    public bool HasErrors()
    {
        return Errors.Count > 0;
    }
}