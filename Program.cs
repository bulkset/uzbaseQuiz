using Telegram.Bot;
using uzbaseQuiz.Configurations;
using uzbaseQuiz.Handlers;

namespace uzbaseQuiz
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new TelegramBotClient(Configuration.BotToken);
            var updateHandler = new UpdateHandler(client);

            client.StartReceiving(
                updateHandler.UpdateHandlerAsync,  // Handler for updates (messages, commands, etc.)
                updateHandler.HandleErrorAsync     // Handler for errors
            );

            Console.WriteLine("Bot is running. Press any key to exit...");
            Console.ReadLine(); // Keeps the application alive
        }
    }
}
