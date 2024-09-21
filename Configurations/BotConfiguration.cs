namespace uzbaseQuiz.Configurations
{
    public partial class Configuration
    {
        private static string password = "Umarjon";
        public static string BotToken => "8030122876:AAH6MXlVpTjl5XtCvbRLYnD8sq3ovF9HQ_E";

        public static string ConnectionString => $"Host=localhost;Port=5432;Username=postgres;Password={password};Database=uzbase;";
    }
}
