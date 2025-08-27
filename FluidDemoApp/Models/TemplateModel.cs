namespace FluidDemoApp.Models;

public class TemplateModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Order { get; set; } = 100;
    public List<TemplateSectionReference> SectionReferences { get; set; } = [];
}