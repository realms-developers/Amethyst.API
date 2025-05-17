using System.Reflection;

namespace Amethyst.Extensions.Base.Utility;

public static class AssemblyUtility
{
    public static Type? TryFindExtensionType(Assembly assembly, out ExtensionMetadataAttribute? attribute)
    {
        attribute = null;

        foreach (var type in assembly.GetTypes())
        {
            attribute = type.GetCustomAttribute<ExtensionMetadataAttribute>();
            if (attribute != null)
            {
                return type;
            }
        }

        return null;
    }

    public static TExtension? TryCreateExtension<TExtension>(Assembly assembly, out ExtensionMetadataAttribute? attribute) where TExtension : class
    {
        var extensionType = TryFindExtensionType(assembly, out attribute);
        if (extensionType == null)
        {
            return null;
        }

        var instance = Activator.CreateInstance(extensionType) as TExtension;
        return instance;
    }
}
