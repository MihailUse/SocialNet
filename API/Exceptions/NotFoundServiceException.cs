namespace API.Exceptions
{
    public class NotFoundServiceException : Exception
    {
        public NotFoundServiceException() : base() { }
        public NotFoundServiceException(string message) : base(message) { }
        public NotFoundServiceException(string message, Exception inner) : base(message, inner) { }
    }
}
