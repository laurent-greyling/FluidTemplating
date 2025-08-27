using System.Text;
using Fluid;
using FluidDemoApp.Models;
using FluidDemoApp.Repositories;
using FluidDemoApp.Sections;

namespace FluidDemoApp.Helpers;

public class ContentBuilder(FluidParser parser, TemplateContext context)
{
    public async Task<StringBuilder> RenderTemplateAsync(string templateName)
    {
        var template = TemplateRepository.Load()
            .FirstOrDefault(t => string.Equals(t.Name, templateName, StringComparison.Ordinal));
        if (template is null)
            throw new InvalidOperationException($"Template '{templateName}' not found.");

        return await RenderTemplateAsync(template);
    }

    public async Task<StringBuilder> RenderTemplateAsync(Guid templateId)
    {
        var template = TemplateRepository.Load().FirstOrDefault(t => t.Id == templateId);
        if (template is null)
            throw new InvalidOperationException($"Template '{templateId}' not found.");

        return await RenderTemplateAsync(template);
    }
    
    public async Task<StringBuilder> RenderAllAsync()
    {
        var stringBuilder = new StringBuilder();
        var sections = SectionRepository.Load()
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .ToList();

        foreach (var section in sections)
        {
            stringBuilder.Append(await RenderSectionAsync(section));
        }

        return stringBuilder;
    }

    private async Task<string> RenderSectionAsync(SectionDetailsModel section)
    {
        var stringBuilder = new StringBuilder();

        foreach (var block in section.Children.OrderBy(b => b.Order))
        {
            stringBuilder.Append(await RenderBlockAsync(block));
        }

        return stringBuilder.ToString();
    }

    private async Task<string> RenderBlockAsync(SectionBlockDetailsModel block) =>
        block.Type switch
        {
            SectionType.Text  => await RenderTextBlockAsync(block.Data.ToObject<SectionModel>()!),
            SectionType.Table => RenderTableBlock(block.Data.ToObject<TableSectionModel>()!),
            SectionType.Image => RenderImageBlock(block.Data.ToObject<ImageSectionModel>()!),
            SectionType.List  => RenderListBlock(block.Data.ToObject<ListSectionModel>()!),
            _ => string.Empty
        };

    private async Task<string> RenderTextBlockAsync(SectionModel model) =>
        await Sections.Sections.RenderSectionHtml(model, context, parser);

    private string RenderTableBlock(TableSectionModel model) =>
        TableSections.RenderTableHtml(model, context, parser);

    private string RenderImageBlock(ImageSectionModel model) =>
        ImageSections.RenderImageHtml(model, context, parser);

    private string RenderListBlock(ListSectionModel model) =>
        ListSections.RenderListHtml(model, context, parser);
    
    private async Task<StringBuilder> RenderTemplateAsync(TemplateModel template)
    {
        var stringBuilder = new StringBuilder();

        var sectionsById = SectionRepository.Load().ToDictionary(s => s.Id, s => s);

        foreach (var r in template.SectionReferences.OrderBy(r => r.Order))
        {
            if (!sectionsById.TryGetValue(r.SectionId, out var section))
            {
                stringBuilder.Append($"<!-- Missing section {r.SectionId} -->");
                continue;
            }
            stringBuilder.Append(await RenderSectionAsync(section));
        }
        return stringBuilder;
    }
}