using Telegram.Bot.Types.ReplyMarkups;
using uzbaseQuiz.Models;
using uzbaseQuiz.Repositories;
using uzbaseQuiz.Configurations;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Linq;

namespace uzbaseQuiz.Keyboards
{
    public class UserQuizKeyboard
    {      
        public  async Task  ChooseSubject(ITelegramBotClient client, Update update)
        {
            ISubjectRepository subjectRepository = new SubjectRepository(Configuration.ConnectionString);
            var subjects = await subjectRepository.GetAllAsync();
            
            var keyboardButtons = subjects.Select(s => 
                InlineKeyboardButton.WithCallbackData(s.Name, $"subject_{s.Id}")).ToArray();

            var keyboard = new InlineKeyboardMarkup(keyboardButtons.Select(b => new[] { b }));

            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Please choose a subject:",
                replyMarkup: keyboard
            );
        }
    }
}
