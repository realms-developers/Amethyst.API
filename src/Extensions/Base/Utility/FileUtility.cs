using System.Reflection;

namespace Amethyst.Extensions.Base.Utility;

public static class FileUtility
{
    public static IEnumerable<string> GetExtensions(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
    }
}
