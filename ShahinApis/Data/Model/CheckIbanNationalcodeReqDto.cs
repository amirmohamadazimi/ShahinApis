namespace ShahinApis.Data.Model;

public record CheckIbanNationalcodeReqDto : BasePublicLogData
{
    public required string nationalCode { get; init; }
    public required string iban { get; init; }
    public required string birthDate { get; init; }
}
