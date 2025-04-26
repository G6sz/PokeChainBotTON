using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;

class Program
{
    static ITelegramBotClient bot;

    public static async Task Main()
    {
        string botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");

        if(string.IsNullOrEmpty(botToken)){
            Console.WriteLine("Por favor, defina a variável de ambiente BOT_TOKEN com o token do seu bot.");
            return;
            
        }
        bot = new TelegramBotClient(botToken);

        using CancellationTokenSource cts = new();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // Recebe todas as atualizações possíveis
        };

        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cts.Token
        );

        Console.WriteLine("Bot está rodando... Pressione Enter para sair.");

        await Task.Delay(-1);
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message || message.Text is not { } messageText)
            return;

        Console.WriteLine($"Recebido: {messageText}");

        if (messageText == "/start")
        {
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl("Play on TON", "https://ton.org/")
            );

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Welcome to PokeChainBot! Click the button below to play.",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Erro na API do Telegram: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine($"Erro: {errorMessage}");
        return Task.CompletedTask;
    }
}