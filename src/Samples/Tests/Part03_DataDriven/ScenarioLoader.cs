using System.Text.Json;

namespace TDD.Part03_DataDriven;

public static class ScenarioLoader
{
    private const string DataPath = "Part03_DataDriven/Data";
    private const string Filename = "PlanningAgentScenarios.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static IEnumerable<TravelPlanningScenario> LoadPlanningWorkflowScenarios()
    {
        var currentDirectory = AppContext.BaseDirectory;
        var filePath = Path.Combine(currentDirectory, DataPath, Filename);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);
        var scenarios = JsonSerializer.Deserialize<List<TravelPlanningScenario>>(json, SerializerOptions);

        return scenarios ?? [];
    }
}