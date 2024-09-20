using Telegram.Bot;
using Telegram.Bot.Types;

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
            throw new NotImplementedException();
        }

        private async Task HandleStatisticsCommandAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}