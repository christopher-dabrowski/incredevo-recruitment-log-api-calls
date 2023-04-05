namespace functionApp.models.responses;

/// <summary>
/// Data abut api response
/// </summary>
public record ResponseInfoResponse(string id, bool success, string? apiName = null);
