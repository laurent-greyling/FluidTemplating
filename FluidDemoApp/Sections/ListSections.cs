using System.Text;
using Fluid;
using FluidDemoApp.Models;

namespace FluidDemoApp.Sections;

public static class ListSections
{
    public static ListSectionModel Create()
    {
        Console.WriteLine("\nCreate a list:");

        Console.Write("Heading (Fluid allowed, or leave empty): ");
        var heading = Console.ReadLine() ?? "";

        ListKind kind;
        while (true)
        {
            Console.Write("Type (bulleted/numbered/checklist): ");
            var listType = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (listType is "bulleted" or "bullet" or "ul") { kind = ListKind.Bulleted; break; }
            if (listType is "numbered" or "ol" or "ordered") { kind = ListKind.Numbered; break; }
            if (listType is "checklist" or "check") { kind = ListKind.Checklist; break; }
            Console.WriteLine("Please enter bulleted, numbered, or checklist.");
        }

        int? start = null;
        if (kind == ListKind.Numbered)
        {
            Console.Write("Start number (empty = 1): ");
            var startNumber = (Console.ReadLine() ?? "").Trim();
            if (int.TryParse(startNumber, out var number) && number > 0) start = number;
        }

        Console.Write("Tight spacing? (y/n): ");
        var tight = (Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

        Console.Write("Dynamic list from data source? (y/n): ");
        var dynamic = (Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

        if (dynamic)
        {
            Console.Write("Enter data source (e.g. report.Findings): ");
            var source = (Console.ReadLine() ?? "").Trim();

            var fields = new List<string>();
            Console.WriteLine("Enter field templates (e.g. {{ item.Severity }}). One per line. Type END to finish.");
            while (true)
            {
                var line = Console.ReadLine();
                if (line is not null && line.Trim() == "END") break;
                fields.Add(line ?? "");
            }

            return new ListSectionModel
            {
                HeadingTemplate = heading,
                Kind = kind,
                StartNumber = start,
                Tight = tight,
                Source = source,
                Fields = fields
            };
        }
        
        Console.WriteLine("Enter list items (Fluid allowed). One per line. Finish with a single line: END");
        var items = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (line is not null && line.Trim() == "END") break;
            items.Add(line ?? "");
        }

        return new ListSectionModel
        {
            HeadingTemplate = heading,
            Kind = kind,
            StartNumber = start,
            Tight = tight,
            ItemTemplates = items
        };
    }
    
    public static string RenderListHtml(ListSectionModel listSection, TemplateContext templateContext, FluidParser parser)
    {
        var stringBuilder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(listSection.HeadingTemplate))
        {
            if (!parser.TryParse(listSection.HeadingTemplate, out var headingTemplate, out var error))
                throw new InvalidOperationException($"List heading template error: {error}");
            var heading = headingTemplate.Render(templateContext);
            stringBuilder.Append($"<h2>{System.Net.WebUtility.HtmlEncode(heading)}</h2>\n");
        }

        var htmlClass = listSection.Tight ? "list tight" : "list";
        switch (listSection.Kind)
        {
            case ListKind.Numbered:
                var startAttr = listSection.StartNumber is int n && n > 1 ? $" start=\"{n}\"" : "";
                stringBuilder.Append($"<ol class=\"{htmlClass}\"{startAttr}>\n");
                break;
            case ListKind.Checklist:
                stringBuilder.Append($"<ul class=\"{htmlClass} checklist\">\n");
                break;
            default:
                stringBuilder.Append($"<ul class=\"{htmlClass}\">\n");
                break;
        }

        if (!string.IsNullOrWhiteSpace(listSection.Source))
        {
            var liBody = string.Join(
                "\n        ",
                listSection.Fields.Select(f => $"<div>{f}</div>")
            );

            var liTemplate = listSection.Kind == ListKind.Checklist
                ? $"<li><span class=\"box\">&#x2610;</span> <span class=\"txt\">{liBody}</span></li>"
                : $"<li>{liBody}</li>";

            // Let Fluid resolve the dotted path by using a real {% for %} loop internally
            var loopTemplateText = $"{{% for item in {listSection.Source} %}}{liTemplate}{{% endfor %}}";

            if (!parser.TryParse(loopTemplateText, out var loopTemplate, out var loopErr))
                throw new InvalidOperationException($"Dynamic list loop template error: {loopErr}");

            var itemsHtml = loopTemplate.Render(templateContext);
            stringBuilder.Append(itemsHtml);
        }
        else
        {
            foreach (var itemTemplateText in listSection.ItemTemplates)
            {
                if (!parser.TryParse(itemTemplateText ?? "", out var itemTemplate, out var error))
                    throw new InvalidOperationException($"List item template error: {error}");

                var rendered = itemTemplate.Render(templateContext);
                if (listSection.Kind == ListKind.Checklist)
                {
                    stringBuilder.Append($"  <li><span class=\"box\">&#x2610;</span> <span class=\"txt\">{rendered}</span></li>\n");
                }
                else
                {
                    stringBuilder.Append($"  <li>{rendered}</li>\n");
                }
            }
        }

        stringBuilder.Append(listSection.Kind == ListKind.Numbered ? "</ol>\n" : "</ul>\n");
        return stringBuilder.ToString();
    }
}