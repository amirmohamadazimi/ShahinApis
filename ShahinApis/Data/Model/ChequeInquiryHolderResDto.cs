namespace ShahinApis.Data.Model;

public class ChequeInquiryHolderResDto
{
    public string? state { get; init; }
    public List<Holderss>? holders { get; init; }
    public List<Signer>? signers { get; init; }
    public string? chequeType { get; init; }
    public string? chequeMedia { get; init; }
    public string? sayadId { get; init; }
    public string? serialNo { get; init; }
    public string? seriesNo { get; init; }
    public int? amount { get; init; }
    public string? dueDate { get; init; }
    public string? description { get; init; }
    public string? fromIban { get; init; }
    public string? branchCode { get; init; }
    public int? currency { get; init; }
    public string? chequeStatus { get; init; }
    public string? reason { get; init; }
    public string? guaranteeStatus { get; init; }
    public string? blockStatus { get; init; }
    public string? locked { get; init; }

    public class Signer
    {
        public string? legalStamps { get; init; }
        public string? name { get; init; }
    }
}

