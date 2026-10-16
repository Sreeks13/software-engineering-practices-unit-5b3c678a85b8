using System.Globalization;

namespace IrrigationPlanner;

public sealed record IrrigationResult(
    int LineNumber, string FieldName, double Minutes);

public sealed record RejectedField(
    int LineNumber, string Reason);

public sealed record PlannerReport(
    IReadOnlyList<IrrigationResult> Accepted,
    IReadOnlyList<RejectedField> Rejected);

public static class Planner
{
    public static PlannerReport Plan(IEnumerable<string> lines)
    {
        var accepted = new List<IrrigationResult>();
        var rejected = new List<RejectedField>();
        int lineNumber = 0;

        foreach (string line in lines)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                rejected.Add(new(lineNumber, "Empty line"));
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length != 4)
            {
                rejected.Add(new(lineNumber,
                    "Expected 4 pipe-separated fields"));
                continue;
            }

            string name = parts[0].Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                rejected.Add(new(lineNumber, "Field name is required"));
                continue;
            }

            if (!TryPositive(parts[1], out double area))
            {
                rejected.Add(new(lineNumber,
                    "Area must be a positive number"));
                continue;
            }

            if (!TryPositive(parts[2], out double flow))
            {
                rejected.Add(new(lineNumber,
                    "Flow rate must be a positive number"));
                continue;
            }

            if (!TryPositive(parts[3], out double water))
            {
                rejected.Add(new(lineNumber,
                    "Water per area must be a positive number"));
                continue;
            }

            double minutes = area * water / flow;

            if (!double.IsFinite(minutes))
            {
                rejected.Add(new(lineNumber,
                    "Calculated runtime is invalid"));
                continue;
            }

            accepted.Add(new(lineNumber, name, minutes));
        }

        return new PlannerReport(accepted, rejected);
    }

    private static bool TryPositive(string value, out double result)
    {
        return double.TryParse(
                   value.Trim(),
                   NumberStyles.Float,
                   CultureInfo.InvariantCulture,
                   out result)
               && double.IsFinite(result)
               && result > 0;
    }
}