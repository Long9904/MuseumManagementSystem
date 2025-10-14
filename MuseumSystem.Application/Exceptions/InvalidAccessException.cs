namespace MuseumSystem.Application.Exceptions
{
    public class InvalidAccessException : Exception
    {
        public InvalidAccessException(string message) : base(message) { }
    }
}
