using System.Collections;
using System.Globalization;
using Amethyst.Commands;

namespace Amethyst.Text;

public sealed class PagesCollection : IEnumerable<TextPage>
{
    public static List<string> PageifyItems(IEnumerable<string> items, int maxLineLength = 80)
    {
        List<string> lines = new List<string>() { "" };
        int curIndex = 0;
        int index = 0;
        int count = items.Count();

        foreach (var curText in items)
        {
            string curItem = curText + (index == count - 1 ? "" : ", ");

            if ((curText + curItem).Length > maxLineLength)
            {
                lines.Add(curItem);
                curIndex++;
            }
            else
            {
                lines[curIndex] += curItem;
            }
            index++;
        }

        return lines;
    }

    public static PagesCollection CreateFromList(IEnumerable<string> items, int maxLineLength = 80, int linesPerPage = 5)
    {
        return SplitByPages(PageifyItems(items, maxLineLength), linesPerPage);
    }

    public static PagesCollection SplitByPages(IEnumerable<string> lines, int linesPerPage = 5)
    {
        List<TextPage> pages = new List<TextPage>();
        int currentPage = 0;

        int i = 0;
        foreach (string line in lines)
        {
            if (pages.Count == currentPage)
                pages.Add(new TextPage($"#{currentPage + 1}", new (), null, false));
            
            pages[currentPage].Add(line);

            if (i > 0 && i % linesPerPage == 0)
                currentPage++;
            
            i++;
        }

        return new PagesCollection(pages);
    }

    public PagesCollection(IEnumerable<TextPage> pages)
    {
        _pages = new List<TextPage>(pages);
    }

    public PagesCollection()
    {
        _pages = new List<TextPage>();
    }

    internal List<TextPage> _pages;

    public IReadOnlyList<TextPage> Pages => _pages.AsReadOnly();

    public void SendPage(ICommandSender sender, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
    {
        var pages = _pages.Where(p => p.IsDisabled == false && p.ShowPermission != null ? sender.HasPermission(p.ShowPermission) : true).ToArray();

        if (pages.Length == 0)
        {
            sender.ReplyError("$LOCALIZE commands.noAvailablePages");
            return;
        }

        page = Math.Clamp(page, 1, pages.Length) - 1;

        if (header != null)
        {
            sender.ReplySuccess($"[c/094729:===] [c/35875f:[][c/43d990:{page + 1}] [c/35875f:|] [c/43d990:{pages.Length}][c/35875f:]] [c/11d476:{sender.Language.LocalizeDirect(header)}]");
        }

        foreach (var text in pages[page]._lines)
        {
            sender.ReplyInfo(text);
        }

        if (footer != null)
        {
            string footerText = string.Format(CultureInfo.InvariantCulture, sender.Language.LocalizeDirect(footer), args: footerArgs ?? Array.Empty<object>());
            sender.ReplySuccess($"[c/094729:===] {(showPageName ? $"[c/3d8562:{pages[page].Name}] [c/11633c:|] " : "")}{footerText}");
        }
    }

    public void AddPage(TextPage page) => _pages.Add(page);
    public void RemovePage(TextPage page) => _pages.Remove(page);
    public void RemovePage(string pageName) => _pages.RemoveAll(p => p.Name == pageName);

    public IEnumerator<TextPage> GetEnumerator() => _pages.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
