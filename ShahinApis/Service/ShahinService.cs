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

    public async Task<OutputModel> PostCheckIbanNationalCode(CheckIbanNationalcodeReqDto clientRequest)
    {
        try
        {
            var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = clientRequest.PublicLogData?.PublicReqId;
            var timestampHeader = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            _logger.LogInformation($"{nameof(PostCheckIbanNationalCode)} request sent - input is : \r\n {clientRequest.PublicLogData}");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriService}inquiry/check-iban-nationalcode");

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

            var mainRequest = new CheckIbanNationalcodeReq
            {
                birthDate = clientRequest.birthDate,
                iban = clientRequest.iban,
                nationalCode = clientRequest.nationalCode,
            };

            request.Content = new StringContent(JsonSerializer.Serialize(mainRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var serilizedResponse = JsonSerializer.Deserialize<CheckIbanNationalcodeRes>(responseBodyJson,
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

    public async Task<OutputModel> PostCheckNationalcodeSourceAccount(CheckNationalcodeSourceAccountReqDto clientRequest)
    {
        try
        {
            //var publicRequestId = _httpContextAccessor.HttpContext!.Items["RequestId"] = clientRequest.PublicLogData?.PublicReqId;
            var timestampHeader = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            //_logger.LogInformation($"{nameof(PostCheckIbanNationalCode)} request sent - input is : \r\n {clientRequest.PublicLogData}");

            //var request = new HttpRequestMessage(HttpMethod.Post, $"{ShahinOptions.ShahinUriService}inquiry/customer-has-account");

            //var requestLogDto = new ShahinRequestLogDto(
            //      clientRequest.PublicLogData.PublicReqId,
            //      clientRequest.ToString(),
            //      clientRequest.PublicLogData.UserId,
            //      clientRequest.PublicLogData.PublicAppId,
            //      clientRequest.PublicLogData.ServiceId);
            //var reqLogId = await _shahinRepository.InsertShahinRequestLog(requestLogDto);


            //request.Headers.Add("X-Obh-signature", $"OBH1-HMAC-SHA256;SignedHeaders=X-Obh-uuid,X-Obh-timestamp;Signature={ShahinOptions.RequestSignature}");
            //request.Headers.Add("X-Obh-uuid", $"{Guid.NewGuid()}");
            //var accessToken = await _shahinRepository.FindShahinAccessToken();

            //request.Headers.Add("X-Obh-timestamp", $"{timestampHeader}");
            //request.Headers.Add("Authorization", $"Bearer {accessToken}");

            //var mainRequest = new CheckNationalcodeSourceAccountReq
            //{
            //    bank = clientRequest.bank,
            //    nationalCode = clientRequest.nationalCode,
            //    sourceAccount = clientRequest.sourceAccount
            //};

            //request.Content = new StringContent(JsonSerializer.Serialize(mainRequest), Encoding.UTF8, "application/json");

            //var client = new HttpClient(new HttpClientHandler
            //{
            //    ServerCertificateCustomValidationCallback =
            //    (httpRequestMessage, cert, certChain, policyErrors) => true
            //});

            //var response = await client.SendAsync(request);

            //var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://10.10.10.112:38453/v0.3/obh/api/inquiry/customer-has-account");
            request.Headers.Add("X-Obh-signature", "OBH1-HMAC-SHA256;SignedHeaders=X-Obh-uuid,X-Obh-timestamp;Signature=C111133F6DA075027BE47701FA153780B19CA1F6C3700568A8B0D3CAEB8EAAE5");
            request.Headers.Add("X-Obh-uuid", $"{Guid.NewGuid()}");
            request.Headers.Add("X-Obh-timestamp", $"{timestampHeader}");
            request.Headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJiYW5rIjoiQk1LIiwiYW1vdW50IjoxMDAwMDAwLCJuYmYiOjE3MzMxMzU1MTYxMzIsInVzZXJfbmFtZSI6IjEwODYxNTUwMzI3Iiwic2NvcGUiOlsiZ2V0Q2hlcXVlU3RhdGVtZW50IiwidHJhbnNmZXJzQ2hhaW5DaGVxdWUiLCJjYXJkVHJhbnNhY3Rpb24iLCJnZXRDdXJyZW5jeVRyYW5zZmVySW5mbyIsImFjaEJhdGNoVHJhbnNmZXIiLCJ0cmFuc2ZlckNvbmZpcm0iLCJ0cmFuc2ZlclZhbGlkYXRpb24iLCJjdXJyZW5jeVdpdGhkcmF3IiwiZ2V0TmF0aW9uYWxDb2RlSWRlbnRpdHkiLCJjaGVxdWVJbnF1aXJ5QnlIb2xkZXIiLCJ1bmJsb2NrQW5kVHJhbnNmZXIiLCJjdXN0b21lckhhc0FjY291bnQiLCJjaGVxdWVBY2NlcHQiLCJnZXRMb2FuU3RhdGVtZW50IiwiZ2V0Q3VycmVuY3lBY2NvdW50SW5mbyIsImNoZXF1ZVJlZ2lzdGVyIiwiZ2V0QmFzaWNDdXN0SW5mbyIsInVubG9ja0NoZXF1ZSIsImdldEJsb2NrQW1vdW50SW5xdWlyeSIsImdldExlZ2FsQWNjb3VudEluZm8iLCJ0cmFuc2ZlciIsImdldEFjY291bnRJbmZvIiwicGF5TG9hbiIsInRyYW5zYWN0aW9uSW5xdWlyeSIsImNoZWNrTWVkaWNhbERlc2VydmUiLCJnZXRDaGVxdWVJbnF1aXIiLCJydGdzVHJhbnNhY3Rpb25zSW5xdWlyeSIsImdldExvYW5MaXN0IiwiZ2V0SUJBTkluZm8iLCJiYXRjaFRyYW5zZmVyIiwiZ2V0QWNjb3VudExpc3QiLCJwYXlCaWxsIiwiZ2V0QWNjb3VudFN0YXRlbWVudCIsIm1hdGNoTmF0aW9uYWxjb2RlQW5kU2hlYmFJZElucXVpcnkiLCJ0cmFuc2ZlclRvIiwiZ2V0QWNjb3VudEJhbGFuY2UiLCJjaGVxdWVJbnF1aXJ5VHJhbnNmZXIiLCJnZXRMb2FuSW5mbyIsImdldENoZXF1ZUJvb2tMaXN0IiwiY2hlY2tJQkFOTmF0aW9uYWxDb2RlIiwiZ2V0SUJBTiIsImxvYW5QYXltZW50VmFsaWRhdGlvbiIsImdldEluc3VyYW5jZUhpc3RvcnkiLCJjaGVxdWVUcmFuc2ZlciIsImJpbGxQYXltZW50VmFsaWRhdGlvbiIsImxvY2tDaGVxdWVGb3JDYXNoaW5nIiwiY2FyZFRyYW5zZmVyIiwiYmxvY2tBbW91bnQiLCJjaGVja1Bob25lVmFsaWRpdHkiLCJwYXlCaWxsQnlDYXJkIiwiYWNoVHJhbnNhY3Rpb25zSW5xdWlyeSIsImdldERldGFpbEN1c3RJbmZvIiwiY3VycmVuY3lEZXBvc2l0IiwiY2hha2FkQ2FydGFibGUiLCJnZXRDdXJyZW5jeUFjY291bnRTdGF0ZW1lbnQiLCJnZXRDYXJkQmFsYW5jZSJdLCJpc3MiOiJTSEFBSElOIiwiYWNjb3VudHMiOlsiNjI4MDIzMTUzNjg5MDUxOSJdLCJleHAiOjE3MzkxMzU1MTYsImlhdCI6MTczMzEzNTUxNjEzMiwiYXV0aG9yaXRpZXMiOlsiUk9MRV9DTElFTlQiXSwianRpIjoiODE2NTZhOTEtNWE0NC00NWE1LWIyYzEtODg2N2ZlYzNiZjk4IiwiY2xpZW50X2lkIjoiVTBmcHZwdHVJVSJ9.1DMMV-tkzHjvZBZoyo00fqaLtKGXqhfMSRQzF3tSYLe69IXZqj6-_06vUtrz2jdlxm16z64HvL5DQiThbO1kmuDthnQ1MnfeL0hE-W213UVRbkz1yri9ilsHRvmFtw0WrB9dJdw0ZAfOFWgYBvQBrCHzFTrQuX99Sq689Btkj7m8zIJQ_4XEmiV9ULiX7V_C0YKb8i9QXNWakaz9KxKVwbmNhar7-4NdjliD9cwY6B-PpEabRMQ-kKx-VKDwjLTjOecBaon_h7rZZyqvhU5gQtM0nL2MZdZF6rpFPcwACFm2G6jf6FAy8GgKhGBqEj6GfCLfIJ3EkyewbGPUe4bNSw");

            //var content = new StringContent("{\n   \"bank\": \"BSI\",\n  \"nationalCode\": \"02352365520\",\n  \"sourceAccount\": \"9633255556334\"\n}", null, "application/json");
            //request.Content = content;

            var mainRequest = new CheckNationalcodeSourceAccountReq
            {
                bank = clientRequest.bank,
                nationalCode = clientRequest.nationalCode,
                sourceAccount = clientRequest.sourceAccount,
                accountOwnerType = clientRequest.accountOwnerType
            };

            request.Content = new StringContent(JsonSerializer.Serialize(mainRequest),null, "application/json");

            var response = await _httpClient.SendAsync(request);
           // response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            var responseBodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var serilizedResponse = JsonSerializer.Deserialize<CheckNationalcodeSourceAccountRes>(responseBodyJson,
                ServiceHelperExtension.JsonSerializerOptions);

            return new OutputModel
            {
                Content = JsonSerializer.Serialize(serilizedResponse.respObject),
                StatusCode = response.StatusCode.ToString(),
                //RequestId = publicRequestId!.ToString(),
                //ReqLogId = reqLogId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(clientRequest.PublicLogData)}");
            throw new Exception(ex.Message);
        }
    }
}