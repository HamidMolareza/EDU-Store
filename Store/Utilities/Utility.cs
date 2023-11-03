using Microsoft.AspNetCore.Mvc;

namespace Store.Utilities;

public static class Utility {
    public static string? ConvertGuidToViewModel(string guid) =>
        Guid.TryParse(guid, out var resultGuid)
            ? resultGuid.ToString("N")[..8]
            : null;

    public static string? SafeReturnUrl(string? returnUrl, IUrlHelper urlHelper) {
        return urlHelper.IsLocalUrl(returnUrl) ? returnUrl : null;
    }

    public static IActionResult RedirectToReturnUrl(string? returnUrl, IActionResult defaultRedirect) {
        return returnUrl is null ? defaultRedirect : new LocalRedirectResult(returnUrl);
    }

    /// <summary>
    /// Convert UTC time to user's local time zone
    /// </summary>
    /// <param name="utcDateTime"></param>
    /// <param name="userTimeZone"></param>
    /// <returns></returns>
    public static DateTime ConvertToLocalTime(this DateTime utcDateTime, string? userTimeZone) {
        if (userTimeZone is null)
            return utcDateTime.ToLocalTime();

        var userTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
        var localTime        = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, userTimeZoneInfo);
        return localTime;
    }
}