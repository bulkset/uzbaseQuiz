namespace uzbaseQuiz.Models
{
    public class BotUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } // 'admin' or 'user'
        public DateTime CreatedAt { get; set; }
        public long user_id { get; set; }
    }
}
