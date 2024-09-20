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
            
            client.StartReceiving(updateHandler: updateHandler.UpdateHandlerAsync,
            pollingErrorHandler: updateHandler.HandleErrorAsync);

            Console.ReadLine();
        }
    }
}
