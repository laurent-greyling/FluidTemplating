using Newtonsoft.Json.Linq;

namespace FluidDemoApp.Models;

public class SectionDetailsModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Order { get; set; } = 100;
    public List<SectionBlockDetailsModel> Children { get; set; } = [];
}