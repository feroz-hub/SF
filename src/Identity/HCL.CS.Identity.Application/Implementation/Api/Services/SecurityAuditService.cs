/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public sealed class SecurityAuditService(
    IAuditRepository auditRepository,
    IHttpContextAccessor httpContextAccessor,
    ILoggerInstance loggerInstance)
    : ISecurityAuditService
{
    private const string AuditTableName = "Authentication";
    private readonly ILoggerService logger =
        loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);

    public async Task WriteAsync(
        SecurityAuditEventModel securityEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(securityEvent);

        try
        {
            var context = httpContextAccessor.HttpContext;
            var correlationId = context?.TraceIdentifier;
            if (context?.Request.Headers.TryGetValue("X-Correlation-ID", out var header) == true &&
                !string.IsNullOrWhiteSpace(header))
                correlationId = header.ToString();

            var safePayload = new
            {
                eventId = Guid.NewGuid(),
                eventType = Limit(securityEvent.EventType, 100),
                timestamp = DateTimeOffset.UtcNow,
                actorUserId = Limit(securityEvent.ActorUserId, 64),
                subjectHash = Limit(securityEvent.PseudonymousIdentifier, 64),
                authenticationSource = Limit(securityEvent.AuthenticationSource, 32),
                clientId = Limit(securityEvent.ClientId, 255),
                grantType = Limit(securityEvent.GrantType, 100),
                result = Limit(securityEvent.Result, 32),
                reasonCode = Limit(securityEvent.ReasonCode, 100),
                correlationId = Limit(correlationId, 128),
                sessionId = Limit(securityEvent.SessionId, 128)
            };

            var audit = new AuditTrail
            {
                Id = safePayload.eventId,
                ActionType = AuditType.Create,
                TableName = AuditTableName,
                ActionName = safePayload.eventType!,
                AffectedColumn = null,
                OldValue = null,
                NewValue = JsonSerializer.Serialize(safePayload),
                CreatedOn = safePayload.timestamp.UtcDateTime,
                CreatedBy = safePayload.actorUserId ?? safePayload.subjectHash ?? "anonymous"
            };

            await auditRepository.InsertAsync(audit, cancellationToken);
            var result = await auditRepository.SaveChangesAsync(cancellationToken);
            if (result.Status == ResultStatus.Failed)
                logger.WriteTo(
                    Log.Error,
                    $"Security audit persistence failed. Event={safePayload.eventType} CorrelationId={safePayload.correlationId}");
        }
        catch (Exception exception)
        {
            // Authentication remains available, but an audit write failure is never silent.
            logger.WriteToWithCaller(
                Log.Error,
                exception,
                $"Security audit persistence failed. Event={Limit(securityEvent.EventType, 100)}");
        }
    }

    private static string? Limit(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        return trimmed.Length <= maximumLength ? trimmed : trimmed[..maximumLength];
    }
}
