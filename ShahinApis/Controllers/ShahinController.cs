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
        /// <param name="ChequeInquiryReqDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ChequeInquiry")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChequeInquiryResDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ChequeInquiryResDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ChequeInquiryResDto))]
        public async Task<ActionResult<ChequeInquiryResDto>> PostChequeInquiry(ChequeInquiryReqDto request)
        {
            var result = await _shahinService.PostChequeInquiry(request);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<ChequeInquiryResDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(PostChequeInquiry)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<ChequeInquiryResDto>(result.Content,
                    result.StatusCode, result.ReqLogId, result.RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while {nameof(PostChequeInquiry)}");
                throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                      $" {nameof(PostChequeInquiry)} => {ex.Message}");
            }
        }

        /// <summary>
        /// استعلام وضعیت انتقال چک صیاد
        /// </summary>
        /// <param name="ChequeInquiryTransferReqDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ChequeInquiryTransferStatus")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChequeInquiryTransferStatusDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ChequeInquiryTransferStatusDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ChequeInquiryTransferStatusDto))]
        public async Task<ActionResult<ChequeInquiryTransferStatusDto>> PostChequeInquiryTransferStatus(ChequeInquiryTransferReqDto request)
        {
            var result = await _shahinService.PostChequeInquiryTransfer(request);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<ChequeInquiryTransferStatusDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(PostChequeInquiryTransferStatus)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<ChequeInquiryTransferStatusDto>(result.Content,
                    result.StatusCode, result.ReqLogId, result.RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while {nameof(PostChequeInquiryTransferStatus)}");
                throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                      $" {nameof(PostChequeInquiryTransferStatus)} => {ex.Message}");
            }
        }

        /// <summary>
        /// تایید یا رد چک صیاد
        /// </summary>
        /// <param name = "ChequeAcceptReqDto" ></ param >
        /// <returns></ returns >
        [AllowAnonymous]
        [HttpPost("ChequeAcceptation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChequeAcceptResDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ChequeAcceptResDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ChequeAcceptResDto))]
        public async Task<ActionResult<ChequeAcceptResDto>> PostChequeAccept(ChequeAcceptReqDto request)
        {
            var result = await _shahinService.PostChequeAccept(request);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<ChequeAcceptResDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(PostChequeAccept)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<ChequeAcceptResDto>(result.Content,
                    result.StatusCode, result.ReqLogId, result.RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while {nameof(PostChequeAccept)}");
                throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                      $" {nameof(PostChequeAccept)} => {ex.Message}");
            }
        }

        /// <summary>
        ///  استعلام چک صیاد توسط دارنده
        /// </summary>
        /// <param name = "ChequeInquiryHolderReqDto"></ param >
        /// <returns></ returns >
        [AllowAnonymous]
        [HttpPost("ChequeInquiryHolder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChequeInquiryHolderResDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ChequeInquiryHolderResDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ChequeInquiryHolderResDto))]
        public async Task<ActionResult<ChequeInquiryHolderResDto>> PostChequeInquiryHolder(ChequeInquiryHolderReqDto request)
        {
            var result = await _shahinService.PostChequeInquiryHolder(request);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<ChequeInquiryHolderResDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(PostChequeInquiryHolder)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<ChequeInquiryHolderResDto>(result.Content,
                    result.StatusCode, result.ReqLogId, result.RequestId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while {nameof(PostChequeInquiryHolder)}");
                throw new RamzNegarException(ErrorCode.InternalError, $"Exception occurred while:" +
                                                                      $" {nameof(PostChequeInquiryHolder)} => {ex.Message}");
            }
        }

    }
}
