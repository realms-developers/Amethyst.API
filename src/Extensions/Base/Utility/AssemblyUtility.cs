using System.Reflection;
using Amethyst.Extensions.Base.Metadata;

namespace Amethyst.Extensions.Base.Utility;

public static class AssemblyUtility
{
    public static Type? TryFindExtensionType(Assembly assembly, out ExtensionMetadataAttribute? attribute)
    {
        attribute = null;

        foreach (Type type in assembly.GetTypes())
        {
            attribute = type.GetCustomAttribute<ExtensionMetadataAttribute>();
            if (attribute != null)
            {
                return type;
            }
        }

        return null;
    }

    public static TExtension? TryCreateExtension<TExtension>(Assembly assembly, out ExtensionMetadataAttribute? attribute, params object[] args) where TExtension : class
    {
        Type? extensionType = TryFindExtensionType(assembly, out attribute);
        if (extensionType == null)
        {
            return null;
        }

        var instance = Activator.CreateInstance(extensionType, args) as TExtension;
        return instance;
    }
}
