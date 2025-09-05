using System.Text.RegularExpressions;
using UglyToad.PdfPig; //currently this is used as prerelease. Only to try and generate ToC.

namespace FluidDemoApp.Helpers;

public static class TableOfContentPageExtractor
{
    private static readonly Regex Marker = new(@"\[toc:(?<id>[^\]]+)\]", RegexOptions.Compiled);

    public static Dictionary<string,int> Extract(string pdfPath)
    {
        var map = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
        using var doc = PdfDocument.Open(pdfPath);
        for (int i = 1; i <= doc.NumberOfPages; i++)
        {
            var page = doc.GetPage(i);
            var text = page.Text;// full page text
            foreach (Match m in Marker.Matches(text))
            {
                var id = m.Groups["id"].Value;
                if (!map.ContainsKey(id))
                    map[id] = i;// 1-based page number
            }
        }
        return map;
    }
}