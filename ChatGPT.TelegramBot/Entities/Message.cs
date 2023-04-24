using System.Text.Json.Serialization;

namespace ChatGPT.TelegramBot.Entities;

public class Message
{
	[JsonPropertyName("role")]
	public string Role { get; set; } = string.Empty;
	[JsonPropertyName("content")]
	public string Content { get; set; } = string.Empty;
}
