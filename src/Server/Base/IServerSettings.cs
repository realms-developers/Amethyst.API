namespace Amethyst.Server.Base;

public interface IServerSettings
{
    byte MaxPlayers { get; }
    int Port { get; }
    string WorldPath { get; }
    string DefaultLanguage { get; }

    void SetMaxPlayers(byte maxPlayers);
    void SetPort(int port);
    void SetWorldPath(string worldPath);
    void SetDefaultLanguage(string defaultLanguage);
}
