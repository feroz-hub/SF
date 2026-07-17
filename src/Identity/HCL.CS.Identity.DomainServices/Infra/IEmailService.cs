using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Infra;

public interface IEmailService
{
    Task<FrameworkResult> SendEmailAsync(NotificationInfoModel message);
}
