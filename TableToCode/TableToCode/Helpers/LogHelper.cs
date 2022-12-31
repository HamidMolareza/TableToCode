using System.Text;
using OnRail.ResultDetails;

namespace TableToCode.Helpers;

public static class LogHelper {
    /// <summary>
    /// Gets StatusCode, Title, Message from ErrorDetail
    /// </summary>
    public static string MinimumLog(ErrorDetail? errorDetail) {
        if (errorDetail is null)
            return "An error occurred. That's all we know!";

        var sb = new StringBuilder();
        if (errorDetail.StatusCode is not null)
            sb.Append($"{errorDetail.StatusCode} - ");
        sb.AppendLine($"{errorDetail.Title}")
            .AppendLine(errorDetail.Message);

        return sb.ToString();
    }

    /// <summary>
    /// Gets StatusCode, Title, Message, MoreDetails and Exception from ErrorDetail
    /// </summary>
    public static string Log(ErrorDetail? errorDetail) {
        var sb = new StringBuilder();
        sb.AppendLine(MinimumLog(errorDetail));

        if (errorDetail is null)
            return sb.ToString();

        if (errorDetail.MoreDetails is not null && errorDetail.MoreDetails.Any()) {
            sb.AppendLine($"{errorDetail.MoreDetails.Count} {nameof(errorDetail.MoreDetails)}:");
            for (var i = 0; i < errorDetail.MoreDetails.Count; i++)
                sb.AppendLine($"{i + 1}) {errorDetail.MoreDetails[i].ToString()}");

            sb.AppendLine( /* Empty line */);
        }

        if (errorDetail.Exception is not null) {
            sb.AppendLine($"{nameof(errorDetail.Exception)}: ");
            sb.AppendLine(errorDetail.Exception.ToString());
            sb.AppendLine( /* Empty line */);
        }

        return sb.ToString();
    }
}