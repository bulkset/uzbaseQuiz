using Telegram.Bot.Types.ReplyMarkups;

namespace uzbaseQuiz.Keyboards
{
    public static class AdminKeyboard
    {
        public static InlineKeyboardMarkup GetAdminMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📋 Export Users to Excel", "export_users_excel"),
                    InlineKeyboardButton.WithCallbackData("📊 Export All Tests to PDF", "export_tests_pdf"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📝 Manage Subjects", "manage_subjects"),
                    InlineKeyboardButton.WithCallbackData("📚 Manage Tests & Answers", "manage_tests"),
                }
            });
        }

        public static InlineKeyboardMarkup GetSubjectManagementMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("➕ Add New Subject", "add_subject"),
                    InlineKeyboardButton.WithCallbackData("✏️ Edit Subject", "edit_subject"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("❌ Delete Subject", "delete_subject"),
                    InlineKeyboardButton.WithCallbackData("🔙 Back to Admin Menu", "back_admin_menu")
                }
            });
        }

        public static InlineKeyboardMarkup GetTestManagementMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("➕ Add New Question", "add_question"),
                    InlineKeyboardButton.WithCallbackData("✏️ Edit Question", "edit_question"),
                    InlineKeyboardButton.WithCallbackData("❌ Delete Question", "delete_question"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("➕ Add New Test", "add_test"),
                    InlineKeyboardButton.WithCallbackData("✏️ Edit Test", "edit_test"),
                    InlineKeyboardButton.WithCallbackData("❌ Delete Test", "delete_test"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔙 Back to Admin Menu", "back_admin_menu")
                }
            });
        }

        public static InlineKeyboardMarkup GetAnswerManagementMenu(int questionId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("➕ Add New Answer", $"add_answer_{questionId}"),
                    InlineKeyboardButton.WithCallbackData("❌ Delete Answer", $"delete_answer_{questionId}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔙 Back to Test Management", "manage_tests")
                }
            });
        }
    }
}
