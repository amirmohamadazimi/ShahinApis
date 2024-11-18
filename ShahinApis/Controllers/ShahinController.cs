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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTokenResDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GetTokenResDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GetTokenResDto))]
        public async Task<ActionResult<GetTokenResDto>> GetShahinToken(BasePublicLogData basePublicLogData)
        {
            var result = await _shahinService.GetAccessToken(basePublicLogData);

            try
            {
                if (result.StatusCode is "OK")
                    return Ok(_baseLog.ApiResponseSuccessByCodeProvider<GetTokenResDto>(result.Content, result.StatusCode,
                        result.ReqLogId, result.RequestId));

                _logger.LogError($"{nameof(GetShahinToken)} not-success request - input \r\n" +
                $"response:{result.StatusCode}-{result.Content}");

                return BadRequest(_baseLog.ApiResponeFailByCodeProvider<GetTokenResDto>(result.Content,
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
