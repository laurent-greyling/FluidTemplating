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
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name required."); return; }

        var type = AskType();
        var order = AskOrder();

        var sectionData = type switch
        {
            SectionType.Text => JObject.FromObject(Sections.Sections.Create()),
            SectionType.Table => JObject.FromObject(TableSections.CreateTableSection()),
            SectionType.Image => JObject.FromObject(ImageSections.GetImage()),
            SectionType.List => JObject.FromObject(ListSections.Create()),
            _ => new JObject()
        };

        var existing = SectionRepository.Load().FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));

        var sectionDetials = new SectionDetailsModel()
        {
            Id = existing?.Id ?? Guid.NewGuid(),
            Name = name,
            Type = type,
            Order = order,
            Data = sectionData
        };

        SectionRepository.AddOrUpdate(sectionDetials);
        Console.WriteLine($"Section '{name}' saved (Order {order}).");
    }
    
    private static SectionType AskType()
    {
        while (true)
        {
            Console.Write("Type (1=Text, 2=Table, 3=Image, 4=List): ");
            switch ((Console.ReadLine() ?? "").Trim())
            {
                case "1": return SectionType.Text;
                case "2": return SectionType.Table;
                case "3": return SectionType.Image;
                case "4": return SectionType.List;
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
}