using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class TeamOverview
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("nameDisplay")]
    public string? NameDisplay { get; set; }

    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    // Static mapping method from Team to TeamOverview
    public static TeamOverview FromTeam(Team team)
    {
        return new TeamOverview
        {
            Id = team.Id!.Value,
            NameDisplay = team.NameDisplay,
            AccountId = team.AccountId,
            IsActive = team.IsActive
        };
    }
}