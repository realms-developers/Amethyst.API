namespace Amethyst.Extensions.Base.Utility;

public static class FileUtility
{
    private static readonly string _extDirectory = Path.Combine(AppContext.BaseDirectory, "..", "extensions");

    public static IEnumerable<string> GetExtensions(string folder)
    {
        if (string.IsNullOrEmpty(folder))
        {
            throw new ArgumentException("Folder cannot be null or empty.", nameof(folder));
        }

        string path = Path.Combine(_extDirectory, folder);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);

            return [];
        }

        return Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
    }
}
