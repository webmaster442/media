using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Views;

public interface IView : IRepositoryItem
{
IMediaFolder Transform(IMediaFolder oldRoot);
}
