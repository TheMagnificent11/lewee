namespace Lewee.Domain;

/// <summary>
/// Domain Exception
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class
    /// </summary>
    /// <param name="messsage">Message</param>
    public DomainException(string messsage)
        : base(messsage)
    {
    }
}
