using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using ShahinApis.Data.Model;
using System.Configuration;

namespace ShahinApis.Data.Repository;

public class ShahinRepository : IShahinRepository
{
    public ILogger<ShahinRepository> _logger { get; set; }
    public IConfiguration _configuration { get; set; }
    public ShahinDbContext _dbContext { get; set; }

    public ShahinRepository(ILogger<ShahinRepository> logger, ShahinDbContext shahinDbContext, IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = shahinDbContext;
        _configuration = configuration;
    }

    public async Task<string> InsertShahinRequestLog(ShahinRequestLogDto request)
    {
        var shahinReq = new ShahinReqLog()
        {
            Id = Guid.NewGuid().ToString(),
            JsonReq = request.jsonRequest,
            LogDateTime = DateTime.UtcNow,
            UserId = request.userId,
            ServiceId = request.serviceId,
            PublicAppId = request.publicAppId,
            PublicReqId = request.publicRequestId
        };

        _dbContext.Shahin_Req.Add(shahinReq);
        
        try
        {
            await _dbContext.SaveChangesAsync();
            return shahinReq.Id;
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(request)}");
            throw new ApplicationException($"Exception occurred while: {nameof(request)}  => {ex.Message}");
        }
    }

    public async Task<string> InsertShahinResponseLog(ShahinResponseLogDto response)
    {
        var shahinRes = new ShahinResLog()
        {
            Id = Guid.NewGuid().ToString(),
            HTTPStatusCode = response.shahinHttpResponseCode,
            JsonRes = response.jsonResponse,
            PublicReqId = response.publicRequestId,
            ReqLogId = response.shahinRequestId
        };

        _dbContext.Shahin_Res.Add(shahinRes);

        try
        {
            await _dbContext.SaveChangesAsync();
            return shahinRes.Id;
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(response)}");
            throw new ApplicationException($"Exception occurred while: {nameof(response)}  => {ex.Message}");
        }
    }
}

