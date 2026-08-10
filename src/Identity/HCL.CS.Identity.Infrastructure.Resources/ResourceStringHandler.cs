/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Resources;

internal class ResourceStringHandler : IResourceStringHandler
{
    private ResourceManager resourceMan;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ResourceManager ResourceManager
    {
        get
        {
            if (ReferenceEquals(resourceMan, null))
            {
                var temp = new ResourceManager(
                    "HCL.CS.Infrastructure.Resources.ValidationMessages",
                    typeof(ResourceStringHandler).Assembly);
                resourceMan = temp;
            }

            return resourceMan;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

    public string GetResourceString(string id, bool skipError = false)
    {
        var result = ResourceManager.GetString(id, Culture);
        if (result == null && !skipError) throw new Exception("Missing validation message for ID : " + id);

        return result;
    }

    public string GetResourceKeyByValue(string value)
    {
        var entry = (ResourceManager.GetResourceSet(Culture, true, true) ?? throw new InvalidOperationException())
            .OfType<DictionaryEntry>()
            .FirstOrDefault(e => e.Value != null && e.Value.ToString() == value);

        var key = entry.Key.ToString();
        return key;
    }
}
