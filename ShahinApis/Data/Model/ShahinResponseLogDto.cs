namespace ShahinApis.Data.Model;

public record ShahinResponseLogDto(string publicRequestId, string jsonResponse,
       string shahinHttpResponseCode, string shahinRequestId, string shahinResCode);