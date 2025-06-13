using System.Globalization;
using Amethyst.Kernel;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Text;

namespace Amethyst.Systems.Users.Common.Messages;

public sealed class ListMessageProvider : IMessageProvider
{
    public ListMessageProvider(IAmethystUser user)
    {
        User = user;
        Language = AmethystSession.Profile.DefaultLanguage;
    }

    public ListMessageProvider(IAmethystUser user, string language)
    {
        User = user;
        Language = language;
    }

    public IAmethystUser User { get; }

    public string Language { get; set; }

    public IReadOnlyList<string> Messages => _messages.AsReadOnly();

    private readonly List<string> _messages = [];

    public void ReplyError(string text, params object[] args)
        => _messages.Add(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplyInfo(string text, params object[] args)
        => _messages.Add(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplySuccess(string text, params object[] args)
        => _messages.Add(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplyWarning(string text, params object[] args)
        => _messages.Add(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args).RemoveColorTags());

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
        => pages.SendPage(User, this, header, footer, footerArgs, showPageName, page);
}
