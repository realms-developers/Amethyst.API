using System.Globalization;
using Amethyst.Kernel.CLI;

namespace Amethyst.Logging;

public sealed class ServerLogger : IDisposable
{
    public ServerLogger(string file, LogLevel logLevel = LogLevel.Debug)
    {
        _stream = File.Open(file, FileMode.OpenOrCreate);
        _writer = new StreamWriter(_stream);

        LogLevel = logLevel;
    }

    private FileStream _stream { get; }
    private TextWriter _writer { get; }
    private bool _isDisposed { get; set; }

    public LogLevel LogLevel { get; set; }

    public void Dispose()
    {
        _stream.Dispose();
        _writer.Dispose();

        _isDisposed = true;
    }

    private void FileLog(string date, LogLevel level, string section, string text)
    {
        _writer.WriteLine($"{level,-9} ({date}) [{section}]: {text}");
    }

    private void Log(LogLevel level, string section, string text, string modernColor)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (level > LogLevel)
        {
            return;
        }

        string date = GetDate();
        string prefix = ModernConsole.LevelPrefix[level];

        FileLog(date, level, section, text);
        ModernConsole.WriteLine($"{prefix} $!r$!d($!r$w{date}$!r$!d) [$!r$!b{section}$!r$!d]: $!r{modernColor}{text}");
    }

    private static string GetDate() => DateTime.UtcNow.ToString("HH:mm:ss:ffff", CultureInfo.InvariantCulture);

    public void Critical(string section, string text)
        => Log(LogLevel.Critical, section, text, "$r$!b");

    public void Error(string section, string text)
        => Log(LogLevel.Error, section, text, "$r");

    public void Warning(string section, string text)
        => Log(LogLevel.Warning, section, text, "$b");

    public void Info(string section, string text)
        => Log(LogLevel.Info, section, text, "$y");

    public void Verbose(string section, string text)
        => Log(LogLevel.Verbose, section, text, "$!d");

    public void Debug(string section, string text)
        => Log(LogLevel.Debug, section, text, "$w");
}
