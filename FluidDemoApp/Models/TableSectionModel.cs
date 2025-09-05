namespace FluidDemoApp.Models;

public class TableSectionModel
{
    public string HeadingTemplate { get; set; } = ""; // optional, like "Assets for {{ company }}"
    public int Columns { get; set; }
    public int Rows { get; set; }
    public bool HeaderRow { get; set; }      // first row is <th>
    public bool HeaderColumn { get; set; }   // first column is <th>
    public string[,] CellTemplates { get; set; } = default!; // [row, col] Fluid-enabled
    
    //dynamic mode
    public string? Source { get; set; } // e.g. "report.Findings"
    public List<string> ColumnHeaders { get; set; } = []; // visible column headers
    public List<string> Fields { get; set; } = []; // per-column cell Fluid, e.g. "{{ item.Severity }}"
}