using System.Runtime.CompilerServices;
using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Services.Implementation;

internal class FrameworkResultService : IFrameworkResultService
{
    private static readonly FrameworkResult Success = new() { Status = ResultStatus.Succeeded };
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;

    public FrameworkResultService(ILoggerInstance instance, IResourceStringHandler resourceStringHandler)
    {
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceStringHandler = resourceStringHandler;
    }

    public FrameworkResult Succeeded()
    {
        return Success;
    }

    public T Failed<T>(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            var errorMessage = ResolveErrorMessage(errorCode, callerMemberName, sourceFilePath);
            loggerService.WriteToWithCaller(Log.Error, errorCode + " : " + errorMessage, null, callerMemberName,
                sourceFilePath);
            if (typeof(FrameworkResult) == typeof(T))
            {
                var result = new FrameworkResult
                {
                    Status = ResultStatus.Failed,
                    Errors = new[]
                    {
                        new FrameworkError
                        {
                            Code = errorCode,
                            Description = errorMessage
                        }
                    }
                };

                // Returning FrameworkResult as return type
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
        else
        {
            loggerService.WriteToWithCaller(Log.Error, "Error code not specified.", null, callerMemberName,
                sourceFilePath);
        }

        // Returning T as return type
        return (T)Convert.ChangeType(null, typeof(T));
    }

    public void Throw(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            var errorMessage = ResolveErrorMessage(errorCode, callerMemberName, sourceFilePath);
            loggerService.WriteToWithCaller(Log.Error, errorCode + " : " + errorMessage, null, callerMemberName,
                sourceFilePath);
            throw new Exception(errorMessage);
        }

        throw new Exception("Error code not specified.");
    }

    public ValidationError Failed(string openIdErrorCode, string specificErrorCode,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null)
    {
        // Getting two error codes for endpoint related scenarios, Need to pass orginal OpenId error codes as result.
        if (!string.IsNullOrWhiteSpace(specificErrorCode))
        {
            var errorMessage = ResolveErrorMessage(specificErrorCode, callerMemberName, sourceFilePath);
            loggerService.WriteToWithCaller(Log.Error, openIdErrorCode + " : " + errorMessage, null, callerMemberName,
                sourceFilePath);
            return new ValidationError
            {
                ErrorCode = openIdErrorCode,
                ErrorMessage = errorMessage
            };
        }

        ThrowCustomMessage("Error code not specified.");
        return null;
    }

    public FrameworkResult Failed(IEnumerable<FrameworkError> errors, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        if (errors.Any())
        {
            var errorMessage = string.Join(", ", errors.Select(error => error.Description));
            loggerService.WriteToWithCaller(Log.Error, errorMessage, null, callerMemberName, sourceFilePath);
            return new FrameworkResult
            {
                Status = ResultStatus.Failed,
                Errors = errors
            };
        }

        ThrowCustomMessage("Error code not specified.");
        return null;
    }

    public FrameworkResult ConstructFailed(string errorCode, string errorMessage,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null)
    {
        if (!string.IsNullOrWhiteSpace(errorCode) && !string.IsNullOrWhiteSpace(errorMessage))
        {
            loggerService.WriteToWithCaller(Log.Error, errorCode + " : " + errorMessage, null, callerMemberName,
                sourceFilePath);
            var result = new FrameworkResult
            {
                Status = ResultStatus.Failed,
                Errors = new[]
                {
                    new FrameworkError
                    {
                        Code = errorCode,
                        Description = errorMessage
                    }
                }
            };

            return result;
        }

        ThrowCustomMessage("Error code not specified.");
        return null;
    }

    public void ThrowCustomMessage(string customErrorMessage, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        if (!string.IsNullOrWhiteSpace(customErrorMessage))
        {
            loggerService.WriteToWithCaller(Log.Error, customErrorMessage, null, callerMemberName, sourceFilePath);
            throw new Exception(customErrorMessage);
        }

        throw new Exception("Custom error message not specified.");
    }

    public T EmptyResult<T>(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            var errorMessage = ResolveErrorMessage(errorCode, callerMemberName, sourceFilePath);
            loggerService.WriteToWithCaller(Log.Debug, errorCode + " : " + errorMessage, null, callerMemberName,
                sourceFilePath);
        }

        return (T)Convert.ChangeType(null, typeof(T));
    }

    private string ResolveErrorMessage(
        string errorCode,
        string callerMemberName,
        string sourceFilePath)
    {
        var errorMessage = resourceStringHandler.GetResourceString(errorCode, true);
        if (!string.IsNullOrWhiteSpace(errorMessage)) return errorMessage;

        loggerService.WriteToWithCaller(
            Log.Warning,
            $"Missing validation message key for error code '{errorCode}'. Falling back to error code.",
            null,
            callerMemberName,
            sourceFilePath);
        return errorCode;
    }
}
