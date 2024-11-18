namespace ShahinApis.Data.Model;

public record ShahinRequestLogDto(string? publicRequestId, string? jsonRequest,
string? userId, string? publicAppId, string? serviceId);