using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
public class MoviesController : ControllerBase {
    
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }
    
    
    [HttpGet(ApiEndpoints.Movies.GetMovie)]
    public async Task<IActionResult> GetMovieByIdAsync([FromRoute] Guid id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }
        return Ok(movie.MapToMovieResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAllMovies)]
    public async Task<IActionResult> GetAllMoviesAsync()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies.MapToMoviesResponses());
    }

    [HttpPost(ApiEndpoints.Movies.CreateMovie)]
    public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request)
    {
        var newMovie = request.MapToMovie();
        await _movieService.CreateMovieAsync(newMovie);
        return Created($"/{ApiEndpoints.Movies.CreateMovie}/{newMovie.Id}",newMovie.MapToMovieResponse());
    }

    [HttpPut(ApiEndpoints.Movies.UpdateMovie)]
    public async Task<IActionResult> UpdateMovieAsync([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateMovieAsync(movie);
        if (updatedMovie is null)
        {
            return NotFound();
        }
        var response = updatedMovie.MapToMovieResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.DeleteMovie)]
    public async Task<IActionResult> DeleteMovieAsync([FromRoute] Guid id)
    {
        var deleted = await _movieService.DeleteMovieAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();

    }
}