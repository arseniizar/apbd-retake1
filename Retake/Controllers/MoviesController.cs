using Microsoft.AspNetCore.Mvc;
using Retake.Dtos;
using Retake.Exceptions;
using Retake.Services.Interface;

namespace Retake.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)

    {
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] DateTime? releaseDate)
    {
        var movies = await _movieService.GetMoviesAsync(releaseDate);
        return Ok(movies);
    }

    [HttpPost("{movieId}/actors")]
    public async Task<IActionResult> AssignActor(int movieId, [FromBody] AssignActorRequestDto request)
    {
        try
        {
            await _movieService.AssignActorToMovieAsync(movieId, request.ActorId, request.CharacterName);
            return Created();
        }
        catch (MovieNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ActorNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}