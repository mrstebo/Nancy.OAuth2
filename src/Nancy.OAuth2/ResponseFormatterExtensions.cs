using Nancy.OAuth2.Models;

namespace Nancy.OAuth2
{
    public static class ResponseFormatterExtensions
    {
        public static Response AsErrorResponse(this IResponseFormatter source, ErrorResponse error, string redirectUri = null)
        {
            return string.IsNullOrEmpty(redirectUri) 
                ? source.AsJson(error, HttpStatusCode.BadRequest) 
                : source.AsRedirect(string.Concat(redirectUri, error.AsQueryString()));
        }
    }
}
