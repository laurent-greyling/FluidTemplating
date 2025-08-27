namespace FluidDemoApp.DBContext;

public class AssessmentDetailsEntity
{
    public Guid AssessmentId { get; set; }
    public Guid ClientId { get; set; }
    public string? ClientName { get; set; }
    public string? Summary { get; set; }
    public int FindingsCount { get; set; }
}