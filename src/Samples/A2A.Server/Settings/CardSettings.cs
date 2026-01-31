namespace A2A.Server.Settings;

public class CardSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    
    public List<AgentCard> AgentCards { get; set; } = [];
}