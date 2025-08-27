namespace FluidDemoApp.DBContext;

public class AssessmentTemplateBindingEntity
{
    public Guid AssessmentId { get; set; }
    public Guid TemplateId { get; set; }
    public int Version { get; set; } = 1;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }   
}