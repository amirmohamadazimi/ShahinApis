using System.Text.Json;

namespace ShahinApis.Data.Model;

public record BasePublicLogData
{
    public PublicLogData PublicLogData { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this); ;
    }
}

public record PublicLogData
{
    public string? UserId { get; set; }
    public string? PublicAppId { get; set; }
    public string? ServiceId { get; set; }
    public string? PublicReqId { get; set; }
}

