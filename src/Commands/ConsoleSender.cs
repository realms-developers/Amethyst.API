
using System.Globalization;
using Amethyst.Core;
using Amethyst.Localization;
using Amethyst.Text;

namespace Amethyst.Commands;

public sealed class ConsoleSender : ICommandSender
{
    public SenderType Type => SenderType.Console;
    public Language Language => LocalizationManager.FindLanguage(AmethystSession.Profile.DefaultLanguage)!;

    public string Name => "Console";

    public bool HasPermission(string permission) => true;
    public bool HasChestPermission(int x, int y) => true;
    public bool HasChestEditPermission(int x, int y) => true;
    public bool HasSignPermission(int x, int y) => true;
    public bool HasSignEditPermission(int x, int y) => true;
    public bool HasTEPermission(int x, int y) => true;
    public bool HasTilePermission(int x, int y, int? width = null, int? height = null) => true;

    public void ReplyError(string text, params object?[] args) 
    {
        AmethystLog.System!.Error("Console.Commands", string.Format(CultureInfo.InvariantCulture, Language.LocalizeDirect(text), args));
    }

    public void ReplyInfo(string text, params object?[] args)
    {
        AmethystLog.System!.Info("Console.Commands", string.Format(CultureInfo.InvariantCulture, Language.LocalizeDirect(text), args));
    }
    
    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
        => pages.SendPage(this, header, footer, footerArgs, showPageName, page);

    public void ReplySuccess(string text, params object?[] args)
    {
        AmethystLog.System!.Info("Console.Commands", string.Format(CultureInfo.InvariantCulture, Language.LocalizeDirect(text), args));
    }

    public void ReplyWarning(string text, params object?[] args)
    {
        AmethystLog.System!.Warning("Console.Commands", string.Format(CultureInfo.InvariantCulture, Language.LocalizeDirect(text), args));
    }
}
