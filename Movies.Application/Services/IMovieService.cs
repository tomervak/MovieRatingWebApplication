using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    public Task<Movie?> GetMovieByIdAsync(Guid id);
    
    public Task<IEnumerable<Movie>> GetAllMoviesAsync();
    
    public Task<bool> CreateMovieAsync(Movie movie);
    
    public Task<Movie?> UpdateMovieAsync(Movie movie);
    
    public Task<bool> DeleteMovieAsync(Guid id);
    
    public Task<Movie?> GetMovieBySlugAsync(string slug);
}