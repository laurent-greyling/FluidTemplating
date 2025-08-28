using FluidDemoApp.DBContext;

namespace FluidDemoApp.Models;

public class DataDetailsModel
{
    public AssessmentDetailsEntity? Assessment { get; set; }
    public List<FindingsDetailsEntity> Findings { get; set; } = [];
    public OtherDetailsEntity? Other { get; set; }

    // Optional convenience stats (handy for summaries)
    public int TotalFindings => Findings.Count;
    public int CriticalCount => Findings.Count(f => f.Severity == SeverityEnum.Critical);
    public int HighCount => Findings.Count(f => f.Severity == SeverityEnum.High);
    public int MediumCount => Findings.Count(f => f.Severity == SeverityEnum.Medium);
    public int LowCount => Findings.Count(f => f.Severity == SeverityEnum.Low);
}