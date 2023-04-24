using System.Text.Json.Serialization;

namespace ChatGPT.TelegramBot.Entities;

public class ResponseData
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;
	[JsonPropertyName("object")]
	public string Object { get; set; } = string.Empty;
	[JsonPropertyName("created")]
	public ulong Created { get; set; }
	[JsonPropertyName("choices")]
	public List<Choice> Choices { get; set; } = new();
	[JsonPropertyName("usage")]
	public Usage Usage { get; set; } = new();
}
