using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Media.Interop;
internal static class CliModelProvider
{
    public static Dto.Cli.Model GetModel()
    {
        XmlSerializer xmlSerializer = new(typeof(Dto.Cli.Model));

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule?.FileName ?? throw new UnreachableException(),
                Arguments = "cli xmldoc",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            },
        };

        process.Start();
        string xml =  process.StandardOutput.ReadToEnd();

        using XmlReader XmlReader = XmlReader.Create(new StringReader(xml));

        return xmlSerializer.Deserialize(XmlReader) as Dto.Cli.Model
            ?? throw new UnreachableException();
    }
}
