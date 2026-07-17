using System.Runtime.CompilerServices;
using DomainValidation.Validation;
using Zentra.Domain;

namespace Zentra.DomainServices.Infra;

public interface IFrameworkResultService
{
    FrameworkResult Succeeded();

    T Failed<T>(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    void Throw(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    ValidationError Failed(string openIdErrorCode, string specificErrorCode,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null);

    FrameworkResult Failed(IEnumerable<FrameworkError> errors, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    FrameworkResult ConstructFailed(string errorCode, string errorMessage,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null);

    void ThrowCustomMessage(string customErrorMessage, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    T EmptyResult<T>(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);
}
