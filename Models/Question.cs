namespace uzbaseQuiz.Models
{
    public class Question
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string QuestionText { get; set; }
    public int CorrectAnswerId { get; set; }
}

}