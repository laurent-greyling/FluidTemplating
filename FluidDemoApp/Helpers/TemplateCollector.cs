using FluidDemoApp.Models;
using FluidDemoApp.Repositories;

namespace FluidDemoApp.Helpers;

public static class TemplateCollector
{
    public static void AddOrUpdateTemplate()
    {
        Console.WriteLine("\nAdd/Update Template");
        Console.Write("Template name: ");
        var name = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name required."); return;
        }

        Console.Write("Order (integer, default 100): ");
        var orderText = (Console.ReadLine() ?? "").Trim();
        var order = int.TryParse(orderText, out var o) ? o : 100;

        var allSections = SectionRepository.Load()
            .OrderBy(s => s.Order)
            .ThenBy(s => s.Name)
            .ToList();

        if (allSections.Count == 0)
        {
            Console.WriteLine("No sections available. Create some first.");
            return;
        }

        Console.WriteLine("\nAvailable sections:");
        for (var i = 0; i < allSections.Count; i++)
        {
            Console.WriteLine($"{i+1}) {allSections[i].Name} (Id: {allSections[i].Id})");
        }

        Console.Write("Enter section numbers to include (comma-separated, e.g. 1,3,4): ");
        var pickedSections = (Console.ReadLine() ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());

        var references = new List<TemplateSectionReference>();
        var position = 1;
        foreach (var pickedSection in pickedSections)
        {
            if (int.TryParse(pickedSection, out var idx) && idx >= 1 && idx <= allSections.Count)
            {
                references.Add(new TemplateSectionReference
                {
                    SectionId = allSections[idx - 1].Id,
                    Order = position++
                });
            }
        }

        var template = new TemplateModel
        {
            Name = name,
            Order = order,
            SectionReferences = references
        };

        TemplateRepository.AddOrUpdate(template);
        Console.WriteLine($"Template '{name}' saved with {references.Count} section(s).");
    }
}