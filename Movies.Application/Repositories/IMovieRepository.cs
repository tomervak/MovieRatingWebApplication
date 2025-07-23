using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    public Task<Movie?> GetMovieById(int id);
    
    public Task<IEnumerable<Movie>> GetAllMovies();
    
    public Task<bool> CreateMovie(Movie movie);
    
    public Task<bool> UpdateMovie(Movie movie);
    
    public Task<bool> DeleteMovie(int id);
    
    public Task<Movie?> GetMovieBySlug(string slug);
    
    public Task<bool> ExistByMovieId(int id);
}