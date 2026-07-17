using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.DomainServices.Infra;

public interface IEmailService
{
    Task<FrameworkResult> SendEmailAsync(NotificationInfoModel message);
}
