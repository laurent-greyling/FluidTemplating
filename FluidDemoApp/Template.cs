using System.Text;
using Fluid;
using FluidDemoApp.Helpers;
using FluidDemoApp.Models;

namespace FluidDemoApp;

public static class Template
{
    public static async Task RenderAsync(Dictionary<string, object?> variables, string templateName)
    {
        var templateContext = new TemplateContext();
        foreach (var (key, value) in variables) templateContext.SetValue(key, value);

        var parser = new FluidParser();
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "base.html");
        var cssPath = Path.Combine(AppContext.BaseDirectory, "Templates", "report.css");

        var baseTemplate = await File.ReadAllTextAsync(templatePath);
        var css = await File.ReadAllTextAsync(cssPath);
        
        var contentBuilder = new ContentBuilder(parser, templateContext);
        var templateContent = await contentBuilder.RenderTemplateAsync(templateName);
        
        var finalHtml = baseTemplate
            .Replace("{{TITLE}}", "Report")
            .Replace("{{STYLES}}", $"<style>\n{css}\n</style>")
            .Replace("{{CONTENT}}", templateContent.ToString());

        await GenerateReport.PdfAsync(finalHtml);
    }
}