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
        for (var column = 0; column < tableSectionModel.Columns; column++) stringBuilder.Append("    <col />\n");
        stringBuilder.Append("  </colgroup>\n  <tbody>\n");

        for (var row = 0; row < tableSectionModel.Rows; row++)
        {
            stringBuilder.Append("    <tr>");
            for (var column = 0; column < tableSectionModel.Columns; column++)
            {
                var templateText = tableSectionModel.CellTemplates[row, column] ?? "";
                if (!parser.TryParse(templateText, out var cellTpl, out var error))
                    throw new InvalidOperationException($"Cell [{row + 1},{column + 1}] template error: {error}");

                var rendered = cellTpl.Render(templateContext);

                var isHeaderCell =
                    (tableSectionModel.HeaderRow && row == 0) ||
                    (tableSectionModel.HeaderColumn && column == 0);

                if (isHeaderCell)
                    stringBuilder.Append($"<th>{rendered}</th>");
                else
                    stringBuilder.Append($"<td>{rendered}</td>");
            }
            stringBuilder.Append("</tr>\n");
        }

        stringBuilder.Append("  </tbody>\n</table>\n");
        return stringBuilder.ToString();
    }
}