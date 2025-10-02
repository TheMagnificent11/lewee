namespace Lewee.Domain;

/// <summary>
/// Represents an abstract base class for defining relationships between entities.
/// </summary>
public abstract class Relationship : AuditableRecord
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Relationship"/> class.
    /// </summary>
    protected Relationship()
        : base()
    {
    }
}
