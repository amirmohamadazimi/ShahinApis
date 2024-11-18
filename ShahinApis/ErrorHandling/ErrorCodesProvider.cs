namespace ShahinApis.ErrorHandling;

public class ErrorCodesProvider
{
    public int SafeReponseCode { get; set; }
    public int OutReponseCode { get; set; }
    public string? SafeReponseMessage { get; set; }
    public string? SafeReponseMesageDecription { get; set; }
    public ErrorCodesProvider errorCodesResponseResult(string input) => input switch
    {

        "400" => new ErrorCodesProvider()
        {
            SafeReponseCode = 400,
            SafeReponseMessage = "BadRequest",
            OutReponseCode = 400,
            SafeReponseMesageDecription = " .ورودی نامعتبر"
        },

        _ => new ErrorCodesProvider()
        {
            SafeReponseCode = 400,
            SafeReponseMessage = "ServiceError",
            OutReponseCode = 400,
            SafeReponseMesageDecription = " .خطای سیستم"

        }
    };
}
