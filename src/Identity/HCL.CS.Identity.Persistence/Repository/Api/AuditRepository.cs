using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;
using Microsoft.EntityFrameworkCore;

//using LinqKit;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class AuditRepository : BaseRepository<AuditTrail>, IAuditRepository
{
    private readonly IApplicationDbContext context;

    public AuditRepository(IApplicationDbContext context)
        : base(context)
    {
        this.context = context;
    }

    public override Task InsertAsync(AuditTrail entity, CancellationToken cancellationToken = default)
    {
        context.AuditTrail.Add(entity);
        return Task.CompletedTask;
    }

    public override async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.AuditTrail.CountAsync(cancellationToken);
    }

    public async Task<int> GetFilteredCountAsync(AuditSearchRequestModel auditSearchModule, CancellationToken cancellationToken = default)
    {
        var query = ApplyAuditFilters(context.AuditTrail.AsNoTracking(), auditSearchModule);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IList<AuditTrail>> GetAuditDetailsAsync(AuditSearchRequestModel auditSearchModule, CancellationToken cancellationToken = default)
    {
        auditSearchModule.Page.CurrentPage =
            auditSearchModule.Page.CurrentPage == 0 ? 1 : auditSearchModule.Page.CurrentPage;
        var recordsToSkip = (auditSearchModule.Page.CurrentPage - 1) * auditSearchModule.Page.ItemsPerPage;

        var query = ApplyAuditFilters(context.AuditTrail.AsNoTracking(), auditSearchModule);

        return await query
            .OrderBy(s => s.CreatedOn)
            .Skip(recordsToSkip)
            .Take(auditSearchModule.Page.ItemsPerPage)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<AuditTrail> ApplyAuditFilters(IQueryable<AuditTrail> query, AuditSearchRequestModel auditSearchModule)
    {
        if (!string.IsNullOrWhiteSpace(auditSearchModule.CreatedBy))
            query = query.Where(s => s.CreatedBy.Equals(auditSearchModule.CreatedBy));

        var searchValue = auditSearchModule.SearchValue?.Trim();
        if (!string.IsNullOrWhiteSpace(searchValue))
            query = query.Where(s =>
                (!string.IsNullOrWhiteSpace(s.NewValue) && s.NewValue.Contains(searchValue)) ||
                (!string.IsNullOrWhiteSpace(s.OldValue) && s.OldValue.Contains(searchValue)));

        if (auditSearchModule.FromDate != null && auditSearchModule.ToDate == null)
            query = query.Where(s => s.CreatedOn == auditSearchModule.FromDate);

        if (auditSearchModule.FromDate != null && auditSearchModule.ToDate != null)
            query = query.Where(s => s.CreatedOn >= auditSearchModule.FromDate && s.CreatedOn <= auditSearchModule.ToDate);

        if (auditSearchModule.ActionType != AuditType.None)
            query = query.Where(s => s.ActionType == auditSearchModule.ActionType);

        return query;
    }
}
