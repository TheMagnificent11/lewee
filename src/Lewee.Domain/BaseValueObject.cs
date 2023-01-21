namespace Lewee.Domain;

/// <summary>
/// Base Value Object
/// </summary>
/// <typeparam name="T">Value object type</typeparam>
public abstract class BaseValueObject<T> : IValueObject<T>
    where T : BaseValueObject<T>
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
