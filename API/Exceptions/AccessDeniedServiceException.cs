using System.Runtime.Serialization;

namespace API.Exceptions
{
    public class AccessDeniedServiceException : Exception
    {
        public AccessDeniedServiceException()
        {
        }

        public AccessDeniedServiceException(string? message) : base(message)
        {
        }

        public AccessDeniedServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AccessDeniedServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
