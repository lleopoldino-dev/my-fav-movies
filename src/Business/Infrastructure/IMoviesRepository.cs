using Business.Models;

namespace Business.Infrastructure;

public interface IMoviesRepository
{
    Task<List<Movie>> ListAllAsync(CancellationToken cancellationToken);
    Task<Movie?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Movie?> CreateAsync(Movie movie, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Movie movie, CancellationToken cancellationToken);
    Task<Movie?> GetByTitleAsync(string title, CancellationToken cancellationToken); 
}
