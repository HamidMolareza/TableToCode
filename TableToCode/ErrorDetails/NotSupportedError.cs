using Microsoft.AspNetCore.Http;
using OnRail.ResultDetails;

namespace TableToCode.ErrorDetails;

public class NotSupportedError : ErrorDetail {
    public NotSupportedError(string title = nameof(NotSupportedError), string? message = null,
        int? statusCode = StatusCodes.Status501NotImplemented, Exception? exception = null,
        object? moreDetails = null) :
        base(title, message, statusCode, exception, moreDetails) { }
}