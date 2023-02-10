namespace Lewee.Shared;

/// <summary>
/// Object Extensions
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Gets the assembly name, full class name and type of the specified object
    /// </summary>
    /// <param name="obj">Object to get type info</param>
    /// <param name="errorMessage">Error message to use of <see cref="InvalidOperationException"/> is type is null</param>
    /// <returns>Assembly name, full class name and type in a tuple</returns>
    /// <exception cref="InvalidOperationException">Thrown if type cannot be determined</exception>
    public static (string assemblyName, string fullCalssName, Type objectType) GetBasicTypeInfo(this object obj, string errorMessage)
    {
        var objType = obj.GetType();

        if (objType == null
            || objType.Assembly == null
            || objType.FullName == null)
        {
            throw new InvalidOperationException(errorMessage);
        }

        var assemblyName = objType.Assembly.GetName();

        if (assemblyName == null || assemblyName.Name == null)
        {
            throw new InvalidOperationException(errorMessage);
        }

        return (assemblyName.Name, objType.FullName, objType);
    }
}
