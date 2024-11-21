namespace ShahinApis.Data.Model;

public record CheckIbanNationalcodeReq
{
    public required string nationalCode { get; init; }
    public required string iban { get; init; }
    public required string birthDate { get; init; }
}