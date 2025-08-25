namespace FluidDemoApp.Models;

public class ListSectionModel
{
    public string HeadingTemplate { get; set; } = "";
    public ListKind Kind { get; set; } = ListKind.Bulleted;
    public int? StartNumber { get; set; }
    public bool Tight { get; set; } = false;
    public List<string> ItemTemplates { get; set; } = new();
}

public enum ListKind
{
    Bulleted, 
    Numbered,
    Checklist
}