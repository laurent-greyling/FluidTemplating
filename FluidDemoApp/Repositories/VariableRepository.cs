using Newtonsoft.Json;

namespace FluidDemoApp.Repositories;

public static class VariableRepository
{
    public static Dictionary<string, object?> Load()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        if (!File.Exists(FilePath))
            return new Dictionary<string, object?>();

        var json = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<Dictionary<string, object?>>(json)
               ?? new Dictionary<string, object?>();
    }

    public static void Save(Dictionary<string, object?> variables)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var json = JsonConvert.SerializeObject(variables, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static void MergeAndSave(Dictionary<string, object?> newVariables)
    {
        var current = Load();
        foreach (var (key, value) in newVariables)
            current[key] = value;
        Save(current);
    }
    
    private static readonly string FilePath =
        Path.Combine(AppContext.BaseDirectory, "Data", "variables.json");
}