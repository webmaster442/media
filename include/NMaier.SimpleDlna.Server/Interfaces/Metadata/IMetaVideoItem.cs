using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Interfaces.Metadata;

public interface IMetaVideoItem
: IMetaInfo, IMetaDescription, IMetaGenre, IMetaDuration, IMetaResolution
{
    IEnumerable<string> MetaActors { get; }

    string MetaDirector { get; }

    Subtitle Subtitle { get; }
}