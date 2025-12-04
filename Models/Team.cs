using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class Team : BaseAuditableEntity
{
    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("nameDisplay")]
    public string? NameDisplay { get; set; }

    public override string PartitionKey => AccountId.ToString();

    public override IList<string> ItemType => new List<string> { "Team" };
}