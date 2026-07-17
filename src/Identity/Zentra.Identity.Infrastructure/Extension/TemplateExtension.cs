using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Services.Extension;

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
