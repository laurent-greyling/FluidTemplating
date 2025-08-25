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
            PrintBackground = true
        });

        var pdfPath = Path.GetFullPath("report.pdf");
        await File.WriteAllBytesAsync(pdfPath, pdfBytes);
        Console.WriteLine($"PDF saved: {pdfPath}");
    }
}