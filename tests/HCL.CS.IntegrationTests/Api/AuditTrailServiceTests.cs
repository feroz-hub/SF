using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using HCL.CS.Domain;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models;
using HCL.CS.Service.Interfaces;
using HCL.CS.TestApp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HCL.CS.IntegrationTests.Api
{
    public class AuditTrailServiceTests : HclCsFakeSetup
    {
        private readonly IAuditTrailService auditTrailService;

        public AuditTrailServiceTests()
        {
            auditTrailService = ServiceProvider.GetService<IAuditTrailService>();
        }

    }
}



