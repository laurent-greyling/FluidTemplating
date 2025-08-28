using FluidDemoApp.DBContext;
using FluidDemoApp.Models;
using FluidDemoApp.Repositories;

namespace FluidDemoApp.Services;

public class DataDetailsService
{
    /// <summary>
    /// Loads and filters all data for a given assessment.
    /// Findings are sorted by severity (Critical => Low), then by Finding text.
    /// </summary>
    public static DataDetailsModel GetDataDetailsForAssessment(Guid assessmentId)
    {
        var assessments = DataDetailsRepository.LoadAssessments();
        var findings = DataDetailsRepository.LoadFindings();
        var others = DataDetailsRepository.LoadOther();

        var result = new DataDetailsModel
        {
            Assessment = assessments.FirstOrDefault(a => a.AssessmentId == assessmentId),
            Findings = findings
                .Where(f => f.AssessmentId == assessmentId)
                .OrderBy(f => SeverityRank(f.Severity))
                .ThenBy(f => f.Finding ?? string.Empty)
                .ToList(),
            Other = others.FirstOrDefault(o => o.AssessmentId == assessmentId)
        };

        return result;
    }

    // Define a rank for deterministic sorting
    private static int SeverityRank(SeverityEnum severity) => severity switch
    {
        SeverityEnum.Critical => 0,
        SeverityEnum.High => 1,
        SeverityEnum.Medium => 2,
        SeverityEnum.Low => 3,
        _ => 9
    };
}