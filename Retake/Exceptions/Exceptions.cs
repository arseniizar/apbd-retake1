namespace Retake.Exceptions;

public class MovieNotFoundException : Exception
{
    public MovieNotFoundException(string message) : base(message)
    {
    }
}

public class ActorNotFoundException : Exception
{
    public ActorNotFoundException(string message) : base(message)
    {
    }
}