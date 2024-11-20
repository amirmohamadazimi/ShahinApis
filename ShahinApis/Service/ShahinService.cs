using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShahinApis.Data.Model;
using ShahinApis.Data.Repository;
using ShahinApis.ErrorHandling;
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
            {
                var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = basePublicLogData.PublicLogData?.PublicReqId;
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
                request.Headers.Add("Authorization", $"Basic {basicAuthorizationParam}");

                var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var tokenOutput =
                   JsonSerializer.Deserialize<GetAccessTokenRes>(responseBodyJson,
                       ServiceHelperExtension.JsonSerializerOptions);

                await _shahinRepository.AddOrUpdateTokenAsync(tokenOutput.access_token);

                return new OutputModel
                {
                    Content = JsonSerializer.Serialize(tokenOutput),
                    StatusCode = response.StatusCode.ToString(),
                    RequestId = publicRequestId!.ToString(),
                    ReqLogId = reqLogId
                };
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(basePublicLogData)}");
            throw new Exception(ex.Message);
        }
    }

    public async Task<OutputModel> PostChequeInquiry(ChequeInquiryReqDto clientRequest)
    {
        try
        {
            var timestampHeader = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            _logger.LogInformation($"{nameof(PostChequeInquiry)} request sent - input is : \r\n {clientRequest.PublicLogData}");

            var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = clientRequest.PublicLogData?.PublicReqId;
            var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriService}inquiry/cheque-inquiry");

            var requestLogDto = new ShahinRequestLogDto(
                  clientRequest.PublicLogData.PublicReqId,
                  clientRequest.ToString(),
                  clientRequest.PublicLogData.UserId,
                  clientRequest.PublicLogData.PublicAppId,
                  clientRequest.PublicLogData.ServiceId);
            var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);

            request.Headers.Add("X-Obh-signature", $"OBH1-HMAC-SHA256;SignedHeaders=X-Obh-uuid,X-Obh-timestamp;Signature={ShahinOptions.RequestSignature}");
            request.Headers.Add("X-Obh-uuid", $"{Guid.NewGuid()}");

            var accessToken = await _shahinRepository.FindShahinAccessToken();

            request.Headers.Add("X-Obh-timestamp", $"{timestampHeader}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            request.Content =
                  new StringContent(
                      JsonSerializer.Serialize(clientRequest.chequeSerial, ServiceHelperExtension.JsonSerializerOptions),
              Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var serilizedResponse = JsonSerializer.Deserialize<ChequeInquiryRes>(responseBodyJson,
                ServiceHelperExtension.JsonSerializerOptions);

            return new OutputModel
            {
                Content = JsonSerializer.Serialize(serilizedResponse.respObject),
                StatusCode = response.StatusCode.ToString(),
                RequestId = publicRequestId!.ToString(),
                ReqLogId = reqLogId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(clientRequest.PublicLogData)}");
            throw new Exception(ex.Message);
        }
    }

    public async Task<OutputModel> PostChequeAccept(ChequeAcceptReqDto clientRequest)
    {
        try
        {
            var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = clientRequest.PublicLogData?.PublicReqId;
            var timestampHeader = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            _logger.LogInformation($"{nameof(PostChequeAccept)} request sent - input is : \r\n {clientRequest.PublicLogData}");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriService}aisp/cheque-accept");

            var requestLogDto = new ShahinRequestLogDto(
                  clientRequest.PublicLogData.PublicReqId,
                  clientRequest.ToString(),
                  clientRequest.PublicLogData.UserId,
                  clientRequest.PublicLogData.PublicAppId,
                  clientRequest.PublicLogData.ServiceId);
            var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);


            request.Headers.Add("X-Obh-signature", $"OBH1-HMAC-SHA256;SignedHeaders=X-Obh-uuid,X-Obh-timestamp;Signature={ShahinOptions.RequestSignature}");
            request.Headers.Add("X-Obh-uuid", $"{Guid.NewGuid()}");
            var accessToken = await _shahinRepository.FindShahinAccessToken();

            request.Headers.Add("X-Obh-timestamp", $"{timestampHeader}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var mainRequest = new ChequeAcceptRequest
            {
                sayadId = clientRequest.sayadId,
                acceptDescription = clientRequest.acceptDescription,
                accepts = clientRequest.accepts,
                bank = clientRequest.bank,
                acceptor = new ChequeAcceptRequest.Acceptor { idCode = clientRequest.acceptor.idCode, idTypes = clientRequest.acceptor.idTypes },
                acceptorAgent = new ChequeAcceptRequest.AcceptorAgent { idCode = clientRequest.acceptorAgent.idCode, idTypes = clientRequest.acceptorAgent.idTypes },
            };

            request.Content = new StringContent(JsonSerializer.Serialize(mainRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var serilizedResponse = JsonSerializer.Deserialize<ChequeAcceptRes>(responseBodyJson,
                ServiceHelperExtension.JsonSerializerOptions);

            return new OutputModel
            {
                Content = JsonSerializer.Serialize(serilizedResponse.respObject),
                StatusCode = response.StatusCode.ToString(),
                RequestId = publicRequestId!.ToString(),
                ReqLogId = reqLogId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(clientRequest.PublicLogData)}");
            throw new Exception(ex.Message);
        }
    }

    public async Task<OutputModel> PostChequeInquiryTransfer(ChequeInquiryTransferReqDto clientRequest)
    {
        try
        {
            var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = clientRequest.PublicLogData?.PublicReqId;
            var timestampHeader = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            _logger.LogInformation($"{nameof(PostChequeInquiryTransfer)} request sent - input is : \r\n {clientRequest.PublicLogData}");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriService}aisp/cheque-inquiry-transfer");

            var requestLogDto = new ShahinRequestLogDto(
                  clientRequest.PublicLogData.PublicReqId,
                  clientRequest.ToString(),
                  clientRequest.PublicLogData.UserId,
                  clientRequest.PublicLogData.PublicAppId,
                  clientRequest.PublicLogData.ServiceId);
            var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);


            request.Headers.Add("X-Obh-signature", $"OBH1-HMAC-SHA256;SignedHeaders=X-Obh-uuid,X-Obh-timestamp;Signature={ShahinOptions.RequestSignature}");
            request.Headers.Add("X-Obh-uuid", $"{Guid.NewGuid()}");
            var accessToken = await _shahinRepository.FindShahinAccessToken();

            request.Headers.Add("X-Obh-timestamp", $"{timestampHeader}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var mainRequest = new ChequeInquiryTransferReq
            {
                bank = clientRequest.bank,
                idCode = clientRequest.idCode,
                idTypes = clientRequest.idTypes,
                sayadId = clientRequest.sayadId,
            };

            request.Content = new StringContent(JsonSerializer.Serialize(mainRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var serilizedResponse = JsonSerializer.Deserialize<ChequeInquiryTransferStatus>(responseBodyJson,
                ServiceHelperExtension.JsonSerializerOptions);

            return new OutputModel
            {
                Content = JsonSerializer.Serialize(serilizedResponse.respObject),
                StatusCode = response.StatusCode.ToString(),
                RequestId = publicRequestId!.ToString(),
                ReqLogId = reqLogId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(clientRequest.PublicLogData)}");
            throw new Exception(ex.Message);
        }
    }

    public async Task<OutputModel> PostChequeInquiryHolder(ChequeInquiryHolderReqDto clientRequest)
    {
        try
        {
            var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = clientRequest.PublicLogData?.PublicReqId;
            var timestampHeader = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            _logger.LogInformation($"{nameof(PostChequeInquiryHolder)} request sent - input is : \r\n {clientRequest.PublicLogData}");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriService}aisp/cheque-inquiry-holder");

            var requestLogDto = new ShahinRequestLogDto(
                  clientRequest.PublicLogData.PublicReqId,
                  clientRequest.ToString(),
                  clientRequest.PublicLogData.UserId,
                  clientRequest.PublicLogData.PublicAppId,
                  clientRequest.PublicLogData.ServiceId);
            var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);


            request.Headers.Add("X-Obh-signature", $"OBH1-HMAC-SHA256;SignedHeaders=X-Obh-uuid,X-Obh-timestamp;Signature={ShahinOptions.RequestSignature}");
            request.Headers.Add("X-Obh-uuid", $"{Guid.NewGuid()}");
            var accessToken = await _shahinRepository.FindShahinAccessToken();

            request.Headers.Add("X-Obh-timestamp", $"{timestampHeader}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var mainRequest = new ChequeInquiryHolderReq
            {
                bank = clientRequest.bank,
                idCode = clientRequest.idCode,
                idTypes = clientRequest.idTypes,
                sayadId = clientRequest.sayadId,
            };

            request.Content = new StringContent(JsonSerializer.Serialize(mainRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var serilizedResponse = JsonSerializer.Deserialize<ChequeInquiryHolderRes>(responseBodyJson,
                ServiceHelperExtension.JsonSerializerOptions);

            return new OutputModel
            {
                Content = JsonSerializer.Serialize(serilizedResponse.respObject),
                StatusCode = response.StatusCode.ToString(),
                RequestId = publicRequestId!.ToString(),
                ReqLogId = reqLogId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(clientRequest.PublicLogData)}");
            throw new Exception(ex.Message);
        }
    }
}