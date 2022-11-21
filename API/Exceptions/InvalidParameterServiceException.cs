namespace API.Exceptions
{
    public class InvalidParameterServiceException : Exception
    {
        public InvalidParameterServiceException() { }
        public InvalidParameterServiceException(string? message) : base(message) { }
        public InvalidParameterServiceException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
