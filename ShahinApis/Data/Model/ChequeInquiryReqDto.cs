namespace ShahinApis.Data.Model;

public record ChequeInquiryReqDto : BasePublicLogData
{
    public required string chequeSerial { get; init; }
}

