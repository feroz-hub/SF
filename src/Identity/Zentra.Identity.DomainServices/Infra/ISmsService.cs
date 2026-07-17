using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.DomainServices.Infra;

public interface ISmsService
{
    Task<FrameworkResult> SendSmsAsync(NotificationInfoModel message);

    Task<FrameworkResult> UpdateSmsStatusAsync();
}
