namespace FluidDemoApp.Models;

public class ListSectionModel
{
    public string HeadingTemplate { get; set; } = "";
    public ListKind Kind { get; set; } = ListKind.Bulleted;
    public int? StartNumber { get; set; }
    public bool Tight { get; set; } = false;
    public List<string> ItemTemplates { get; set; } = [];
    
    public string? Source { get; set; }// e.g., "report.Findings"
    public List<string> Fields { get; set; } = [];
}

public enum ListKind
{
    Bulleted, 
    Numbered,
    Checklist
}