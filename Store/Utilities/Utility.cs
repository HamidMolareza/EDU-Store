namespace Store.Utilities;

public static class Utility {
    public static string? ConvertGuidToViewModel(string guid) =>
        Guid.TryParse(guid, out var resultGuid)
            ? resultGuid.ToString("N")[..8]
            : null;
}