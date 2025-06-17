using Retake.Dtos;

namespace Retake.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<MovieDto>> GetMoviesAsync(DateTime? releaseDate);
    Task AssignActorToMovieAsync(int movieId, int actorId, string characterName);
}