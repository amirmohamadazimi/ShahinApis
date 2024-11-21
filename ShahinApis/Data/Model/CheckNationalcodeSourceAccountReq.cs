namespace ShahinApis.Data.Model;

public record CheckNationalcodeSourceAccountReq
{
    public string bank { get; set; }
    public string nationalCode { get; set; }
    public string sourceAccount { get; set; }
    public string accountOwnerType { get; set; }

}