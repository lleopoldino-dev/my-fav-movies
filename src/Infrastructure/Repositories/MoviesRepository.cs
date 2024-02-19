using Business.Infrastructure;
using Business.Models;
using Infrastructure.Mappers;
using Infrastructure.Adapters;

namespace Infrastructure.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private readonly IDbConnectionAdapter _dbConnectionAdapter;

    public MoviesRepository(IDbConnectionAdapter dbConnectionAdapter)
    {
        _dbConnectionAdapter = dbConnectionAdapter;
    }

    public async Task<Movie?> CreateAsync(Movie movie, CancellationToken cancellationToken)
    {
        string sql = InsertMovieQuery(movie);

        int rows = await _dbConnectionAdapter.ExecuteNonQueryAsync(sql, cancellationToken);

        if (rows <= 0)
        {
            return null;
        }

        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        var movieFounded = await GetAsync(movie.Id, cancellationToken);
        if (movieFounded == null)
        {
            return false;
        }

        movieFounded.Title = movie.Title;
        movieFounded.Category = movie.Category;
        movieFounded.ReleaseDate = movie.ReleaseDate;

        var sql = UpdateMovieQuery(movieFounded);

        int rows = await _dbConnectionAdapter.ExecuteNonQueryAsync(sql, cancellationToken);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Movie movie, CancellationToken cancellationToken)
    {
        string sql = DeleteByIdQuery(movie);

        int rows = await _dbConnectionAdapter.ExecuteNonQueryAsync(sql, cancellationToken);

        return rows > 0;
    }

    public Task<Movie?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbConnectionAdapter.ExecuteReaderSingleAsync<Movie?>(GetByIdQuery(id), MovieMapper.MapFromDbResult, cancellationToken);
    }

    public Task<List<Movie>> ListAllAsync(CancellationToken cancellationToken)
    {
        return _dbConnectionAdapter.ExecuteReaderAsync<Movie>(ListAllQuery(), MovieMapper.MapFromDbResult, cancellationToken);
    }

    public Task<Movie?> GetByTitleAsync(string title, CancellationToken cancellationToken)
    {
        return _dbConnectionAdapter.ExecuteReaderSingleAsync<Movie?>(GetByTitleQuery(title), MovieMapper.MapFromDbResult, cancellationToken);
    }

    private static string InsertMovieQuery(Movie movie)
    {
        return $"INSERT INTO movies (id, title, category, releasedate) VALUES('{movie.Id}', '{movie.Title}', '{movie.Category}', '{movie.ReleaseDate}')";
    }

    private static string UpdateMovieQuery(Movie movieFounded)
    {
        return $"UPDATE movies SET title = '{movieFounded.Title}', category = '{movieFounded.Category}', releasedate = '{movieFounded.ReleaseDate}' WHERE id = '{movieFounded.Id}'";
    }

    private static string DeleteByIdQuery(Movie movie)
    {
        return $"DELETE FROM movies WHERE id = '{movie.Id}'";
    }

    private static string GetByIdQuery(Guid id)
    {
        return $"SELECT id, title, category, releaseDate FROM movies WHERE id = '{id}' limit 1";
    }

    private static string ListAllQuery()
    {
        return $"SELECT id, title, category, releaseDate FROM movies limit 100";
    }

    private static string GetByTitleQuery(string title)
    {
        return $"SELECT id, title, category, releaseDate FROM movies WHERE title = '{title}' limit 1";
    }

}
