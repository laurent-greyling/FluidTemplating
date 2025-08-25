using System.Text;
using Fluid;
using FluidDemoApp.Models;

namespace FluidDemoApp.Sections;

public static class Sections
{
    public static SectionModel Create()
    {
        Console.WriteLine("\nAdd sections. Type 'done' when prompted for heading level to finish.");
        Console.Write("Heading level (1 or 2): ");

        var levelText = (Console.ReadLine() ?? "").Trim();

        if (!int.TryParse(levelText, out var level) || (level != 1 && level != 2))
        {
            Console.WriteLine("Please enter 1 or 2.");
            return new SectionModel();
        }

        Console.Write("Heading text (Fluid allowed, e.g., 'Findings for {{ company }}'): ");
        var headingTemplate = Console.ReadLine() ?? "";

        Console.WriteLine("Enter body (Fluid allowed). End with a line containing only: END");

        var bodyLines = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (line is not null && line.Trim() == "END") break;
            bodyLines.Add(line ?? "");
        }

        Console.WriteLine("SectionModel added.\n");

        return new SectionModel
        {
            Level = level,
            HeadingTemplate = headingTemplate,
            BodyTemplate = string.Join(Environment.NewLine, bodyLines)
        };
    }

    public static async Task<string> RenderSectionHtml(SectionModel section, 
        TemplateContext templateContext,
        FluidParser parser)
    {
        var stringBuilder = new StringBuilder();
        
        if (!parser.TryParse(section.HeadingTemplate, out var headingTemplate, out var headingError))
            throw new InvalidOperationException($"Heading template error: {headingError}");
    
        var heading = await headingTemplate.RenderAsync(templateContext);
        stringBuilder.Append(section.Level == 1 
            ? $"<h1>{System.Net.WebUtility.HtmlEncode(heading)}</h1>\n" 
            : $"<h2>{System.Net.WebUtility.HtmlEncode(heading)}</h2>\n");
    
        if (!parser.TryParse(section.BodyTemplate, out var bodyTemplate, out var bodyError))
            throw new InvalidOperationException($"Body template error: {bodyError}");
    
        var bodyHtml = await bodyTemplate.RenderAsync(templateContext);
        stringBuilder.Append($"<div>{bodyHtml}</div>\n");
        return stringBuilder.ToString();
    }
}