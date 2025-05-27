namespace Amethyst.Server;

public interface IServerLauncher
{
    bool IsStarted { get; }

    void Initialize();

    void StartServer();

    void StopServer(bool force = false);
}
