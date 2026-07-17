using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api.Response;

public class SignInResponseModel
{
    public bool Succeeded { get; set; } = false;

    public bool IsLockedOut { get; set; } = false;

    public bool IsNotAllowed { get; set; } = false;

    public bool RequiresTwoFactor { get; set; } = false;

    public TwoFactorType TwoFactorVerificationMode { get; set; }

    public bool TwoFactorVerificationCodeSent { get; set; } = false;

    public string Message { get; set; }

    public string ErrorCode { get; set; }

    public string UserVerificationCode { get; set; }
}
