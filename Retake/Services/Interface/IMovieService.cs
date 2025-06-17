using Retake.Dtos;

namespace Retake.Services.Interface;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetMoviesAsync(DateTime? releaseDate);
    Task AssignActorToMovieAsync(int movieId, int actorId, string characterName);
}