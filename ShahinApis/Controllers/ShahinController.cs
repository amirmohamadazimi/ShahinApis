using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShahinApis.Data.Model;
using ShahinApis.ErrorHandling;
using ShahinApis.Filters;
using ShahinApis.Service;
using ShahinApis.Infrastucture;

namespace ShahinApis.Controllers
{
    [ApiExplorerSettings]
    [ApiVersion("1")]
    [Route("OCRSayadCheque/v1/[controller]")]
    [ApiController]
    [ApiResultFilter]

    public class ShahinController : ControllerBase
    {
        private ILogger<ShahinController> _logger;
        private IShahinService _shahinService;
        private BaseLog _baseLog;

        public ShahinController(ILogger<ShahinController> logger, IShahinService shahinService, BaseLog baseLog)
        {
            _logger = logger;
            _shahinService = shahinService;
            _baseLog = baseLog;
        }


        /// <summary>
        /// فراخوانی توکن
        /// </summary>
        /// <param name="GetTokenReqDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("GetShahinToken")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAccessTokenResDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GetAccessTokenResDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GetAccessTokenResDto))]
        public async Task<ActionResult<GetAccessTokenResDto>> GetShahinToken(BasePublicLogData basePublicLogData)
        {
            var result = await _shahinService.GetAccessToken(basePublicLogData);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<GetAccessTokenResDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(GetShahinToken)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<GetAccessTokenResDto>(result.Content,
                    result.StatusCode, result.ReqLogId, result.RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while {nameof(GetShahinToken)}");
                throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                      $" {nameof(GetShahinToken)} => {ex.Message}");
            }
        }

        /// <summary>
        /// استعلام وضعیت اعتباری وضعیت چک
        /// </summary>
        /// <param name="ChequeInquiry"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ChequeInquiry")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutputModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OutputModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(OutputModel))]
        public async Task<ActionResult<ChequeInquiryResDto>> PostChequeInquiry(ChequeInquiryReqDto request)
        {
            var result = await _shahinService.PostChequeInquiry(request);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<ChequeInquiryResDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(GetShahinToken)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<ChequeInquiryResDto>(result.Content,
                    result.StatusCode, result.ReqLogId, result.RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while {nameof(GetShahinToken)}");
                throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                      $" {nameof(GetShahinToken)} => {ex.Message}");
            }
        }
    }
}
