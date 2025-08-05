using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IRatingRepository
{
    Task<bool> RateMovieAsync(Guid movieId,int rating, Guid userId);
    
    Task<float?> GetRatingAsync(Guid id);
    
    Task<(float? Rating, int? UserRating )> GetRatingAsync(Guid movieId, Guid userId);
    
    Task<bool> DeleteRatingAsync(Guid movieId,  Guid userId);
    
    Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId);
}