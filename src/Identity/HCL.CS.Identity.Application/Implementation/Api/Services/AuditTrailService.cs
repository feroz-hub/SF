using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public class AuditTrailService(
    IAuditRepository auditRepository,
    ILoggerInstance loggerInstance,
    IMapper mapper,
    IFrameworkResultService frameworkResult)
    : SecurityBase, IAuditTrailService
{
    private readonly ILoggerService loggerService =
        loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);

    public virtual async Task<FrameworkResult> AddAuditTrailAsync(AuditTrailModel audit)
    {
        if (audit == null) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.AuditModelIsNull);

        if (audit.ActionType == AuditType.None)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidAuditActionType);

        if (!string.IsNullOrWhiteSpace(audit.TableName) && audit.TableName.Length > Constants.ColumnLength255)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.AuditTableNameTooLong);

        if (!string.IsNullOrWhiteSpace(audit.CreatedBy) && audit.CreatedBy.Length > Constants.ColumnLength255)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.CreatedByTooLong);

        try
        {
            var auditTrail = mapper.Map<AuditTrailModel, AuditTrail>(audit);
            await auditRepository.InsertAsync(auditTrail);
            return await auditRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add audit trail.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddAuditTrailAsync(IEnumerable<AuditTrailModel> audits)
    {
        try
        {
            if (audits == null) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.AuditModelIsNull);

            var auditTrails = mapper.Map<IEnumerable<AuditTrailModel>, IEnumerable<AuditTrail>>(audits);
            foreach (var audit in auditTrails)
            {
                if (audit.ActionType == AuditType.None)
                    return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidAuditActionType);

                if (!string.IsNullOrWhiteSpace(audit.TableName) && audit.TableName.Length > Constants.ColumnLength255)
                    return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.AuditTableNameTooLong);

                if (!string.IsNullOrWhiteSpace(audit.CreatedBy) && audit.CreatedBy.Length > Constants.ColumnLength255)
                    return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.CreatedByTooLong);

                await auditRepository.InsertAsync(audit);
            }

            return await auditRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add audit trail.");
            throw;
        }
    }

    public virtual async Task<AuditResponseModel> GetAuditDetailsAsync(AuditSearchRequestModel auditSearchRequestModel)
    {
        try
        {
            if (auditSearchRequestModel.Page == null)
                frameworkResult.Throw(ApiErrorCodes.AuditModelIsNull); // Change Error Code to PageModelIsNull.

            var auditResponse = new AuditResponseModel();
            if (auditSearchRequestModel.FromDate > auditSearchRequestModel.ToDate)
                frameworkResult.Throw(ApiErrorCodes.FromDateGreaterThanToDate);

            auditSearchRequestModel.Page.TotalItems = await auditRepository.GetFilteredCountAsync(auditSearchRequestModel);
            auditResponse.PageInfo = auditSearchRequestModel.Page;

            //  IList<AuditTrail> searchResults = await auditRepository.GetAuditDetailsAsync(page, option, createdBy, actionType, fromDate, toDate);
            var searchResults = await auditRepository.GetAuditDetailsAsync(auditSearchRequestModel);
            if (!searchResults.ContainsAny())
            {
                auditResponse.AuditList = new List<AuditTrailModel>();
                return auditResponse;
            }

            auditResponse.AuditList = mapper.Map<IEnumerable<AuditTrail>, IEnumerable<AuditTrailModel>>(searchResults)
                .ToList();
            return auditResponse;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve audit details.");
            throw;
        }
    }

    //public virtual async Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, DateTime? createdOn, PagingModel page)
    //{
    //    return await GetAuditDetailsAsync(page, QueryOption.ChangeByAndDate, createdBy, AuditType.None, createdOn);
    //}

    //public virtual async Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, DateTime? fromDate, DateTime? toDate, PagingModel page)
    //{
    //    return await GetAuditDetailsAsync(page, QueryOption.ChangeByAndBetweenDates, createdBy, AuditType.None, fromDate, toDate);
    //}

    //public virtual async Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, AuditType actionType, DateTime? fromDate, DateTime? toDate, PagingModel page)
    //{
    //    return await GetAuditDetailsAsync(page, QueryOption.ChangeBywithActionAndBetweenDates, createdBy, actionType, fromDate, toDate);
    //}
}
