using Microsoft.Extensions.DependencyInjection;
using Zentra.Service.Interfaces.Interfaces.Api;

namespace IntegrationTests;

public class AuditCheck : ZentraFakeSetup
{
    private readonly IAuditTrailService auditTrailService;

    public AuditCheck()
    {
        auditTrailService = ServiceProvider.GetService<IAuditTrailService>();
    }

    //public async Task<AuditResponseModel> GetAudit (string createdBy, AuditType auditType)
    //{
    //    DateTime fromDate = DateTime.UtcNow.AddDays(-1);
    //    DateTime toDate = DateTime.UtcNow.AddDays(1);
    //    string roleCreatedModifedBy = createdBy;
    //    PagingModel page = new PagingModel()
    //    {
    //        CurrentPage = 1,
    //        ItemsPerPage = 1000,
    //    };
    //    // Audit Type => None:0, Create/Add:1, Update:2, Delete:3
    //    AuditResponseModel auditResponseModelResult = await auditTrailService.GetAuditDetailsAsync(roleCreatedModifedBy, auditType, fromDate, toDate, page);

    //    if (auditResponseModelResult != null)
    //    {
    //        return auditResponseModelResult;
    //    }
    //    else
    //    {
    //        return null;
    //    }

    //}
}
