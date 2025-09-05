using Fluid;
using FluidDemoApp;
using FluidDemoApp.Helpers;
using FluidDemoApp.Repositories;
using FluidDemoApp.Services;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Main menu: What would you like to do?");
    Console.WriteLine("1) Add/Update Variables");
    Console.WriteLine("2) Add/Update Sections");
    Console.WriteLine("3) Add/Update Templates");
    Console.WriteLine("4) Assign Template to Assessment");
    Console.WriteLine("5) Render (choose Assessment; use binding or override)");
    Console.WriteLine("6) Exit");
    Console.Write("Select (1-6): ");

    var choice = (Console.ReadLine() ?? "").Trim();

    switch (choice)
    {
        case "1":
            var variables = VariableCollector.Generate();
            VariableRepository.MergeAndSave(variables);
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
            TemplateBindingCollector.Assign();
            break;
        case "5":
        {
            //pick assessment
            var index = DataDetailsRepository.GetAssessmentIndex(100);
            if (index.Count == 0)
            {
                Console.WriteLine("No assessments found.");
                break;
            }

            Console.WriteLine("\nAvailable assessments:");
            for (var i = 0; i < index.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {index[i].Label}");
            }

            Console.Write("Pick number or paste AssessmentId: ");
            var assessmentNumber = (Console.ReadLine() ?? "").Trim();

            Guid assessmentId;
            if (int.TryParse(assessmentNumber, out var number) 
                && number >= 1 
                && number <= index.Count)
            {
                assessmentId = index[number - 1].AssessmentId;
            }
            else if (!Guid.TryParse(assessmentNumber, out assessmentId))
            {
                Console.WriteLine("Invalid selection.");
                break;
            }

            //check binding(s)
            var templateName = ResolveTemplateNameForAssessment(assessmentId);
            if (string.IsNullOrEmpty(templateName))
            {
                Console.WriteLine("No template binding found. Please assign a template first.");
                break;
            }

            //resolve data & render
            var cssFile = ThemeSelector.PickVisualTemplate();
            var data = DataDetailsService.GetDataDetailsForAssessment(assessmentId);
            var variableList = VariableRepository.Load();

            await Template.RenderAsync(variableList, templateName, data, cssFile);
            break;
        }
        case "6":
            return;
        default:
            Console.WriteLine("Unknown option. Please choose 1–6.");
            break;
    }
}

string? ResolveTemplateNameForAssessment(Guid assessmentId)
{
    var bindings = AssessmentTemplateBindingRepository
        .GetForAssessment(assessmentId)
        .OrderByDescending(b => b.Version)
        .ThenByDescending(b => b.CreatedDate)
        .ToList();

    if (bindings.Count == 0) return null;

    var templatesById = TemplateRepository.Load()
        .ToDictionary(t => t.Id, t => t);

    var chosen = bindings[0]; // best one
    return templatesById.TryGetValue(chosen.TemplateId, out var template)
        ? template.Name
        : null;
}