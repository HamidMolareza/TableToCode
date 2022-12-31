using Microsoft.AspNetCore.Http;
using OnRail.ResultDetails;

namespace TableToCode.ErrorDetails;

public class ConfigurationError : ErrorDetail {
    public ConfigurationError(string title = "An error occurred in retrieving configuration information.",
        string? message = null, int? statusCode = StatusCodes.Status500InternalServerError,
        Exception? exception = null, object? moreDetails = null) :
        base(title, message, statusCode, exception, moreDetails) { }
}