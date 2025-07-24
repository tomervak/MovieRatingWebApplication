using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    public Task<Movie?> GetMovieByIdAsync(Guid id);
    
    public Task<IEnumerable<Movie>> GetAllMoviesAsync();
    
    public Task<bool> CreateMovieAsync(Movie movie);
    
    public Task<bool> UpdateMovieAsync(Movie movie);
    
    public Task<bool> DeleteMovieAsync(Guid id);
    
    public Task<Movie?> GetMovieBySlugAsync(string slug);
    
    public Task<bool> ExistByMovieIdAsync(Guid id);
}