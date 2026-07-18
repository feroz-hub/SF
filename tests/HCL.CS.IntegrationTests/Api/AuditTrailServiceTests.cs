/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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



