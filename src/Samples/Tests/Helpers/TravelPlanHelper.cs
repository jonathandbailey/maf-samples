using System.Text.Json;
using Microsoft.Extensions.AI;

namespace Tests.Helpers;

public static class TravelPlanHelper
{
    public static ChatMessage CreateTravelPlanMessage(TravelPlanDto travelPlan)
    {
        var serializedPlan = JsonSerializer.Serialize(travelPlan);
        var template = $"TravelPlanSummary : {serializedPlan}";
        return new ChatMessage(ChatRole.User, template);
    }
}