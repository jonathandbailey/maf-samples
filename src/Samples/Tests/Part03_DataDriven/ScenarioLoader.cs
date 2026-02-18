using System.Text.Json;

namespace TDD.Part03_DataDriven;

public class ScenarioLoader
{
    public static IEnumerable<TravelPlanningScenario> LoadPlanningWorkflowScenarios(string fileName = "PlanningAgentScenarios.json")
    {
        var currentDirectory = AppContext.BaseDirectory;
        var filePath = Path.Combine(currentDirectory, "Part03_DataDriven\\Data", fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);
        var scenarios = JsonSerializer.Deserialize<List<TravelPlanningScenario>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        return scenarios ?? [];
    }
}