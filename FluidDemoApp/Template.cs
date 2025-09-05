using System.Text;
using Fluid;
using Fluid.Values;
using FluidDemoApp.DBContext;
using FluidDemoApp.Helpers;
using FluidDemoApp.Models;

namespace FluidDemoApp;

public static class Template
{
    public static async Task RenderAsync(Dictionary<string, object?> variables,
        string templateName,
        DataDetailsModel dataDetails,
        string cssFileName = "report.css")
    {
        var options = new TemplateOptions();

        options.MemberAccessStrategy.Register<DataDetailsModel>();
        options.MemberAccessStrategy.Register<AssessmentDetailsEntity>();
        options.MemberAccessStrategy.Register<FindingsDetailsEntity>();
        options.MemberAccessStrategy.Register<OtherDetailsEntity>();
        options.ValueConverters.Add(x => x is Enum enumValue ? new StringValue(enumValue.ToString()) : null);
        
        var templateContext = new TemplateContext(options);
        foreach (var (key, value) in variables) templateContext.SetValue(key, value);

        //Dynamic data
        templateContext.SetValue("report", dataDetails);
        
        var parser = new FluidParser();
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "base.html");
        var cssPath = Path.Combine(AppContext.BaseDirectory, "Templates", cssFileName);

        var baseTemplate = await File.ReadAllTextAsync(templatePath);
        var css = await File.ReadAllTextAsync(cssPath);
        
        var templatesDir = Path.Combine(AppContext.BaseDirectory, "Templates");
        var coverPath = Path.Combine(templatesDir, "cover.html");
        var disclaimerPath = Path.Combine(templatesDir, "disclaimer.html");

        var coverHtml = File.Exists(coverPath)
            ? await RenderPartialAsync(coverPath, templateContext, parser)
            : "";
        var disclaimerHtml = File.Exists(disclaimerPath)
            ? await RenderPartialAsync(disclaimerPath, templateContext, parser)
            : "";
        var prefaceHtml = coverHtml + disclaimerHtml;
        
        //Pass 1 – provisional ToC (no page numbers)
        TableOfContentCollector.Reset();
        
        var contentBuilder = new ContentBuilder(parser, templateContext);
        var templateContent = await contentBuilder.RenderTemplateAsync(templateName);

        //Hold only links no page numbers, why its temp
        var tableOfContentTemp = TableOfContentCollector.GenerateHtml();
        var initialHtml = BuildHtml(baseTemplate, css, prefaceHtml, tableOfContentTemp, templateContent);
        
        var tmpPdf = Path.GetFullPath("report_tmp.pdf");
        await GenerateReport.PdfAsync(initialHtml, tmpPdf);
        
        var pagesMapped = TableOfContentPageExtractor.Extract(tmpPdf);
        
        var tableOfContentFinal = TableOfContentCollector.GenerateHtmlWithPages(pagesMapped);
        var finalHtml = BuildHtml(baseTemplate, css, prefaceHtml, tableOfContentFinal, templateContent);
        
        var finalPdf = Path.GetFullPath("report.pdf");
        await GenerateReport.PdfAsync(finalHtml, finalPdf);
        
        TryDelete(tmpPdf);
    }
    
    private static string BuildHtml(
        string baseTemplate,
        string css,
        string prefaceHtml,
        string tocHtml,
        StringBuilder content) =>
        baseTemplate
            .Replace("{{TITLE}}", "Report")
            .Replace("{{STYLES}}", $"<style>\n{css}\n</style>")
            .Replace("{{CONTENT}}", prefaceHtml + tocHtml + content.ToString());
    
    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        } catch { /* ignore */ }
    }
    
    private static async Task<string> RenderPartialAsync(string path, TemplateContext context, FluidParser parser)
    {
        var text = await File.ReadAllTextAsync(path);
        if (!parser.TryParse(text, out var template, out var err))
            throw new InvalidOperationException($"Partial parse error ({Path.GetFileName(path)}): {err}");
        return await template.RenderAsync(context);
    }
}