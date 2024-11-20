namespace ShahinApis.Data.Model;

public class ChequeInquiryRes
{
    public string transactionState { get; init; }
    public long transactionTime { get; init; }
    public string uuid { get; init; }
    public RespObject respObject { get; init; }

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

}
