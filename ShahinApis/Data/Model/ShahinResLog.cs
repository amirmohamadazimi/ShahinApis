using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ShahinApis.Data.Model;

[Table("Shahin_Res")]
public sealed class ShahinResLog : BaseEntity<string>
{
    public string? ResCode { get; set; }
    public string? HTTPStatusCode { get; set; }
    public string? JsonRes { get; set; }
    public string? PublicReqId { get; set; }
    public string? ReqLogId { get; set; }
    public ShahinReqLog ReqLog { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
