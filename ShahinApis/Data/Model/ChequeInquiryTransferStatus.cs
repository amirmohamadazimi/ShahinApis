using ShahinApis.ErrorHandling;

namespace ShahinApis.Data.Model;

public class ChequeInquiryTransferStatus : ErrorResult
{
    public required string transactionState { get; init; }
    public required long transactionTime { get; init; }
    public required string uuid { get; init; }
    public required RespObject respObject { get; init; }

    public class Holderss
    {
        public required string name { get; init; }
        public required string idCode { get; init; }
        public required string idTypes { get; init; }
        public required string acceptTransfer { get; init; }
        public required string lastActionDate { get; init; }
    }

    public class RespObject
    {
        public string? state { get; init; }
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
        public List<Holderss>? holderss { get; init; }
        public string? giveBack { get; init; }
        public string? curTransferDescripion { get; init; }
        public string? curTransferReason { get; init; }
    }
}
