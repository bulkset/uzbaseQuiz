using Telegram.Bot;
using Telegram.Bot.Types;
using uzbaseQuiz.Repositories;
using uzbaseQuiz.Configurations;
using uzbaseQuiz.Models;
using Telegram.Bot.Types.ReplyMarkups;
using uzbaseQuiz.Keyboards;

namespace uzbaseQuiz.Handlers
{
    public partial class UpdateHandler
    {
        private async Task HandleMessageAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            if(message.Contact != null )
            {
                long chatId = update.Message.Chat.Id;

                IUserRepository userRepository = new UserRepository(Configuration.ConnectionString);
                var AllUsers = userRepository.GetAllAsync();
                foreach (var user1 in await AllUsers)
                {
                    if (user1.user_id == chatId)
                    {
                        await client.SendTextMessageAsync(chatId, "You are already registered. You can start testing now!");
                        return;
                    }
                }
                var user = new BotUser
                {
                    user_id = message.Chat.Id,
                    Name = message.Contact.FirstName,
                    PhoneNumber = message.Contact.PhoneNumber,
                    Role = "user",
                    CreatedAt = DateTime.Now,
                };    
                await userRepository.SaveUser(user);

                
                MainMenuKeyboard keyboard = new MainMenuKeyboard();
                keyboard.MainMenu(client , update , cancellationToken, user);
            } 

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
            IUserRepository userRepository = new UserRepository(Configuration.ConnectionString);
            var AllUsers = userRepository.GetAllAsync();
            foreach (var user in await AllUsers)
            {
                if (user.user_id == chatId)
                {
                    await client.SendTextMessageAsync(chatId, "You are already registered. You can start testing now!");
                    return;
                }
            }

            string introMessage = "Welcome to the Quiz Bot! 🎉\nGet ready to test your knowledge.";
            await client.SendTextMessageAsync(chatId, introMessage);
            var NewUser = new BotUser();
            var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton("Share Contact") { RequestContact = true }
            })
            {
                ResizeKeyboard = true, 
                OneTimeKeyboard = true
            };
                await client.SendTextMessageAsync(
                chatId: chatId,
                text: "Please share your contact with us.",
                replyMarkup: replyKeyboard
            );
        }
                

                private async Task HandleStatisticsCommandAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
                {
                    throw new NotImplementedException();
                }
            }
}