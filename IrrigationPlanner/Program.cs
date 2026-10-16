using System.Globalization;
using IrrigationPlanner;

string filePath = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "fields.txt");

if (!File.Exists(filePath))
{
    Console.Error.WriteLine($"File not found: {filePath}");
    return;
}

PlannerReport report = Planner.Plan(File.ReadAllLines(filePath));

Console.WriteLine("ACCEPTED");

foreach (IrrigationResult field in report.Accepted)
{
    Console.WriteLine(
        $"{field.LineNumber}|{field.FieldName}|{field.Minutes.ToString("0.##", CultureInfo.InvariantCulture)}");
}

Console.WriteLine("REJECTED");

foreach (RejectedField field in report.Rejected)
{
    Console.WriteLine($"{field.LineNumber}|{field.Reason}");
}