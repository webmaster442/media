namespace NMaier.SimpleDlna.Server.Interfaces;

internal interface IPrefixHandler : IHandler
{
    string Prefix { get; }
}