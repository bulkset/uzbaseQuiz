using Telegram.Bot;
using Telegram.Bot.Types;
using uzbaseQuiz.Models;
using System.Threading;
using System.Threading.Tasks;

namespace uzbaseQuiz.Keyboards
{
    public class MainMenuKeyboard
    {
        public async Task MainMenu(ITelegramBotClient client, Update update, CancellationToken cancellationToken, BotUser user)
        {
            UserQuizKeyboard userQuizKeyboard = new UserQuizKeyboard();
            AdminKeyboard adminKeyboard = new AdminKeyboard();

            // if (user.Role == "Admin")
            // {
            //     // Show admin menu
            // }
            // else
            // {
                await userQuizKeyboard.ChooseSubject(client, update);
           // }
        }
    }
}
