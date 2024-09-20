namespace uzbaseQuiz.Configurations
{
    public partial class Configuration
    {
        private static string password = "Umarjon";
        public static string BotToken => "8030122876:AAH6MXlVpTjl5XtCvbRLYnD8sq3ovF9HQ_E";

        public static string ConnectionString => $"Server=localhost;Port=5432;Database=uzbase;User id = postgres; Password ={password}";
    }
}
