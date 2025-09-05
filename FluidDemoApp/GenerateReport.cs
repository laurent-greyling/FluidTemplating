using Microsoft.Playwright;

namespace FluidDemoApp;

public static class GenerateReport
{
    public static async Task PdfAsync(string html)
    {
        var htmlPath = Path.GetFullPath("report.html");
        await File.WriteAllTextAsync(htmlPath, html);
        Console.WriteLine($"\nHTML saved: {htmlPath}");

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();

        await page.SetContentAsync(html, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.Load
        });
        
        var year = DateTime.UtcNow.Year;
        const string companyName = "MyCompany Name"; // hardcode for POC
        
        var pdfBytes = await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            Margin = new Margin
            {
                Top = "20mm", 
                Bottom = "20mm", 
                Left = "15mm", 
                Right = "15mm"
            },
            PrintBackground = true,
            DisplayHeaderFooter = true,
            HeaderTemplate = @"<div></div>", // empty header (we only use a footer)
            FooterTemplate = $@"
              <div style=""font-size:9px; color:#666; width:100%; padding:0 10mm;"">
                <div style=""display:flex; justify-content:space-between; align-items:center;"">
                  <span>{year}&nbsp;&nbsp;{companyName}&nbsp;Confidential</span>
                  <span><span class=""pageNumber""></span>/<span class=""totalPages""></span></span>
                </div>
              </div>"
        });

        var pdfPath = Path.GetFullPath("report.pdf");
        await File.WriteAllBytesAsync(pdfPath, pdfBytes);
        Console.WriteLine($"PDF saved: {pdfPath}");
    }
}