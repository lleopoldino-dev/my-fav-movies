using Business.Models;

namespace Business.Infrastructure;

public interface IMoviesRepository : IRepository<Movie>
{
    Task<Movie?> GetByTitleAsync(string title, CancellationToken cancellationToken); 
}
