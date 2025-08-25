using System.Text;
using Fluid;
using FluidDemoApp.Models;
using FluidDemoApp.Repositories;
using FluidDemoApp.Sections;

namespace FluidDemoApp.Helpers;

public class ContentBuilder(FluidParser parser, TemplateContext context)
{
    public async Task<StringBuilder> RenderAllAsync()
    {
        var stringBuilder = new StringBuilder();
        var sections = SectionRepository.Load().OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();

        foreach (var section in sections)
        {
            switch (section.Type)
            {
                case SectionType.Text:
                {
                    var model = section.Data.ToObject<SectionModel>()!;
                    stringBuilder.Append(await Sections.Sections.RenderSectionHtml(model, context, parser));
                    break;
                }
                case SectionType.Table:
                {
                    var model = section.Data.ToObject<TableSectionModel>()!;
                    stringBuilder.Append(TableSections.RenderTableHtml(model, context, parser));
                    break;
                }
                case SectionType.Image:
                {
                    var model = section.Data.ToObject<ImageSectionModel>()!;
                    stringBuilder.Append(ImageSections.RenderImageHtml(model, context, parser));
                    break;
                }
                case SectionType.List:
                {
                    var model = section.Data.ToObject<ListSectionModel>()!;
                    stringBuilder.Append(ListSections.RenderListHtml(model, context, parser));
                    break;
                }
            }
        }
        return stringBuilder;
    }
    
    // public async Task<StringBuilder> BuildSectionsAsync()
    // {
    //     var templateContent = new StringBuilder();
    //     var sections = Sections.Sections.Create();
    //     
    //     foreach (var section in sections)
    //     {
    //         if (!parser.TryParse(section.HeadingTemplate, out var headingTemplate, out var headingError))
    //             throw new InvalidOperationException($"Heading template error: {headingError}");
    //
    //         var heading = await headingTemplate.RenderAsync(context);
    //         templateContent.Append(section.Level == 1 
    //             ? $"<h1>{System.Net.WebUtility.HtmlEncode(heading)}</h1>\n" 
    //             : $"<h2>{System.Net.WebUtility.HtmlEncode(heading)}</h2>\n");
    //
    //         if (!parser.TryParse(section.BodyTemplate, out var bodyTemplate, out var bodyError))
    //             throw new InvalidOperationException($"Body template error: {bodyError}");
    //
    //         var bodyHtml = await bodyTemplate.RenderAsync(context);
    //         templateContent.Append($"<div>{bodyHtml}</div>\n");
    //     }
    //     
    //     return templateContent;
    // }
    //
    // public StringBuilder BuildTableSection()
    // {
    //     var templateContent = new StringBuilder();
    //     var tables = new List<TableSectionModel>();
    //     Console.WriteLine("\nAdd tables? Type 'y' to create a table, anything else to skip.");
    //     if ((Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
    //     {
    //         while (true)
    //         {
    //             var tableSection = TableSections.CreateTableSection();
    //             tables.Add(tableSection);
    //
    //             Console.Write("Add another table? (y/n): ");
    //             var more = (Console.ReadLine() ?? "").Trim();
    //             if (!more.Equals("y", StringComparison.OrdinalIgnoreCase)) break;
    //         }
    //     }
    //
    //     foreach (var table in tables)
    //     {
    //         var tableHtml = TableSections.RenderTableHtml(table, context, parser);
    //         templateContent.Append(tableHtml);
    //     }
    //     
    //     return templateContent;
    // }
    //
    // public StringBuilder BuildImageSection()
    // {
    //     var templateContent = new StringBuilder();
    //     var images = new List<ImageSectionModel>();
    //     
    //     Console.WriteLine("\nAdd images? Type 'y' to add one, anything else to skip.");
    //     if ((Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
    //     {
    //         while (true)
    //         {
    //             var img = ImageSections.GetImage();
    //             images.Add(img);
    //
    //             Console.Write("Add another image? (y/n): ");
    //             var more = (Console.ReadLine() ?? "").Trim();
    //             if (!more.Equals("y", StringComparison.OrdinalIgnoreCase)) break;
    //         }
    //     }
    //     
    //     foreach (var img in images)
    //     {
    //         var imgHtml = ImageSections.RenderImageHtml(img, context, parser);
    //         templateContent.Append(imgHtml);
    //     }
    //
    //     return templateContent;
    // }
    //
    // public StringBuilder BuildListSection()
    // {
    //     var templateContent = new StringBuilder();
    //     var lists = new List<ListSectionModel>();
    //     
    //     Console.WriteLine("\nAdd lists? Type 'y' to create one, anything else to skip.");
    //     if ((Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
    //     {
    //         while (true)
    //         {
    //             var list = ListSections.Create();
    //             lists.Add(list);
    //
    //             Console.Write("Add another list? (y/n): ");
    //             var more = (Console.ReadLine() ?? "").Trim();
    //             if (!more.Equals("y", StringComparison.OrdinalIgnoreCase)) break;
    //         }
    //     }
    //     
    //     foreach (var list in lists)
    //     {
    //         var listHtml = ListSections.RenderListHtml(list, context, parser);
    //         templateContent.Append(listHtml);
    //     }
    //
    //     return templateContent;
    // }
}