namespace ShahinApis.Data.Model;

public class ChequeInquiryTransferStatusDto
{
    public string? state { get; set; }
    public string? chequeType { get; set; }
    public string? chequeMedia { get; set; }
    public string? sayadId { get; set; }
    public string? serialNo { get; set; }
    public string? seriesNo { get; set; }
    public int amount { get; set; }
    public string? dueDate { get; set; }
    public string? description { get; set; }
    public string? fromIban { get; set; }
    public string? branchCode { get; set; }
    public int currency { get; set; }
    public string? chequeStatus { get; set; }
    public string? reason { get; set; }
    public string? guaranteeStatus { get; set; }
    public string? blockStatus { get; set; }
    public string? locked { get; set; }
    public List<Holderss>? holderss { get; set; }
    public string? giveBack { get; set; }
    public string? curTransferDescripion { get; set; }
    public string? curTransferReason { get; set; }
    public class Holderss
    {
        public required string name { get; init; }
        public required string idCode { get; init; }
        public required string idTypes { get; init; }
        public required string acceptTransfer { get; init; }
        public required string lastActionDate { get; init; }
    }
}