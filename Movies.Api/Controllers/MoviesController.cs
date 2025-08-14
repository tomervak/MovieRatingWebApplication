using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {
    
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }
    
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetMovie)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id) 
        ? await _movieService.GetMovieByIdAsync(id,userId)
        : await _movieService.GetMovieBySlugAsync(idOrSlug);
        if (movie is null)
        {
            return NotFound();
        }
        return Ok(movie.MapToMovieResponse());
    }
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAllMovies)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMoviesAsync(
        [FromQuery] GetAllMoviesRequest? request )
    {
        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions().WithUser(userId);
        var movies = await _movieService.GetAllMoviesAsync(options);
        var moviesCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease);
        var moviesResponse = movies.MapToMoviesResponses(request.Page, request.PageSize, moviesCount);
        return Ok(moviesResponse);
    }
    
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.CreateMovie)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFaliureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request)
    {
        var newMovie = request.MapToMovie();
        await _movieService.CreateMovieAsync(newMovie);
        return Created($"/{ApiEndpoints.Movies.CreateMovie}/{newMovie.Id}",newMovie.MapToMovieResponse());
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.UpdateMovie)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFaliureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMovieAsync([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateMovieAsync(movie,userId);
        if (updatedMovie is null)
        {
            return NotFound();
        }
        var response = updatedMovie.MapToMovieResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.DeleteMovie)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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