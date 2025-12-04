using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class Account : BaseAuditableEntity
{
    [JsonProperty("displayName")]
    public string? DisplayName { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    public override string PartitionKey => Id?.ToString() ?? "unknown";

    public override IList<string> ItemType => new List<string> { "Account" };
}