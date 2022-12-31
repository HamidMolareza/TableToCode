using OnRail.ResultDetails;

namespace TableToCode.ErrorDetails;

public class ParseError : ErrorDetail {
    public ParseError(string title = "An error occurred in parsing the information.",
        string? message = null, int? statusCode = null, Exception? exception = null, object? moreDetails = null) : base(
        title, message, statusCode, exception, moreDetails) { }
}