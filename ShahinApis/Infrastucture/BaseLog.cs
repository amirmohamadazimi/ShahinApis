using Microsoft.Extensions.Options;
using ShahinApis.ErrorHandling;
using System.Text.Json;
using ShahinApis.Data.Repository;
using ShahinApis.Data.Model;

namespace ShahinApis.Infrastucture;
public class BaseLog
{
    public IShahinRepository _repository { get; }
    private ILogger<BaseLog> _logger { get; }
    private ShahinOptions _options { get; }
    private readonly HttpClient _httpClient;

    public BaseLog(IShahinRepository repository, ILogger<BaseLog> logger, IOptions<ShahinOptions> options
            , HttpClient httpClient)
    {
        _repository = repository;
        _logger = logger;
        _options = options.Value;
        _httpClient = httpClient;
    }

    public T ApiResponseSuccessByCodeProvider<T>(string response, string statusCode, string RequestId, string publicReqId) where T : new()
    {
        var requestId = _repository.InsertShahinResponseLog(new ShahinResponseLogDto(publicReqId, Convert.ToString(response), statusCode, RequestId, statusCode));
        var responseResult = JsonSerializer.Deserialize<T>(response);
        return responseResult;
    }
    public ErrorResult ApiResponeFailByCodeProvider<T>(string response, string statusCode, string RequestId, string publicReqId) where T : new()
    {
        var codeProvider = new ErrorCodesProvider();
        codeProvider = codeProvider.ErrorCodesResponseResult(statusCode.ToString());

        _repository.InsertShahinResponseLog(new ShahinResponseLogDto
            (publicReqId, Convert.ToString(response), codeProvider?.OutReponseCode.ToString(),
                     RequestId, codeProvider.SafeReponseCode.ToString()));

        return ServiceHelperExtension.GenerateApiErrorResponse<ErrorResult>(codeProvider);
    }
}

