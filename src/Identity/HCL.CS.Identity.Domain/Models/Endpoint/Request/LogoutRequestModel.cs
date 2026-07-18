/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class LogoutRequestModel : LogoutMessageModel
{
    public LogoutRequestModel(string endSessionCallBackUrl, LogoutMessageModel message)
    {
        if (message != null)
        {
            ClientId = message.ClientId;
            PostLogoutRedirectUri = message.PostLogoutRedirectUri;
            SubjectId = message.SubjectId;
            SessionId = message.SessionId;
            ClientIdCollection = message.ClientIdCollection;
            Parameters = message.Parameters;
        }

        EndSessionCallBackUrl = endSessionCallBackUrl;
    }

    public string EndSessionCallBackUrl { get; set; }

    public bool ShowSignoutPrompt => string.IsNullOrWhiteSpace(ClientId);
}
