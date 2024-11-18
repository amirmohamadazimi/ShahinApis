using System.ComponentModel;

namespace ShahinApis.ErrorHandling;

public enum ErrorCode
{
    [Description("عملیات با موفقیت انجام شد")]
    Success = 0,

    [Description("درخواست اشتباه")]
    BadRequest = 400,

    [Description("یافت نشد")]
    NotFound = 403,

    [Description("خطای داخلی .")]
    InternalError = 500,
}
