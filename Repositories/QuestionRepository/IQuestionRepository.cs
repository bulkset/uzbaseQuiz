using System.Collections.Generic;
using System.Threading.Tasks;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> FindQuestionById(int id);
        Task<List<Question>> GetAllQuestionsAsync();
        Task<List<Question>> GetQuestionsBySubjectIdAsync(int subjectId);
        Task<Question> SaveQuestion(Question question);
        Task UpdateQuestion(Question question);
        Task DeleteQuestion(int id);
    }
}
