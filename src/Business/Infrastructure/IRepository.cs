using Business.Models;

namespace Business.Infrastructure;

public interface IRepository<T> where T : BaseModel
{
    public Task<List<T>> ListAllAsync(CancellationToken cancellationToken);
    public Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
    public Task<T?> CreateAsync(T entity, CancellationToken cancellationToken);
    public Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);
    public Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken);
}
