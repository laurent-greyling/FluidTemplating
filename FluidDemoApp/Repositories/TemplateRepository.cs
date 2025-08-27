using FluidDemoApp.Models;
using Newtonsoft.Json;

namespace FluidDemoApp.Repositories;

public static class TemplateRepository
{
    public static List<TemplateModel> Load()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        if (!File.Exists(FilePath)) return [];
        var templateText = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<List<TemplateModel>>(templateText) ?? [];
    }

    public static void Save(List<TemplateModel> templates)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var templateText = JsonConvert.SerializeObject(templates, Formatting.Indented);
        File.WriteAllText(FilePath, templateText);
    }

    public static void AddOrUpdate(TemplateModel template)
    {
        var templates = Load();
        var existing = templates.FirstOrDefault(x => x.Id == template.Id)
                       ?? templates.FirstOrDefault(x => x.Name.Equals(template.Name, StringComparison.Ordinal));

        if (existing == null)
            templates.Add(template);
        else
        {
            existing.Name = template.Name;
            existing.Order = template.Order;
            existing.SectionReferences = template.SectionReferences;
        }

        templates = templates.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();
        Save(templates);
    }
    
    private static readonly string FilePath =
        Path.Combine(AppContext.BaseDirectory, "Data", "templates.json");
}