using FluidDemoApp;
using FluidDemoApp.Helpers;
using FluidDemoApp.Repositories;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Main menu: What would you like to do?");
    Console.WriteLine("1) Add/Update Variables");
    Console.WriteLine("2) Add/Update Sections");
    Console.WriteLine("3) Render Template (build PDF)");
    Console.WriteLine("4) Exit");
    Console.Write("Select (1-4): ");

    var choice = (Console.ReadLine() ?? "").Trim();

    switch (choice)
    {
        case "1":
            var variables = VariableCollector.Generate();
            VariableRepository.MergeAndSave(variables);
            Console.WriteLine("Variables captured/updated.");
            Console.WriteLine("Variables saved to Data/variables.json");
            break;
        case "2":
            while (true)
            {
                SectionCollector.AddOrUpdateSection();
                Console.Write("Add/update another section? (y/n): ");
                if (!((Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase)))
                    break;
            }
            Console.WriteLine("Sections saved to Data/sections.json");
            break;
        case "3":
            var variablesFromFile = VariableRepository.Load();
            await Template.RenderAsync(variablesFromFile);
            return;
        case "4":
            return;
        default:
            Console.WriteLine("Unknown option. Please choose 1–4.");
            break;
    }
}