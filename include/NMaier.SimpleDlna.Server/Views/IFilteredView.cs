using NMaier.SimpleDlna.Server.Interfaces;

namespace NMaier.SimpleDlna.Server.Views;

public interface IFilteredView : IView
{
bool Allowed(IMediaResource item);
}
