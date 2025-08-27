namespace FluidDemoApp.DBContext;

public class FindingsDetailsEntity
{
    public Guid AssessmentId { get; set; }
    public Guid ClientId { get; set; }
    public SeverityEnum Severity { get; set; }
    public string? Finding { get; set; }
    public string? Recommendation { get; set; }
    public string? ScreenshotPath { get; set; }
}