namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IHeaders : IDictionary<string, string>
{
    string HeaderBlock { get; }

    Stream HeaderStream { get; }
}