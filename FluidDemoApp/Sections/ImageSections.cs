using System.Text;
using Fluid;
using FluidDemoApp.Models;

namespace FluidDemoApp.Sections;

public static class ImageSections
{
    public static ImageSectionModel GetImage()
    {
        Console.WriteLine("\nAdd an image:");

        Console.Write("Heading (Fluid allowed, or leave empty): ");
        var heading = Console.ReadLine() ?? "";

        string path;
        while (true)
        {
            Console.Write("Image file path: ");
            path = (Console.ReadLine() ?? "").Trim();
            if (File.Exists(path)) break;
            Console.WriteLine("File not found. Try again.");
        }

        Console.Write("Alt text (for accessibility): ");
        var alternativeText = Console.ReadLine() ?? "";

        Console.Write("Caption (Fluid allowed, or leave empty): ");
        var caption = Console.ReadLine() ?? "";

        Console.Write("Max width in px (empty for auto): ");
        var widthText = Console.ReadLine();
        int? maxWidth = int.TryParse(widthText, out var width) && width > 0 ? width : null;

        Console.Write("Align (left/center/right, default center): ");
        var align = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (align is not ("left" or "center" or "right")) align = "center";

        return new ImageSectionModel()
        {
            HeadingTemplate = heading,
            ImagePath = path,
            AlternativeText = alternativeText,
            CaptionTemplate = caption,
            MaxWidthPx = maxWidth,
            Align = align
        };
    }

    public static string RenderImageHtml(ImageSectionModel imageSection, TemplateContext templateContext, FluidParser parser)
    {
        var stringBuilder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(imageSection.HeadingTemplate))
        {
            if (!parser.TryParse(imageSection.HeadingTemplate, out var headingTemplate, out var error))
                throw new InvalidOperationException($"Image heading template error: {error}");
            var heading = headingTemplate.Render(templateContext);
            stringBuilder.Append($"<h2>{System.Net.WebUtility.HtmlEncode(heading)}</h2>\n");
        }

        var dataUri = ToDataUri(imageSection.ImagePath);
        var style = new StringBuilder();
        if (imageSection.MaxWidthPx is int width) style.Append($"max-width:{width}px;");
        style.Append("height:auto;");

        var wrapperClass = imageSection.Align switch
        {
            "left" => "img-left",
            "right" => "img-right",
            _ => "img-center"
        };

        stringBuilder.Append($"<figure class=\"img {wrapperClass}\">");
        stringBuilder.Append($"<img src=\"{dataUri}\" alt=\"{System.Net.WebUtility.HtmlEncode(imageSection.AlternativeText)}\" style=\"{style}\" />");

        if (!string.IsNullOrWhiteSpace(imageSection.CaptionTemplate))
        {
            if (!parser.TryParse(imageSection.CaptionTemplate, out var captionTemplate, out var captionError))
                throw new InvalidOperationException($"Image caption template error: {captionError}");
            var caption = captionTemplate.Render(templateContext);
            stringBuilder.Append($"<figcaption>{caption}</figcaption>");
        }

        stringBuilder.Append("</figure>\n");
        return stringBuilder.ToString();
    }
    
    private static string GuessMime(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".png"  => "image/png",
            ".jpg"  => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif"  => "image/gif",
            ".webp" => "image/webp",
            _       => "application/octet-stream"
        };
    }

    private static string ToDataUri(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var b64 = Convert.ToBase64String(bytes);
        var mime = GuessMime(path);
        return $"data:{mime};base64,{b64}";
    }
}