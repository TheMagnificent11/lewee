namespace Lewee.Domain;

/// <summary>
/// Specification Interface
/// </summary>
/// <typeparam name="T">Type to validate</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Determines whether the <paramref name="input"/> satifies the this specification/rule
    /// </summary>
    /// <param name="input">Input to validate</param>
    /// <returns>True if valid, otherwise false</returns>
    bool IsValid(T input);
}
