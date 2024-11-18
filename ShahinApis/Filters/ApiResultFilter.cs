using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ShahinApis.ErrorHandling;
using ShahinApis.Infrastucture;

namespace ShahinApis.Filters;
public class ApiResultFilter : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var requestId = context.HttpContext.Items["RequestId"]?.ToString();

        if (context.Result is OkObjectResult okObjectResult)
        {
            var apiResult = new ApiResult<object>(true, ErrorCode.Success, okObjectResult.Value, requestId);
            context.Result = new JsonResult(apiResult) { StatusCode = okObjectResult.StatusCode };
        }
        else if (context.Result is OkResult okResult)
        {
            var apiResult = new ApiResult(true, ErrorCode.Success, requestId);
            context.Result = new JsonResult(apiResult) { StatusCode = okResult.StatusCode };
        }
        else if (context.Result is ObjectResult badRequestObjectResult && badRequestObjectResult.StatusCode == 400)
        {
            var apiResult = new ApiResult(false, ErrorCode.BadRequest, requestId);
            context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
        }
        else if (context.Result is ObjectResult notFoundObjectResult && notFoundObjectResult.StatusCode == 404)
        {
            var apiResult = new ApiResult(false, ErrorCode.NotFound, requestId);
            context.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };
        }
        else if (context.Result is ContentResult contentResult)
        {
            var apiResult = new ApiResult(true, ErrorCode.Success, contentResult.Content, requestId);
            context.Result = new JsonResult(apiResult) { StatusCode = contentResult.StatusCode };
        }

        else if (context.Result is ObjectResult objectResult && objectResult.StatusCode == null && !(objectResult.Value is ApiResult))
        {
            var apiResult = new ApiResult<object>(true, ErrorCode.Success, objectResult.Value, requestId);
            context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
        }


        base.OnResultExecuting(context);
    }
}