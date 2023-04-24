using System.Net.Http.Json;
using ChatGPT.TelegramBot.Entities;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

const string telegramToken = "telegram_token";
const string apiKey = "chatgpt_token";
const string endpoint = "https://api.openai.com/v1/chat/completions";

var messages = new List<ChatGPT.TelegramBot.Entities.Message>();
var httpClient = new HttpClient();

httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

var botClient = new TelegramBotClient(telegramToken);

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
	AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
	updateHandler: HandleUpdateAsync,
	pollingErrorHandler: HandlePollingErrorAsync,
	receiverOptions: receiverOptions,
	cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.ReadLine();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
	if (update.Message is not { } message)
		return;

	if (message.Text is not { } messageText)
		return;

	var chatId = message.Chat.Id;

	Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

	var asnwer = await GetAnswer(messageText);

	Telegram.Bot.Types.Message sentMessage = await botClient.SendTextMessageAsync(
		chatId: chatId,
		text: asnwer,
		cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
	var ErrorMessage = exception switch
	{
		ApiRequestException apiRequestException
			=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
		_ => exception.ToString()
	};

	Console.WriteLine(ErrorMessage);
	return Task.CompletedTask;
}

async Task<string> GetAnswer(string question)
{
	if (question is not { Length: > 0 })
		return "Вопрос слишком короткий";

	var message = new ChatGPT.TelegramBot.Entities.Message() { Role = "user", Content = question };
	messages.Add(message);

	var requestData = new Request()
	{
		ModelId = "gpt-3.5-turbo",
		Messages = messages
	};

	using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);

	if (!response.IsSuccessStatusCode)
		return $"{(int)response.StatusCode} {response.StatusCode}";

	ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

	var choices = responseData?.Choices ?? new List<Choice>();
	if (choices.Count == 0)
		return "Ошибка, не удалось получить ответ";

	var choice = choices[0];
	var responseMessage = choice.Message;

	messages.Add(responseMessage);
	var responseText = responseMessage.Content.Trim();

	return responseText;
}