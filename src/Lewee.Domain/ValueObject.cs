namespace Lewee.Domain;

/// <summary>
/// Value Object
/// </summary>
/// <typeparam name="T">Value object type</typeparam>
public abstract class ValueObject<T> : IEquatable<T>
    where T : ValueObject<T>
{
    /// <inheritdoc />
    public bool Equals(T? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns the an enumerable collection of object properties to use in equality checks
    /// </summary>
    /// <returns>An enemerable collection of property values</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();
}
