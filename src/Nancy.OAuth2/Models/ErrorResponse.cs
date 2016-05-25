namespace Nancy.OAuth2.Models
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public string State { get; set; }
    }
}
