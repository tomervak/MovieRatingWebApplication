using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId);
    
    Task<bool> DeleteRatingAsync(Guid movieId,  Guid userId);
    
    Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId);

}