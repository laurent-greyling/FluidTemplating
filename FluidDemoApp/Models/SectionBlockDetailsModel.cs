using Newtonsoft.Json.Linq;

namespace FluidDemoApp.Models;

public class SectionBlockDetailsModel
{
    public SectionType Type { get; set; } = SectionType.Text;
    public JObject Data { get; set; } = new();
    public int Order { get; set; } = 100; // maybe we want to order blocks in a section as well??
}