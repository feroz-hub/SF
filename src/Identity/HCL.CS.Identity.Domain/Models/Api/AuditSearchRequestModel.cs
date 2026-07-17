using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api;

public class AuditSearchRequestModel
{
    public AuditType ActionType { get; set; } = AuditType.None;

    public string CreatedBy { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public PagingModel Page { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string SearchValue { get; set; }
}
