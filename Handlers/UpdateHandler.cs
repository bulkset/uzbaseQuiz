using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace uzbaseQuiz.Handlers
{
    public partial class UpdateHandler
    {
        public async Task UpdateHandlerAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var updateHandler = update.Type switch
            {
                UpdateType.Message => HandleMessageAsync(client, update, cancellationToken),
                UpdateType.CallbackQuery => HandleCallbackQueryAsync(client, update, cancellationToken),
                _ => HandleUnknownUpdateTypeAsync(client, update, cancellationToken)
            };

            try
            {
                await updateHandler;
            }
            catch (Exception ex)
            {
                HandleErrorAsync(client, ex, cancellationToken);
            }
        }
        private async Task HandleUnknownUpdateTypeAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
