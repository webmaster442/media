using System.Linq;

using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Handlers;

internal sealed class IndexHandler : IPrefixHandler
{
    private readonly HttpServer _owner;

    public IndexHandler(HttpServer owner)
    {
        _owner = owner;
    }

    public string Prefix => "/";

    public IResponse HandleRequest(IRequest req)
    {
        var article = HtmlTools.CreateHtmlArticle("Index");
        var document = article.OwnerDocument ?? throw new HttpStatusException(HttpCode.InternalError);
        var list = document.EL("ul");
        var mounts = _owner.MediaMounts.OrderBy(m => m.Value, NaturalStringComparer.Comparer);
        foreach (var m in mounts)
        {
            var li = document.EL("li");
            li.AppendChild(document.EL(
              "a",
              new AttributeCollection { { "href", m.Key } },
              m.Value));
            list.AppendChild(li);
        }

        article.AppendChild(list);

        return new StringResponse(HttpCode.Ok, document.OuterXml);
    }
}
