using System.Runtime.CompilerServices;
using Zentra.Domain;

namespace Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

public interface IApiValidator
{
    Task<FrameworkResult> ValidateRequest([CallerMemberName] string callerMemberName = null);
}
