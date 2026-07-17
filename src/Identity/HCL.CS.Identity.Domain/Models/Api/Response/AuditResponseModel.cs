namespace HCL.CS.Domain.Models.Api.Response;

public class AuditResponseModel
{
    public List<AuditTrailModel> AuditList { get; set; }

    public PagingModel PageInfo { get; set; }
}
