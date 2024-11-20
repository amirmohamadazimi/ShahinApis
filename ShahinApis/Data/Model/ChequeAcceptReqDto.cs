namespace ShahinApis.Data.Model;

public record ChequeAcceptReqDto : BasePublicLogData
{
    public required string bank { get; init; }
    public required string sayadId { get; init; }
    public Acceptor acceptor { get; init; }
    public AcceptorAgent acceptorAgent { get; init; }
    public required string accepts { get; init; }
    public required string acceptDescription { get; init; }

    public class Acceptor
    {
        public required string idCode { get; init; }
        public required string idTypes { get; init; }
    }

    public class AcceptorAgent
    {
        public required string idCode { get; init; }
        public required string idTypes { get; init; }
    }
}
