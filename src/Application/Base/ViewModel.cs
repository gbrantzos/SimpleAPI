using System.Text.Json.Serialization;

namespace SimpleAPI.Application.Base;

public abstract class ViewModel
{
    [JsonPropertyOrder(-20)]
    public int ID { get; set; }

    [JsonPropertyOrder(-19)]
    public int RowVersion { get; set; }
}
