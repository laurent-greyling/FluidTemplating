using FluidDemoApp;
using FluidDemoApp.Helpers;
using FluidDemoApp.Repositories;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Main menu: What would you like to do?");
    Console.WriteLine("1) Add/Update Variables");
    Console.WriteLine("2) Add/Update Sections");
    Console.WriteLine("3) Add/Update Templates");
    Console.WriteLine("4) Render Template (build PDF)");
    Console.WriteLine("5) Exit");
    Console.Write("Select (1-5): ");

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
            while (true)
            {
                TemplateCollector.AddOrUpdateTemplate();
                Console.Write("Add/update another template? (y/n): ");
                if (!((Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase)))
                    break;
            }
            Console.WriteLine("Templates saved to Data/templates.json");
            break;
        case "4":
            var templates = TemplateRepository.Load();
            if (templates.Count == 0)
            {
                Console.WriteLine("No templates exist. Create one first.");
                break;
            }

            Console.WriteLine("\nAvailable templates:");
            for (var i = 0; i < templates.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {templates[i].Name}");
            }

            Console.Write("Select template number to render: ");
            if (int.TryParse(Console.ReadLine(), out var idx) && idx >= 1 && idx <= templates.Count)
            {
                var selectedTemplate = templates[idx - 1];
                var variablesFromFile = VariableRepository.Load();
                await Template.RenderAsync(variablesFromFile, selectedTemplate.Name);
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Unknown option. Please choose 1–5.");
            break;
    }
}