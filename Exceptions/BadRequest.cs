namespace AcesCore
{
    public class BadRequest : Exception
    {
        public BadRequest(string? message) : base(message)
        {
        }
    }
}