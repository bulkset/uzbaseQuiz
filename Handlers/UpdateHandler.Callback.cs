using Telegram.Bot;
using Telegram.Bot.Types;
namespace uzbaseQuiz.Handlers
{
    public partial class UpdateHandler
    {
        private TelegramBotClient client;

        public UpdateHandler(TelegramBotClient client)
        {
            this.client = client;
        }

        private async Task HandleCallbackQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            switch(update.CallbackQuery.Data){
                case "query":
                    break;
                default:
                    break;
            }
        }
    }

}
