﻿using Business.Infrastructure;
using Business.Models;

namespace Business.Services.MovieServices;

public class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;

    public MovieService(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    public async Task<IServiceResult> CreateMovieAsync(Movie movie, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(movie, cancellationToken);

        if (validationResult.HasErrors())
        {
            return validationResult;
        }

        return await GetMovieCreationResultAsync(movie, cancellationToken);
    }

    private async Task<IServiceResult> ValidateAsync(Movie movie, CancellationToken cancellationToken)
    {
        var validation = new ServiceValidationResult();
        var movieSameTitle = await _moviesRepository.GetByTitleAsync(movie.Title, cancellationToken);

        if (movieSameTitle != null)
        {
            validation.Errors.Add("A movie with same title already exists");
        }

        return validation;
    }

    private async Task<IServiceResult> GetMovieCreationResultAsync(Movie movie, CancellationToken cancellationToken)
    {
        var createdMovie = await _moviesRepository.CreateAsync(movie, cancellationToken);

        if (createdMovie == null)
        {
            return new ServiceResult<Movie>("Failed creating movie");
        }

        return new ServiceResult<Movie>(createdMovie);
    }
}
