using System;
using System.Collections.Generic;
using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;

namespace Nancy.OAuth2
{
    public interface IErrorResponseBuilder
    {
        ErrorResponse Build(ErrorType errorType, string state);
    }

    public class DefaultErrorResponseBuilder : IErrorResponseBuilder
    {
        private readonly IDictionary<ErrorType, Tuple<string, string>> _errorDescriptions;

        public DefaultErrorResponseBuilder()
        {
            _errorDescriptions = new Dictionary<ErrorType, Tuple<string, string>>
            {
                {
                    ErrorType.AccessDenied,
                    Tuple.Create("access_denied", "The user denied your request.")
                },
                {
                    ErrorType.InvalidClient,
                    Tuple.Create("invalid_client", "Client identifier is required.")
                },
                {
                    ErrorType.InvalidGrant,
                    Tuple.Create("invalid_grant", "Unsupported grant type.")
                },
                {
                    ErrorType.InvalidRequest,
                    Tuple.Create("invalid_request",
                        "The request is missing a required parameter, includes an unsupported parameter or parameter value, or is otherwise malformed.")
                },
                {
                    ErrorType.InvalidScope,
                    Tuple.Create("invalid_scope", "The requested scope is invalid, unknown, or malformed.")
                },
                {
                    ErrorType.ServerError,
                    Tuple.Create("server_error",
                        "The authorization server encountered an unexpected condition which prevented it from fulfilling the request.")
                },
                {
                    ErrorType.TemporarilyUnavailable,
                    Tuple.Create("temporarily_unavailable",
                        "The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.")
                },
                {
                    ErrorType.UnauthorizedClient,
                    Tuple.Create("unauthorized_client",
                        "The client is not authorized to request an authorization code using this method.")
                },
                {
                    ErrorType.UnsupportedGrantType, 
                    Tuple.Create("invalid_grant", "Unsupported grant type.")
                },
                {
                    ErrorType.UnsupportedResponseType,
                    Tuple.Create("unsupported_response_type",
                        "The authorization server does not support obtaining an authorization code using this method.")
                }
            };
        }

        public ErrorResponse Build(ErrorType errorType, string state)
        {
            var descriptions = _errorDescriptions[errorType];

            return new ErrorResponse
            {
                Error = descriptions.Item1,
                ErrorDescription = descriptions.Item2,
                State = state
            };
        }
    }
}
