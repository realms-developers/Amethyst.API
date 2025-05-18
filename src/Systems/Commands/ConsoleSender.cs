
using System.Globalization;
using Amethyst.Infrastructure;
using Amethyst.Text;

namespace Amethyst.Systems.Commands;

public sealed class ConsoleSender : ICommandSender
{
    public SenderType Type => SenderType.Console;

    public string Name => "Console";

    public string Language { get; set; } = AmethystSession.Profile.DefaultLanguage;

    public bool HasPermission(string permission) => true;
    public bool HasChestPermission(int x, int y) => true;
    public bool HasChestEditPermission(int x, int y) => true;
    public bool HasSignPermission(int x, int y) => true;
    public bool HasSignEditPermission(int x, int y) => true;
    public bool HasTEPermission(int x, int y) => true;
    public bool HasTilePermission(int x, int y, int? width = null, int? height = null) => true;

    public void ReplyError(string text, params object?[] args) =>
        AmethystLog.System!.Error(nameof(ConsoleSender),
            string.Format(CultureInfo.InvariantCulture, Localization.Get(text, AmethystSession.Profile.DefaultLanguage), args).RemoveColorTags());

    public void ReplyInfo(string text, params object?[] args) =>
        AmethystLog.System!.Info(nameof(ConsoleSender),
            string.Format(CultureInfo.InvariantCulture, Localization.Get(text, AmethystSession.Profile.DefaultLanguage), args).RemoveColorTags());

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
        => pages.SendPage(this, header, footer, footerArgs, showPageName, page);

    public void ReplySuccess(string text, params object?[] args) =>
        AmethystLog.System!.Info(nameof(ConsoleSender),
            string.Format(CultureInfo.InvariantCulture, Localization.Get(text, AmethystSession.Profile.DefaultLanguage), args).RemoveColorTags());

    public void ReplyWarning(string text, params object?[] args) =>
        AmethystLog.System!.Warning(nameof(ConsoleSender),
            string.Format(CultureInfo.InvariantCulture, Localization.Get(text, AmethystSession.Profile.DefaultLanguage), args).RemoveColorTags());
}
