using System.Globalization;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Text;

namespace Amethyst.Systems.Users.Artificial.Messages;

public sealed class ArtificialMessageProvider : IMessageProvider
{
    internal ArtificialMessageProvider(ArtificialUser user, string culture)
    {
        User = user;
        Language = culture;
    }

    public ArtificialUser User { get; }

    public string Language { get; set; }

    public void ReplyError(string text, params object[] args)
        => AmethystLog.System.Verbose('@' + User.Name, "$!r$r" +string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplyInfo(string text, params object[] args)
        => AmethystLog.System.Verbose('@' + User.Name, "$!r$y" +string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplySuccess(string text, params object[] args)
        => AmethystLog.System.Verbose('@' + User.Name, "$!r$g" +string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplyWarning(string text, params object[] args)
        => AmethystLog.System.Verbose('@' + User.Name, "$!r$b" + string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
        => pages.SendPage(User, this, header, footer, footerArgs, showPageName, page);
}
