namespace uzbaseQuiz.Configurations
{
    public partial class Configuration
    {
        private static string password = "admin123";
        public static string BotToken => "7412851275:AAGSilvpt9B5kqtmPTdXd4ptC219CwRSOjo";

        public static string ConnectionString => $"Host=localhost;Port=5432;Username=postgres;Password={password};Database=QuizBot;";
    }
}
