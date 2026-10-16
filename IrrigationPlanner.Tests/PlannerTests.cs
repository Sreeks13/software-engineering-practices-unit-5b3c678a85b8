using IrrigationPlanner;
using Xunit;

public class PlannerTests
{
    [Fact]
    public void CalculatesRuntimeCorrectly()
    {
        var report = Planner.Plan(new[]
        {
            "NorthField|100|20|5"
        });

        Assert.Single(report.Accepted);
        Assert.Empty(report.Rejected);
        Assert.Equal(25, report.Accepted[0].Minutes, precision: 6);
    }

    [Fact]
    public void RejectsZeroFlowRate()
    {
        var report = Planner.Plan(new[]
        {
            "EastField|50|0|3"
        });

        Assert.Empty(report.Accepted);
        Assert.Single(report.Rejected);
        Assert.Equal(1, report.Rejected[0].LineNumber);
    }

    [Fact]
    public void RejectsMalformedLine()
    {
        var report = Planner.Plan(new[]
        {
            "BadField|50|10"
        });

        Assert.Empty(report.Accepted);
        Assert.Single(report.Rejected);
    }

    [Fact]
    public void PreservesOriginalLineNumbers()
    {
        var report = Planner.Plan(new[]
        {
            "GoodField|100|20|5",
            "BadField|50|0|3",
            "AnotherGoodField|80|10|4"
        });

        Assert.Equal(2, report.Accepted.Count);
        Assert.Single(report.Rejected);
        Assert.Equal(1, report.Accepted[0].LineNumber);
        Assert.Equal(3, report.Accepted[1].LineNumber);
        Assert.Equal(2, report.Rejected[0].LineNumber);
    }

    [Fact]
    public void RejectsEmptyFieldName()
    {
        var report = Planner.Plan(new[]
        {
            "|40|10|2"
        });

        Assert.Empty(report.Accepted);
        Assert.Single(report.Rejected);
    }
}