namespace NMaier.SimpleDlna.Server.Interfaces.Metadata;

public interface IMetaImageItem
: IMetaInfo, IMetaResolution, IMetaDescription
{
    string MetaCreator { get; }
}