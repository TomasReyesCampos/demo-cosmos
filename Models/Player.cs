using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class TeamAssignment
{
    [JsonProperty("teamId")]
    public Guid TeamId { get; set; }

    [JsonProperty("teamOverview")]
    public TeamOverview? TeamOverview { get; set; }

    [JsonProperty("role")]
    public string? Role { get; set; }

    [JsonProperty("joinedAt")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public class Player : User
{
    [JsonProperty("birthDate")]
    public DateTime? BirthDate { get; set; }

    [JsonProperty("teamAssignments")]
    public List<TeamAssignment> TeamAssignments { get; set; } = new();

    public override IList<string> ItemType => new List<string> { "User", "Player" };
}