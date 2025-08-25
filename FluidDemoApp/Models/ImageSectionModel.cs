namespace FluidDemoApp.Models;

public class ImageSectionModel
{
    public string HeadingTemplate { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public string AlternativeText { get; set; } = "";
    public string CaptionTemplate { get; set; } = "";
    public int? MaxWidthPx { get; set; }
    public string Align { get; set; } = "center"; 
}