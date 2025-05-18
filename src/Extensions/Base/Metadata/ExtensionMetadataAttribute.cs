namespace Amethyst.Extensions.Base.Metadata;

[AttributeUsage(AttributeTargets.Class)]
public class ExtensionMetadataAttribute(string name, string author, string? description, Version version) : Attribute
{
    public string Name { get; } = name;
    public string Author { get; } = author;
    public string? Description { get; } = description;
    public Version Version { get; } = version;

    public ExtensionMetadataAttribute(string name, string author, string? description,
            int major, int minor = 0, int build = 0, int revision = 0)
            : this(name, author, description, new Version(major, minor, build, revision))
    {
    }

    public ExtensionMetadataAttribute(string name, string author, Version version)
        : this(name, author, null, version)
    {
    }

    public ExtensionMetadataAttribute(string name, string author,
        int major, int minor = 0, int build = 0, int revision = 0)
        : this(name, author, null, new Version(major, minor, build, revision)) { }

    public ExtensionMetadataAttribute(string name, string author, string description)
        : this(name, author, description, new())
    {
    }

    public ExtensionMetadataAttribute(string name, string author)
        : this(name, author, null, new())
    {
    }

    public ExtensionMetadataAttribute(string name)
        : this(name, "Unknown", null, new())
    {
    }
}
