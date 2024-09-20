using Telegram.Bot.Types.ReplyMarkups;

namespace uzbaseQuiz.Keyboards
{
    public class MainMenuKeyboard
    {
        public InlineKeyboardMarkup GetMainMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData("Start Quiz", "start_quiz") },
                new [] { InlineKeyboardButton.WithCallbackData("My stats", "view_stats") }
            });
        }

        //main inline keyboard
    }
}
