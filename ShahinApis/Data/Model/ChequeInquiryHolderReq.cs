namespace ShahinApis.Data.Model;

public record ChequeInquiryHolderReq
{
    public required string bank { get; init; }
    public required string sayadId { get; init; }
    public required string idCode { get; init; }
    public required string idTypes { get; init; }
}
