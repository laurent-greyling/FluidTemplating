using System.Text;
using Fluid;
using FluidDemoApp.Models;

namespace FluidDemoApp.Sections;

public static class TableSections
{
    public static TableSectionModel CreateTableSection()
    {
        Console.WriteLine("\nCreate a table:");

        Console.Write("Heading (Fluid allowed, or leave empty): ");
        var heading = Console.ReadLine() ?? "";
        
        Console.Write("Dynamic table from data source? (y/n): ");
        var isDynamic = (Console.ReadLine() ?? "").Trim()
            .Equals("y", StringComparison.OrdinalIgnoreCase);

        if (isDynamic)
        {
            Console.Write("Enter data source (e.g. report.Findings): ");
            var source = (Console.ReadLine() ?? "").Trim();

            var headers = new List<string>();
            Console.WriteLine("Enter column headers (one per line). Type END when finished:");
            while (true)
            {
                var line = Console.ReadLine();
                if (line is not null && line.Trim().Equals("END", StringComparison.OrdinalIgnoreCase))
                    break;
                if (!string.IsNullOrWhiteSpace(line)) headers.Add(line);
            }

            var fields = new List<string>();
            foreach (var header in headers)
            {
                Console.Write($"  Field template for '{header}' (e.g. {{ item.Finding }}): ");
                fields.Add(Console.ReadLine() ?? "");
            }

            return new TableSectionModel
            {
                HeadingTemplate = heading,
                Source = source,
                ColumnHeaders = headers,
                Fields = fields,
                Columns = headers.Count,
                Rows = 0,
                HeaderRow = true,
                HeaderColumn = false,
                CellTemplates = new string[0,0]
            };
        }

        int columns;
        while (true)
        {
            Console.Write("Number of columns: ");
            if (int.TryParse(Console.ReadLine(), out columns) && columns > 0) break;
            Console.WriteLine("Enter a positive integer.");
        }

        int rows;
        while (true)
        {
            Console.Write("Number of rows: ");
            if (int.TryParse(Console.ReadLine(), out rows) && rows > 0) break;
            Console.WriteLine("Enter a positive integer.");
        }

        Console.Write("Header row? (y/n): ");
        var headerRow = (Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

        Console.Write("Header column? (y/n): ");
        var headerCol = (Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

        var cells = new string[rows, columns];
        Console.WriteLine("\nEnter cell contents (Fluid allowed). Use {{ var }} to insert variables. Type a blank line to keep empty.\n");
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                Console.Write($"Cell [{row + 1},{column + 1}]: ");
                cells[row, column] = Console.ReadLine() ?? "";
            }
        }

        return new TableSectionModel
        {
            HeadingTemplate = heading,
            Columns = columns,
            Rows = rows,
            HeaderRow = headerRow,
            HeaderColumn = headerCol,
            CellTemplates = cells
        };
    }
    
    public static string RenderTableHtml(TableSectionModel tableSectionModel, TemplateContext templateContext, FluidParser parser)
    {
        var stringBuilder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(tableSectionModel.HeadingTemplate))
        {
            if (!parser.TryParse(tableSectionModel.HeadingTemplate, out var ht, out var herr))
                throw new InvalidOperationException($"Table heading template error: {herr}");
            var heading = ht.Render(templateContext);
            stringBuilder.Append($"<h2>{System.Net.WebUtility.HtmlEncode(heading)}</h2>\n");
        }

        stringBuilder.Append("""
                  <table class="tbl">
                    <colgroup>
                  """);
        
        var totalColumns =
            !string.IsNullOrWhiteSpace(tableSectionModel.Source)
                ? Math.Max(0, tableSectionModel.Fields?.Count ?? 0)
                : tableSectionModel.Columns;
        for (var column = 0; column < totalColumns; column++) stringBuilder.Append("    <col />\n");
        
        stringBuilder.Append("  </colgroup>\n");
        if (!string.IsNullOrWhiteSpace(tableSectionModel.Source))
        {
            stringBuilder.Append("  <thead><tr>");
            foreach (var colHeader in tableSectionModel.ColumnHeaders)
                stringBuilder.Append($"<th>{System.Net.WebUtility.HtmlEncode(colHeader ?? "")}</th>");
            stringBuilder.Append("</tr></thead>\n");
        }
        else if (tableSectionModel.HeaderRow)
        {
            stringBuilder.Append("  <thead><tr>");
            for (var c = 0; c < tableSectionModel.Columns; c++)
            {
                var tplText = tableSectionModel.CellTemplates[0, c] ?? "";
                if (!parser.TryParse(tplText, out var cellTpl, out var err))
                    throw new InvalidOperationException($"Header cell [1,{c+1}] template error: {err}");
                stringBuilder.Append($"<th>{cellTpl.Render(templateContext)}</th>");
            }
            stringBuilder.Append("</tr></thead>\n");
        }

        stringBuilder.Append("  <tbody>\n");

        if (!string.IsNullOrWhiteSpace(tableSectionModel.Source))
        {
            var rowCells = string.Join("", tableSectionModel.Fields.Select(f => $"<td>{(f ?? "")}</td>"));
            var loopTemplateText = $"{{% for item in {tableSectionModel.Source} %}}<tr>{rowCells}</tr>{{% endfor %}}";

            if (!parser.TryParse(loopTemplateText, out var loopTpl, out var loopErr))
                throw new InvalidOperationException($"Table loop template error: {loopErr}");

            stringBuilder.Append(loopTpl.Render(templateContext));
        }
        else
        {
            var startRow = tableSectionModel.HeaderRow ? 1 : 0;

            for (var row = startRow; row < tableSectionModel.Rows; row++)
            {
                stringBuilder.Append("    <tr>");
                for (var column = 0; column < tableSectionModel.Columns; column++)
                {
                    var templateText = tableSectionModel.CellTemplates[row, column] ?? "";
                    if (!parser.TryParse(templateText, out var cellTpl, out var error))
                        throw new InvalidOperationException($"Cell [{row + 1},{column + 1}] template error: {error}");

                    var rendered = cellTpl.Render(templateContext);

                    var isHeaderCell =
                        (!tableSectionModel.HeaderRow && tableSectionModel.HeaderColumn && column == 0);

                    if (isHeaderCell)
                        stringBuilder.Append($"<th>{rendered}</th>");
                    else
                        stringBuilder.Append($"<td>{rendered}</td>");
                }
                stringBuilder.Append("</tr>\n");
            }
        }

        stringBuilder.Append("  </tbody>\n</table>\n");
        return stringBuilder.ToString();
    }
}