namespace Source.Middleware
{
    public class ExceptionError
    {
        public string Message { get; set; }

        public ExceptionError(string message)
        {
            Message = message;
        }
    }
}
