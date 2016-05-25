using Nancy.OAuth2.Enums;

namespace Nancy.OAuth2.Models
{
    public class OAuthValidationResult
    {
        public ErrorType ErrorType { get; private set; }

        public bool IsValid
        {
            get { return ErrorType == ErrorType.None; }
        }

        public OAuthValidationResult(ErrorType errorType)
        {
            ErrorType = errorType;
        }
        
        public static implicit operator OAuthValidationResult(ErrorType errorType)
        {
            return new OAuthValidationResult(errorType);
        }
    }
}
