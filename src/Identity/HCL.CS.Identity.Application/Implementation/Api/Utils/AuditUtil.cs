using Newtonsoft.Json;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Utils;

internal class AuditUtil(
    IAuditTrailService auditTrailService,
    ILoggerService loggerService)
{
    internal async Task<FrameworkResult> Create(object model, string tableName)
    {
        var value = GetAuditValues(model, out var createdBy);
        var audit = new AuditTrailModel
        {
            ActionType = AuditType.Create,
            TableName = tableName,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = createdBy,
            OldValue = null,
            NewValue = value,
            AffectedColumn = null
        };
        return await SaveAuditDetails(audit);
    }

    internal async Task<FrameworkResult> Delete(object model, string tableName)
    {
        var value = GetAuditValues(model, out var createdBy);
        var audit = new AuditTrailModel
        {
            ActionType = AuditType.Delete,
            TableName = tableName,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = createdBy,
            OldValue = value,
            NewValue = null,
            AffectedColumn = null
        };
        return await SaveAuditDetails(audit);
    }

    internal async Task<FrameworkResult?> Update(object oldModel, object newModel, string tableName)
    {
        var audit = GenerateAuditObject(oldModel, newModel, tableName);
        if (audit != null) return await SaveAuditDetails(audit);

        return null;
    }

    internal async Task<FrameworkResult> Update(Dictionary<string, object> oldModel,
        Dictionary<string, object> newModel, string tableName, string createdBy)
    {
        var audit = new AuditTrailModel
        {
            ActionType = AuditType.Update,
            TableName = tableName,
            CreatedOn = DateTime.UtcNow,
            OldValue = JsonConvert.SerializeObject(oldModel),
            NewValue = JsonConvert.SerializeObject(newModel),
            AffectedColumn = newModel.Select(x => x.Key).ToString(),
            CreatedBy = createdBy
        };
        return await SaveAuditDetails(audit);
    }

    private AuditTrailModel? GenerateAuditObject(object oldModel, object newModel, string tableName)
    {
        if (ReferenceEquals(oldModel, newModel))
        {
            loggerService.WriteTo(Log.Debug, "Source and destination model are same.");
            return null;
        }

        if (oldModel.GetType() != newModel.GetType())
        {
            loggerService.WriteTo(Log.Debug, "Different type od source and destination model passed");
            return null;
        }

        var audit = new AuditTrailModel
        {
            ActionType = AuditType.Update,
            TableName = tableName,
            CreatedOn = DateTime.UtcNow
        };

        var auditModel = new AuditModel();
        var valueChanged = false;
        foreach (var property in oldModel.GetType().GetProperties())
        {
            var propertyName = property.Name;
            if (Constants.IgnoredAuditColumns.Contains(propertyName)) continue;
            var oldValue = property.GetValue(oldModel);
            var newValue = property.GetValue(newModel);
            if ((oldValue == null && newValue != null) || (oldValue != null && newValue == null) ||
                (oldValue != null && newValue != null && !oldValue.Equals(newValue)))
            {
                auditModel.AffectedColumns.Add(propertyName);
                auditModel.OldValues[propertyName] = oldValue;
                auditModel.NewValues[propertyName] = newValue;
                valueChanged = true;
            }

            if (propertyName == Constants.CreatedBy) audit.CreatedBy = Convert.ToString(property.GetValue(newModel));
        }

        if (valueChanged)
        {
            audit.OldValue = JsonConvert.SerializeObject(auditModel.OldValues);
            audit.NewValue = JsonConvert.SerializeObject(auditModel.NewValues);
            audit.AffectedColumn = JsonConvert.SerializeObject(auditModel.AffectedColumns);
            return audit;
        }

        loggerService.WriteTo(Log.Debug, "Source and destination model are same.");
        return null;
    }

    private async Task<FrameworkResult> SaveAuditDetails(AuditTrailModel audit)
    {
        return await auditTrailService.AddAuditTrailAsync(audit);
    }

    private string GetAuditValues(object model, out string? createdBy)
    {
        createdBy = string.Empty;
        var values = new Dictionary<string, object>();
        foreach (var property in model.GetType().GetProperties())
        {
            var propertyName = property.Name;
            if (Constants.IgnoredAuditColumns.Contains(propertyName)) continue;
            values[propertyName] = property.GetValue(model);
            if (propertyName == Constants.CreatedBy) createdBy = Convert.ToString(property.GetValue(model));
        }

        return JsonConvert.SerializeObject(values);
    }
}
