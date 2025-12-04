using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class Game : BaseAuditableEntity
{
    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("homeTeamId")]
    public Guid HomeTeamId { get; set; }

    [JsonProperty("awayTeamId")]
    public Guid AwayTeamId { get; set; }

    [JsonProperty("scheduledAt")]
    public DateTime ScheduledAt { get; set; }

    public override string PartitionKey => AccountId.ToString();

    public override IList<string> ItemType => new List<string> { "Game" };
}