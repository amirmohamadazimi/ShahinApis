using System.Text.Json.Serialization;

namespace ShahinApis.Data.Model;

public class RespObject
{
    public string trackId { get; init; }
    public string chequeSerial { get; init; }
    public string bankName { get; init; }
    public string branchCode { get; init; }
    public string name { get; init; }
    public string lastUpdate { get; init; }
    public string status { get; init; }
    public string iban { get; init; }
}
