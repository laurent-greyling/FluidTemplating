namespace FluidDemoApp.Helpers;

public static class ThemeSelector
{
    private static string TemplatesDir => Path.Combine(AppContext.BaseDirectory, "Templates");

    public static List<string?> ListCssThemes(string pattern = "report*.css")
    {
        if (!Directory.Exists(TemplatesDir)) return ["report.css"];
        return Directory.EnumerateFiles(TemplatesDir, pattern)
            .Select(Path.GetFileName)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string? PickVisualTemplate()
    {
        var themes = ListCssThemes();
        if (themes.Count == 0) return "report.css";

        Console.WriteLine("\nChoose visual theme:");
        for (int i = 0; i < themes.Count; i++)
            Console.WriteLine($"{i + 1}) {themes[i]}");

        Console.Write("Select number (default 1): ");
        var raw = (Console.ReadLine() ?? "").Trim();

        if (int.TryParse(raw, out var n) && n >= 1 && n <= themes.Count)
            return themes[n - 1];

        return themes[0];
    }
}