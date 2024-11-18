namespace ShahinApis.ErrorHandling;

public class RamzNegarException : System.Exception
{
    public ErrorCode Code { get; set; }
    public RamzNegarException(ErrorCode code, string message) : base(message)
    {
        Code = code;
    }
}
