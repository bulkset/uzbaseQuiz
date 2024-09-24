using System.Collections.Generic;
using System.Threading.Tasks;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Repositories
{
    public interface IAnswerRepository
    {
        Task<List<Answer>> GetAllAnswersAsync();          // Получить все ответы
        Task<Answer> FindAnswerById(int id);              // Найти ответ по ID
        Task SaveAnswer(Answer answer);                   // Сохранить новый ответ
        Task UpdateAnswer(Answer answer);                 // Обновить существующий ответ
        Task DeleteAnswer(int id);                        // Удалить ответ по ID
    }
}
