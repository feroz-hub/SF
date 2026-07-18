/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Runtime.CompilerServices;
using DomainValidation.Validation;
using HCL.CS.Domain;

namespace HCL.CS.DomainServices.Infra;

public interface IFrameworkResultService
{
    FrameworkResult Succeeded();

    T Failed<T>(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    void Throw(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    ValidationError Failed(string openIdErrorCode, string specificErrorCode,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null);

    FrameworkResult Failed(IEnumerable<FrameworkError> errors, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    FrameworkResult ConstructFailed(string errorCode, string errorMessage,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null);

    void ThrowCustomMessage(string customErrorMessage, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);

    T EmptyResult<T>(string errorCode, [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null);
}
