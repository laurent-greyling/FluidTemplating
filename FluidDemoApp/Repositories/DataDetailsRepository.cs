using FluidDemoApp.DBContext;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FluidDemoApp.Repositories;

public static class DataDetailsRepository
{
    public static List<AssessmentDetailsEntity> LoadAssessments()
        => ReadList<AssessmentDetailsEntity>(DbContext.Assessments);

    public static List<FindingsDetailsEntity> LoadFindings()
        => ReadList<FindingsDetailsEntity>(DbContext.Findings);

    public static List<OtherDetailsEntity> LoadOther()
        => ReadList<OtherDetailsEntity>(DbContext.Other);

    // For a quick “picker” UI in the console
    public static List<(Guid AssessmentId, string Label)> GetAssessmentIndex(int take = 50)
        => LoadAssessments()
            .OrderBy(a => a.ClientName ?? string.Empty)
            .Take(take)
            .Select(a => (a.AssessmentId, $"{a.ClientName ?? "(no client)"} — {a.AssessmentId}"))
            .ToList();
    
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Converters =
        {
            new StringEnumConverter()
        },
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Include
    };
    
    private static List<T> ReadList<T>(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        if (!File.Exists(path)) return [];

        var text = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(text)) return [];

        try
        {
            var list = JsonConvert.DeserializeObject<List<T>>(text, JsonSettings);
            return list ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Failed to read {Path.GetFileName(path)}: {ex.Message}");
            return [];
        }
    }
}