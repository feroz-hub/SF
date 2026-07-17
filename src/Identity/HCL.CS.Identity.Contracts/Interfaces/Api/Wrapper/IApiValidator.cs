using System.Runtime.CompilerServices;
using HCL.CS.Domain;

namespace HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

public interface IApiValidator
{
    Task<FrameworkResult> ValidateRequest([CallerMemberName] string callerMemberName = null);
}
