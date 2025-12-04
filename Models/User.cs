using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public abstract class User : BaseAuditableEntity
{
    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("fullName")]
    public string? FullName { get; set; }

    public override string PartitionKey => AccountId.ToString();
}