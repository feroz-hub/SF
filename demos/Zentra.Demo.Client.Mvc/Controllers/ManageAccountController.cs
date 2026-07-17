using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zentra.DemoClientMvc.Constants;
using Zentra.DemoClientMvc.Extension;
using Zentra.DemoClientMvc.Interface;
using Zentra.DemoClientMvc.Models;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Enums;
using Zentra.Domain.Models.Api;
using Zentra.Domain.Models.Api.Response;

namespace Zentra.DemoClientMvc.Controllers;

public class ManageAccountController(IHttpService httpService) : Controller
{
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    public async Task<ActionResult> UpdateProfile(string returnUrl = null)
    {
        try
        {
            ViewData["ReturnUrl"] = returnUrl;
            var userModel =
                await httpService.PostSecureAsync<UserModel>(ApiRoutePathConstants.GetUserByName, User.Identity.Name);
            if (userModel != null)
            {
                var model = new UpdateViewModel();
                model.Email = userModel.Email;
                model.PhoneNumber = userModel.PhoneNumber;
                model.FirstName = userModel.FirstName;
                model.LastName = userModel.LastName;
                return View(model);
            }

            return View();
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> UpdateProfile(UpdateViewModel model, string returnUrl = null)
    {
        try
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var userModel =
                    await httpService.PostSecureAsync<UserModel>(ApiRoutePathConstants.GetUserByName,
                        User.Identity.Name);
                if (userModel != null)
                {
                    userModel.Email = !string.IsNullOrWhiteSpace(model.Email) ? model.Email : userModel.Email;
                    userModel.PhoneNumber = !string.IsNullOrWhiteSpace(model.PhoneNumber)
                        ? model.PhoneNumber
                        : userModel.PhoneNumber;
                    userModel.FirstName = !string.IsNullOrWhiteSpace(model.FirstName)
                        ? model.FirstName
                        : userModel.FirstName;
                    userModel.LastName =
                        !string.IsNullOrWhiteSpace(model.LastName) ? model.LastName : userModel.LastName;
                    userModel.ModifiedBy = User.Identity.Name;

                    var frameworkResult =
                        await httpService.PostSecureAsync<FrameworkResult>(ApiRoutePathConstants.UpdateUser, userModel);
                    if (frameworkResult.Status == ResultStatus.Succeeded)
                    {
                        TempData["Message"] = "User Updated successfully!";
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(frameworkResult.Errors.ToList()[0].Code,
                        frameworkResult.Errors.ToList()[0].Description);
                }
            }

            return View(model);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    public ActionResult TwoFactorAuthentication(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TwoFactorAuthentication(string action, string returnUrl = null)
    {
        try
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                FrameworkResult frameworkResult = null;
                var userid = HttpContext.Session.GetString(ApplicationConstants.SessionUserId);
                if (userid != null)
                {
                    if (action.Equals("EmailAuthentication"))
                    {
                        var emailFactor = new { user_id = userid, two_factor_type = TwoFactorType.Email };
                        frameworkResult =
                            await httpService.PostSecureAsync<FrameworkResult>(
                                ApiRoutePathConstants.UpdateUserTwoFactorType, emailFactor);
                        if (frameworkResult.Status == ResultStatus.Failed) return ShowErrorPage(frameworkResult);

                        TempData["Message"] = "Two-factor authentication type registered as email.";
                    }
                    else if (action.Equals("PhonenumberAuthentication"))
                    {
                        var smsFactor = new { user_id = userid, two_factor_type = TwoFactorType.Sms };
                        frameworkResult =
                            await httpService.PostSecureAsync<FrameworkResult>(
                                ApiRoutePathConstants.UpdateUserTwoFactorType, smsFactor);
                        if (frameworkResult.Status == ResultStatus.Failed) return ShowErrorPage(frameworkResult);

                        TempData["Message"] = "Two-factor authentication type registered as phonenumber.";
                    }
                    else if (action.Equals("SetupAuthenticatorApp"))
                    {
                        return RedirectToAction("SetupAuthenticator");
                    }
                    else if (action.Equals("ResetAuthenticatorApp"))
                    {
                        frameworkResult =
                            await httpService.PostAsync<FrameworkResult>(ApiRoutePathConstants.ResetAuthenticatorApp,
                                userid);
                        if (frameworkResult.Status == ResultStatus.Failed) return ShowErrorPage(frameworkResult);

                        TempData["Message"] = "Authenticator app reset successfull.";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (action.Equals("None"))
                    {
                        var noFactor = new { user_id = userid, two_factor_type = TwoFactorType.None };
                        frameworkResult =
                            await httpService.PostSecureAsync<FrameworkResult>(
                                ApiRoutePathConstants.UpdateUserTwoFactorType, noFactor);
                        if (frameworkResult.Status == ResultStatus.Failed) return ShowErrorPage(frameworkResult);

                        TempData["Message"] = "Two-factor authentication type removed.";
                        return RedirectToAction("Index", "Home");
                    }

                    var recoveryCodeCount =
                        await httpService.PostSecureAsync<int>(ApiRoutePathConstants.CountRecoveryCodesAsync, userid);
                    if (recoveryCodeCount <= 0)
                    {
                        var recoveryCodes =
                            await httpService.PostSecureAsync<IEnumerable<string>>(
                                ApiRoutePathConstants.GenerateRecoveryCodes, userid);
                        if (recoveryCodes.ContainsAny())
                        {
                            TempData["RecoveryCodesKey"] = recoveryCodes;
                            return RedirectToAction(nameof(ShowRecoveryCodes));
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return ShowErrorPage("User not found in DB");
                }
            }

            return View();
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    public async Task<IActionResult> SetupAuthenticator(string returnUrl = null)
    {
        try
        {
            ViewData["ReturnUrl"] = returnUrl;
            var userid = HttpContext.Session.GetString(ApplicationConstants.SessionUserId);
            if (!string.IsNullOrWhiteSpace(userid))
            {
                var setup = new { user_id = userid, application_name = ApplicationConstants.ApplicationName };
                var setupAuthenticatorViewModel =
                    await httpService.PostSecureAsync<SetupAuthenticatorViewModel>(
                        ApiRoutePathConstants.SetupAuthenticatorApp, setup);
                return View(setupAuthenticatorViewModel);
            }

            return View();
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetupAuthenticator(SetupAuthenticatorViewModel model, string returnUrl = null)
    {
        try
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var userid = HttpContext.Session.GetString(ApplicationConstants.SessionUserId);
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    var verify = new { user_id = userid, user_token = model.VerificationCode };
                    var setupAuthenticatorViewModel =
                        await httpService.PostSecureAsync<AuthenticatorAppResponseModel>(
                            ApiRoutePathConstants.VerifyAuthenticatorAppSetup, verify);
                    if (setupAuthenticatorViewModel.Succeeded)
                    {
                        TempData["Message"] = "Two-factor authentication type registered as authenticator app.";
                        if (setupAuthenticatorViewModel.RecoveryCodes.ContainsAny())
                        {
                            TempData["RecoveryCodesKey"] = setupAuthenticatorViewModel.RecoveryCodes;
                            return RedirectToAction(nameof(ShowRecoveryCodes));
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", setupAuthenticatorViewModel.Message);
                }
            }

            return View();
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    public IActionResult ShowRecoveryCodes()
    {
        try
        {
            var recoveryCodes = (string[])TempData["RecoveryCodesKey"];
            if (recoveryCodes == null) return ShowErrorPage("No recovery codes find");

            var model = new AuthenticatorAppResponseModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    public ActionResult ChangePassword(string returnUrl = null)
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "ZentraAdmin, ZentraUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model, string returnUrl = null)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userid = HttpContext.Session.GetString(ApplicationConstants.SessionUserId);
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    var changePassword = new
                        { user_id = userid, current_password = model.OldPassword, new_password = model.NewPassword };
                    var frameworkResult =
                        await httpService.PostSecureAsync<FrameworkResult>(ApiRoutePathConstants.ChangePassword,
                            changePassword);
                    if (frameworkResult.Status == ResultStatus.Succeeded)
                    {
                        TempData["Message"] = "Password changed!";
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(frameworkResult.Errors.ToList()[0].Code,
                        frameworkResult.Errors.ToList()[0].Description);
                }

                return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = "User not found" });
            }

            return View();
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    private ActionResult ShowErrorPage(FrameworkResult frameworkResult)
    {
        try
        {
            var errorModel = new ErrorViewModel
            {
                ErrorCode = frameworkResult.Errors.ToList()[0].Code,
                ErrorMessage = frameworkResult.Errors.ToList()[0].Description
            };
            return RedirectToAction("Error", "Error", errorModel);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    private ActionResult ShowErrorPage(string errorMessage)
    {
        var errorModel = new ErrorViewModel
        {
            ErrorCode = "AppError",
            ErrorMessage = errorMessage
        };
        return RedirectToAction("Error", "Error", errorModel);
    }
}
