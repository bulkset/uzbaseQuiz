using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using uzbaseQuiz.Configurations;
using uzbaseQuiz.Keyboards;
using uzbaseQuiz.Repositories;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Handlers
{
    public partial class UpdateHandler
    {
        public static int subject_id = 2;
        private TelegramBotClient client;
        private Dictionary<long, (Question Question, bool IsAwaitingAnswer)> currentQuestionData = new Dictionary<long, (Question, bool)>();

        public UpdateHandler(TelegramBotClient client)
        {
            this.client = client;
        }

        private async Task HandleMessageAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;
            ISubjectRepository subjectRepository = new SubjectRepository(Configuration.ConnectionString);
            IQuestionRepository questionRepository = new QuestionRepository(Configuration.ConnectionString);

            var messageHandler = message.Text switch
            {
                "/start" => HandleStartCommandAsync(client, update, cancellationToken),
                "/statistics" => HandleStatisticsCommandAsync(client, update, cancellationToken),
                "/admin" => HandleAdminCommandAsync(client, update, cancellationToken),
                _ => null
            };

            try
            {
                if (message.ReplyToMessage.Text.Contains("Enter the new ID of the correct answer:"))
                {
                    if (int.TryParse(message.Text, out int correctAnswerId))
                    {
                        var (newQuestion, _) = currentQuestionData[chatId];
                        newQuestion.CorrectAnswerId = correctAnswerId;
                        await questionRepository.UpdateQuestion(newQuestion);
                        await client.SendTextMessageAsync(chatId, $"Question '{newQuestion.QuestionText}' updated to subject ID {subject_id} with correct answer ID {correctAnswerId}.");
                        await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());

                        currentQuestionData.Remove(chatId);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Invalid ID format. Please enter a valid number for the correct answer ID.");
                    }
                }
                if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains("Add a new subject:"))
                {
                    var subjectName = message.Text;
                    var newSubject = new Subject { Name = subjectName, MaxScore = 100 };
                    await subjectRepository.SaveSubject(newSubject);
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

                if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains("Enter the text of the new question:"))
                {
                    var newQuestionText = message.Text;
                    var newQuestion = new Question { QuestionText = newQuestionText, SubjectId = subject_id };
                    currentQuestionData[chatId] = (newQuestion, true);

                    await client.SendTextMessageAsync(chatId, "Enter the ID of the correct answer:", replyMarkup: new ForceReplyMarkup { Selective = true });
                    return;
                }

                if (currentQuestionData.ContainsKey(chatId) && currentQuestionData[chatId].IsAwaitingAnswer)
                {
                    if (int.TryParse(message.Text, out int correctAnswerId))
                    {
                        var (newQuestion, _) = currentQuestionData[chatId];
                        newQuestion.CorrectAnswerId = correctAnswerId;
                        await questionRepository.SaveQuestion(newQuestion);
                        await client.SendTextMessageAsync(chatId, $"Question '{newQuestion.QuestionText}' added to subject ID {subject_id} with correct answer ID {correctAnswerId}.");
                        await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());

                        currentQuestionData.Remove(chatId);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Invalid ID format. Please enter a valid number for the correct answer ID.");
                    }
                }

                if (currentQuestionData.ContainsKey(chatId) && !currentQuestionData[chatId].IsAwaitingAnswer)
                {
                    var (questionToUpdate, _) = currentQuestionData[chatId];
                    questionToUpdate.QuestionText = message.Text;

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Yes", "change_correct_answer_yes"),
                            InlineKeyboardButton.WithCallbackData("No", "change_correct_answer_no")
                        }
                    });

                    await client.SendTextMessageAsync(chatId, $"Do you want to change the correct answer ID for the question '{questionToUpdate.QuestionText}'?", replyMarkup: inlineKeyboard);
                    currentQuestionData[chatId] = (questionToUpdate, false); 
                    return;
                }

                if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains("Enter the ID of the question to delete:"))
                {
                    if (int.TryParse(message.Text, out int questionId))
                    {
                        var question = await questionRepository.FindQuestionById(questionId);
                        if (question != null)
                        {
                            await questionRepository.DeleteQuestion(questionId);
                            await client.SendTextMessageAsync(chatId, $"Question '{question.QuestionText}' deleted successfully.");
                            await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());
                        }
                        else
                        {
                            await client.SendTextMessageAsync(chatId, $"Question with ID {questionId} not found.");
                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Invalid question ID format. Please enter a valid number.");
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
