/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Services.Extension;

internal static class TemplateExtension
{
    internal static string UpdateNotificationTemplatePlaceholder(this string template, Users users,
        Dictionary<string, string> parameters)
    {
        foreach (var keyValue in parameters)
            if (template.Contains(keyValue.Key) && !string.IsNullOrWhiteSpace(keyValue.Value))
                template = template.Replace(keyValue.Key, keyValue.Value);

        if (template.Contains("{USERNAME}")) template = template.Replace("{USERNAME}", users.FirstName);

        if (template.Contains("{FIRSTNAME}")) template = template.Replace("{FIRSTNAME}", users.FirstName);

        if (template.Contains("{LASTNAME}")) template = template.Replace("{LASTNAME}", users.LastName);

        if (template.Contains("{FULLNAME}"))
            template = template.Replace("{FULLNAME}", users.FirstName + " " + users.LastName);

        if (template.Contains("{USERID}")) template = template.Replace("{USERID}", Convert.ToString(users.Id));

        if (template.Contains("{EMAIL}")) template = template.Replace("{EMAIL}", users.Email);

        return template;
    }
}
