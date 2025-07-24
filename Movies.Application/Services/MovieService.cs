using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public  MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public Task<Movie?> GetMovieByIdAsync(Guid id)
    {
        return _movieRepository.GetMovieByIdAsync(id);
    }

    public Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        return _movieRepository.GetAllMoviesAsync();
    }

    public Task<bool> CreateMovieAsync(Movie movie)
    {
        return _movieRepository.CreateMovieAsync(movie);
    }

    public async Task<Movie?> UpdateMovieAsync(Movie movie)
    {
        var movieExists = await _movieRepository.ExistByMovieIdAsync(movie.Id);
        if (!movieExists)
            return null;
        
        await _movieRepository.UpdateMovieAsync(movie); //maybe add exception catcher 
        return movie;
    }

    public Task<bool> DeleteMovieAsync(Guid id)
    {
        return _movieRepository.DeleteMovieAsync(id);
    }

    public Task<Movie?> GetMovieBySlugAsync(string slug)
    {
        return _movieRepository.GetMovieBySlugAsync(slug);
    }
}