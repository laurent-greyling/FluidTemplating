using FluidDemoApp.DBContext;
using Newtonsoft.Json;

namespace FluidDemoApp.Repositories;

public static class AssessmentTemplateBindingRepository
{
    public static List<AssessmentTemplateBindingEntity> Load()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        if (!File.Exists(FilePath)) 
            return [];
        
        var json = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<List<AssessmentTemplateBindingEntity>>(json) ?? [];
    }

    public static void Save(List<AssessmentTemplateBindingEntity> list)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(list, Formatting.Indented));
    }

    public static void Upsert(AssessmentTemplateBindingEntity binding)
    {
        var list = Load();

        var existing = list.FirstOrDefault(b =>
            b.AssessmentId == binding.AssessmentId);

        if (existing is null)
        {
            list.Add(binding);
        }
        else
        {
            existing.TemplateId = binding.TemplateId;
            existing.Version = binding.Version;
            existing.CreatedDate = binding.CreatedDate;
            existing.CreatedBy = binding.CreatedBy;
        }

        Save(list);
    }

    public static IReadOnlyList<AssessmentTemplateBindingEntity> GetForAssessment(Guid assessmentId)
        => Load().Where(b => b.AssessmentId == assessmentId).ToList();
    
    private static readonly string FilePath =
        Path.Combine(AppContext.BaseDirectory, "Data", "assessment-templates.json");
}