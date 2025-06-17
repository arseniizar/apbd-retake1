using System.Text;
using Microsoft.Data.SqlClient;
using Retake.Dtos;
using Retake.Exceptions;

namespace Retake.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IConfiguration _configuration;

    public MovieRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesAsync(DateTime? releaseDate)
    {
        var movies = new Dictionary<int, MovieDto>();
        
        var connectionString = _configuration.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var queryB = new StringBuilder(@"
            SELECT 
                m.IdMovie, m.Name AS MovieName, m.ReleaseDate,
                ar.Name AS AgeRatingName,
                a.Name AS ActorFirstName, a.Surname AS ActorSurname,
                am.CharacterName
            FROM dbo.Movie m
            JOIN dbo.AgeRating ar ON m.IdAgeRating = ar.IdRating
            JOIN dbo.Actor_Movie am ON m.IdMovie = am.IdMovie
            JOIN dbo.Actor a ON am.IdActor = a.IdActor");

        if (releaseDate.HasValue)
        {
            queryB.Append(" WHERE m.ReleaseDate = @ReleaseDate");
        }

        queryB.Append(" ORDER BY m.ReleaseDate DESC, m.IdMovie;");

        await using var command = new SqlCommand(queryB.ToString(), connection);

        if (releaseDate.HasValue)
        {
            command.Parameters.AddWithValue("@ReleaseDate", releaseDate.Value);
        }

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var movieId = reader.GetInt32(reader.GetOrdinal("IdMovie"));
            if (!movies.ContainsKey(movieId))
            {
                movies[movieId] = new MovieDto
                {
                    Id = movieId,
                    Name = reader.GetString(reader.GetOrdinal("MovieName")),
                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                    AgeRating = reader.GetString(reader.GetOrdinal("AgeRatingName"))
                };
            }

            movies[movieId].Actors.Add(new ActorDto
            {
                FirstName = reader.GetString(reader.GetOrdinal("ActorFirstName")),
                Surname = reader.GetString(reader.GetOrdinal("ActorSurname")),
                CharacterName = reader.GetString(reader.GetOrdinal("CharacterName"))
            });
        }

        return movies.Values;
    }

   public async Task AssignActorToMovieAsync(int movieId, int actorId, string characterName)
    {
        var connectionString = _configuration.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var movieCheckQuery = "SELECT 1 FROM Movie WHERE IdMovie = @IdMovie";
            await using (var movieCmd = new SqlCommand(movieCheckQuery, connection, (SqlTransaction)transaction))
            {
                movieCmd.Parameters.AddWithValue("@IdMovie", movieId);
                var movieExists = await movieCmd.ExecuteScalarAsync();
                if (movieExists == null)
                {
                    throw new MovieNotFoundException($"Movie with ID {movieId} not found.");
                }
            }

            var actorCheckQuery = "SELECT 1 FROM Actor WHERE IdActor = @IdActor";
            await using (var actorCmd = new SqlCommand(actorCheckQuery, connection, (SqlTransaction)transaction))
            {
                actorCmd.Parameters.AddWithValue("@IdActor", actorId);
                var actorExists = await actorCmd.ExecuteScalarAsync();
                if (actorExists == null)
                {
                    throw new ActorNotFoundException($"Actor with ID {actorId} not found.");
                }
            }
           
            var insertQuery = "INSERT INTO Actor_Movie (IdMovie, IdActor, CharacterName) VALUES (@IdMovie, @IdActor, @CharacterName);";
            await using (var insertCmd = new SqlCommand(insertQuery, connection, (SqlTransaction)transaction))
            {
                insertCmd.Parameters.AddWithValue("@IdMovie", movieId);
                insertCmd.Parameters.AddWithValue("@IdActor", actorId);
                insertCmd.Parameters.AddWithValue("@CharacterName", characterName);
                await insertCmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }
}