using FluidDemoApp.Models;
using FluidDemoApp.Repositories;
using FluidDemoApp.Sections;
using Newtonsoft.Json.Linq;

namespace FluidDemoApp.Helpers;

public static class SectionCollector
{
    public static void AddOrUpdateSection()
    {
        Console.WriteLine("\nAdd/Update Section");
        Console.Write("Unique section name: ");
        var name = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name required."); return;
        }

        var order = AskOrder();

        var existingSections = SectionRepository.Load();
        var section = existingSections.FirstOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal))
                      ?? new SectionDetailsModel { Name = name, Order = order };
        
        
        while (true)
        {
            var type = AskType();
            if (type is null) break;

            var block = BuildBlock(type.Value);
            if (block == null) continue;

            block.Order = section.Children.Count * 10 + 100;
            section.Children.Add(block);
            Console.WriteLine($"Added {block.Type} block.");
        }

        section.Order = order;
        SectionRepository.AddOrUpdate(section);
        Console.WriteLine($"Section '{section.Name}' saved with {section.Children.Count} block(s).");
    }
    
    private static SectionType? AskType()
    {
        while (true)
        {
            Console.Write("Type (1=Text, 2=Table, 3=Image, 4=List, 5=Done): ");
            switch ((Console.ReadLine() ?? "").Trim())
            {
                case "1": return SectionType.Text;
                case "2": return SectionType.Table;
                case "3": return SectionType.Image;
                case "4": return SectionType.List;
                case "5": return null;
                default: Console.WriteLine("Unknown."); break;
            }
        }
    }
    
    private static int AskOrder()
    {
        Console.Write("Order (integer, lower renders first, default 100): ");
        var userDefinedOrder = (Console.ReadLine() ?? "").Trim();
        return int.TryParse(userDefinedOrder, out var orderNumber) ? orderNumber : 100;
    }
    
    private static SectionBlockDetailsModel? BuildBlock(SectionType type) =>
        type switch
        {
            SectionType.Text  => new SectionBlockDetailsModel { Type = SectionType.Text,  Data = JObject.FromObject(Sections.Sections.Create()) },
            SectionType.Table => new SectionBlockDetailsModel { Type = SectionType.Table, Data = JObject.FromObject(TableSections.CreateTableSection()) },
            SectionType.Image => new SectionBlockDetailsModel { Type = SectionType.Image, Data = JObject.FromObject(ImageSections.GetImage()) },
            SectionType.List  => new SectionBlockDetailsModel { Type = SectionType.List,  Data = JObject.FromObject(ListSections.Create()) },
            _ => null
        };
}