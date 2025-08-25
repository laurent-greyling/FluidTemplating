using FluidDemoApp.Models;

namespace FluidDemoApp;

public static class SectionSubMenu
{
    public static List<SectionType> SectionPlan { get; set; }
    
    public static void Show()
    {
        SectionPlan ??= [];
        
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Add Sections — choose one to add (it will be appended to the plan):");
            Console.WriteLine("1) Text");
            Console.WriteLine("2) Table");
            Console.WriteLine("3) Image");
            Console.WriteLine("4) List");
            Console.WriteLine("5) Done (return to main menu)");
            Console.Write("Select (1-5): ");

            var pick = (Console.ReadLine() ?? "").Trim();
            switch (pick)
            {
                case "1": 
                    SectionPlan.Add(SectionType.Text);
                    Console.WriteLine("Added: Text");  break;
                case "2": 
                    SectionPlan.Add(SectionType.Table); 
                    Console.WriteLine("Added: Table"); break;
                case "3": 
                    SectionPlan.Add(SectionType.Image); 
                    Console.WriteLine("Added: Image"); break;
                case "4": 
                    SectionPlan.Add(SectionType.List);
                    Console.WriteLine("Added: List");  break;
                case "5":
                    Console.WriteLine($"Current section order: {string.Join(" - ", SectionPlan)}");
                    return;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
    }
}