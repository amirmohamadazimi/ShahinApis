using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShahinApis.Data.Model;
using ShahinApis.Data.Repository;
using ShahinApis.Infrastucture;

namespace ShahinApis.Service;

public class ShahinService : IShahinService
{
    public required HttpClient _httpClient { get; set; }
    private readonly ILogger<ShahinService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IShahinRepository _shahinRepository;
    public required ShahinOptions ShahinOptions { get; set; }

    public ShahinService(HttpClient httpClient, IOptions<ShahinOptions> options, ILogger<ShahinService> logger, IHttpContextAccessor httpContextAccessor, IShahinRepository shahinRepository)
    {
        ShahinOptions = options.Value;
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _shahinRepository = shahinRepository;
    }

    public async Task<OutputModel> GetAccessToken(BasePublicLogData basePublicLogData)
    {
        try
        {
            var publicRequestId = _httpContextAccessor.HttpContext.Items["RequestId"] = basePublicLogData.PublicLogData?.PublicReqId;
            var basicAuthorizationParam =
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ShahinOptions.Username}:{ShahinOptions.Password}"));

            _logger.LogInformation($"{nameof(GetAccessToken)} request sent - input is : \r\n {basePublicLogData}");
            var requestLogDto = new ShahinRequestLogDto(
                basePublicLogData.PublicLogData.PublicReqId,
                basePublicLogData.ToString(),
                basePublicLogData.PublicLogData.UserId,
                basePublicLogData.PublicLogData.PublicAppId,
                basePublicLogData.PublicLogData.ServiceId);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriToken}token?grant_type={ShahinOptions.GrantType}&bank={ShahinOptions.Bank}");
            request.Headers.Add("Authorization", "Basic VTBmcHZwdHVJVTp1Z3RsWUF5Q080");

            var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var tokenOutput =
               JsonSerializer.Deserialize<GetAccessTokenRes>(responseBodyJson,
                   ServiceHelperExtension.JsonSerializerOptions);

            return new OutputModel
            {
                Content = JsonSerializer.Serialize(tokenOutput),
                StatusCode = response.StatusCode.ToString(),
                RequestId = publicRequestId!.ToString(),
                ReqLogId = reqLogId
            };
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(basePublicLogData)}");
            throw new System.Exception(ex.Message);
        }
    }
}