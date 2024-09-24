using System.Collections.Generic;
using System.Linq;
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
        private async Task HandleCallbackQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var data = callbackQuery.Data;
            long chatId = update.CallbackQuery.Message.Chat.Id;
            int messageId = callbackQuery.Message.MessageId;
            ISubjectRepository subjectRepository = new SubjectRepository(Configuration.ConnectionString);
            IQuestionRepository questionRepository = new QuestionRepository(Configuration.ConnectionString);
            IAnswerRepository answerRepository = new AnswerRepository(Configuration.ConnectionString);

            switch (data)
            {
                case "export_users_excel":
                    await client.SendTextMessageAsync(chatId, "Exporting users to Excel...");
                    break;
                case "export_tests_pdf":
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
                    await client.SendTextMessageAsync(chatId, "Enter the name of the new test:", replyMarkup: new ForceReplyMarkup { Selective = true });
                    break;
                case "add_question":
                    var subjectsForQuestion = await subjectRepository.GetAllSubjectsAsync();
                    if (subjectsForQuestion != null)
                    {
                        var subjectKeyboard = new InlineKeyboardMarkup(
                            subjectsForQuestion.Select(s => new[] { InlineKeyboardButton.WithCallbackData(s.Name, $"select_subject_for_question_{s.Id}") })
                        );
                        await client.SendTextMessageAsync(chatId, "Select a subject to add a question:", replyMarkup: subjectKeyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "No available subjects.");
                    }
                    break;
                case string callbackData when data.StartsWith("select_subject_for_question_"):
                    subject_id = int.Parse(data.Split('_').Last());
                    await client.SendTextMessageAsync(chatId, "Enter the text of the new question:", replyMarkup: new ForceReplyMarkup { Selective = true });
                    break;
                case "edit_question":
                    var subjectsForEdit = await subjectRepository.GetAllSubjectsAsync();
                    if (subjectsForEdit != null && subjectsForEdit.Count > 0)
                    {
                        var subjectKeyboard = new InlineKeyboardMarkup(
                            subjectsForEdit.Select(s => new[] { InlineKeyboardButton.WithCallbackData(s.Name, $"select_subject_for_edit_question_{s.Id}") })
                        );
                        await client.SendTextMessageAsync(chatId, "Select a subject to edit a question:", replyMarkup: subjectKeyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "No available subjects.");
                    }
                    break;
                case string callbackData when data.StartsWith("select_subject_for_edit_question_"):
                    int selectedSubjectId = int.Parse(data.Split('_').Last());
                    var questionsForSelectedSubject = await questionRepository.GetQuestionsBySubjectIdAsync(selectedSubjectId);

                    if (questionsForSelectedSubject != null && questionsForSelectedSubject.Count > 0)
                    {
                        var keyboard = new InlineKeyboardMarkup(
                            questionsForSelectedSubject.Select(q => new[] { InlineKeyboardButton.WithCallbackData(q.QuestionText, $"edit_question_{q.Id}") })
                        );
                        await client.SendTextMessageAsync(chatId, "Edit a question:", replyMarkup: keyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "No questions available for this subject.");
                    }
                    break;
                case string callbackData when data.StartsWith("edit_question_"):
                    var questionId = int.Parse(data.Split('_')[2]);
                    var question = await questionRepository.FindQuestionById(questionId);

                    if (question != null)
                    {
                        await client.SendTextMessageAsync(chatId, "Enter the new text for the question:", replyMarkup: new ForceReplyMarkup { Selective = true });
                        currentQuestionData[chatId] = (question, false);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, $"Question with ID {questionId} not found.");
                    }
                    break;
                case "delete_question":
                    var subjectsForDeleteQuestion = await subjectRepository.GetAllSubjectsAsync();
                    if (subjectsForDeleteQuestion != null && subjectsForDeleteQuestion.Count > 0)
                    {
                        var subjectKeyboard = new InlineKeyboardMarkup(
                            subjectsForDeleteQuestion.Select(s => new[] { InlineKeyboardButton.WithCallbackData(s.Name, $"select_subject_for_delete_question_{s.Id}") })
                        );
                        await client.SendTextMessageAsync(chatId, "Select a subject to delete a question:", replyMarkup: subjectKeyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "No available subjects.");
                    }
                    break;
                case string callbackData when data.StartsWith("select_subject_for_delete_question_"):
                    int deleteSubjectId = int.Parse(data.Split('_').Last());
                    var questionsToDelete = await questionRepository.GetQuestionsBySubjectIdAsync(deleteSubjectId);
                    if (questionsToDelete != null && questionsToDelete.Count > 0)
                    {
                        var keyboard = new InlineKeyboardMarkup(
                            questionsToDelete.Select(q => new[] { InlineKeyboardButton.WithCallbackData(q.QuestionText, $"delete_question_{q.Id}") })
                        );
                        await client.SendTextMessageAsync(chatId, "Delete a question:", replyMarkup: keyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "No questions available for this subject.");
                    }
                    break;
                case string callbackData when data.StartsWith("delete_question_"):
                    var questionIdToDelete = int.Parse(data.Split('_')[2]);
                    var questionToDelete = await questionRepository.FindQuestionById(questionIdToDelete);
                    if (questionToDelete != null)
                    {
                        await client.SendTextMessageAsync(chatId, $"Are you sure you want to delete the question: '{questionToDelete.QuestionText}'?", replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Yes", $"confirm_delete_question_{questionIdToDelete}"),
                            InlineKeyboardButton.WithCallbackData("No", $"cancel_delete_question")
                        }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, $"Question with ID {questionIdToDelete} not found.");
                    }
                    await client.DeleteMessageAsync(chatId, messageId);
                    break;
                case string callbackData when data.StartsWith("confirm_delete_question_"):
                    var confirmedQuestionId = int.Parse(data.Split('_')[3]);
                    var questionForConfirmation = await questionRepository.FindQuestionById(confirmedQuestionId);
                    if (questionForConfirmation != null)
                    {
                        await questionRepository.DeleteQuestion(confirmedQuestionId);
                        await client.SendTextMessageAsync(chatId, $"Question '{questionForConfirmation.QuestionText}' deleted.");
                        await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, $"Cannot delete question with ID {confirmedQuestionId}!");
                    }
                    await client.DeleteMessageAsync(chatId, messageId);
                    break;
                case string callbackData when data.StartsWith("cancel_delete_question"):
                    await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());
                    await client.DeleteMessageAsync(chatId, messageId);
                    break;
                case string callbackData when data.StartsWith("edit_subject_"):
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
                    await client.DeleteMessageAsync(chatId, messageId);
                    break;
                case string callbackData when data.StartsWith("delete_subject_"):
                    var subjectIdToDelete = int.Parse(data.Split('_')[2]);
                    Subject subjectToDelete = await subjectRepository.FindSubjectById(subjectIdToDelete);
                    if (subjectToDelete != null)
                    {
                        await client.SendTextMessageAsync(chatId, $"Are you sure you want to delete the item? '{subjectToDelete.Name}'?", replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Yes", $"confirm_delete_subject_{subjectIdToDelete}"),
                            InlineKeyboardButton.WithCallbackData("No", $"cancel_delete_subject")
                        }));
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, $"Subject with ID {subjectIdToDelete} not found.");
                    }
                    await client.DeleteMessageAsync(chatId, messageId);
                    break;
                case string callbackData when data.StartsWith("confirm_delete_subject_"):
                    var subjectIdToConfirm = int.Parse(data.Split('_')[3]);
                    Subject subjectForConfirmation = await subjectRepository.FindSubjectById(subjectIdToConfirm);
                    if (subjectForConfirmation != null)
                    {
                        subjectRepository.DeleteSubject(subjectIdToConfirm);
                        await client.SendTextMessageAsync(chatId, $"Subject with name '{subjectForConfirmation.Name}' deleted");
                        await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, $"Cannot delete subject with name '{subjectForConfirmation.Name}'!");
                    }
                    await client.DeleteMessageAsync(chatId, messageId);
                    break;
                case string callbackData when data.StartsWith("change_correct_answer_no"):
                    var (questionToUpdate, _) = currentQuestionData[chatId];
                    await questionRepository.UpdateQuestion(questionToUpdate);
                    await client.SendTextMessageAsync(chatId, $"Question updated to '{questionToUpdate.QuestionText}'. Correct answer ID remains unchanged.");
                    await client.SendTextMessageAsync(chatId, "Back to Admin Menu", replyMarkup: AdminKeyboard.GetAdminMenu());

                    currentQuestionData.Remove(chatId);
                    break;
                case string callbackData when data.StartsWith("change_correct_answer_yes"):
                    await client.SendTextMessageAsync(chatId, "Enter the new ID of the correct answer:", replyMarkup: new ForceReplyMarkup { Selective = true });

                    break;
                default:
                    break;
            }
        }
    }
}
