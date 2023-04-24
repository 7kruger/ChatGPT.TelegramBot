using System.Text.Json.Serialization;

namespace ChatGPT.TelegramBot.Entities;

public class Request
{
	[JsonPropertyName("model")]
	public string ModelId { get; set; } = string.Empty;
	[JsonPropertyName("messages")]
	public List<Message> Messages { get; set; } = new();
}
