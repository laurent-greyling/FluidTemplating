using FluidDemoApp.DBContext;
using FluidDemoApp.Models;
using FluidDemoApp.Repositories;

namespace FluidDemoApp.Helpers;

public static class TemplateBindingCollector
{
    public static void Assign()
    {
        var assessmentId = SelectAssessment();
        if (assessmentId == Guid.Empty)
        {
            return;
        }

        var template = SelectTemplate();
        if (template == null)
        {
            return;
        }

        Console.Write("Version (default 1): ");
        var rawVersion = (Console.ReadLine() ?? "").Trim();
        var version = int.TryParse(rawVersion, out var v) ? v : 1;

        var binding = new AssessmentTemplateBindingEntity
        {
            AssessmentId = assessmentId,
            TemplateId = template.Id,
            Version = version,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = Environment.UserName
        };

        AssessmentTemplateBindingRepository.Upsert(binding);
        Console.WriteLine($"Bound assessment {assessmentId} -> template '{template.Name}' (v{version}).");
    }

    private static TemplateModel? SelectTemplate()
    {
        var templates = TemplateRepository.Load();
        if (templates.Count == 0)
        {
            Console.WriteLine("No templates exist. Create one first.");
            return null;
        }

        Console.WriteLine("\nSelect template:");
        for (var i = 0; i < templates.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {templates[i].Name} ({templates[i].Id})");
        }

        Console.Write("Template number: ");
        if (int.TryParse(Console.ReadLine(), out var templateIndex)
            && templateIndex >= 1
            && templateIndex <= templates.Count)
        {
            return templates[templateIndex - 1];
        }
        
        Console.WriteLine("Invalid template selection.");
        return null;
    }

    private static Guid SelectAssessment()
    {
        var assessments = DataDetailsRepository.GetAssessmentIndex(100);
        if (assessments.Count == 0)
        {
            Console.WriteLine("No assessments found.");
            return Guid.Empty;
        }

        Console.WriteLine("\nSelect assessment:");
        for (var i = 0; i < assessments.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {assessments[i].Label}");
        }

        Console.Write("Number or paste AssessmentId: ");
        var selectedAssessment = (Console.ReadLine() ?? "").Trim();

        if (!Guid.TryParse(selectedAssessment, out var assessmentId))
        {
            Console.WriteLine("Invalid assessment selection.");
            return Guid.Empty;
        }
        
        if (int.TryParse(selectedAssessment, out var assessmentNumber)
            && assessmentNumber >= 1
            && assessmentNumber <= assessments.Count)
        {
            assessmentId = assessments[assessmentNumber - 1].AssessmentId;
        }

        return assessmentId;
    }
}