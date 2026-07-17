using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;

namespace TestApp.Helper.Api;

public static class AuditHelper
{
    public static List<AuditTrail> GetAuditList()
    {
        var auditList = new List<AuditTrail>();
        var auditEntity = new AuditTrail();
        auditEntity.ActionType = AuditType.Create;
        auditEntity.AffectedColumn = "AffectedColumn";
        auditEntity.TableName = "Mod_Name";
        auditEntity.NewValue = "123";
        auditEntity.OldValue = "456";
        //auditEntity.PrimaryKey = "Privacy Modified";
        auditEntity.CreatedBy = "123";
        auditEntity.CreatedOn = DateTime.UtcNow;
        auditList.Add(auditEntity);

        auditEntity = new AuditTrail();
        auditEntity.ActionType = AuditType.Create;
        auditEntity.AffectedColumn = "AffectedColumn";
        auditEntity.TableName = "Mod_Name";
        auditEntity.NewValue = "123";
        auditEntity.OldValue = "123";
        //auditEntity.PrimaryKey = "Privacy Modified";
        auditEntity.CreatedBy = "ABC";
        auditEntity.CreatedOn = DateTime.UtcNow;
        auditList.Add(auditEntity);
        return auditList;
    }

    public static AuditTrail GetAuditDetailsForSave()
    {
        var auditEntity = new AuditTrail
        {
            ActionType = AuditType.Create,
            AffectedColumn = "AffectedColumn",
            TableName = "Mod_Name",
            NewValue = "123",
            OldValue = "456",
            //PrimaryKey = "Privacy Modified",
            CreatedBy = "123",
            CreatedOn = DateTime.UtcNow
        };
        return auditEntity;
    }

    public static AuditTrail GetEmptyAuditDetails()
    {
        var auditEntity = new AuditTrail
        {
            ActionType = AuditType.Create,
            AffectedColumn = "",
            TableName = "",
            NewValue = "",
            OldValue = "",
            //PrimaryKey = "",
            CreatedBy = ""
        };
        return auditEntity;
    }

    public static AuditTrail GetNullAuditDetails()
    {
        var auditEntity = new AuditTrail
        {
            ActionType = AuditType.Create,
            AffectedColumn = null,
            TableName = null,
            NewValue = null,
            OldValue = null,
            //PrimaryKey = null,
            CreatedBy = null
        };
        return auditEntity;
    }
}
