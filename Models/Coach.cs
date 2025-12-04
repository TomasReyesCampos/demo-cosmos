using Newtonsoft.Json;

namespace DemoCosmos.ConsoleApp.Models;

public class Coach : User
{
    [JsonProperty("licenseNumber")]
    public string? LicenseNumber { get; set; }

    public override IList<string> ItemType => new List<string> { "User", "Coach" };
}