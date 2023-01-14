using System.ComponentModel;
using System.Reflection;

namespace Lewee.Util;

/// <summary>
/// Enum Extension Methods
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the <see cref="DescriptionAttribute"/> description from an enum value if it exists, otherwise returns ToString() of the value
    /// </summary>
    /// <param name="value">Enum item</param>
    /// <returns><see cref="DescriptionAttribute"/> description or ToString() of enum value</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value
            .GetType()
            .GetField(value.ToString());

        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Determines whether the enum value is equvialent to zero
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="value">Enum value</param>
    /// <returns>True if equivalent to zero, otherwise false</returns>
    public static bool IsEquivalentToZero<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        return Enum.IsDefined(typeof(TEnum), 0) && Enum.ToObject(typeof(TEnum), 0).Equals(value);
    }
}
