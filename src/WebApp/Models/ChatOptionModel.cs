using System.Text.Json.Serialization;

namespace WebApp.Models;

public class ChatOptionModel
{
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string User { get; set; } = string.Empty;

    [JsonPropertyName("option")]
    public int Option { get; set; }
}
