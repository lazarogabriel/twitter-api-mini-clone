namespace twitter.api.web.Models.Responses
{
    public class ErrorResponse
    {
        public ErrorResponse(string type, string message, string referenceValue = null)
        {
            Type = type;
            Message = message;
            ReferenceValue = referenceValue;
        }

        public string Type { get; set; }

        public string Message { get; set; }

        public string ReferenceValue { get; set; }
    }
}
