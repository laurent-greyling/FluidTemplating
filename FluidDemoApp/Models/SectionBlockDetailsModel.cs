using Newtonsoft.Json.Linq;

namespace FluidDemoApp.Models;

public class SectionBlockDetailsModel
{
    public SectionType Type { get; set; } = SectionType.Text;
    public JObject Data { get; set; } = new();
}