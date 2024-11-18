using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ShahinApis.Data.Model;



[Table("Shahin_Req")]
public sealed class ShahinReqLog : BaseEntity<string>
{
    public ShahinReqLog()
    {
        LogDateTime = DateTime.Now;
    }
    public DateTime LogDateTime { get; set; }
    public string JsonReq { get; set; }
    public string UserId { get; set; }
    public string PublicAppId { get; set; }
    public string ServiceId { get; set; }
    public string PublicReqId { get; set; }

    public ICollection<ShahinResLog> ShahinResLogs { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}