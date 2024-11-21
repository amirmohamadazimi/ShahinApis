using Microsoft.AspNetCore.Mvc.Rendering;
using ShahinApis.Data.Model;

namespace ShahinApis.Service;
public interface IShahinService
{
    Task<OutputModel> GetAccessToken(BasePublicLogData basePublicLogData);
    Task<OutputModel> PostChequeInquiry(ChequeInquiryReqDto request);
    Task<OutputModel> PostChequeAccept(ChequeAcceptReqDto request);
    Task<OutputModel> PostChequeInquiryTransfer(ChequeInquiryTransferReqDto request);
    Task<OutputModel> PostChequeInquiryHolder(ChequeInquiryHolderReqDto request);
    Task<OutputModel> PostCheckIbanNationalCode(CheckIbanNationalcodeReqDto request);
    Task<OutputModel> PostCheckNationalcodeSourceAccount(CheckNationalcodeSourceAccountReqDto request);
}

