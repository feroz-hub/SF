namespace Zentra.DemoClientMvc.Extension;

public static class ApplicationExtension
{
    public static bool ContainsAny<T>(this IEnumerable<T>? data)
    {
        return data != null && data.Any();
    }
}
