using Retake.Dtos;
using Retake.Repositories;
using Retake.Services.Interface;

namespace Retake.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }
    
    public async Task<IEnumerable<MovieDto>> GetMoviesAsync(DateTime? releaseDate)
    {
        return await _movieRepository.GetMoviesAsync(releaseDate);
    }

    public async Task AssignActorToMovieAsync(int movieId, int actorId, string characterName)
    {
        await _movieRepository.AssignActorToMovieAsync(movieId, actorId, characterName);
    }
}