namespace uzbaseQuiz.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int UserTestId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public bool IsCorrect { get; set; }
    }

}