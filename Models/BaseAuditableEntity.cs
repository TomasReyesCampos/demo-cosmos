using Newtonsoft.Json;
using SDDev.Net.GenericRepository.Contracts.BaseEntity;

namespace DemoCosmos.ConsoleApp.Models;

public class AuditMetadata
{
    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    [JsonProperty("modifiedDateTime")]
    public DateTime ModifiedDateTime { get; set; } = DateTime.UtcNow;

    [JsonProperty("createdBy")]
    public string? CreatedBy { get; set; }

    [JsonProperty("modifiedBy")]
    public string? ModifiedBy { get; set; }
}

public abstract class BaseAuditableEntity : BaseStorableEntity
{
    [JsonProperty("auditMetadata")]
    public AuditMetadata? AuditMetadata { get; set; } = new AuditMetadata();
}