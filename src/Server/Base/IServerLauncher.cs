namespace Amethyst.Server.Base;

public interface IServerLauncher
{
    bool IsStarted { get; }

    IServerSettings Settings  { get; }

    void Initialize();

    void StartServer();

    void StopServer(bool force = false);
}
