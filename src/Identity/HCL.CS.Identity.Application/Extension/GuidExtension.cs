namespace HCL.CS.Service.Extension;

internal static class GuidExtension
{
    internal static bool IsValid(this Guid id)
    {
        return id != Guid.Empty && id != default;
    }

    internal static bool IsGuid(this string id)
    {
        return Guid.TryParse(id, out _);
    }
}
