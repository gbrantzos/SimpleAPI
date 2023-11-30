using System.Text.Json.Serialization;

namespace SimpleAPI.Application.Base;

public abstract class ViewModel
{
    [JsonPropertyOrder(-20), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ID { get; set; }

    [JsonPropertyOrder(-19), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int RowVersion { get; set; }
}
