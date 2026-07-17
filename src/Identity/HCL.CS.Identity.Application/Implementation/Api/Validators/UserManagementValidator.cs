using System.Globalization;
using System.Text.RegularExpressions;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api.Response;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Service.Implementation.Api.Validators;

internal class UserManagementValidator
{
    internal SignInResponseModel ValidatePasswordExpiryAsync(
        Users user,
        SystemSettings settings,
        IResourceStringHandler resourceStringHandler,
        ILoggerService loggerService)
    {
        var signinResponse = new SignInResponseModel();
        signinResponse.Succeeded = true;
        if (user.LastPasswordChangedDate != null)
        {
            var days = Convert.ToDateTime(user.LastPasswordChangedDate)
                .AddDays(settings.PasswordConfig.MaxPasswordExpiry).Subtract(DateTime.UtcNow).TotalDays;
            if (days < 0)
            {
                loggerService.WriteTo(Log.Error, "Password expired for user: " + user.UserName);
                return ConstructError(ApiErrorCodes.PasswordExpired, resourceStringHandler);
            }

            if (days < settings.PasswordConfig.PasswordNotificationBeforeExpiry)
            {
                var day = days.ToString().Split('.');
                var message =
                    string.Format(resourceStringHandler.GetResourceString(ApiErrorCodes.PasswordAboutToExpire), day[0]);
                loggerService.WriteTo(Log.Debug, message);
                signinResponse.Message = message;
                return signinResponse;
            }
        }

        return signinResponse;
    }

    internal bool IsValidEmailAddress(string email)
    {
        // Reference - https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        if (string.IsNullOrWhiteSpace(email)) return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    internal bool IsValidPhoneNumber(string phoneNumber, int minLength, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

        if (phoneNumber.Length > minLength && phoneNumber.Length <= maxLength) return true;

        return false;
    }

    internal bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

        var regex = @"^(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";
        var match = Regex.Match(phoneNumber, regex, RegexOptions.IgnoreCase);

        if (!match.Success) return false;

        return true;
    }

    internal bool IsValidDateOfBirth(DateTime? dob, int minAge, int maxAge)
    {
        if (dob == null) return false;

        DateTime date;
        if (DateTime.TryParse(dob.ToString(), out date))
        {
            var diff = DateTime.UtcNow.Year - date.Year;
            if (diff > minAge && diff <= maxAge) return true;
        }

        return false;
    }

    internal bool IsComplexPassword(string password, PasswordConfig passwordConfig, out string errorCodes)
    {
        var input = password;
        errorCodes = string.Empty;
        try
        {
            if (!string.IsNullOrWhiteSpace(passwordConfig.PasswordPattern))
            {
                var pattern = new Regex(passwordConfig.PasswordPattern);
                if (pattern.IsMatch(input)) return true;

                errorCodes = ApiErrorCodes.PasswordPatternNotMatched;
                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasMiniMaxChars = new Regex(@"^.{" + passwordConfig.MinPasswordLength + "," +
                                            passwordConfig.MaxPasswordLength + "}$");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (passwordConfig.RequireLowercase && !hasLowerChar.IsMatch(input))
            {
                errorCodes = ApiErrorCodes.PasswordRequiredLowerCase;
                return false;
            }

            if (passwordConfig.RequireUppercase && !hasUpperChar.IsMatch(input))
            {
                errorCodes = ApiErrorCodes.PasswordRequiredUpperCase;
                return false;
            }

            if (passwordConfig.MinPasswordLength > 0 && passwordConfig.MaxPasswordLength > 0 &&
                !hasMiniMaxChars.IsMatch(input))
            {
                errorCodes = ApiErrorCodes.InvalidPasswordLength;
                return false;
            }

            if (passwordConfig.RequireDigit && !hasNumber.IsMatch(input))
            {
                errorCodes = ApiErrorCodes.PasswordRequiredNumericValue;
                return false;
            }

            if (passwordConfig.RequireSpecialChar && !hasSymbols.IsMatch(input))
            {
                errorCodes = ApiErrorCodes.PasswordRequiredSpecialCharacters;
                return false;
            }

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private SignInResponseModel ConstructError(string errorCode, IResourceStringHandler resourceStringHandler)
    {
        var signinResponse = new SignInResponseModel();
        signinResponse.Succeeded = false;
        signinResponse.Message = resourceStringHandler.GetResourceString(errorCode);
        signinResponse.ErrorCode = errorCode;
        return signinResponse;
    }
}
