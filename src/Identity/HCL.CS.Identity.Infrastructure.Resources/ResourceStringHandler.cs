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
