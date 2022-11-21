namespace API.Models
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ErrorModel(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
