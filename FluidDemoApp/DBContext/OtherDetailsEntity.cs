namespace FluidDemoApp.DBContext;

public class OtherDetailsEntity
{
    public Guid AssessmentId { get; set; }
    public Guid ClientId { get; set; }
    public bool IsProved { get; set; }
    public string? PathToProof { get; set; }
    public string? Contact { get; set; }
}