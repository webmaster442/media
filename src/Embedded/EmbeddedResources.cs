namespace Media.Embedded;

internal static class EmbeddedResources
{
    public const string UpdatePS1 = "Update.ps1";

    public static Stream GetFile(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return typeof(EmbeddedResources).Assembly.GetManifestResourceStream($"Media.Embedded.{name}")
            ?? throw new InvalidOperationException($"Resource {name} not found");
    }
}
