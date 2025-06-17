namespace Retake.Dtos;

public class MovieDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string AgeRating { get; set; }
    public List<ActorDto> Actors { get; set; } = new List<ActorDto>();
}

public class ActorDto
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string CharacterName { get; set; }
}

public class AssignActorRequestDto
{
    public int ActorId { get; set; }
    public string CharacterName { get; set; }
}


