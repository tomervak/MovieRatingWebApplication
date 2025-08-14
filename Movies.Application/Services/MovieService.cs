using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator;
    public  MovieService(IMovieRepository movieRepository, IRatingRepository ratingRepository, IValidator<Movie> movieValidator, IValidator<GetAllMoviesOptions> optionsValidator)
    {
        _movieRepository = movieRepository;
        _ratingRepository = ratingRepository;
        _movieValidator = movieValidator;
        _optionsValidator = optionsValidator;
    }

    public Task<Movie?> GetMovieByIdAsync(Guid id,Guid? userId = default)
    {
        return _movieRepository.GetMovieByIdAsync(id, userId);
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync(GetAllMoviesOptions options)
    {
        await _optionsValidator.ValidateAndThrowAsync(options);
        return await _movieRepository.GetAllMoviesAsync(options);
    }

    public async Task<bool> CreateMovieAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        return  await _movieRepository.CreateMovieAsync(movie);
    }

    public async Task<Movie?> UpdateMovieAsync(Movie movie,  Guid? userId = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        var movieExists = await _movieRepository.ExistByMovieIdAsync(movie.Id);
        if (!movieExists)
            return null;
        
        await _movieRepository.UpdateMovieAsync(movie); //maybe add exception catcher 
        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id);
            movie.Rating = rating;
            return movie;
        }
        var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        return movie;
    }

    public Task<bool> DeleteMovieAsync(Guid id)
    {
        return _movieRepository.DeleteMovieAsync(id);
    }

    public Task<Movie?> GetMovieBySlugAsync(string slug ,  Guid? userId = default)
    {
        return _movieRepository.GetMovieBySlugAsync(slug, userId);
    }

    public Task<int> GetCountAsync(string? title, int? yearOfRelease)
    {
        return _movieRepository.GetCountAsync(title, yearOfRelease);
    }
}