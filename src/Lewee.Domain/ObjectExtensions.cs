namespace Lewee.Domain;

internal static class ObjectExtensions
{
    public static (string assemblyName, string fullCalssName, Type objectType) GetAssemblyInfo(this object obj, string errorMessage)
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
