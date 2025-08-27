namespace FluidDemoApp.DBContext;

public static class DbContext
{
    private static readonly string BaseDir = Path.Combine(AppContext.BaseDirectory, "FakeDbTables");
    public static string Assessments => Path.Combine(BaseDir, "AssessmentDataDetails.json");
    public static string Findings => Path.Combine(BaseDir, "FindingsDataDetails.json");
    public static string Other => Path.Combine(BaseDir, "OtherDataDetails.json");
}