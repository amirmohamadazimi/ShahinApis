using ShahinApis.ErrorHandling;

namespace ShahinApis.Data.Model;

public class CheckIbanNationalcodeRes : ErrorResult
{
    public class RespObject
    {
        public string? ibanCheckResult { get; init; }
    }

    public string? transactionState { get; init; }
    public long transactionTime { get; init; }
    public string? uuid { get; init; }
    public RespObject? respObject { get; init; }
}
