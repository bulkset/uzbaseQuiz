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

        public UpdateHandler(TelegramBotClient client)
        {
            this.client = client;
        }

        private async Task HandleCallbackQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {

            var data = update.CallbackQuery.Data;
            long chatId = update.CallbackQuery.Message.Chat.Id;
            ISubjectRepository subjectRepository = new SubjectRepository(Configuration.ConnectionString);
            switch (data)
            {
                case "export_users_excel":
                    // TODO: logic to export users to Excel
                    await client.SendTextMessageAsync(chatId, "Exporting users to Excel...");
                    break;
                case "export_tests_pdf":
                    // TODO: logic to export tests to PDF
                    await client.SendTextMessageAsync(chatId, "Exporting tests to PDF...");
                    break;
                case "manage_subjects":
                    await client.SendTextMessageAsync(chatId, "Subject Management", replyMarkup: AdminKeyboard.GetSubjectManagementMenu());
                    break;
                case "manage_tests":
                    await client.SendTextMessageAsync(chatId, "Test Management", replyMarkup: AdminKeyboard.GetTestManagementMenu());
                    break;
                case "add_subject":
                    await client.SendTextMessageAsync(chatId, "Add a new subject: ", replyMarkup: new ForceReplyMarkup { Selective = true });
                    break;
                case "edit_subject":
                    var subjects = await subjectRepository.GetAllSubjectsAsync();
                    if (subjects != null)
                    {
                        var keyboard = new InlineKeyboardMarkup(
                            subjects.Select(s => new[] { InlineKeyboardButton.WithCallbackData(s.Name, $"edit_subject_{s.Id}") })
                        );
                        await client.SendTextMessageAsync(chatId, "Edit a subject:", replyMarkup: keyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Zero subjects.");
                    }
                    break;
                case "delete_subject":
                    var subjectsToDelete = await subjectRepository.GetAllSubjectsAsync();
                    if (subjectsToDelete != null && subjectsToDelete.Count > 0)
                    {
                        var keyboard = new InlineKeyboardMarkup(
                            subjectsToDelete.Select(s => new[] { InlineKeyboardButton.WithCallbackData(s.Name, $"delete_subject_{s.Id}") })
                        );

                        await client.SendTextMessageAsync(chatId, "Delete a subject:", replyMarkup: keyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "Zero subjects.");
                    }
                    break;
                case "back_admin_menu":
                    await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());
                    break;
                case "add_test":
                    // TODO: logic to add a new test
                    await client.SendTextMessageAsync(chatId, "Adding a new test...");
                    break;
                case "edit_test":
                    // TODO: logic to edit a test
                    await client.SendTextMessageAsync(chatId, "Editing a test...");
                    break;
                case "delete_test":
                    // TODO: logic to delete a test
                    await client.SendTextMessageAsync(chatId, "Deleting a test...");
                    break;
                default:
                    break;
            }
            if (data.StartsWith("edit_subject_"))
            {
                var subjectId = int.Parse(data.Split('_')[2]);
                subject_id = subjectId;
                Subject subject = await subjectRepository.FindSubjectById(subjectId);
                if (subject != null)
                {
                    await client.SendTextMessageAsync(chatId, "Enter a new item name:", replyMarkup: new ForceReplyMarkup { Selective = true });
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, $"Subject with ID {subjectId} not found.");
                }
            }
            else if (data.StartsWith("delete_subject_"))
            {
                var subjectId = int.Parse(data.Split('_')[2]);
                Subject subject = await subjectRepository.FindSubjectById(subjectId);
                if (subject != null)
                {
                    await client.SendTextMessageAsync(chatId, $"Are you sure you want to delete the item? '{subject.Name}'?", replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                InlineKeyboardButton.WithCallbackData("Yes", $"confirm_delete_subject_{subjectId}"),
                InlineKeyboardButton.WithCallbackData("No", $"cancel_delete_subject")
            }));
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, $"Subject with ID {subjectId} not found.");
                }
            }
        }

    }
}