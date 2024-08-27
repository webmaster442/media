namespace NMaier.SimpleDlna.Server.Interfaces.Metadata;

public interface IMetaInfo
{
    DateTime InfoDate { get; }

    long? InfoSize { get; }
}