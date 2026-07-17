using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Infra;

public interface ISmsService
{
    Task<FrameworkResult> SendSmsAsync(NotificationInfoModel message);

    Task<FrameworkResult> UpdateSmsStatusAsync();
}
