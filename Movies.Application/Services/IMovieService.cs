using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    public Task<Movie?> GetMovieByIdAsync(Guid id, Guid? userId = default);
    
    public Task<IEnumerable<Movie>> GetAllMoviesAsync(GetAllMoviesOptions options);
    
    public Task<bool> CreateMovieAsync(Movie movie);
    
    public Task<Movie?> UpdateMovieAsync(Movie movie, Guid? userId=default);
    
    public Task<bool> DeleteMovieAsync(Guid id);
    
    public Task<Movie?> GetMovieBySlugAsync(string slug,  Guid? userId = default);
    
    public Task<int> GetCountAsync(string? title, int? yearOfRelease);

}