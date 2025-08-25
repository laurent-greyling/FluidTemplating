using FluidDemoApp.Models;
using Newtonsoft.Json;

namespace FluidDemoApp.Repositories;

public static class SectionRepository
{
    public static List<SectionDetailsModel> Load()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        if (!File.Exists(FilePath)) return [];
        var sectionsText = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<List<SectionDetailsModel>>(sectionsText) ?? [];
    }

    public static void Save(List<SectionDetailsModel> sectionDetailsList)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var sectionsJson = JsonConvert.SerializeObject(sectionDetailsList, Formatting.Indented);
        File.WriteAllText(FilePath, sectionsJson);
    }

    public static void AddOrUpdate(SectionDetailsModel sectionDetails)
    {
        var list = Load();

        var existing = list.FirstOrDefault(x => x.Id == sectionDetails.Id)
                       ?? list.FirstOrDefault(x => string.Equals(x.Name, sectionDetails.Name, StringComparison.Ordinal));

        if (existing is null) list.Add(sectionDetails);
        else
        {
            existing.Name = sectionDetails.Name;
            existing.Type = sectionDetails.Type;
            existing.Order = sectionDetails.Order;
            existing.Data = sectionDetails.Data;
        }

        list = list.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();
        Save(list);
    }
    
    private static readonly string FilePath =
        Path.Combine(AppContext.BaseDirectory, "Data", "sections.json");
}