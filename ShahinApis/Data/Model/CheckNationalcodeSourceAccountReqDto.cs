namespace ShahinApis.Data.Model;

public record CheckNationalcodeSourceAccountReqDto : BasePublicLogData
{ 
    public required string bank { get; init; }
    public required string nationalCode { get; init; }
    public required string sourceAccount { get; init; }
    public required string accountOwnerType { get; init; }
}
