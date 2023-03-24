namespace Lewee.Domain;

/// <summary>
/// Value Object
/// </summary>
/// <typeparam name="T">Value object type</typeparam>
public abstract class ValueObject<T> : IValueObject<T>
    where T : ValueObject<T>
{
    /// <inheritdoc />
    public bool Equals(T? other)
    {
        if (other is not T valueObject)
        {
            return false;
        }

        return this.Equals(valueObject);
    }

    /// <summary>
    /// Equality check
    /// </summary>
    /// <param name="other">Object to compare against</param>
    /// <returns>True if equivalent, otherwise false</returns>
    /// <remarks>Null check not required</remarks>
    protected abstract bool EqualsCore(T other);
}
