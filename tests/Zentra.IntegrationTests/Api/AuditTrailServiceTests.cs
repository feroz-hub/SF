using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Zentra.Domain;
using Zentra.Domain.Enums;
using Zentra.Domain.Models;
using Zentra.Service.Interfaces;
using Zentra.TestApp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Zentra.IntegrationTests.Api
{
    public class AuditTrailServiceTests : ZentraFakeSetup
    {
        private readonly IAuditTrailService auditTrailService;

        public AuditTrailServiceTests()
        {
            auditTrailService = ServiceProvider.GetService<IAuditTrailService>();
        }

    }
}



