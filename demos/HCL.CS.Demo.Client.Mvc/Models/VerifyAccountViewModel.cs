using System.ComponentModel.DataAnnotations;

namespace HCL.CS.DemoClientMvc.Models;

public class VerifyAccountViewModel
{
    [Display(Name = "UserName")] public string UserName { get; set; }

    [Display(Name = "Email verification code")]
    public string EmailVerificationCode { get; set; }

    [Display(Name = "Phone number verification code")]
    public string PhoneVerificationCode { get; set; }

    public string ReturnUrl { get; set; }
}

//public class VerifyUserAccountViewModel
//{
//    [Display(Name = "Email verification code")]
//    public string EmailVerificationCode { get; set; }

//    [Display(Name = "Phone number verification code")]
//    public string PhoneVerificationCode { get; set; }

//    public string ReturnUrl { get; set; }
//}
