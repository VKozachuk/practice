using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // API ключ
        var botClient = new TelegramBotClient("5703917931:AAEgp5dDEcG1UwT_4GeAiD-mOz2I3Q1ySfQ");

        var cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        // Підключення до Бота
        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        // Сповіщення в консолі про вдалий запуск сервера
        Console.WriteLine($"Bot @{me.Username} started.");
        Console.ReadLine();
        cts.Cancel();
    }

    private static int fieldSize = 0;
    private static int numMines = 0;

    // Обробка отриманих оновлень
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Перевірка, чи отримано текстове повідомлення з діалогу
        if (update.Message is not { Text: { } messageText, Chat: { Id: var chatId }, From: { Username: var username }, From: { FirstName: var firstName } })
            return;

        var messageId = update.Message.MessageId;

        await Buttons(botClient, update, cancellationToken, chatId, messageText, firstName, messageId);

        Console.WriteLine($"User {username} sent message {messageId} to chat {chatId}.\nReceived message: '{messageText}'\n---------------------------------------------------------");
    }

    // Обробка помилок телеграм API
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private static async Task Buttons(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, long chatId, string messageText, string firstName, int messageId)
    {
        // Маркування клавіш відповіді
        ReplyKeyboardMarkup playOrInstruction = new(new[] { new KeyboardButton[] { "Play", "How to play?" }, }) { ResizeKeyboard = true };
        ReplyKeyboardMarkup buildButton = new(new[] { new KeyboardButton[] { "Build Field" }, }) { ResizeKeyboard = true };
        ReplyKeyboardMarkup restart = new(new[] { new KeyboardButton[] { "Restart" }, }) { ResizeKeyboard = true };

        if (messageText == "/start")
        {
            // Відправлення привітального повідомлення з клавіатурою
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text:
                $"Hello {firstName} 👋\n" +
                $"Let's play this game 🎮",
                replyMarkup: playOrInstruction,
                cancellationToken: cancellationToken);
            await Task.Delay(500);
        }

        else if (messageText.StartsWith("Play"))
        {
            // Відправлення повідомлення з запитом на введення розміру поля
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Enter field size 🔲\n(e.g. /set_fieldsize 10)",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            await Task.Delay(500);
        }

        else if (messageText.StartsWith("How to play?"))
        {
            // Відправлення повідомлення з описом правил гри
            await howToPlay(botClient, chatId, messageId);
            await Task.Delay(500);
        }

        else if (messageText == "Restart")
        {
            // Відправлення повідомлення з запитом на введення розміру поля
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Enter field size 🔲\n(e.g. /set_fieldsize 10)",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            await Task.Delay(500);
        }

        else if (messageText.StartsWith("/set_fieldsize"))
        {
            // Обробка команди встановлення розміру поля
            if (int.TryParse(messageText.Replace("/set_fieldsize", ""), out fieldSize))
            {
                if (fieldSize <= 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "⚠️Invalid input. Please enter a positive integer!",
                        cancellationToken: cancellationToken);

                    await Task.Delay(1000);
                }

                else if (fieldSize <= 10)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Field Size set to: {fieldSize} ✅",
                        cancellationToken: cancellationToken);

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Enter Number Of Mines 💣\n(e.g. /set_mines 5)",
                        cancellationToken: cancellationToken);

                    await Task.Delay(1000);
                }

                else if (fieldSize > 10)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "⚠️Invalid input. The field size cannot be more than 10!",
                        cancellationToken: cancellationToken);

                    await Task.Delay(1000);
                }
            }

            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "⚠️Invalid input. Please enter a valid number!",
                    cancellationToken: cancellationToken);

                await Task.Delay(1000);
            }
        }

        else if (messageText.StartsWith("/set_mines"))
        {
            // Обробка команди встановлення кількості мін
            if (int.TryParse(messageText.Replace("/set_mines", ""), out numMines))
            {
                if (numMines <= 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "⚠️Invalid input. Please enter a positive integer!",
                        cancellationToken: cancellationToken);

                    await Task.Delay(1000);
                }

                else if (numMines >= fieldSize * fieldSize)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "⚠️Invalid input. The number of mines cannot be greater than the size of the field multiplied by itself!",
                        cancellationToken: cancellationToken);

                    await Task.Delay(1000);
                }

                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Number of Mines set to: {numMines} ✅",
                        cancellationToken: cancellationToken);

                    await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "🔽 Press Button to Build Field 🔽",
                    replyMarkup: buildButton,
                    cancellationToken: cancellationToken);

                    await Task.Delay(1000);

                }
            }

            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Invalid input. Please enter a valid number!",
                    cancellationToken: cancellationToken);

                await Task.Delay(1000);
            }
        }

        else if (messageText == "Build Field")
        {
            // Побудова поля з мінами
            if (fieldSize > 0 && numMines > 0)
            {
                bool[,] field = CreateField(fieldSize, numMines);

                var textField = DisplayField(field);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: textField,
                    replyMarkup: restart,
                    cancellationToken: cancellationToken);

                await Task.Delay(2000);
            }

            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "⚠️Please set the field size and number of mines first!",
                    cancellationToken: cancellationToken);

                await Task.Delay(1000);
            }
        }

        else
        {
            // Відправлення повідомлення про невідому команду
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Unknown command",
                replyMarkup: playOrInstruction,
                cancellationToken: cancellationToken);

            await Task.Delay(1000);
        }
    }

    private static async Task howToPlay(ITelegramBotClient botClient, long chatId, int messageId)
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "At first set the Field Size\n(e.g. /set_fieldsize 10)\nThe field size cannot be more than 10!",
            cancellationToken: default);

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "After set the number of Mines\n(e.g. /set_mines 5)\nThe number of mines cannot be greater than the size of the field multiplied by itself!",
            cancellationToken: default);
    }


    // Функція для створення поля з мінами
    private static bool[,] CreateField(int fieldSize, int numMines)
    {
        int rows = fieldSize;
        int cols = fieldSize;
        bool[,] field = new bool[rows, cols];
        Random rand = new Random();

        List<(int, int)> positions = new List<(int, int)>();

        // Додавання усіх позицій у список
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                positions.Add((i, j));
            }
        }

        // Розставлення мін на полі
        while (numMines > 0 && positions.Count > 0)
        {
            int index = rand.Next(positions.Count);
            int row = positions[index].Item1;
            int col = positions[index].Item2;

            field[row, col] = true;
            positions.RemoveAt(index);
            numMines--;
        }

        return field;
    }

    // Функція для відображення поля
    private static string DisplayField(bool[,] field)
    {
        StringBuilder fieldBuilder = new StringBuilder();

        int rows = field.GetLength(0);
        int cols = field.GetLength(1);

        fieldBuilder.Append("   ");
        for (int j = 0; j < cols; j++)
        {
            fieldBuilder.Append(j + " ");
        }
        fieldBuilder.AppendLine();

        fieldBuilder.Append("ㅤ");
        for (int j = 0; j < cols; j++)
        {
            fieldBuilder.Append(" -▿-");
        }
        fieldBuilder.AppendLine();

        for (int i = 0; i < rows; i++)
        {
            fieldBuilder.Append(i + "| ");
            for (int j = 0; j < cols; j++)
            {
                fieldBuilder.Append(field[i, j] ? "💥" : "◽");
            }
            fieldBuilder.AppendLine(" |");
        }

        fieldBuilder.Append("ㅤ");
        for (int j = 0; j < cols; j++)
        {
            fieldBuilder.Append(" -▵-");
        }
        fieldBuilder.AppendLine();

        return fieldBuilder.ToString();
    }
}
