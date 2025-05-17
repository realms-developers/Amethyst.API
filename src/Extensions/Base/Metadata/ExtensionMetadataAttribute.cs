namespace Amethyst.Extensions.Base;

[AttributeUsage(AttributeTargets.Class)]
public class ExtensionMetadataAttribute : Attribute
{
    public string Name { get; }
    public string Author { get; }
    public string? Description { get; }
    public string Version { get; }

    public ExtensionMetadataAttribute(string name, string author, string? description, string version)
    {
        Name = name;
        Author = author;
        Description = description;
        Version = version;
    }

    public ExtensionMetadataAttribute(string name, string author, string version)
        : this(name, author, null, version)
    {
    }

    public ExtensionMetadataAttribute(string name, string author)
        : this(name, author, null, "1.0.0")
    {
    }

    public ExtensionMetadataAttribute(string name)
        : this(name, "Unknown", null, "1.0.0")
    {
    }
}
