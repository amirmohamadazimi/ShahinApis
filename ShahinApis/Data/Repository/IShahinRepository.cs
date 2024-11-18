using ShahinApis.Data.Model;

namespace ShahinApis.Data.Repository;

public interface IShahinRepository
{
    Task<string> InsertShahinRequestLog(ShahinRequestLogDto shahinRequestLogDto);
    Task<string> InsertShahinResponseLog(ShahinResponseLogDto shahinResponseLogDto);
}

