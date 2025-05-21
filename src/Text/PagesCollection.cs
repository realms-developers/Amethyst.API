using System.Collections;
using System.Globalization;
using System.Text;
using Amethyst.Systems.Commands;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Text;

public sealed class PagesCollection : IEnumerable<TextPage>
{
    private readonly List<TextPage> _pages = [];

    public PagesCollection(IEnumerable<TextPage> pages) => _pages.AddRange(pages);
    public PagesCollection() { }

    public IReadOnlyList<TextPage> Pages => _pages.AsReadOnly();

    public static List<string> PageifyItems(IEnumerable<string> items, int maxLineLength = 80)
    {
        List<string> lines = [];
        StringBuilder currentLine = new(maxLineLength);
        List<string> itemsList = [.. items];
        int count = itemsList.Count;

        for (int i = 0; i < count; i++)
        {
            string item = itemsList[i];
            string separator = i == count - 1 ? string.Empty : ", ";

            if (currentLine.RemoveColorTags().Length + item.RemoveColorTags().Length + separator.Length > maxLineLength)
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear();
            }

            currentLine.Append(item).Append(separator);
        }

        if (currentLine.Length > 0)
        {
            lines.Add(currentLine.ToString());
        }

        return lines;
    }

    public static PagesCollection CreateFromList(IEnumerable<string> items, int maxLineLength = 80, int linesPerPage = 5)
        => SplitByPages(PageifyItems(items, maxLineLength), linesPerPage);

    public static PagesCollection SplitByPages(IEnumerable<string> lines, int linesPerPage = 5)
    {
        List<TextPage> pages = [];
        List<string> lineList = [.. lines];
        int pageCount = (int)Math.Ceiling(lineList.Count / (double)linesPerPage);

        for (int i = 0; i < pageCount; i++)
        {
            var pageLines = lineList
                .Skip(i * linesPerPage)
                .Take(linesPerPage)
                .ToList();

            pages.Add(new TextPage($"#{i + 1}", pageLines, null, false));
        }

#pragma warning disable IDE0306
        return new(pages);
#pragma warning restore IDE0306
    }

    public void SendPage(IAmethystUser user, IMessageProvider provider, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
    {
        var visiblePages = _pages
            .Where(p => !p.IsDisabled && (p.ShowPermission == null || user.Permissions.HasPermission(p.ShowPermission) == PermissionAccess.HasPermission))
            .ToList();

        if (visiblePages.Count == 0)
        {
            provider.ReplyError("commands.noAvailablePages");
            return;
        }

        page = Math.Clamp(page, 1, visiblePages.Count) - 1;
        TextPage currentPage = visiblePages[page];

        if (header != null)
        {
            provider.ReplySuccess(
                $"[c/094729:===] [c/35875f:[][c/43d990:{page + 1}] [c/35875f:|] [c/43d990:{visiblePages.Count}][c/35875f:]] [c/11d476:{Localization.Get(header, provider.Language)}]");
        }

        foreach (string text in currentPage._lines)
        {
            provider.ReplyInfo(text);
        }

        if (footer != null)
        {
            string footerText = string.Format(CultureInfo.InvariantCulture,
                Localization.Get(footer, provider.Language),
                footerArgs ?? []);

            string pageInfo = showPageName ? $"[c/3d8562:{currentPage.Name}] [c/11633c:|] " : "";
            provider.ReplySuccess($"[c/094729:===] {pageInfo}{footerText}");
        }
    }

    public void AddPage(TextPage page) => _pages.Add(page);
    public void RemovePage(TextPage page) => _pages.Remove(page);
    public void RemovePage(string pageName) => _pages.RemoveAll(p => p.Name == pageName);

    public IEnumerator<TextPage> GetEnumerator() => _pages.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
