using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    public Task<Movie?> GetMovieByIdAsync(Guid id,  Guid? userId = default);
    
    public Task<IEnumerable<Movie>> GetAllMoviesAsync(GetAllMoviesOptions options);
    
    public Task<bool> CreateMovieAsync(Movie movie);
    
    public Task<bool> UpdateMovieAsync(Movie movie);
    
    public Task<bool> DeleteMovieAsync(Guid id);
    
    public Task<Movie?> GetMovieBySlugAsync(string slug,   Guid? userId = default);
    
    public Task<bool> ExistByMovieIdAsync(Guid id);

    public Task<int> GetCountAsync(string? title, int? yearOfRelease);
}