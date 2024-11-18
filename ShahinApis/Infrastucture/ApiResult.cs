using Microsoft.AspNetCore.Mvc;
using ShahinApis.ErrorHandling;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;

namespace ShahinApis.Infrastucture;
public class ApiResult
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string RequestId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }
    public ApiResult()
    {

    }
    public ApiResult(bool isSuccess, ErrorCode statusCode, string? requestId = null, string? message = null)
    {
        IsSuccess = isSuccess;
        StatusCode = (int)statusCode;
        Message = message ?? statusCode.GetDisplayName();
        RequestId = requestId;

    }

    public ApiResult(bool isSuccess, int statusCode, string? message)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Message = message ?? "خطای ناشناخته";
    }

    public static implicit operator ApiResult(OkResult result)
    {
        return new ApiResult(true, ErrorCode.Success);
    }

    public static implicit operator ApiResult(BadRequestResult result)
    {
        return new ApiResult(false, ErrorCode.BadRequest);
    }

    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join('|', errorMessages);
        }

        return new ApiResult(false, ErrorCode.BadRequest, message);
    }

    public static implicit operator ApiResult(ContentResult result)
    {
        return new ApiResult(true, ErrorCode.Success, result.Content);
    }

    public static implicit operator ApiResult(NotFoundResult result)
    {
        return new ApiResult(false, ErrorCode.NotFound);
    }
}

public class ApiResult<TData> : ApiResult
where TData : class
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TData Data { get; set; }

    public ApiResult(bool isSuccess, ErrorCode statusCode, TData data, string? message = null)
        : base(isSuccess, statusCode, message)
    {
        Data = data;
    }

    public static implicit operator ApiResult<TData>(TData data)
    {
        return new ApiResult<TData>(true, ErrorCode.Success, data);
    }

    public static implicit operator ApiResult<TData>(OkResult result)
    {
        return new ApiResult<TData>(true, ErrorCode.Success, null);
    }

    public static implicit operator ApiResult<TData>(OkObjectResult result)
    {
        return new ApiResult<TData>(true, ErrorCode.Success, (TData)result.Value);
    }

    public static implicit operator ApiResult<TData>(BadRequestResult result)
    {
        return new ApiResult<TData>(false, ErrorCode.BadRequest, null);
    }

    public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }
        return new ApiResult<TData>(false, ErrorCode.BadRequest, null, message);
    }

    public static implicit operator ApiResult<TData>(ContentResult result)
    {
        return new ApiResult<TData>(true, ErrorCode.Success, null, result.Content);
    }

    public static implicit operator ApiResult<TData>(NotFoundResult result)
    {
        return new ApiResult<TData>(false, ErrorCode.NotFound, null);
    }

    public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
    {
        return new ApiResult<TData>(false, ErrorCode.NotFound, (TData)result.Value);
    }
}