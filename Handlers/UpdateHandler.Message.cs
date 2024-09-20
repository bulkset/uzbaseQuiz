using Telegram.Bot;
using Telegram.Bot.Types;
using uzbaseQuiz.Repositories;
using uzbaseQuiz.Configurations;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Handlers
{
    public partial class UpdateHandler
    {
        private async Task HandleMessageAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            var messageHandler = message.Text switch
            {
                "/start" => HandleStartCommandAsync(client, update, cancellationToken),
                "/statistics" => HandleStatisticsCommandAsync(client, update, cancellationToken),
                _ => null
            };

            try
            {
                await messageHandler;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(client, ex, cancellationToken);
            }
        }

        private async Task HandleStartCommandAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            long chatId = update.Message.Chat.Id;

            string introMessage = "Welcome to the Quiz Bot! 🎉\nGet ready to test your knowledge.";
            await client.SendTextMessageAsync(chatId, introMessage);
            IUserRepository userRepository = new UserRepository(Configuration.ConnectionString);
            BotUser user = await userRepository.FindUserById(chatId);
            System.Console.WriteLine("checking out");
            System.Console.WriteLine(user.Name);
        }
        

        private async Task HandleStatisticsCommandAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}