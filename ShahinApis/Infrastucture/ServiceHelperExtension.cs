using System.Text.Json.Serialization;
using System.Text.Json;
using ShahinApis.ErrorHandling;
using Microsoft.OpenApi.Extensions;

namespace ShahinApis.Infrastucture;

public static class ServiceHelperExtension
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        IgnoreNullValues = true
    };

    public static T GenerateApiErrorResponse<T>(ErrorCodesProvider errorCode) where T : ErrorResult, new()
    {
        return new T
        {
            StatusCode = errorCode.OutReponseCode.ToString(),
            IsSuccess = false,
            ResultMessage = errorCode?.SafeReponseMesageDecription,

        };
    }

    public static T GenerateErrorMethodResponse<T>(ErrorCode errorCode) where T : ErrorResult, new()
    {
        return new T
        {
            StatusCode = errorCode.ToString(),
            IsSuccess = false,
            ResultMessage = errorCode.GetDisplayName()
        };
    }

}

