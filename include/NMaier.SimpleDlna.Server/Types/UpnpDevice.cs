using System.Net;

namespace NMaier.SimpleDlna.Server.Types;

internal sealed class UpnpDevice
{
    public IPAddress Address { get; }

    public Uri Descriptor { get; }

    public string Type { get; }

    public string USN { get; }

    public Guid UUID { get; }

    public UpnpDevice(Guid uuid,
                      string type,
                      Uri descriptor,
                      IPAddress address)
    {
        UUID = uuid;
        Type = type;
        Descriptor = descriptor;
        Address = address;

        USN = Type.StartsWith("uuid:", StringComparison.Ordinal)
            ? Type
            : $"uuid:{UUID}::{Type}";
    }
}