using System.Reflection;

namespace Lewee.Infrastructure.AspNet.Observability;

internal static class VersionHelper
{
    public static string GetVersion()
    {
        return Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
    }
}
