using System.Text;
using System.Text.RegularExpressions;

namespace FluidDemoApp.Helpers;

public static class TableOfContentCollector
{
    private static readonly List<(int Level, string Text, string Id)> _items = [];

    public static void Reset() => _items.Clear();

    public static string GenerateAnchorId(string text, int occurrenceIndex = 0)
    {
        var anchorId = Regex.Replace(text.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        if (string.IsNullOrEmpty(anchorId)) anchorId = "section";
        return occurrenceIndex == 0 ? anchorId : $"{anchorId}-{occurrenceIndex}";
    }

    public static void Add(int level, string text)
    {
        // ensure unique id
        var baseId = GenerateAnchorId(text);
        var id = baseId;
        var n = 1;
        while (_items.Any(i => i.Id == id))
            id = GenerateAnchorId(text, n++);

        _items.Add((level, text, id));
    }

    public static string GenerateHtml()
    {
        if (_items.Count == 0) return string.Empty;

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<div class=\"toc\">");
        stringBuilder.AppendLine("<h1>Table of Contents</h1>");
        stringBuilder.AppendLine("<ul class=\"toc-list\">");

        foreach (var (level, text, id) in _items)
        {
            // simple indent by level (h1=1, h2=2, etc.)
            var indent = new string(' ', Math.Clamp(level - 1, 0, 4) * 2);
            stringBuilder.Append(indent).Append("<li class=\"toc-l").Append(level).Append("\">")
                .Append("<a href=\"#").Append(id).Append("\">")
                .Append(System.Net.WebUtility.HtmlEncode(text))
                .AppendLine("</a></li>");
        }

        stringBuilder.AppendLine("</ul>");
        stringBuilder.AppendLine("</div>");
        return stringBuilder.ToString();
    }
    
    public static string GenerateHtmlWithPages(IReadOnlyDictionary<string,int> pageMap)
    {
        if (_items.Count == 0) return string.Empty;

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("<div class=\"toc\">");
        stringBuilder.AppendLine("<h1>Table of Contents</h1>");
        stringBuilder.AppendLine("<ul class=\"toc-list\">");

        foreach (var (level, text, id) in _items)
        {
            var page = pageMap.TryGetValue(id, out var p) ? p.ToString() : "";
            stringBuilder.AppendLine($@"
                <li class=""toc-row toc-l{level}"">
                  <a href=""#{id}"" class=""toc-title"">{System.Net.WebUtility.HtmlEncode(text)}</a>
                  <span class=""toc-dots""></span>
                  <span class=""toc-page"">{page}</span>
                </li>");
        }

        stringBuilder.AppendLine("</ul>");
        stringBuilder.AppendLine("</div>");
        return stringBuilder.ToString();
    }
}