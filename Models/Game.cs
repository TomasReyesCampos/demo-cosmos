using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class Game : BaseAuditableEntity
{
    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("homeTeam")]
    public TeamOverview? HomeTeam { get; set; }

    [JsonProperty("awayTeam")]
    public TeamOverview? AwayTeam { get; set; }

    [JsonProperty("scheduledAt")]
    public DateTime ScheduledAt { get; set; }

    public override string PartitionKey => AccountId.ToString();

    public override IList<string> ItemType => new List<string> { "Game" };
}