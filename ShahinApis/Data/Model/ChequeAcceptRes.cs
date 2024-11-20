using ShahinApis.ErrorHandling;

namespace ShahinApis.Data.Model;

public class ChequeAcceptRes : ErrorResult
{ 
    public string transactionState { get; init; }
    public long transactionTime { get; init; }
    public string uuid { get; init; }
    public RespObject respObject { get; init; }
    public class RespObject
    {
        public string state { get; init; }
    }
}
