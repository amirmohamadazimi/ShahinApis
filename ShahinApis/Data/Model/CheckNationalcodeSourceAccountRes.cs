using ShahinApis.ErrorHandling;

namespace ShahinApis.Data.Model;

public class CheckNationalcodeSourceAccountRes : ErrorResult
{
    public class RespObject
    {
        public required string checkResult { get; init; }
    }

    public required string transactionState { get; init; }
    public required long transactionTime { get; init; }
    public required string uuid { get; init; }
    public RespObject respObject { get; init; }
}