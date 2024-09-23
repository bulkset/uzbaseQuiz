using Telegram.Bot;
using Telegram.Bot.Types;
using uzbaseQuiz.Repositories;
using uzbaseQuiz.Configurations;
using uzbaseQuiz.Models;
using uzbaseQuiz.Keyboards;
using System.Security.Cryptography.X509Certificates;

namespace uzbaseQuiz.Handlers
{
    public partial class UpdateHandler
    {

        private async Task HandleMessageAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)

        {
            ISubjectRepository subjectRepository = new SubjectRepository(Configuration.ConnectionString);

            var message = update.Message;
            var chatId = message.Chat.Id;
            var messageHandler = message.Text switch
            {
                "/start" => HandleStartCommandAsync(client, update, cancellationToken),
                "/statistics" => HandleStatisticsCommandAsync(client, update, cancellationToken),
                "/admin" => HandleAdminCommandAsync(client, update, cancellationToken),
                _ => null
            };

            try
            {

                if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains("Add a new subject:"))
                {
                    var subjectName = message.Text;
                    // TODO: Get max score
                    var newSubject = new Subject { Name = subjectName, MaxScore = 100 };
                    Subject subject = await subjectRepository.SaveSubject(newSubject);
                    await client.SendTextMessageAsync(chatId, $"Subject '{subjectName}' added.");
                    await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());

                }
                else if (message.ReplyToMessage.Text.Contains("Enter a new item name:"))
                {
                    var newSubjectName = message.Text;
                    Subject subject = await subjectRepository.FindSubjectById(subject_id);
                    if (subject != null)
                    {
                        subject.Name = newSubjectName;
                        await subjectRepository.UpdateSubject(subject);
                        await client.SendTextMessageAsync(chatId, $"Subject '{subject.Name}' updated successfully.");
                        await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());

                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, $"Subject with ID {subject_id} not found.");
                    }
                }

                await messageHandler;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(client, ex, cancellationToken);
            }
            Console.WriteLine(message.ReplyToMessage);

        }

        private async Task HandleAdminCommandAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            long chatId = update.Message.Chat.Id;
            // TODO: Add logic to check if the user is an admin
            await client.SendTextMessageAsync(chatId, @$"Welcome, Admin {update.Message.From.FirstName}! 🌟
Please choose an option below:", replyMarkup: AdminKeyboard.GetAdminMenu());
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