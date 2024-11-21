using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShahinApis.Data.Model;
using ShahinApis.ErrorHandling;
using ShahinApis.Filters;
using ShahinApis.Infrastucture;
using ShahinApis.Service;

namespace ShahinApis.Controllers;

[ApiExplorerSettings]
[ApiVersion("1")]
[Route("ShahinCheque/v1/[controller]")]
[ApiController]
[ApiResultFilter]
public class ShahinInquiryController : Controller
{
    private ILogger<ShahinInquiryController> _logger;
    private IShahinService _shahinService;
    private BaseLog _baseLog;

    public ShahinInquiryController(ILogger<ShahinInquiryController> logger, IShahinService shahinService, BaseLog baseLog)
    {
        _logger = logger;
        _shahinService = shahinService;
        _baseLog = baseLog;
    }

    /// <summary>
    /// تطابق کد ملی و شماره شبا    
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("CheckIbanNationalcode")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckIbanNationalcodeResDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CheckIbanNationalcodeResDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CheckIbanNationalcodeResDto))]
    public async Task<ActionResult<CheckIbanNationalcodeResDto>> PostCheckIbanNationalcode(CheckIbanNationalcodeReqDto request)
    {
        var result = await _shahinService.PostCheckIbanNationalCode(request);

        try
        {
            if (result.StatusCode is "OK")
                return Ok(_baseLog.ApiResponseSuccessByCodeProvider<CheckIbanNationalcodeResDto>(result.Content, result.StatusCode,
                    result.ReqLogId, result.RequestId));

            _logger.LogError($"{nameof(PostCheckIbanNationalcode)} not-success request - input \r\n" +
            $"response:{result.StatusCode}-{result.Content}");

            return BadRequest(_baseLog.ApiResponeFailByCodeProvider<CheckIbanNationalcodeResDto>(result.Content,
                result.StatusCode, result.ReqLogId, result.RequestId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(PostCheckIbanNationalcode)}");
            throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                  $" {nameof(PostCheckIbanNationalcode)} => {ex.Message}");
        }
    }


    /// <summary>
    /// تطابق کد ملی و شماره حساب    
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("CheckNationalcodeSourceAccount")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckNationalcodeSourceAccountResDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CheckNationalcodeSourceAccountResDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CheckNationalcodeSourceAccountResDto))]
    public async Task<ActionResult<CheckNationalcodeSourceAccountResDto>> PostCheckNationalcodeSourceAccount(CheckNationalcodeSourceAccountReqDto request)
    {
        var result = await _shahinService.PostCheckNationalcodeSourceAccount(request);

        try
        {
            if (result.StatusCode is "OK")
                return Ok(_baseLog.ApiResponseSuccessByCodeProvider<CheckNationalcodeSourceAccountResDto>(result.Content, result.StatusCode,
                    result.ReqLogId, result.RequestId));

            _logger.LogError($"{nameof(PostCheckNationalcodeSourceAccount)} not-success request - input \r\n" +
            $"response:{result.StatusCode}-{result.Content}");

            return BadRequest(_baseLog.ApiResponeFailByCodeProvider<CheckNationalcodeSourceAccountResDto>(result.Content,
                result.StatusCode, result.ReqLogId, result.RequestId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while {nameof(PostCheckNationalcodeSourceAccount)}");
            throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                  $" {nameof(PostCheckNationalcodeSourceAccount)} => {ex.Message}");
        }
    }

}
