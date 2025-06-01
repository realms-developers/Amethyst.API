using Amethyst.Kernel;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Text;

namespace Amethyst.Systems.Users.Common.Messages;

public sealed class SilentMessageProvider : IMessageProvider
{
    public SilentMessageProvider(IAmethystUser user)
    {
        User = user;
        Language = AmethystSession.Profile.DefaultLanguage;
    }

    public SilentMessageProvider(IAmethystUser user, string language)
    {
        User = user;
        Language = language;
    }

    public IAmethystUser User { get; }

    public string Language { get; set; }

    public void ReplyError(string text, params object[] args) {}

    public void ReplyInfo(string text, params object[] args) {}

    public void ReplySuccess(string text, params object[] args) {}

    public void ReplyWarning(string text, params object[] args) {}

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0) {}
}
