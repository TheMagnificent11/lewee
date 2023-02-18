using System.Text.RegularExpressions;

namespace Lewee.Infrastructure.Controllers;

internal static partial class StringExtensions
{
    public static string[] SplitCamelCase(this string str)
    {
        return CamelCaseRegex().Split(str);
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex CamelCaseRegex();
}
