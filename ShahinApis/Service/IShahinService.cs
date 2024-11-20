using Microsoft.AspNetCore.Mvc.Rendering;
using ShahinApis.Data.Model;

namespace ShahinApis.Service;
public interface IShahinService
{
    Task<OutputModel> GetAccessToken(BasePublicLogData basePublicLogData);
    Task<OutputModel> PostChequeInquiry(ChequeInquiryReqDto request);

}

