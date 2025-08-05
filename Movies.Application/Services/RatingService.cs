using System.ComponentModel.DataAnnotations;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;

    public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
        _movieRepository = movieRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException("Rating must be between 0 and 5.");
        }
        
        var movieExists = await _movieRepository.ExistByMovieIdAsync(movieId);
        if (!movieExists)
            return false;
        
        return await _ratingRepository.RateMovieAsync(movieId, rating, userId);
        
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId)
    {  
        return await _ratingRepository.DeleteRatingAsync(movieId, userId);
    }

    public Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId)
    {
        return _ratingRepository.GetUserRatingsAsync(userId);
    }
}