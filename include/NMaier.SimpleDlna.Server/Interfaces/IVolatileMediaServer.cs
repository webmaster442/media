namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IVolatileMediaServer
{
    bool Rescanning { get; set; }

    void Rescan();

    event EventHandler Changed;
}